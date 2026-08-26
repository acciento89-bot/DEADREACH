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
User-confirmed visual smoke check on the current Production 0.14 head.

Shipping presentation now uses:
- large readable DEADREACH header and resource counters
- six large navigation tabs without numbered debug-terminal styling
- left-side Next Deployment mission card
- central clean Bunker hero window; the old prototype cube/hologram presentation is intentionally removed
- right-side Campaign Status and Active Operator cards
- bottom Bunker Feed and large Deploy action
- safe-area aware landscape layout
- pixel-perfect Canvas with increased dynamic text pixel density

Visual design is frozen for 0.14. Do not reopen redesign unless a blocking defect appears.

## D — Command Center interaction smoke — PENDING
One consolidated Bunker session only:
- Arsenal equip/salvage updates state and Scrap
- Operator selection persists after tab switching
- Campaign level selection updates deployment state
- Workshop upgrade/calibration updates progression when affordable
- Supply opens and returns without legacy/DEV UI
- Deploy cannot double-start

## E — Stable expedition regression — PENDING
One expedition only:
1. Deploy from the 0.14 Bunker.
2. Confirm selected operator is visible.
3. Confirm MOVE / AIM-FIRE / Ability controls.
4. Complete the primary objective; optional BLACK CACHE only when available.
5. Extract and return to Bunker.
6. Confirm rewards/progression and selected operator/level/weapon persist.
7. Final gameplay/runtime Console has no red DEADREACH errors.

Note: the known Unity Device Simulator `StoreSerializedStates` / duplicate `AdaptivePerformanceUIExtension` exceptions are Unity editor-state defects, not DEADREACH gameplay exceptions. Release gameplay validation should use normal Game View if that local Device Simulator state remains corrupted.

## Release decision
Production 0.14 presentation and compile gate are accepted. PR may leave Draft. Final merge/release packaging requires only the consolidated D + E runtime smoke above; no further UI redesign or static micro-check loop is required.
