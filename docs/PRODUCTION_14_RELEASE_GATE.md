# DEADREACH — Production 0.14 Release Gate

Goal: close Production 0.14 without reopening the visual redesign loop.

## A — Fresh compile — PASS
- Current head compiles with 0 red compiler errors in Unity.
- The duplicate `LateUpdate()` partial-class regression was fixed in `989b646`.
- The release hardening block no longer overrides the final 1440x810 readability scaler.

## B — Operator asset integrity — PASS
- SAM / RAVEN / BRIGGS are validated by real non-weapon body meshes, not wrapper-prefab existence alone.
- The Quaternius shared atlas Git-LFS pointer recovery path is in place.
- Runtime visual fallback remains only as a safety net; it does not rewrite the selected operator.

## C — Final Command Center presentation — PASS
User-confirmed visual smoke check on the final Production 0.14 presentation.

Shipping presentation uses:
- large readable DEADREACH header and resource counters
- six large navigation tabs without numbered debug-terminal styling
- left-side Next Deployment mission card
- central clean Bunker hero window; the old prototype cube/hologram presentation is intentionally removed
- right-side Campaign Status and Active Operator cards
- bottom Bunker Feed and large Deploy action
- safe-area aware landscape layout
- pixel-perfect Canvas with increased dynamic text pixel density

Visual design is frozen for 0.14.

## Device Simulator editor state — RESOLVED
- The local Unity Device Simulator `StoreSerializedStates` / duplicate `AdaptivePerformanceUIExtension` exceptions were cleared by rebuilding local Unity project state.
- These were Unity editor-state exceptions, not DEADREACH runtime/gameplay faults.
- Normal Game View is clean.

## D — Command Center interaction smoke — PASS
User-confirmed final Bunker interaction smoke completed successfully on the shipping 0.14 UI/state flow.

Covered release-critical behavior:
- Command Center navigation/state remained usable
- deployment state remained valid
- no legacy/DEV presentation regression blocked the flow
- Deploy proceeded into the expedition normally

## E — Stable expedition regression — PASS
User-confirmed final end-to-end run completed successfully:
- Deploy from the 0.14 Bunker
- selected operator visible
- MOVE / AIM-FIRE / Ability controls usable
- expedition objective/extraction flow completed
- returned to Bunker successfully
- rewards/progression/persistent selections remained intact
- no blocking red DEADREACH runtime errors

## Release decision — PASS
Production 0.14 has passed the final compile, operator-integrity, Command Center presentation, editor-state cleanup, interaction, expedition/extraction, and persistence gates.

PR #14 is approved for final merge into `main`. No further UI redesign or QA loop is required for Production 0.14.
