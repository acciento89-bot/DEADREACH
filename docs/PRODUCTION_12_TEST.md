# DEADREACH — Production 0.12 Test Gate

Production 0.12 branches from the fully real-Unity-validated Production 0.11 `main` baseline.

## Goal

Turn the expedition map from one repeated street into a true sector system with alternate routes, sector-specific layouts, hazards, spawn geography and mission placement while preserving the complete 0.11 mission/risk-reward loop.

## Compile / build gate

1. Pull `production/0.12-sector-expansion`.
2. Let Unity finish compiling.
3. Fresh real-Unity compile: **PASSED — 0 red compiler errors** ✅ 2026-08-24.
4. Next: run `DEADREACH > Build Production Slice 0.12`.
5. Require no blocking red generation/build errors.
6. Confirm the generated Dead City scene contains `Production_SectorNetwork_0_12` and `Production_SectorLayouts_0_12`.

## Editor sector test override

To test all four layouts without waiting for automatic rotation, use:
- `DEADREACH > Dev > Sector 0.12 > AUTO`
- `DEADREACH > Dev > Sector 0.12 > QUARANTINE WARD`
- `DEADREACH > Dev > Sector 0.12 > TRANSIT COLLAPSE`
- `DEADREACH > Dev > Sector 0.12 > INDUSTRIAL SPILL`
- `DEADREACH > Dev > Sector 0.12 > BLACKOUT PLAZA`

The override applies on the next expedition scene load and is compiled out of player builds; release/mobile builds always use automatic sector selection.

## World expansion gate

Validate:
- central intersection → west spur → back
- central intersection → east spur → back
- expanded world-safety bounds do not pull the player back on side routes
- outer boundaries still stop leaving the playable rectangle
- north extraction support from 0.11 remains solid and reversible
- camera follows cleanly across the wider map

## Sector archetypes

Use the editor override for deterministic validation, then return it to `AUTO`.

### QUARANTINE WARD
- FIELD OPS shows `QUARANTINE WARD`
- green/teal atmosphere
- checkpoint barriers / quarantine containers alter route silhouette
- east-side extraction reachable
- contamination hazard damages only while inside and clears on exit

### TRANSIT COLLAPSE
- FIELD OPS shows `TRANSIT COLLAPSE`
- wrecked truck/cars materially change route choice
- west-side extraction reachable
- electrical hazard damages only while inside
- alternate route around wreck cluster remains traversable

### INDUSTRIAL SPILL
- FIELD OPS shows `INDUSTRIAL SPILL`
- containers/pipes/barrels create a distinct channelled layout
- north extraction reachable
- chemical + fire hazards visible and distinct
- hazards never physically trap the CharacterController

### BLACKOUT PLAZA
- FIELD OPS shows `BLACKOUT PLAZA`
- dark violet/red emergency atmosphere
- route blockers differ from the other sectors
- east-side extraction reachable
- arc + fire hazards remain readable during combat

## Sector-aware geography gate

For the active sector:
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

## Hazard gameplay / HUD gate

- FIELD OPS includes `SECTOR`
- sector hazard profile readable on mobile
- entering hazard switches to live danger state
- center warning shows `HAZARD // ... // MOVE CLEAR`
- hazard damage uses normal player damage feedback
- warning/damage stop after exit
- hazard presentation does not steal touch input

## Sector risk/reward gate

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

## Production 0.11 mission regression

- Mission HUD appears
- Objective marker appears at sector-specific location
- pre-Primary extraction still shows `EXTRACTION SEALED`
- RECOVERY / PURGE / HOLDOUT / BLACKSITE logic unchanged
- Primary unlocks extraction
- BLACK CACHE works
- reinforcement roles / 0.10 VFX intact
- successful extraction returns to Bunker

## Mobile regression

- MOVE fixed lower-left, full 360°
- AIM/FIRE fixed lower-right
- Ability independent upper-right
- enlarged FIELD OPS readable and outside control zones
- east/west routes controllable without camera/input dead zones
- hazard/objective alerts do not steal touch input

## Full regression

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
