# DEADREACH — Production 0.14 One-Pass Release Gate

Goal: finish Production 0.14 in one consolidated validation run instead of returning to small isolated checks.

## A — Fresh compile — RECHECK REQUIRED
- The previous compile passed before the operator-asset integrity fix.
- Current 0.14 now includes runtime fallback, mesh-aware operator repair, and a stronger release validator.
- Run one fresh compile after pulling the current head.

## B — Static release validator — RECHECK REQUIRED
The previous user-confirmed result was:
`DEADREACH 0.14 RELEASE STATIC CHECK: PASS`

That PASS is superseded because the validator did not previously verify that SAM / RAVEN / BRIGGS prefabs contained resolvable body meshes.

Current validator additionally checks:
- Bunker and expedition scene assets
- both scenes enabled in Build Settings
- required Quaternius command-center Resources
- SAM / RAVEN / BRIGGS operator prefabs contain real non-weapon meshes
- Wenrexa preparation/fallback availability
- landscape-only orientation compatibility

If RAVEN/Shaun or BRIGGS/Matt wrappers exist but their glTF-backed meshes are unresolved, the validator now fails and directs the operator-art repair/build path to regenerate them.

## C — Command Center deep interaction sweep — PENDING
Do this in one Bunker session:
1. Overview visible; central hologram present.
2. Arsenal: equip another secured weapon if available.
3. Arsenal: salvage one unequipped weapon if available; Scrap counter must update.
4. Operators: select another operator, then switch tabs and back; selection must persist.
5. Campaign: select any unlocked level; footer must update.
6. Workshop: buy one affordable Bunker upgrade if available.
7. Workshop: calibrate equipped weapon if affordable/within cap.
8. Supply: open screen and return; no legacy/DEV UI may appear.
9. Overview: hologram must return after leaving and re-entering Overview.

Expected release-hardening behavior:
- action/status toast appears for profile changes
- header counters refresh without restarting Bunker
- footer deployment summary refreshes after level/operator changes
- no double activation on rapid taps

## D — Mobile landscape / safe-area sweep
Test one phone-like landscape aspect in Game view, preferably an ultrawide/notched preset:
- all command-center content remains inside safe area
- navigation buttons remain touchable
- Deploy remains fully visible/touchable
- Arsenal scroll remains draggable
- no portrait layout is used

## E — Full stable expedition regression
From the hardened 0.14 Bunker:
1. Deploy.
2. Confirm the selected production operator is visibly rendered. Runtime may fall back to the validated SAM visual only as a safety net if a selected operator asset is still broken; the release gate itself must not PASS until all three operator meshes are healthy.
3. Validate fixed MOVE / AIM-FIRE / Ability controls.
4. Validate active 0.12 sector layout and its hazard behavior.
5. Complete Primary objective.
6. Complete optional BLACK CACHE when available.
7. Confirm objective-gated extraction.
8. Extract.
9. Return to Bunker.
10. Confirm Scrap/reward/progression persistence.
11. Confirm selected operator/level/equipped weapon still persist.
12. Confirm Overview hologram still restores after tab switching.
13. Final Unity Console: **0 red runtime errors**. Device Simulator editor-state exceptions are a separate local editor/layout defect and must be cleared before counting the final console gate.

## Release decision
Only after current A + B are green again and C–E pass is Production 0.14 ready to leave Draft and proceed to final merge/release packaging. Do not reopen visual redesign unless a real blocking visual defect appears.
