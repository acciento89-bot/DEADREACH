# DEADREACH — Production 0.13 Test Gate

Production 0.13 branches from the fully real-Unity-validated Production 0.12 `main` baseline.

## Scope

Production 0.13 is the premium Bunker / menu art-direction pass.

### 0.13a — premium shell foundation
Implemented and real-Unity checked as an intermediate foundation:
- animated tactical UI backdrop / scan treatment
- premium corner-frame chrome
- graphite / cyan / amber palette
- upgraded header / navigation / content / deploy framing
- dynamic restyling for Overview / Arsenal / Operators / Campaign / Supply / Workshop
- first cinematic Bunker pass with command table, server banks, tactical wall and lighting
- `DEADREACH > Build Production Slice 0.13`

User result:
- fresh compile: **PASSED — 0 red compiler errors**
- Build Production Slice 0.13: **PASSED**
- visual acceptance: **REJECTED AS FINAL** — user correctly identified that this still read mainly as a decorated DEV menu and expected real authored game-art assets.

### 0.13b — real-asset command-center pass
Implemented after the 0.13a review:
- imported CC0 Quaternius Modular SciFi MegaKit geometry is now shipped inside the project
- genuine Quaternius `Door_Frame_A` and `Door_DarkMetal` model geometry is exposed through `Resources`
- new runtime `Production13RealAssetCommandCenter` instantiates a full rear blast-door assembly plus secondary authored bulkhead frames
- imported mesh renderers are re-materialed into the DEADREACH graphite / steel / cyan / amber palette
- real authored geometry is decorative only and its colliders are removed at runtime
- selected Kenney UI Pack: Sci-fi CC0 graphics are shipped as actual runtime UI textures
- Kenney plates are installed behind header / content / navigation / deploy UI
- the old nearly opaque tactical backdrop is disabled by the 0.13b layer
- Backdrop / ContentFrame / feature panels are made substantially more transparent so the 3D command center is visibly part of the menu composition
- all added UI graphics remain raycast-disabled
- third-party provenance / CC0 licenses are retained in `Assets/Deadreach/Art/Production13`

Because 0.13b adds runtime code and imported model/texture assets, all previous 0.13a compile/build results are **STALE for the current branch**.

## Gate A — Fresh Unity compile — PENDING AGAIN

After pulling the current 0.13b branch, let Unity finish importing the new OBJ / MTL / PNG assets and compiling.

Require:
- **0 red compiler errors**
- no blocking model-import errors
- no blocking missing-type / Resources / UI errors

## Gate B — Build Production Slice 0.13 — PENDING AGAIN

Only after Gate A passes, run:
`DEADREACH > Build Production Slice 0.13`

Require:
- stable 0.12 sector generation still completes
- accepted 0.12 layout polish still completes
- Production13BunkerScenePass completes
- Bunker scene reopens
- no blocking red generation errors

## Gate C — 0.13b real-asset visual acceptance — PENDING

At Bunker Overview validate specifically:
- the menu no longer reads primarily as a DEV / prototype screen
- the 3D Bunker is clearly visible through the UI rather than hidden behind an opaque full-screen treatment
- the central Quaternius blast-door / authored bulkhead silhouettes are visibly real model geometry rather than Unity cube construction
- Kenney sci-fi plates are visible in header / navigation / deploy treatment without covering text
- graphite / cyan / amber DEADREACH identity remains coherent
- text remains readable despite the more open 3D composition
- active navigation state remains obvious
- navigation and Deploy remain clickable
- real-asset geometry does not visually clip through the important foreground menu content

## Gate D — Screen-by-screen visual / interaction acceptance — PENDING

Validate every screen:
1. Overview
2. Arsenal
3. Operators
4. Campaign
5. Workshop
6. Supply Network

Require:
- 3D scene remains visible enough to make each screen feel like part of one command center
- no legacy opaque panel visually dominates the new art direction
- text hierarchy is clear on phone landscape
- Kenney / frame chrome never covers buttons or content
- scroll areas still scroll
- Arsenal equip actions still work
- Operator selection still works
- Campaign level selection still works
- Workshop upgrade / calibration / salvage interactions still work
- Supply content remains non-pay-to-win presentation

## Gate E — Mobile / responsive — PENDING

Require:
- landscape safe area remains correct
- header, nav, content and deploy zones do not overlap
- touch targets remain usable
- imported 3D scenery cannot intercept UI touches
- decorative Kenney graphics have raycast disabled
- navigation / deploy / Workshop / Arsenal interactions receive touches normally

## Gate F — Full regression — PENDING

1. Bunker opens with the accepted 0.13b real-asset command-center presentation.
2. Workshop and Arsenal state remain present.
3. Select operator / weapon / campaign level.
4. Deploy.
5. 0.12 sector selection / hazards / missions remain intact.
6. Complete Primary and optional BLACK CACHE.
7. Extract successfully.
8. Return to the premium Bunker.
9. Secured rewards / progression persist.
10. 0.10 combat-impact / boss / reward presentation remains intact.
11. Unity Console ends with **0 red runtime errors**.

Production 0.13 remains Draft/unmerged until all current 0.13b real-Unity gates pass.
