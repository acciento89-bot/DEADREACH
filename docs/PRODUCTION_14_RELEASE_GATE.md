# DEADREACH — Production 0.14 One-Pass Release Gate

Goal: finish Production 0.14 in one consolidated validation run instead of returning to small isolated checks.

## A — Fresh compile
1. Pull latest `production/0.14-premium-command-center`.
2. Let Unity import/compile.
3. Require **0 red compile errors**.
4. Wenrexa setup may log: `DEADREACH 0.14 Wenrexa UI pack READY: 9 CC0 panel/button sprites available.`

## B — Static release validator
Run:
`DEADREACH > Validate Production 0.14 Release Readiness`

Required result:
`DEADREACH 0.14 RELEASE STATIC CHECK: PASS`

This checks:
- Bunker and expedition scene assets
- both scenes enabled in Build Settings
- required Quaternius command-center Resources
- Wenrexa preparation/fallback availability
- landscape-only orientation compatibility

## C — Command Center deep interaction sweep
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
2. Validate fixed MOVE / AIM-FIRE / Ability controls.
3. Validate active 0.12 sector layout and its hazard behavior.
4. Complete Primary objective.
5. Complete optional BLACK CACHE when available.
6. Confirm objective-gated extraction.
7. Extract.
8. Return to Bunker.
9. Confirm Scrap/reward/progression persistence.
10. Confirm selected operator/level/equipped weapon still persist.
11. Confirm Overview hologram still restores after tab switching.
12. Final Unity Console: **0 red runtime errors**.

## Release decision
If A–E pass, Production 0.14 is ready to leave Draft and proceed to final merge/release packaging. Do not reopen visual redesign unless a real blocking visual defect appears.
