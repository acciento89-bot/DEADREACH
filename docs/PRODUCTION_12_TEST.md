# DEADREACH — Production 0.12 Test Gate

Production 0.12 branches from the fully real-Unity-validated Production 0.11 `main` baseline.

## Goal

Turn the expedition map from one repeated street into a true sector system with alternate routes, sector-specific layouts, hazards, spawn geography and mission placement while preserving the complete 0.11 mission/risk-reward loop.

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

Real Unity runtime acceptance confirmed:
- FIELD OPS sector identity / BIOHAZARD presentation accepted
- green/teal atmosphere accepted
- central intersection → west spur → back accepted
- central intersection → east spur → back accepted
- no world-safety snap-back on the tested side routes
- east-side extraction reachable
- pre-Primary extraction remains sealed
- mission marker appears in the sector geography
- contamination hazard warning appears on entry
- contamination hazard damages only while inside
- hazard damage/warning clears after exit
- hazard / containers do not trap the CharacterController

This gives a first real-runtime pass for the expanded cross-street traversal, sector-specific extraction, mission geography and hazard enter/damage/exit behavior.

## TRANSIT COLLAPSE — NEXT

Validate:
- FIELD OPS shows `TRANSIT COLLAPSE`
- cold blue sector identity is obvious
- wrecked truck/cars materially change route choice
- alternate route around the wreck cluster remains traversable
- west-side extraction is reachable and reversible
- electrical hazard is visible and pulses damage only while inside
- leaving the electrical field stops damage and clears the hazard warning
- mission marker remains on supported sector geometry
- ordinary infected use sector enemy anchors
- runtime reinforcements arrive from sector reinforcement anchors

## INDUSTRIAL SPILL — PENDING

Validate:
- FIELD OPS shows `INDUSTRIAL SPILL`
- containers/pipes/barrels create a distinct channelled layout
- north extraction reachable
- chemical + fire hazards visible and distinct
- hazards never physically trap the CharacterController

## BLACKOUT PLAZA — PENDING

Validate:
- FIELD OPS shows `BLACKOUT PLAZA`
- dark violet/red emergency atmosphere
- route blockers differ from the other sectors
- east-side extraction reachable
- arc + fire hazards remain readable during combat

## Sector-aware geography gate

Still requires broader acceptance across the remaining sectors:
- player spawn uses layout spawn anchor
- ExtractionZone + both extraction beacon presentations move to layout extraction anchor
- ordinary infected reposition to enemy anchors
- Scrap/weapon loot reposition to loot anchors
- 0.11 Primary marker relocates to sector objective anchor
- BLACKSITE vault stage relocates to sector vault/objective anchor
- optional BLACK CACHE relocates to a distant sector objective anchor
- Holdout / Blacksite / cache reinforcements relocate to sector reinforcement anchors
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
