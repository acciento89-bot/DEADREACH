# DEADREACH — Production 0.14 Test Gate

Production 0.14 branches directly from the fully validated Production 0.12 `main` baseline. It does not inherit the rejected Production 0.13 presentation stack.

## Pass 1 scope — Premium Overview / Command Center reboot

Implemented:
- one runtime presentation owner: `Production14CommandCenterUI`
- stable legacy Bunker canvas is allowed to initialize, then its presentation is removed when 0.14 takes ownership
- no 0.13 UI layers are used
- runtime-generated industrial nine-slice skin with brushed gunmetal, clipped corners, bevel rails, rivets and restrained cyan/amber accents
- premium header with separate SCRAP / EXTRACTS / BOSS KILLS counter modules
- six segmented horizontal operations tabs
- compact physical-style mission console on the left
- compact glass campaign console on the right
- large unobstructed center composition
- central animated tactical hologram / city diorama with objective markers and command-table base
- lower cinematic Bunker camera aimed through the hero center
- premium footer / Deploy action strip
- decorative UI is raycast-disabled
- hologram decoration has no gameplay colliders
- `DEADREACH > Build Production Slice 0.14` keeps the accepted 0.12 sector and layout passes in the build pipeline

Pass 1 deliberately validates Overview before the remaining tabs are rebuilt. Arsenal / Operators / Campaign / Workshop / Supply are visually marked pending and must not fall back to the legacy DEV dashboard.

## Gate A — Fresh Unity compile — PENDING

After pulling the current branch:
- allow asset database / script compilation to finish
- require **0 red compiler errors**

Do not run the 0.14 build until this gate passes.

## Gate B — Build Production Slice 0.14 — PENDING

Run:
`DEADREACH > Build Production Slice 0.14`

Require:
- accepted base scene generation completes
- accepted Production 0.12 sector world pass completes
- accepted Production 0.12 layout polish completes
- Bunker scene reopens
- no blocking red generation error

## Gate C — Overview visual acceptance — PENDING

In Play Mode validate the Overview against the approved art-direction reference:
- no left-side DEV navigation
- no large flat prototype dashboard filling the screen
- header reads as physical / industrial sci-fi UI rather than plain rectangles
- resource counters are separate modules
- navigation reads as six segmented metal tabs
- mission panel reads as a physical console/card, not a flat colored rectangle
- campaign status is compact and subordinate
- center is the visual hero area
- command table + holographic city/map are clearly visible in the center
- 3D Bunker remains readable behind the UI
- footer / Deploy reads as a premium action console
- restrained gunmetal + cyan + amber palette
- no 0.13 layered UI artifacts

Pass only if the screen clearly reads as a finished game command center rather than a DEV menu.

## Gate D — Pass 1 interaction sanity — PENDING

Require:
- Overview remains readable in landscape
- Deploy button is clickable and loads the expedition
- decorative UI does not consume touches
- hologram colliders cannot block gameplay/navigation
- non-Overview tabs do not expose the old DEV dashboard during Pass 1

## Gate E — Stable 0.12 regression — PENDING AFTER VISUAL ACCEPTANCE

Only after Overview is accepted:
1. Deploy from the new command center.
2. Validate fixed MOVE / AIM-FIRE / Ability controls.
3. Validate selected 0.12 sector layout and hazards.
4. Complete Primary and optional BLACK CACHE.
5. Extract.
6. Return to Bunker.
7. Confirm rewards/progression persistence.
8. Confirm final Unity Console has 0 red runtime errors.

Production 0.14 remains Draft/unmerged until the current Pass 1 visual direction is accepted and follow-up screen passes are completed.
