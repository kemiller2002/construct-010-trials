/// Frozen test-facing adapter for the chrona-timer-01 acceptance suite. The
/// contract types (Scenario, Step, Command, Response, RunResult, TimerAdapter)
/// are in pilot/ChronaTimer/contract/Adapter.fs, namespace ChronaTimerAcceptance:
/// host-owned and frozen, read it but never edit it. Replace the body of
/// `adapter`; keep the module name, the value name and its type.
module ChronaTimerPilot.ChronaTimerAdapter

open System
open ChronaTimerAcceptance
open Generated.Timer
open ChronaTimerPilot.Extensions

/// Per-device state. `Record` is the persisted TimerRecord (written on every
/// change, so a Restart loses nothing but the recovery flag); `NeedsRecovery`
/// is set by Restart while a record exists and cleared by Recover (R12).
type private Device = { Record: TimerRecord option; NeedsRecovery: bool }

type private World =
    { Devices: Map<DeviceId, Device>
      /// The person's authoritative activities, oldest first.
      Ledger: Activity list
      Obligations: int
      Sequence: int }

let private emptyDevice = { Record = None; NeedsRecovery = false }

let private toPiece (a: Activity) : Piece =
    { Start = a.Start; Minutes = a.Minutes; LocalDate = a.LocalDate; Lineage = a.Lineage }

let private phaseOf (r: TimerRecord option) =
    match r with
    | None -> Phase.Idle
    | Some { Held = Some _ } -> Phase.Held
    | Some r ->
        match r.State with
        | TimerState.Idle -> Phase.Idle
        | TimerState.Running -> Phase.Running
        | TimerState.Paused -> Phase.Paused

let private view (r: TimerRecord option) (at: DateTimeOffset) : TimerView =
    match r with
    | None -> { Phase = Phase.Idle; Worked = TimeSpan.Zero }
    | Some { Held = Some w } -> { Phase = Phase.Held; Worked = w }
    | Some r -> { Phase = phaseOf (Some r); Worked = Worked.asOf r.Segments at }

let private stateOf (r: TimerRecord option) =
    match r with
    | None -> TimerState.Idle
    | Some r -> r.State

/// Runs one lifecycle command through the generated orchestration.
let private lifecycle (zone: TimeZoneInfo option) (person: string) (world: World) (step: Step) (command: TimerCommand) =
    let device = world.Devices |> Map.tryFind step.Device |> Option.defaultValue emptyDevice
    let at = step.At
    let reject code = world, Rejected code
    match device.Record with
    // R12 / question 8 (PROVISIONAL): a persisted timer must be recovered explicitly first.
    | Some _ when device.NeedsRecovery -> reject Codes.recoveryRequired
    // Question 12 (PROVISIONAL): held time blocks the device until reviewed; it is never overwritten.
    | Some { Held = Some _ } -> reject Codes.heldPendingReview
    // Question 6 (PROVISIONAL): a command earlier than the timer's last event is refused.
    | Some r when at < r.LastEvent -> reject Codes.clockMovedBack
    | record ->
        let ports = TimerPorts(at, zone, world.Ledger, record)
        match TimerOrchestration.handle ports (stateOf record) command with
        | TimerResult.Refused _ -> reject Codes.invalidTransition
        | TimerResult.Accepted(decision, at, effects) ->
            let failed =
                effects
                |> List.tryPick (fun (_, o) ->
                    match o with
                    | TimerEffectOutcome.Completed -> None
                    | TimerEffectOutcome.Failed _ -> Some Codes.stopEffectFailed
                    | TimerEffectOutcome.Unknown _ -> Some Codes.stopEffectUnknown)
            let put r = { world with Devices = world.Devices |> Map.add step.Device { device with Record = r } }
            match failed with
            // Question 23 (PROVISIONAL): a failed or unknown Stop leaves the timer in its source state.
            | Some code -> reject code
            | None ->
                match decision.Command, record with
                | TimerCommand.Start, _ ->
                    let n = world.Sequence + 1
                    let r =
                        { Lineage = sprintf "timer:%s:%s:%d" person step.Device n
                          State = decision.Next
                          Segments = [ { From = at; Until = None } ]
                          Held = None
                          LastEvent = at
                          Conflicted = false }
                    { put (Some r) with Sequence = n }, Accepted
                | TimerCommand.Pause, Some r ->
                    put (Some { r with State = decision.Next; Segments = Worked.closeAll r.Segments at; LastEvent = at }), Accepted
                | TimerCommand.Resume, Some r ->
                    put (Some { r with State = decision.Next; Segments = r.Segments @ [ { From = at; Until = None } ]; LastEvent = at }),
                    Accepted
                | TimerCommand.Stop, Some r ->
                    match ports.Outcome with
                    | None -> reject Codes.stopEffectUnknown
                    | Some(StopOutcome.Recorded pieces) ->
                        { put None with Ledger = world.Ledger @ pieces }, Stopped(StopResult.Recorded(List.map toPiece pieces))
                    | Some StopOutcome.NothingRecorded -> put None, Stopped StopResult.NothingRecorded
                    | Some(StopOutcome.HeldForReview w) ->
                        let held =
                            { r with State = decision.Next; Segments = Worked.closeAll r.Segments at; Held = Some w; LastEvent = at }
                        put (Some held), Stopped(StopResult.HeldForReview w)
                    | Some(StopOutcome.ReconciliationRequired pieces) ->
                        let w = put None
                        { w with Obligations = (if r.Conflicted then w.Obligations else w.Obligations + 1) },
                        Stopped(StopResult.ReconciliationRequired(List.map toPiece pieces))
                | _, None -> reject Codes.invalidTransition

let private recordManual (zone: TimeZoneInfo option) (world: World) (step: Step) (minutes: int) =
    match zone with
    | None -> world, Rejected Codes.unknownTimeZone
    | Some tz ->
        let n = world.Sequence + 1
        let a =
            { Start = step.At
              Minutes = minutes
              LocalDate = BusinessTime.localDate tz step.At
              Lineage = sprintf "manual:%d" n }
        match Validation.validate tz world.Ledger a with
        | Error code -> world, Rejected code
        | Ok() ->
            // R6: a running timer overlapping time recorded since it started is an
            // obligation (CHRONA.TIMER.CONCURRENT_CONFLICT), opened once per timer.
            let mutable opened = 0
            let devices =
                world.Devices
                |> Map.map (fun _ d ->
                    match d.Record with
                    | Some r when r.Held.IsNone && r.State = TimerState.Running && not r.Conflicted ->
                        opened <- opened + 1
                        { d with Record = Some { r with Conflicted = true } }
                    | _ -> d)
            { world with
                Devices = devices
                Ledger = world.Ledger @ [ a ]
                Obligations = world.Obligations + opened
                Sequence = n },
            Accepted

let private step (zone: TimeZoneInfo option) (person: string) (world: World) (s: Step) : World * Response =
    let device = world.Devices |> Map.tryFind s.Device |> Option.defaultValue emptyDevice
    match s.Command with
    | Command.Start -> lifecycle zone person world s TimerCommand.Start
    | Command.Pause -> lifecycle zone person world s TimerCommand.Pause
    | Command.Resume -> lifecycle zone person world s TimerCommand.Resume
    | Command.Stop -> lifecycle zone person world s TimerCommand.Stop
    | Command.Inspect -> world, Viewed(view device.Record s.At)
    | Command.Restart ->
        { world with Devices = world.Devices |> Map.add s.Device { device with NeedsRecovery = device.Record.IsSome } },
        Accepted
    | Command.Recover ->
        { world with Devices = world.Devices |> Map.add s.Device { device with NeedsRecovery = false } },
        RecoveredTimer(device.Record |> Option.map (fun r -> view (Some r) s.At))
    | Command.RecordManual minutes -> recordManual zone world s minutes

/// Runs one scenario as a pure function: exactly one Response per step, in
/// order, plus the person's authoritative ledger and open obligations at the end.
let adapter: TimerAdapter =
    { Name = "candidate"
      Run =
        fun scenario ->
            let zone = BusinessTime.tryZone scenario.Zone
            let start = { Devices = Map.empty; Ledger = []; Obligations = 0; Sequence = 0 }
            let world, responses =
                scenario.Steps
                |> List.fold
                    (fun (w, acc) s ->
                        let w', r = step zone scenario.Person w s
                        w', r :: acc)
                    (start, [])
            { Responses = List.rev responses
              Ledger = world.Ledger |> List.sortBy (fun a -> a.Start) |> List.map toPiece
              Obligations = world.Obligations } }
