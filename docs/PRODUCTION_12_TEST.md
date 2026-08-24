# DEADREACH — Production 0.12 Test Gate

Production 0.12 branches from the fully real-Unity-validated Production 0.11 `main` baseline.

## Current validation state

- Q-WARD runtime gate: **PASSED / remains accepted** — the 0.12b polish pass does not touch Q-WARD.
- Fresh Unity compile after 0.12b: **PASSED — 0 red compiler errors** ✅ 2026-08-24.
- `DEADREACH > Build Production Slice 0.12` after 0.12b: **PASSED** ✅ 2026-08-24.
- TRANSIT COLLAPSE after 0.12b declutter: **PASSED** ✅ 2026-08-24.
- INDUSTRIAL SPILL after 0.12b declutter: **PASSED** ✅ 2026-08-24.
- BLACKOUT PLAZA after 0.12b declutter: **PASSED** ✅ 2026-08-24.
- All four sector layouts now have accepted real-Unity runtime coverage.
- sector reward gate: pending.
- fixed-zone mobile regression: pending.
- full Bunker → Workshop/Arsenal → Deploy → mission/risk-reward → extraction → Bunker regression: pending.
- final Unity Console 0 red runtime errors: pending.

## 0.12b layout polish — ACCEPTED

`Production12LayoutPolishPass` runs after the normal sector scene pass. Q-WARD is intentionally unchanged.

### TRANSIT COLLAPSE
- large wreck truck / sports wreck / pickup / barrier moved toward route edges
- HOLDOUT arena moved to a deliberately open central point
- RECOVERY / BLACKSITE / PURGE anchors redistributed to clearer authored spaces
- electrical hazard moved away from the main objective circle
- moved/rotated prop collision bounds are recalculated after their final pose
- real-Unity runtime revalidation: **PASSED**

### INDUSTRIAL SPILL
- central barrels moved toward the channel edges
- service truck pushed farther onto the east spur
- north barrier moved off the center lane
- HOLDOUT / RECOVERY / BLACKSITE / PURGE anchors redistributed
- chemical / fire hazards separated from mission-marker lanes
- moved/rotated prop collision bounds are recalculated after their final pose
- real-Unity runtime revalidation: **PASSED**

### BLACKOUT PLAZA
- strongest declutter pass
- both wrecked vehicles moved to opposite lane edges
- barriers moved outward
- HOLDOUT marker moved to a large open central plaza
- BLACKSITE / RECOVERY / PURGE anchors redistributed
- nearby loot/enemy spawn points moved out of the holdout circle
- ARC hazard moved west; FIRE hazard moved north-east
- emergency lights follow the new hazard positions
- moved/rotated prop collision bounds are recalculated after their final pose
- real-Unity runtime revalidation: **PASSED**

## Q-WARD accepted coverage

The existing Q-WARD real-Unity pass remains accepted because 0.12b modifies no Q-WARD object or anchor:
- west/east spur traversal
- east extraction
- pre-Primary extraction seal
- mission marker geography
- contamination hazard enter/damage/exit
- no CharacterController trapping

## Sector risk/reward gate — NEXT

Primary completion additional unsecured Scrap:
- Quarantine +4
- Transit +6
- Industrial +8
- Blackout +10

BLACK CACHE Item Power bonus:
- Quarantine +2
- Transit +3
- Industrial +5
- Blackout +6

Validate both reward paths:
- free run-inventory slot: carried clone receives bonus
- full inventory: pending reward retains bonus and banks only after successful extraction
- death/abandon still loses unsecured state

## Mobile regression — PENDING

- MOVE fixed lower-left, full 360°
- AIM/FIRE fixed lower-right
- Ability independent upper-right
- enlarged FIELD OPS readable and outside control zones
- east/west routes controllable without camera/input dead zones
- hazard/objective alerts do not steal touch input

## Full regression — PENDING

1. Return sector override to `AUTO`.
2. Bunker → Workshop present.
3. Arsenal orientation/framing intact.
4. Deploy into a 0.12 sector.
5. Traverse main street + one side spur.
6. Trigger and leave one hazard.
7. Complete Primary and verify the sector Scrap bonus.
8. Complete BLACK CACHE and verify the sector Item Power bonus.
9. Extract from sector-specific extraction point.
10. Return to Bunker.
11. Workshop/progression persist.
12. Optional cache reward banks only after successful extraction.
13. Boss/reward/0.10 combat-impact presentation intact.
14. Unity Console ends with **0 red runtime errors**.

Production 0.12 remains Draft/unmerged until the reward, mobile and full regression gates pass.
