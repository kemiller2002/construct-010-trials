# Construction handoff: COH-CHRONA-TIMER

- Write only these files, by flat file name, into the outbox: Extensions.fs, ChronaTimerAdapter.fs; plus dispositions.json.
- dispositions.json: {"dispositions":[{"obligationId":"OBL-...","disposition":"implemented|escalated","note":"...","files":["<flat file name>"]}]}, one entry per obligation.
- Disposition every obligation in dispositions.json. A conflict, undecidedEffectFailure, uncoveredRequirement or unsupportedVocabulary obligation needs an approved decision the agent cannot make: escalate it, and state any provisional code behavior in its note (each `Escalate:` item below). An extensionBehavior obligation is implemented, or escalated when its text leaves a choice open.
- `try` builds the candidate; `submit` commits it (at most 3 submits). Nothing else: no other file, path, project, dependency or command.
- Generated, project and dependency files stay untouched. Every required interface member is implemented in a permitted file; there is no default implementation.

## Obligations

### OBL-COH-CHRONA-TIMER-conflict-0ff863bc5950
Escalate: CONFLICT-EXACT-TOTAL-VS-WHOLE-MINUTES: CHR-GAP-009: 10.3 requires 'exact total working time' across a midnight split while the legacy rule rounds the total to whole minutes; which day receives a rounded remainder is not decided. Which outcome is authoritative for the cohort's behavior?
Requirements: R1, R2, R3
Decisions: IFC-TIMER-PORTS, DEC-TIMER-LIFECYCLE
Verified by: V1

### OBL-COH-CHRONA-TIMER-conflict-23cf927c72c9
Escalate: CONFLICT-OVERLAP-AUTHORITY: CHR-GAP-017/023: when timers (or a timer and time recorded elsewhere) overlap, which one, if either, becomes authoritative and whether the non-overlapping remainder is recorded is not stated. Which outcome is authoritative for the cohort's behavior?
Requirements: R4, R5, R6
Decisions: DF-CHRONA-2026-0005
Verified by: V1

### OBL-COH-CHRONA-TIMER-conflict-3e2f7ba3c4d7
Escalate: CONFLICT-LONG-RUNNING-DETECTION: CHR-GAP-011/012: 'unusually long timers' has no threshold, and whether it is measured as worked or wall time, while running or at stop, is not stated. Which outcome is authoritative for the cohort's behavior?
Requirements: R7
Verified by: V1

### OBL-COH-CHRONA-TIMER-conflict-45469cd6b249
Escalate: CONFLICT-SEGMENTS-TO-ACTIVITIES: CHR-GAP-008: a paused timer becomes one activity excluding pauses or one activity per working segment; the number and placement of pieces is not decided. Which outcome is authoritative for the cohort's behavior?
Requirements: R2, R8
Decisions: IFC-TIMER-PORTS, DEC-TIMER-LIFECYCLE
Verified by: V1

### OBL-COH-CHRONA-TIMER-conflict-6c9144421437
Escalate: CONFLICT-START-WHILE-ACTIVE: CHR-GAP-003: 'Only one timer may be active at a time' does not say whether a second Start while a timer is running or paused is refused or stops the first timer. Which outcome is authoritative for the cohort's behavior?
Requirements: R9, R2
Decisions: IFC-TIMER-PORTS, DEC-TIMER-LIFECYCLE
Verified by: V1

### OBL-COH-CHRONA-TIMER-conflict-7b0d9714cbff
Escalate: CONFLICT-CLOCK-MOVED-BACK: CHR-GAP-018: a command whose instant is earlier than the timer's previous event is not addressed. Which outcome is authoritative for the cohort's behavior?
Requirements: R10
Decisions: IFC-TIMER-PORTS
Verified by: V1

### OBL-COH-CHRONA-TIMER-conflict-8e646f47d2ae
Escalate: CONFLICT-ROUNDING-TIES: CHR-GAP-006: 'rounded to the nearest whole minute' does not say how an exact half minute rounds. Which outcome is authoritative for the cohort's behavior?
Requirements: R11, R2
Decisions: IFC-TIMER-PORTS, DEC-TIMER-LIFECYCLE, DF-CHRONA-2026-0002
Verified by: V1

### OBL-COH-CHRONA-TIMER-conflict-9e469ed1a615
Escalate: CONFLICT-COMMANDS-BEFORE-RECOVERY: CHR-GAP-015: whether timer commands are refused between a restart and the explicit recovery is not stated. Which outcome is authoritative for the cohort's behavior?
Requirements: R12
Verified by: V1

### OBL-COH-CHRONA-TIMER-conflict-b75d1ee28d0b
Escalate: CONFLICT-BUSINESS-DAY-ZONE: CHR-GAP-010/021: no text says whose time zone defines the midnight business-day boundary. Which outcome is authoritative for the cohort's behavior?
Requirements: R1, R10
Decisions: IFC-TIMER-PORTS
Verified by: V1

### OBL-COH-CHRONA-TIMER-conflict-da40c42e1116
Escalate: CONFLICT-INSTANT-PRECISION: CHR-GAP-025: the sub-second precision of stored instants is not specified. Which outcome is authoritative for the cohort's behavior?
Requirements: R10
Decisions: IFC-TIMER-PORTS
Verified by: V1

### OBL-COH-CHRONA-TIMER-conflict-dad72230d3a6
Escalate: CONFLICT-ACTIVE-TIMER-SCOPE: CHR-GAP-016: whether the one-active-timer rule is per person or per device, and whether a second device may start while another device's timer runs, is not stated. Which outcome is authoritative for the cohort's behavior?
Requirements: R9, R4, R13
Decisions: DEC-TIMER-LIFECYCLE
Verified by: V1

### OBL-COH-CHRONA-TIMER-conflict-e645519ae0a1
Escalate: CONFLICT-HELD-TIME-REVIEW: CHR-GAP-013: what review consists of and how held time later becomes activities is not specified. Which outcome is authoritative for the cohort's behavior?
Requirements: R7, R6
Decisions: DF-CHRONA-2026-0005
Verified by: V1

### OBL-COH-CHRONA-TIMER-conflict-f07b94d9b406
Escalate: CONFLICT-STOP-WHILE-PAUSED: CHR-GAP-005: stopping a paused timer is neither permitted nor forbidden by the text. Which outcome is authoritative for the cohort's behavior?
Requirements: R9, R2
Decisions: IFC-TIMER-PORTS, DEC-TIMER-LIFECYCLE
Verified by: V1

### OBL-COH-CHRONA-TIMER-conflict-f87fce779ccf
Escalate: CONFLICT-INVALID-COMMAND-RESPONSE: CHR-GAP-004: an illegal command (pause when idle or paused, resume when idle or running, stop when idle) is either refused with a diagnostic or accepted as a no-op; no text decides which or names a diagnostic code. Which outcome is authoritative for the cohort's behavior?
Requirements: R9, R14, R2
Decisions: IFC-TIMER-PORTS, DEC-TIMER-LIFECYCLE
Verified by: V1

### OBL-COH-CHRONA-TIMER-extensionBehavior-09ac3c0ecabe
Implement: Requirement R3 is realized only through the extension surface ITimerPorts.Perform/Stop. Its behavior is not generated: implement it in handwritten extension code against the requirement text. Independent verification: VER-TIMER-RECORDING.
Requirements: R3, R16, R1, R10, R11, R15, R8, R2
Decisions: IFC-TIMER-PORTS, DEC-TIMER-LIFECYCLE, DF-CHRONA-2026-0002, DF-CHRONA-2026-0003
Interfaces: `ITimerPorts.Perform/Stop` in `pilot/ChronaTimer/Extensions.fs`
Disposition: implemented or escalated
Verified by: V1

### OBL-COH-CHRONA-TIMER-extensionBehavior-478842b330de
Implement: Requirement R1 is realized only through the extension surface ITimerPorts.Perform/Stop. Its behavior is not generated: implement it in handwritten extension code against the requirement text. Independent verification: VER-TIMER-RECORDING.
Requirements: R1, R16, R10, R11, R15, R8, R3, R2
Decisions: IFC-TIMER-PORTS, DEC-TIMER-LIFECYCLE, DF-CHRONA-2026-0002, DF-CHRONA-2026-0003
Interfaces: `ITimerPorts.Perform/Stop` in `pilot/ChronaTimer/Extensions.fs`
Disposition: implemented or escalated
Verified by: V1

### OBL-COH-CHRONA-TIMER-extensionBehavior-90476b9445b0
Implement: Requirement R15 is realized only through the extension surface ITimerPorts.CurrentTime. Its behavior is not generated: implement it in handwritten extension code against the requirement text. Independent verification: VER-TIMER-RECOVERY.
Requirements: R15, R16, R1, R10, R11, R8, R3, R2, R12, R6
Decisions: IFC-TIMER-PORTS, DEC-TIMER-LIFECYCLE, DF-CHRONA-2026-0005, DF-CHRONA-2026-0002, DF-CHRONA-2026-0003
Interfaces: `ITimerPorts.CurrentTime` in `pilot/ChronaTimer/Extensions.fs`
Disposition: implemented or escalated
Verified by: V1

### OBL-COH-CHRONA-TIMER-extensionBehavior-9faab2356f5c
Implement: Requirement R16 is realized only through the extension surface ITimerPorts.CurrentTime. Its behavior is not generated: implement it in handwritten extension code against the requirement text. Independent verification: VER-TIMER-RECOVERY.
Requirements: R16, R1, R10, R11, R15, R8, R3, R2, R12, R6
Decisions: IFC-TIMER-PORTS, DEC-TIMER-LIFECYCLE, DF-CHRONA-2026-0005, DF-CHRONA-2026-0002, DF-CHRONA-2026-0003
Interfaces: `ITimerPorts.CurrentTime` in `pilot/ChronaTimer/Extensions.fs`
Disposition: implemented or escalated
Verified by: V1

### OBL-COH-CHRONA-TIMER-extensionBehavior-d71b9ac4792f
Implement: Requirement R10 is realized only through the extension surface ITimerPorts.CurrentTime. Its behavior is not generated: implement it in handwritten extension code against the requirement text. Independent verification: VER-TIMER-RECORDING.
Requirements: R10, R16, R1, R11, R15, R8, R3, R2
Decisions: IFC-TIMER-PORTS, DEC-TIMER-LIFECYCLE, DF-CHRONA-2026-0002, DF-CHRONA-2026-0003
Interfaces: `ITimerPorts.CurrentTime` in `pilot/ChronaTimer/Extensions.fs`
Disposition: implemented or escalated
Verified by: V1

### OBL-COH-CHRONA-TIMER-extensionBehavior-e5b836c48211
Implement: Requirement R8 is realized only through the extension surface ITimerPorts.Perform/Stop. Its behavior is not generated: implement it in handwritten extension code against the requirement text. Independent verification: VER-TIMER-RECORDING.
Requirements: R8, R16, R1, R10, R11, R15, R3, R2
Decisions: IFC-TIMER-PORTS, DEC-TIMER-LIFECYCLE, DF-CHRONA-2026-0002, DF-CHRONA-2026-0003
Interfaces: `ITimerPorts.Perform/Stop` in `pilot/ChronaTimer/Extensions.fs`
Disposition: implemented or escalated
Verified by: V1

### OBL-COH-CHRONA-TIMER-uncoveredRequirement-2307233d584e
Escalate: No approved decision constructs R6; which decision or extension implements it? Independent verification: VER-TIMER-RECOVERY, VER-TIMER-LONG-RUNNING, VER-TIMER-DEVICES.
Requirements: R6, R16, R12, R15, R7, R4, R5, R13, R14
Decisions: IFC-TIMER-PORTS, DF-CHRONA-2026-0005, DF-CHRONA-2026-0003
Verified by: V1

### OBL-COH-CHRONA-TIMER-uncoveredRequirement-37a52d28e653
Escalate: No approved decision constructs R4; which decision or extension implements it? Independent verification: VER-TIMER-DEVICES.
Requirements: R4, R5, R13, R14, R6, R9
Decisions: DEC-TIMER-LIFECYCLE, DF-CHRONA-2026-0005
Verified by: V1

### OBL-COH-CHRONA-TIMER-uncoveredRequirement-73106e0c6634
Escalate: No approved decision constructs R5; which decision or extension implements it? Independent verification: VER-TIMER-DEVICES.
Requirements: R5, R4, R13, R14, R6
Decisions: DF-CHRONA-2026-0005
Verified by: V1

### OBL-COH-CHRONA-TIMER-uncoveredRequirement-786030204124
Escalate: No approved decision constructs R13; which decision or extension implements it? Independent verification: VER-TIMER-DEVICES.
Requirements: R13, R4, R5, R14, R6, R9
Decisions: DEC-TIMER-LIFECYCLE, DF-CHRONA-2026-0005
Verified by: V1

### OBL-COH-CHRONA-TIMER-uncoveredRequirement-a5d548c801de
Escalate: No approved decision constructs R12; which decision or extension implements it? Independent verification: VER-TIMER-RECOVERY.
Requirements: R12, R16, R15, R6
Decisions: IFC-TIMER-PORTS, DF-CHRONA-2026-0005, DF-CHRONA-2026-0003
Verified by: V1

### OBL-COH-CHRONA-TIMER-uncoveredRequirement-acb377bf3e7a
Escalate: No approved decision constructs R17; which decision or extension implements it? Independent verification: VER-TIMER-LIFECYCLE.
Requirements: R17, R2, R9, R18, R11, R14
Decisions: IFC-TIMER-PORTS, DEC-TIMER-LIFECYCLE, DF-CHRONA-2026-0002
Verified by: V1

### OBL-COH-CHRONA-TIMER-uncoveredRequirement-c9c829c6e34d
Escalate: No approved decision constructs R14; which decision or extension implements it? Independent verification: VER-TIMER-LIFECYCLE, VER-TIMER-DEVICES.
Requirements: R14, R2, R9, R18, R11, R17, R4, R5, R13, R6
Decisions: IFC-TIMER-PORTS, DEC-TIMER-LIFECYCLE, DF-CHRONA-2026-0005, DF-CHRONA-2026-0002
Verified by: V1

### OBL-COH-CHRONA-TIMER-uncoveredRequirement-fc70dac23f01
Escalate: No approved decision constructs R7; which decision or extension implements it? Independent verification: VER-TIMER-LONG-RUNNING.
Requirements: R7, R6
Decisions: DF-CHRONA-2026-0005
Verified by: V1

### OBL-COH-CHRONA-TIMER-undecidedEffectFailure-fdcdb9d55b8d
Escalate: Decision record IFC-TIMER-PORTS leaves the failure disposition of the Stop effect undecided. When the Stop effect fails or its outcome is unknown, does workflow Timer remain in the transition's source state or proceed to its target state?
Requirements: R1, R11, R2, R3, R8, R16, R10, R15
Decisions: IFC-TIMER-PORTS, DEC-TIMER-LIFECYCLE, DF-CHRONA-2026-0002, DF-CHRONA-2026-0003
Interfaces: `ITimerPorts.Perform/Stop` in `pilot/ChronaTimer/Extensions.fs`
Verified by: V1

## Independent verification

- **V1**: Behavior against the original requirements. Generated structure and its derived checks are not independent acceptance evidence.

## Approved decisions

- **IFC-TIMER-PORTS** (constructionRecord) [R1, R11, R2, R3, R8, R16, R10, R15]: Ports: transitionEffect for Stop (failure handling NOT DECIDED); currentTime.
- **DEC-TIMER-LIFECYCLE** (constructionRecord) [R9, R11, R18, R2]: Workflow Timer. States: Idle (initial), Paused, Running. Transitions: Pause: Running -> Paused; Resume: Paused -> Running; Start: Idle -> Running; Stop: Running -> Idle.
- **DF-CHRONA-2026-0005** (decisionDocument) [R6]: 12. **The device's timer (WI-0055).** The active timer, or a stopped one
- **DF-CHRONA-2026-0002** (decisionDocument) [R11, R18]: | Legacy rule | Chrona.Domain | Result |
- **DF-CHRONA-2026-0003** (decisionDocument) [R15]: 2. **Effects are data.** The engine asks for `Navigate`, `Wake` (the running

## Requirements

**R1** `kemiller2002/chrona/docs/requirements/CHRONA-REQUIREMENTS-EXPANSION.txt#L340-L341`
```text
### 10.3 Cross-midnight
Because an activity cannot cross the business-day boundary, a timer running across midnight should be split automatically at the boundary, preserving lineage and exact total working time.
```

**R2** `kemiller2002/time-tracking-application/docs/DOMAIN-REQUIREMENTS.md#L168-L179`
```text
## Recording time with a timer

- Only one timer may be active at a time.
- Pausing and resuming produces multiple tracked working segments; time
  spent paused is excluded from what is ultimately recorded.
- Stopping the timer computes total elapsed working time, rounded to the
  nearest whole minute.
- If the timer is stopped after running for less than thirty seconds,
  nothing is recorded.
- A timer-produced activity goes through the exact same validation as a
  manually entered one — the same overlap, project, activity-type,
  description, and business-purpose rules apply uniformly.
```

**R3** `kemiller2002/time-tracking-application/docs/DOMAIN-REQUIREMENTS.md#L58-L72`
```text
## Time representation

- Duration and start time are recorded exact to the minute. Six-minute
  quick-entry buttons (execution contract, Capability 6) are an input
  convenience only — they must never round, inflate, or replace the exact
  value the owner actually enters or the timer actually measures
  (execution contract §4: "must not silently inflate or replace exact
  elapsed time").
- An activity's start time plus duration must not cross midnight into the
  next day.
- A derived, separate six-minute **billing figure** is computed per
  activity by rounding its exact duration *up* to the next multiple of six
  minutes. Billing is computed per activity, not by rounding a daily or
  project total: two independent two-minute activities bill as twelve
  minutes combined, never as one four-minute total rounded once.
```

**R4** `kemiller2002/chrona/docs/requirements/CHRONA-REQUIREMENTS-EXPANSION.txt#L346-L347`
```text
### 10.5 Multiple devices
The same person may use multiple devices. Concurrent/offline active timers must not silently create authoritative overlaps. Ambiguous cases create a reconciliation obligation.
```

**R5** `kemiller2002/chrona/docs/requirements/CHRONA-REQUIREMENTS-EXPANSION.txt#L369-L381`
```text
## 12. Overlap and conflict rules

Existing overlap prevention remains.

Add:
- overlap is scoped by actor and organization
- Voided/Superseded entries do not consume active time
- Submitted/Approved entries still consume time
- imports cannot bypass overlap rules
- restore rechecks against current state
- concurrent divergent edits to the same activity must not silently resolve by last-write-wins

Same-record divergence becomes an explicit semantic conflict.
```

**R6** `kemiller2002/chrona/research/decisions/DF-CHRONA-2026-0005--records-on-github-through-arca-behind-the-store-port.md#L269-L276`
```text
12. **The device's timer (WI-0055).** The active timer, or a stopped one
    whose time is held until completed, is kept in this browser's storage
    under `chrona.timer.<organization>.<person>` (`TimerRecord`): device
    state, never a record and never an authority, written whenever it
    changes and recovered explicitly at startup. It keeps working when the
    records cannot be opened. Sign-out counts it as unsent work under the
    shared-device policy. A running timer that overlaps time recorded since
    it started is an obligation (`CHRONA.TIMER.CONCURRENT_CONFLICT`).
```

**R7** `kemiller2002/chrona/docs/requirements/CHRONA-REQUIREMENTS-EXPANSION.txt#L343-L344`
```text
### 10.4 Long-running safety
Detect unusually long timers and require review rather than assuming all elapsed time was worked.
```

**R8** `kemiller2002/time-tracking-application/docs/DOMAIN-REQUIREMENTS.md#L89-L101`
```text
## Creating an activity requires

- A start time exact to the minute, or a timer-derived start/stop pair.
- A positive whole number of minutes for duration; the activity must not
  cross midnight.
- A non-blank description.
- A non-blank **business purpose** — a statement of why the work relates
  to the business, distinct from the description of what was done.
- An existing, Active project.
- An existing, Active activity type.
- Every assigned tag must exist and currently be active.
- The activity's time interval must not overlap any other Recorded
  activity on the same date.
```

**R9** `kemiller2002/chrona/docs/requirements/CHRONA-REQUIREMENTS-EXPANSION.txt#L324-L326`
```text
## 10. Timer additions

Existing one-active-timer and pause/resume rules remain.
```

**R10** `kemiller2002/chrona/docs/requirements/CHRONA-REQUIREMENTS-EXPANSION.txt#L351-L365`
```text
## 11. Time zone and calendar semantics

Time zone is a domain concern.

Timer instants must be stored unambiguously.

Activities must preserve enough context to reconstruct intended business time:
- IANA timezone
- UTC offset at occurrence
- local date
- local start time

DST transitions must not corrupt elapsed duration.

Changing device timezone must not rewrite historical activities.
```

**R11** `kemiller2002/chrona/research/decisions/DF-CHRONA-2026-0002--reconstruct-not-migrate-legacy-reconciliation-and-data-status.md#L126-L145`
```text
| Legacy rule | Chrona.Domain | Result |
|---|---|---|
| Exact minutes; six-minute billing is derived, per activity, rounded up | `Billing.legacyDefault`, `billableMinutes`; minutes never changed | Kept |
| Activity cannot cross midnight | `CrossesBusinessDay`; timers split at the boundary | Kept (timer split is an expansion addition) |
| Status Recorded / Voided / Superseded; revision starts at 1 | `RecordState`, `Revision = 1` | Kept |
| Only Recorded may be amended, split, merged or voided; voided restores; superseded is final | `requireRecorded`, `Restore` | Kept |
| Amend, split and void check the expected revision | `RevisionConflict` | Kept |
| Description required | `MissingField "description"` | Kept |
| **Business purpose required** | not checked | **Gap R1** |
| Past-date manual entry needs a reason | `ReasonRequiredForHistoricalEntry` (window configurable) | Kept |
| Entry method recorded and shown | `EntryMethod` | Kept |
| No overlap with another Recorded activity | `Overlap.check` (half-open, scoped) | Kept |
| **Amended fields revalidated by the creation rules** | only overlap rechecked | **Gap R2** |
| Split: source at least two minutes, point strictly inside, durations sum exactly | at least two positive parts summing exactly | Kept |
| **Merge: two or more sources, same date, contiguous** | same date checked; non-contiguous merged as a duration | **Gap R3** |
| Restore revalidates against current state | overlap rechecked on restore | Kept |
| **Evidence: link and unlink on Recorded; link only on Voided; none on Superseded** | no record-state check | **Gap R4** |
| One active timer; pauses excluded | `Timer.start`, segments | Kept |
| **Stop rounds the working total to the nearest minute; under 30 seconds records nothing** | each piece floored separately, so pauses can lose up to a minute each | **Gap R5** |
| Timer activity validated like a manual one | `toActivities` builds records; validation happens on `Ledger.Record` (overlap only) | Covered by R1 and R2 |
```

**R12** `kemiller2002/chrona/docs/requirements/CHRONA-REQUIREMENTS-EXPANSION.txt#L337-L338`
```text
### 10.2 Recovery
On startup, recover a previously active timer explicitly. Never silently discard elapsed time.
```

**R13** `kemiller2002/chrona/docs/requirements/CHRONA-REQUIREMENTS-EXPANSION.txt#L589-L603`
```text
## 23. Offline behavior

Core time recording remains usable offline.

Required:
- timer works
- manual entry works
- edits persist locally
- sync state is visible
- unsynced data is not represented as globally synchronized
- reconnect performs safe reconciliation
- retries are idempotent
- unknown outcomes remain explicit

Offline/remote conflicts create visible reconciliation obligations. Neither side is silently discarded.
```

**R14** `kemiller2002/chrona/docs/requirements/CHRONA-REQUIREMENTS-EXPANSION.txt#L660-L669`
```text
Extend stable diagnostics to include:
- unauthorized capability
- organization mismatch
- actor mismatch
- submitted-period restriction
- approval conflict
- stale approval
- duplicate publication
- already-invoiced correction
- concurrent timer conflict
```

**R15** `kemiller2002/chrona/research/decisions/DF-CHRONA-2026-0003--product-ui-on-limen-in-memory-with-identity-and-store-ports.md#L59-L63`
```text
2. **Effects are data.** The engine asks for `Navigate`, `Wake` (the running
   timer's once-a-second refresh, through Limen's `limen.schedule` pack),
   `DescribeEnvironment` (the browser's time zone, through
   `limen.environment`) and `Store`. The display is always computed from
   timestamps, never counted in the browser (CHX-101).
```

**R16** `kemiller2002/chrona/docs/requirements/CHRONA-REQUIREMENTS-EXPANSION.txt#L328-L335`
```text
### 10.1 Persistence
An active timer must survive:
- refresh
- browser restart where supported
- temporary network loss
- device sleep

Elapsed time must be derived from persisted timestamps/segments, not a UI interval counter.
```

**R17** `kemiller2002/chrona/docs/requirements/CHRONA-REQUIREMENTS-EXPANSION.txt#L963-L975`
```text
## 43. Required scenario tests

At minimum test:

1. Manual exact-minute entry
2. Timer entry
3. Pause/resume
4. Timer recovery after browser restart
5. Timer crossing midnight
6. DST transition
7. Travel/timezone change
8. Overlap rejection
9. Concurrent-device overlap
```

**R18** `kemiller2002/chrona/research/decisions/DF-CHRONA-2026-0002--reconstruct-not-migrate-legacy-reconciliation-and-data-status.md#L67-L73`
```text
2. **The legacy rules are part of the requirements.** The expansion says
   "existing manual-entry rules remain", "existing duration invariants
   remain", "existing one-active-timer and pause/resume rules remain" and
   "existing daily attestation remains". Those existing rules are
   `time-tracking-application/docs/DOMAIN-REQUIREMENTS.md`. Every rule was
   compared with `Chrona.Domain`; the gaps (R1-R6) are WI-0043 and are closed
   in the domain before the UI is rebuilt on it.
```

## Extension interfaces

### `ITimerPorts` (declared in `pilot/ChronaTimer/generated/Timer/Timer.Orchestration.g.fs`; implement in `pilot/ChronaTimer/Extensions.fs`; required: ITimerPorts.CurrentTime, ITimerPorts.Perform/Stop)
```fsharp
abstract CurrentTime: unit -> global.System.DateTimeOffset
abstract Perform: effect: TimerEffect * decision: TimerDecision -> TimerEffectOutcome
```

## Acceptance adapter `ChronaTimerPilot.ChronaTimerAdapter` (replace the stub in `pilot/ChronaTimer/ChronaTimerAdapter.fs`; keep names and signatures)

```fsharp
/// Frozen test-facing adapter for the chrona-timer-01 acceptance suite. The
/// contract types (Scenario, Step, Command, Response, RunResult, TimerAdapter)
/// are in pilot/ChronaTimer/contract/Adapter.fs, namespace ChronaTimerAcceptance:
/// host-owned and frozen, read it but never edit it. Replace the body of
/// `adapter`; keep the module name, the value name and its type.
/// Runs one scenario as a pure function: exactly one Response per step, in
/// order, plus the person's authoritative ledger and open obligations at the end.
let adapter: TimerAdapter
```

### Contract types

#### `pilot/ChronaTimer/contract/Adapter.fs` (`ChronaTimerAcceptance`)
```fsharp
type DeviceId = string

type Command =
    /// Start a timer on the step's device.
    | Start
    /// Pause the device's running timer.
    | Pause
    /// Resume the device's paused timer.
    | Resume
    /// Stop the device's timer and turn its working time into activities.
    | Stop
    /// Read the device's timer as of the step's instant.
    | Inspect
    /// The device's process is lost (refresh, browser restart, device sleep,
    /// network loss): every piece of state it did not persist is gone.
    | Restart
    /// Explicit startup recovery on the device after a Restart.
    | Recover
    /// Record an authoritative manual activity of this many minutes starting at
    /// the step's instant (used to create time recorded elsewhere).
    | RecordManual of minutes: int

type Step = { At: DateTimeOffset; Device: DeviceId; Command: Command }

type Scenario = { Person: string; Zone: string; Steps: Step list }

type Phase =
    | Idle
    | Running
    | Paused
    /// Stopped, but its time is held for review instead of being recorded.
    | Held

/// Worked = time spent running, excluding pauses, as of the step's instant.
type TimerView = { Phase: Phase; Worked: TimeSpan }

/// One authoritative activity. Start is a UTC instant, Minutes a whole number,
/// LocalDate the business date in the scenario zone, Lineage the identity of
/// the timer (or manual entry) that produced it.
type Piece =
    { Start: DateTimeOffset
      Minutes: int
      LocalDate: DateOnly
      Lineage: string }

type StopResult =
    /// Activities that became authoritative.
    | Recorded of Piece list
    /// The timer ended and nothing was recorded (too short).
    | NothingRecorded
    /// Time not recorded; held for review. Carries the preserved worked time.
    | HeldForReview of worked: TimeSpan
    /// Not authoritative: a reconciliation obligation was created instead.
    | ReconciliationRequired of Piece list

type Response =
    /// Start, Pause, Resume, Restart and RecordManual succeeded.
    | Accepted
    /// The command was refused; the code is the implementation's diagnostic.
    | Rejected of code: string
    /// Answer to Stop.
    | Stopped of StopResult
    /// Answer to Inspect.
    | Viewed of TimerView
    /// Answer to Recover: the recovered timer, or None when there was none.
    | RecoveredTimer of TimerView option
    /// The adapter cannot express this command against its implementation.
    | Unsupported of reason: string

type RunResult =
    { Responses: Response list
      /// The person's authoritative activities at the end of the scenario.
      Ledger: Piece list
      /// Open reconciliation obligations at the end of the scenario.
      Obligations: int }

type TimerAdapter = { Name: string; Run: Scenario -> RunResult }
```


## Generated API (signatures only)

#### `pilot/ChronaTimer/generated/Timer/Timer.Model.g.fs` (`Generated.Timer`)
```fsharp
/// The states of the Timer workflow; the initial state is listed first.
[<RequireQualifiedAccess>]
type TimerState =
    | Idle
    | Paused
    | Running

/// The commands the Timer workflow accepts.
[<RequireQualifiedAccess>]
type TimerCommand =
    | Pause
    | Resume
    | Start
    | Stop

/// Effects an accepted command requests. Requested as data; only a host performs them.
[<RequireQualifiedAccess>]
type TimerEffect =
    | Stop
```

#### `pilot/ChronaTimer/generated/Timer/Timer.Transition.g.fs` (`Generated.Timer`)
```fsharp
/// Why a command was not applied. Each case is a distinct remedy.
[<RequireQualifiedAccess>]
type TimerRefusal =
    | InvalidTransition of state: TimerState * command: TimerCommand

/// An accepted command. Requested effects are data; this tier performs none.
type TimerDecision =
    { Previous: TimerState
      Next: TimerState
      Command: TimerCommand
      Effects: TimerEffect list }

/// The legal transitions, decided from one plan. Depends only on Tier 1.
[<RequireQualifiedAccess>]
module TimerTransition =
    /// Every legal transition as (from, command, to).
    let legal : (TimerState * TimerCommand * TimerState) list
    /// The target of a command, or None where the command is not legal. Every
    /// (state, command) pair is listed; there is no catch-all.
    let target (state: TimerState) (command: TimerCommand) : TimerState option
    /// Effects an accepted command requests.
    let effects (command: TimerCommand) : TimerEffect list
    /// Decides one command. Refusal is data; nothing is applied or performed here.
    let decide (state: TimerState) (command: TimerCommand) : Result<TimerDecision, TimerRefusal>
```

#### `pilot/ChronaTimer/generated/Timer/Timer.Orchestration.g.fs` (`Generated.Timer`)
```fsharp
/// What the host reports about a requested effect. Unknown is a first-class outcome, not an error to swallow.
[<RequireQualifiedAccess>]
type TimerEffectOutcome =
    | Completed
    | Failed of reason: string
    | Unknown of reason: string

/// Ports this workflow requires. Implemented by a host (Tier 4); generated code never implements them.
type ITimerPorts =
    abstract CurrentTime: unit -> global.System.DateTimeOffset
    abstract Perform: effect: TimerEffect * decision: TimerDecision -> TimerEffectOutcome

/// The result of handling one command. It reports effect outcomes; it does not decide what a failed or unknown effect means for the workflow.
[<RequireQualifiedAccess>]
type TimerResult =
    | Accepted of decision: TimerDecision * at: global.System.DateTimeOffset * effects: (TimerEffect * TimerEffectOutcome) list
    | Refused of refusal: TimerRefusal * at: global.System.DateTimeOffset

/// Coordinates the workflow. Depends on Tiers 1 and 2 only.
[<RequireQualifiedAccess>]
module TimerOrchestration =
    /// Coordinates one command: reads ports, delegates the decision to Tier 2 and
    /// asks the host to perform requested effects. It makes no semantic decision.
    let handle (ports: ITimerPorts) (state: TimerState) (command: TimerCommand) : TimerResult
```

## Permitted files

- `pilot/ChronaTimer/Extensions.fs` (at most 65536 bytes): ITimerPorts.CurrentTime, ITimerPorts.Perform/Stop
- `pilot/ChronaTimer/ChronaTimerAdapter.fs` (at most 65536 bytes) acceptance adapter: ChronaTimerPilot.ChronaTimerAdapter
