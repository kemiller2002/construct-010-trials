/// Handwritten extension code for the generated Timer workflow (cohort
/// COH-CHRONA-TIMER). Implements ITimerPorts.CurrentTime and
/// ITimerPorts.Perform/Stop, plus the pure domain helpers they need.
///
/// Everything marked PROVISIONAL is behavior the requirements and approved
/// decisions do not decide; each is escalated in dispositions.json.
module ChronaTimerPilot.Extensions

open System
open Generated.Timer

/// Diagnostic codes. Only CHRONA.TIMER.CONCURRENT_CONFLICT is named by a
/// requirement (R6); every other code is PROVISIONAL (question 14).
module Codes =
    let invalidTransition = "CHRONA.TIMER.INVALID_TRANSITION"
    let recoveryRequired = "CHRONA.TIMER.RECOVERY_REQUIRED"
    let clockMovedBack = "CHRONA.TIMER.CLOCK_MOVED_BACK"
    let heldPendingReview = "CHRONA.TIMER.HELD_PENDING_REVIEW"
    let stopEffectFailed = "CHRONA.TIMER.STOP_EFFECT_FAILED"
    let stopEffectUnknown = "CHRONA.TIMER.STOP_EFFECT_UNKNOWN"
    let concurrentConflict = "CHRONA.TIMER.CONCURRENT_CONFLICT"
    let overlap = "CHRONA.ACTIVITY.OVERLAP"
    let crossesBusinessDay = "CHRONA.ACTIVITY.CROSSES_BUSINESS_DAY"
    let invalidDuration = "CHRONA.ACTIVITY.INVALID_DURATION"
    let unknownTimeZone = "CHRONA.TIME_ZONE.UNKNOWN"

/// Tunable policy values. The threshold is PROVISIONAL (questions 3, 22).
module Policy =
    /// R2: a timer stopped after running less than this records nothing.
    let minimumRecorded = TimeSpan.FromSeconds 30.0
    /// R7: worked time strictly above this, measured at Stop, is held for review.
    /// PROVISIONAL threshold (no requirement gives one).
    let longRunningThreshold = TimeSpan.FromHours 12.0

/// One working segment. `Until = None` means the timer is running in it.
/// Instants are kept exactly as received (full DateTimeOffset tick precision,
/// PROVISIONAL, question 10) and compared as UTC instants.
type Segment = { From: DateTimeOffset; Until: DateTimeOffset option }

/// The device's timer record (R6 `TimerRecord`): device state, never an
/// authority. It holds only timestamps; elapsed time is always derived from
/// them (R15, R16), never counted.
type TimerRecord =
    { Lineage: string
      State: TimerState
      /// Chronological working segments.
      Segments: Segment list
      /// Some worked time when the timer was stopped but its time is held for review.
      Held: TimeSpan option
      /// Instant of the last accepted command; used to detect a clock moved back.
      LastEvent: DateTimeOffset
      /// A reconciliation obligation was already opened for this timer (R6).
      Conflicted: bool }

/// An activity: exact UTC start, whole minutes, business date, lineage.
type Activity =
    { Start: DateTimeOffset
      Minutes: int
      LocalDate: DateOnly
      Lineage: string }

/// What performing the Stop effect decided.
type StopOutcome =
    | Recorded of Activity list
    | NothingRecorded
    | HeldForReview of worked: TimeSpan
    | ReconciliationRequired of Activity list

/// Business-day calendar in the person's IANA zone. Using the scenario
/// (person's) zone for the midnight boundary is PROVISIONAL (question 9).
module BusinessTime =
    let tryZone (zone: string) : TimeZoneInfo option =
        try Some(TimeZoneInfo.FindSystemTimeZoneById zone) with _ -> None

    let localDate (tz: TimeZoneInfo) (instant: DateTimeOffset) : DateOnly =
        DateOnly.FromDateTime((TimeZoneInfo.ConvertTime(instant, tz)).DateTime)

    /// The first instant of the business day following the one containing
    /// `instant`. Handles a local midnight that does not exist (DST gap) or
    /// occurs twice (DST overlap, earliest occurrence).
    let nextMidnight (tz: TimeZoneInfo) (instant: DateTimeOffset) : DateTimeOffset =
        let local = TimeZoneInfo.ConvertTime(instant, tz).DateTime
        let mutable t = DateTime.SpecifyKind(local.Date.AddDays 1.0, DateTimeKind.Unspecified)
        while tz.IsInvalidTime t do
            t <- t.AddMinutes 1.0
        let offset =
            if tz.IsAmbiguousTime t then tz.GetAmbiguousTimeOffsets t |> Array.max
            else tz.GetUtcOffset t
        DateTimeOffset(t, offset).ToUniversalTime()

    /// Splits [from, until) at every business-day boundary.
    let rec split (tz: TimeZoneInfo) (from: DateTimeOffset) (until: DateTimeOffset) =
        if until <= from then []
        else
            let m = nextMidnight tz from
            if until <= m then [ (localDate tz from, from, until) ]
            else (localDate tz from, from, m) :: split tz m until

/// Worked-time arithmetic, always in exact ticks between UTC instants (so DST
/// changes cannot corrupt elapsed duration, R10).
module Worked =
    let private span (s: Segment) (asOf: DateTimeOffset) =
        let until = defaultArg s.Until asOf
        let d = until - s.From
        if d < TimeSpan.Zero then TimeSpan.Zero else d

    let asOf (segments: Segment list) (at: DateTimeOffset) : TimeSpan =
        segments |> List.fold (fun acc s -> acc + span s at) TimeSpan.Zero

    /// Rounds non-negative ticks to the nearest whole minute. An exact half
    /// minute rounds up (PROVISIONAL, question 7).
    let roundMinutes (ticks: int64) : int =
        int ((ticks + TimeSpan.TicksPerMinute / 2L) / TimeSpan.TicksPerMinute)

    let closeAll (segments: Segment list) (at: DateTimeOffset) =
        segments |> List.map (fun s -> if s.Until.IsNone then { s with Until = Some at } else s)

/// The validation every activity goes through, manual or timer-produced (R2,
/// R8). Only the rules the acceptance contract can express are checked here:
/// positive whole minutes, no business-day crossing, no overlap with another
/// authoritative activity. Project, activity type, tags, description and
/// business purpose are not part of the contract's scenario.
module Validation =
    let finish (a: Activity) = a.Start.AddMinutes(float a.Minutes)

    let overlapsInterval (a: Activity) (from: DateTimeOffset) (until: DateTimeOffset) =
        a.Start < until && from < finish a

    let overlaps (a: Activity) (b: Activity) = overlapsInterval a b.Start (finish b)

    let validate (tz: TimeZoneInfo) (ledger: Activity list) (a: Activity) : Result<unit, string> =
        if a.Minutes <= 0 then Error Codes.invalidDuration
        elif finish a > BusinessTime.nextMidnight tz a.Start then Error Codes.crossesBusinessDay
        elif ledger |> List.exists (overlaps a) then Error Codes.overlap
        else Ok()

/// Turning a stopped timer's working segments into activities.
module Conversion =
    let private floorMinute (d: DateTimeOffset) =
        let u = d.UtcTicks
        DateTimeOffset(u - u % TimeSpan.TicksPerMinute, TimeSpan.Zero)

    /// One activity per business day the timer worked in (PROVISIONAL,
    /// question 4): paused time excluded, start = the day's first working
    /// instant floored to the minute. The total is rounded once (R2, legacy gap
    /// R5) and distributed by cumulative rounding so the pieces sum exactly to
    /// the rounded total; the rounding remainder lands where cumulative rounding
    /// puts it (PROVISIONAL, question 1). A day whose share rounds to zero
    /// minutes produces no piece. Every piece keeps the timer's lineage (R1).
    let toActivities (tz: TimeZoneInfo) (lineage: string) (closed: Segment list) : Activity list =
        let parts =
            closed
            |> List.collect (fun s -> BusinessTime.split tz s.From (defaultArg s.Until s.From))
        let days =
            parts
            |> List.fold
                (fun acc (date, f: DateTimeOffset, u: DateTimeOffset) ->
                    match acc with
                    | (d, first, ticks) :: rest when d = date -> (d, first, ticks + (u - f).Ticks) :: rest
                    | _ -> (date, f, (u - f).Ticks) :: acc)
                []
            |> List.rev
        let _, _, pieces =
            days
            |> List.fold
                (fun (cumTicks, cumMinutes, out) (date, first, ticks) ->
                    let cum = cumTicks + ticks
                    let rounded = Worked.roundMinutes cum
                    let minutes = rounded - cumMinutes
                    if minutes <= 0 then (cum, rounded, out)
                    else
                        let dayEnd = BusinessTime.nextMidnight tz first
                        let start = floorMinute first
                        let start =
                            if start.AddMinutes(float minutes) > dayEnd then dayEnd.AddMinutes(-(float minutes))
                            else start
                        let piece = { Start = start; Minutes = minutes; LocalDate = date; Lineage = lineage }
                        (cum, rounded, piece :: out))
                (0L, 0, [])
        List.rev pieces

    /// Decides what a Stop at `at` means for `record` against the authoritative ledger.
    let decideStop (tz: TimeZoneInfo) (ledger: Activity list) (record: TimerRecord) (at: DateTimeOffset) : StopOutcome =
        let closed = Worked.closeAll record.Segments at
        let worked = Worked.asOf closed at
        if worked > Policy.longRunningThreshold then HeldForReview worked
        elif worked < Policy.minimumRecorded then NothingRecorded
        else
            let pieces = toActivities tz record.Lineage closed
            let segmentConflict =
                closed
                |> List.exists (fun s ->
                    ledger |> List.exists (fun a -> Validation.overlapsInterval a s.From (defaultArg s.Until at)))
            let invalid = pieces |> List.exists (fun p -> Validation.validate tz ledger p |> Result.isError)
            // R4/R5/R6: an overlap with authoritative time never silently resolves;
            // it becomes a reconciliation obligation (PROVISIONAL, question 2).
            if record.Conflicted || segmentConflict || invalid then ReconciliationRequired pieces
            else Recorded pieces

/// The ITimerPorts implementation for one command.
/// CurrentTime (R10, R15, R16): the host-supplied instant of the command, never
/// a clock read or a counter; all elapsed time is derived from stored timestamps.
/// Perform/Stop (R1, R2, R3, R8): computes the stop outcome from the persisted
/// segments and reports Completed; the outcome is published through `Outcome`.
/// If the outcome cannot be computed it reports Failed or Unknown, and the
/// caller keeps the timer as it was (PROVISIONAL, question 23).
type TimerPorts(now: DateTimeOffset, zone: TimeZoneInfo option, ledger: Activity list, record: TimerRecord option) =
    let mutable outcome: StopOutcome option = None
    member _.Outcome = outcome
    interface ITimerPorts with
        member _.CurrentTime() = now
        member _.Perform(effect, _decision) =
            match effect with
            | TimerEffect.Stop ->
                match zone, record with
                | None, _ -> TimerEffectOutcome.Failed Codes.unknownTimeZone
                | _, None -> TimerEffectOutcome.Failed "no timer record"
                | Some tz, Some r ->
                    try
                        outcome <- Some(Conversion.decideStop tz ledger r now)
                        TimerEffectOutcome.Completed
                    with ex ->
                        TimerEffectOutcome.Unknown ex.Message
