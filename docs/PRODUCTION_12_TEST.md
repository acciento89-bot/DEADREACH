# DEADREACH — Production 0.12 Test Gate

Production 0.12 branches from the fully real-Unity-validated Production 0.11 `main` baseline.

## Goal

Turn the expedition map from one repeated street into a true sector system with alternate routes, sector-specific layouts, hazards, spawn geography and mission placement while preserving the complete 0.11 mission/risk-reward loop.

## Compile / build gate

1. Pull `production/0.12-sector-expansion`.
2. Let Unity finish compiling.
3. Require **0 red compiler errors**.
4. Run `DEADREACH > Build Production Slice 0.12`.
5. Require no blocking red generation/build errors.
6. Confirm the generated Dead City scene contains `Production_SectorNetwork_0_12` and `Production_SectorLayouts_0_12`.

## World expansion gate

The base Dead City now has a full east/west cross-street route in addition to the north/south main street.

Validate:
- player can travel from the central intersection to the west spur and back
- player can travel from the central intersection to the east spur and back
- expanded world-safety bounds do not pull the player back while using either spur
- outer world boundaries still prevent leaving the playable rectangle
- north extraction support from 0.11 remains solid and reversible
- camera follows cleanly across the wider map

## Sector archetypes

0.12 authors four complete sector layouts. Across multiple deployments, confirm the FIELD OPS sector line changes and the world geometry visibly matches the selected sector.

### QUARANTINE WARD
- FIELD OPS shows `QUARANTINE WARD`
- green/teal atmosphere is visible
- checkpoint barriers and quarantine containers alter the route silhouette
- east-side extraction is reachable
- contamination hazard is visible and damages the player only inside its trigger
- leaving the contamination zone stops hazard damage and clears the hazard warning

### TRANSIT COLLAPSE
- FIELD OPS shows `TRANSIT COLLAPSE`
- wrecked truck/cars materially change route choice
- west-side extraction is reachable
- electrical hazard is visible and pulses damage only while inside
- alternate route around the wreck cluster remains traversable

### INDUSTRIAL SPILL
- FIELD OPS shows `INDUSTRIAL SPILL`
- containers/pipes/barrels create a distinct channelled layout
- north extraction remains reachable
- chemical + fire hazards are both visible and distinct
- hazard zones never physically trap the CharacterController

### BLACKOUT PLAZA
- FIELD OPS shows `BLACKOUT PLAZA`
- darker violet/red emergency atmosphere is obvious
- route blockers differ from all other sectors
- east-side extraction is reachable
- arc + fire hazards are visible and readable during combat

## Sector-aware geography gate

For the active sector:
- player spawn uses the layout spawn anchor
- ExtractionZone and both extraction beacon presentations move to the layout extraction anchor
- ordinary infected reposition to layout enemy anchors
- existing Scrap/weapon loot reposition to layout loot anchors
- 0.11 Primary objective marker relocates to a sector objective anchor
- BLACKSITE vault stage relocates to the sector vault/objective anchor
- optional BLACK CACHE relocates to a distant sector objective anchor
- newly spawned Holdout / Blacksite / Black Cache reinforcements relocate to sector reinforcement anchors
- no objective, loot, enemy or extraction target spawns outside the supported world

## Hazard gameplay / HUD gate

- FIELD OPS includes a dedicated `SECTOR` line
- sector hazard profile is readable on mobile
- entering a hazard changes the sector line to a live danger state
- center warning shows `HAZARD // ... // MOVE CLEAR`
- hazard damage uses normal player damage feedback
- hazard warning clears after exit
- hazard presentation does not steal touch input
- hazard line/ring/light remains readable without covering the player or objective marker

## Sector risk/reward gate

Primary completion adds the sector risk bonus as additional unsecured Scrap:
- Quarantine Ward +4
- Transit Collapse +6
- Industrial Spill +8
- Blackout Plaza +10

BLACK CACHE weapon receives the sector Item Power bonus:
- Quarantine Ward +2
- Transit Collapse +3
- Industrial Spill +5
- Blackout Plaza +6

Validate both inventory paths:
- free run-inventory slot: carried weapon clone receives the Item Power bonus
- full run inventory: pending mission reward retains the bonus and banks it only after successful extraction
- death/abandon still loses unsecured mission reward state

## Production 0.11 mission regression

In at least one 0.12 sector run:
- Mission HUD appears
- Objective marker appears at sector-specific location
- entering extraction before Primary still shows `EXTRACTION SEALED`
- RECOVERY / PURGE / HOLDOUT / BLACKSITE logic remains unchanged
- Primary completion unlocks extraction
- BLACK CACHE optional path works
- reinforcement roles / 0.10 VFX remain intact
- successful extraction returns to Bunker

## Mobile regression

Use landscape Device Simulator / phone setup:
- MOVE remains fixed lower-left and full 360°
- AIM/FIRE remains fixed lower-right
- Ability remains independent upper-right
- larger FIELD OPS panel remains readable and does not cover control zones
- wider east/west routes are controllable without camera/input dead zones
- hazard warnings and objective alerts do not steal touch input

## Full regression

1. Bunker → Workshop present.
2. Arsenal orientation/framing intact.
3. Deploy into a 0.12 sector.
4. Traverse main street + at least one side spur.
5. Trigger and leave one hazard.
6. Complete Primary.
7. Complete BLACK CACHE or deliberately skip it.
8. Extract from the sector-specific extraction point.
9. Return to Bunker.
10. Workshop / progression persist.
11. Optional cache reward banks only after successful extraction.
12. Boss / reward / 0.10 combat impact presentation remains intact.
13. Unity Console ends with **0 red runtime errors**.

Production 0.12 remains Draft/unmerged until the full gate passes.
