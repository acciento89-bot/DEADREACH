# DEADREACH — Production 0.13 Test Gate

Production 0.13 branches from the fully real-Unity-validated Production 0.12 `main` baseline.

## Scope

Production 0.13 is the premium Bunker / menu art-direction pass.

### 0.13a — premium shell foundation — SUPERSEDED

Real-Unity result:
- fresh compile: **PASSED — 0 red compiler errors**
- Build Production Slice 0.13: **PASSED**
- visual acceptance: **FAILED AS FINAL** — presentation still read mainly as a decorated DEV menu.

### 0.13b — real-asset command-center pass — VISUAL FAIL

0.13b added genuine CC0 Quaternius geometry and Kenney UI assets, horizontal navigation and a transparent 3D hero composition.

Real-Unity result:
- fresh compile + asset import: **PASSED — 0 red compiler errors**
- Build Production Slice 0.13: **PASSED**
- Overview screenshot review: **FAILED**

Observed visual problems from the real Unity screenshot:
- multiple style/layout layers were stacked over the original Bunker UI
- large cyan/translucent panels dominated the screen
- 3D background read as dark intersecting shapes rather than authored environment
- doubled/too-near blast-door geometry produced poor silhouettes
- Kenney surface treatment made the UI busier rather than more premium
- composition still resembled a dashboard / DEV screen instead of a finished game menu

The 0.13b visual result is not accepted and must not be merged.

### 0.13c — single-system command-center rework — IMPLEMENTED / VALIDATION ACTIVE

The 0.13c runtime rework replaces the layered approach instead of adding another layer:
- `Production13PremiumBunkerUI` is disabled when the 0.13c command-center component takes ownership
- old `P13_` / `P13B_` UI decoration is removed from the active UI tree
- Kenney plates are no longer used in the active presentation
- primitive 0.13 scene-dressing root is hidden at runtime
- central old DEV command-table / workshop / generator / primitive blast-door sightline props are hidden
- one genuine Quaternius `Door_Frame_A` + one `Door_DarkMetal` form the rear focal blast-door assembly
- secondary Quaternius frames provide side bulkhead silhouettes without filling the screen
- camera moves lower and more cinematic toward the rear blast door instead of the previous top-down DEV view
- fog / ambient lighting is reduced and rebalanced for readable authored geometry
- active UI uses near-black glass panels with restrained cyan / amber line accents rather than cyan-filled surfaces
- horizontal Operations navigation remains, but with smaller dark buttons and a thin active-state rail
- Overview removes the static Bunker Intel block entirely and leaves a large unobstructed 3D center field
- mission information becomes a compact left console; campaign status becomes a compact right console
- Deploy remains a dedicated bottom action strip
- decorative UI remains raycast-disabled
- imported geometry colliders remain removed

## Gate A — Fresh Unity compile — PASSED ✅

User-confirmed real Unity result on 2026-08-24:
- **0 red compiler errors**
- no blocking Resources / UI compile errors reported

## Gate B — Build Production Slice 0.13 — PENDING

Run:
`DEADREACH > Build Production Slice 0.13`

Require:
- stable 0.12 sector generation completes
- accepted 0.12 layout polish completes
- Production13BunkerScenePass completes
- Bunker scene reopens
- no blocking red generation errors

## Gate C — 0.13c Overview visual acceptance — PENDING

At Bunker Overview validate specifically:
- no large turquoise/cyan filled panels
- no visible stacked 0.13a / 0.13b chrome
- no Kenney plates in the active screen
- rear Quaternius blast door is the clear 3D focal point
- no doubled/intersecting black door geometry
- 3D Bunker is readable, not nearly black
- Overview center remains substantially open
- mission console is compact on the left
- campaign console is compact on the right
- navigation is clean, dark and readable
- Deploy remains obvious without looking like a DEV button
- text remains readable at phone-landscape scale

## Gate D — Screen-by-screen visual / interaction acceptance — PENDING

Validate:
1. Overview
2. Arsenal
3. Operators
4. Campaign
5. Workshop
6. Supply Network

Require:
- no legacy style layer reappears after tab changes
- dark-glass styling remains consistent
- list / inspector layouts remain readable
- scroll areas still scroll
- Arsenal equip works
- Operator selection works
- Campaign selection works
- Workshop upgrade / calibration / salvage works
- Supply interactions remain intact

## Gate E — Mobile / responsive — PENDING

Require:
- landscape safe area remains correct
- header / navigation / content / deploy do not overlap
- touch targets remain usable
- decorative UI cannot intercept touches
- imported 3D scenery cannot intercept UI touches

## Gate F — Full regression — PENDING

1. Bunker opens with accepted 0.13c presentation.
2. Workshop and Arsenal state remain present.
3. Select operator / weapon / campaign level.
4. Deploy.
5. 0.12 sector selection / hazards / missions remain intact.
6. Complete Primary and optional BLACK CACHE.
7. Extract successfully.
8. Return to Bunker.
9. Secured rewards / progression persist.
10. 0.10 combat-impact / boss / reward presentation remains intact.
11. Unity Console ends with **0 red runtime errors**.

Production 0.13 remains Draft/unmerged until all current 0.13c real-Unity gates pass.
