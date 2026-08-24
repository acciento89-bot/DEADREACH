# DEADREACH — Production 0.12 Test Gate

Production 0.12 branches from the fully real-Unity-validated Production 0.11 `main` baseline.

## Compile / build gate

1. Fresh real-Unity compile: **PASSED — 0 red compiler errors** ✅ 2026-08-24.
2. `DEADREACH > Build Production Slice 0.12`: **PASSED** ✅ 2026-08-24.
3. No blocking red generation/build errors reported ✅.

## Editor sector test override

Use:
- `DEADREACH > Dev > Sector 0.12 > AUTO`
- `DEADREACH > Dev > Sector 0.12 > QUARANTINE WARD`
- `DEADREACH > Dev > Sector 0.12 > TRANSIT COLLAPSE`
- `DEADREACH > Dev > Sector 0.12 > INDUSTRIAL SPILL`
- `DEADREACH > Dev > Sector 0.12 > BLACKOUT PLAZA`

The override applies on the next expedition scene load and is compiled out of player builds; release/mobile builds always use automatic sector selection.

## QUARANTINE WARD — PASSED 2026-08-24 ✅

Accepted in real Unity runtime:
- Q-WARD / BIOHAZARD identity and green/teal atmosphere
- west and east spur out-and-back traversal
- no tested side-route world-safety snap-back
- east extraction reachable
- pre-Primary extraction remains sealed
- mission marker works in expanded sector geography
- contamination warning/damage while inside
- warning and damage clear after exit
- no CharacterController trapping on tested hazard/container geometry

## TRANSIT COLLAPSE — PASSED 2026-08-24 ✅

Accepted in real Unity runtime:
- TRANSIT COLLAPSE sector identity and cold/blue presentation
- wreck cluster materially changes the route
- alternate path around the wrecks is traversable
- west-side extraction is reachable and reversible
- electrical hazard warning/damage works only while inside
- hazard damage and warning clear after leaving
- mission marker remains on supported sector geometry
- ordinary infected remain on normal enemy geography
- runtime reinforcements arrive from valid sector geography

## INDUSTRIAL SPILL — NEXT

Validate:
- FIELD OPS shows `INDUSTRIAL SPILL`
- amber industrial identity is obvious
- containers/pipes/barrels create a distinct channelled layout
- north extraction is reachable and reversible
- chemical and fire hazards are both visible and distinct
- each hazard damages only while inside and clears on exit
- neither hazard physically traps the CharacterController
- mission marker stays on supported geometry
- loot/enemy placement remains reachable

## BLACKOUT PLAZA — PENDING

Validate:
- FIELD OPS shows `BLACKOUT PLAZA`
- dark violet/red emergency atmosphere
- route blockers differ from the other sectors
- east-side extraction reachable
- arc + fire hazards remain readable during combat

## Sector-aware geography gate

Q-WARD and TRANSIT have real-runtime coverage. Still broaden across INDUSTRIAL / BLACKOUT:
- player spawn uses layout spawn anchor
- ExtractionZone + beacon presentation move to layout extraction anchor
- ordinary infected reposition to enemy anchors
- Scrap/weapon loot reposition to loot anchors
- Primary / BLACKSITE / BLACK CACHE markers stay on supported sector anchors
- Holdout / Blacksite / cache reinforcements use sector reinforcement geography
- ordinary Runner enemies are never mistaken for `_R##` reinforcements
- no target spawns outside supported world

## Sector risk/reward gate — PENDING

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

## Production 0.11 mission regression — PENDING FINAL PASS

- Mission HUD appears
- Objective marker appears at sector-specific location
- pre-Primary extraction still shows `EXTRACTION SEALED`
- RECOVERY / PURGE / HOLDOUT / BLACKSITE logic unchanged
- Primary unlocks extraction
- BLACK CACHE works
- reinforcement roles / 0.10 VFX intact
- successful extraction returns to Bunker

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
7. Complete Primary.
8. Complete BLACK CACHE or deliberately skip it.
9. Extract from sector-specific extraction point.
10. Return to Bunker.
11. Workshop/progression persist.
12. Optional cache reward banks only after successful extraction.
13. Boss/reward/0.10 combat-impact presentation intact.
14. Unity Console ends with **0 red runtime errors**.

Production 0.12 remains Draft/unmerged until the full gate passes.
