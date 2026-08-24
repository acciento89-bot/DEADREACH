# DEADREACH — Production 0.12 Test Gate

Production 0.12 branches from the fully real-Unity-validated Production 0.11 `main` baseline.

## Current validation state

- Q-WARD runtime gate: **PASSED / remains accepted** — the 0.12b polish pass does not touch Q-WARD.
- TRANSIT COLLAPSE: previous runtime pass is **superseded** by the new declutter geometry and must be revalidated.
- INDUSTRIAL SPILL: previous runtime pass is **superseded** by the new declutter geometry and must be revalidated.
- BLACKOUT PLAZA: first runtime layout was rejected because vehicle / marker / hazard spacing was too crowded; must be revalidated after 0.12b polish.
- fresh Unity compile: **required again** because build/editor code changed.
- `DEADREACH > Build Production Slice 0.12`: **required again** because the generated scene now runs the layout-polish pass.

## 0.12b layout polish

`Production12LayoutPolishPass` runs after the normal sector scene pass. Q-WARD is intentionally unchanged.

### TRANSIT COLLAPSE
- large wreck truck / sports wreck / pickup / barrier moved toward route edges
- HOLDOUT arena moved to a deliberately open central point
- RECOVERY / BLACKSITE / PURGE anchors redistributed to clearer authored spaces
- electrical hazard moved away from the main objective circle
- moved/rotated prop collision bounds are recalculated after their final pose

### INDUSTRIAL SPILL
- central barrels moved toward the channel edges
- service truck pushed farther onto the east spur
- north barrier moved off the center lane
- HOLDOUT / RECOVERY / BLACKSITE / PURGE anchors redistributed
- chemical / fire hazards separated from mission-marker lanes
- moved/rotated prop collision bounds are recalculated after their final pose

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

## Exact revalidation order

1. pull latest `production/0.12-sector-expansion`
2. fresh Unity compile → require **0 red compiler errors**
3. run `DEADREACH > Build Production Slice 0.12`
4. require no blocking red generation/build errors
5. TRANSIT COLLAPSE runtime recheck
6. INDUSTRIAL SPILL runtime recheck
7. BLACKOUT PLAZA runtime recheck
8. return sector override to `AUTO`
9. sector reward gate
10. fixed-zone mobile regression
11. full Bunker → Workshop/Arsenal → Deploy → mission/risk-reward → extraction → Bunker regression
12. Unity Console ends with **0 red runtime errors**

## Runtime sector criteria

For each of TRANSIT / INDUSTRIAL / BLACKOUT:
- sector identity / atmosphere is correct
- main route and side route are clearly traversable
- vehicles / containers / barriers do not make objective circles feel cramped
- objective marker is on supported, reachable ground with useful combat space around it
- sector extraction is reachable, reversible and still correctly sealed before Primary
- hazard warning/damage works only while inside and clears on exit
- no hazard or prop physically traps the CharacterController
- loot / ordinary infected remain on reachable geography
- runtime reinforcements arrive from valid sector anchors
- 0.10 combat-impact VFX remain intact

## Q-WARD accepted coverage

The existing Q-WARD real-Unity pass remains accepted because 0.12b modifies no Q-WARD object or anchor:
- west/east spur traversal
- east extraction
- pre-Primary extraction seal
- mission marker geography
- contamination hazard enter/damage/exit
- no CharacterController trapping

## Sector risk/reward gate — pending

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

Production 0.12 remains Draft/unmerged until the full gate passes.
