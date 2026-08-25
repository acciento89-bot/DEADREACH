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
- authored Quaternius `Door_Frame_A` / `Door_DarkMetal` geometry is loaded by Production 0.14 for the Bunker rear architecture
- `DEADREACH > Build Production Slice 0.14` keeps the accepted 0.12 sector and layout passes in the build pipeline

Pass 1 deliberately validates Overview before the remaining tabs are rebuilt. Arsenal / Operators / Campaign / Workshop / Supply are visually marked pending and must not fall back to the legacy DEV dashboard.

## Recovery checkpoint — COMPLETED ✅

A local-clean operation exposed that the generated Unity scenes, production prefabs/materials, GUID `.meta` files and project settings had never been versioned. They were recovered from the pre-0.14 safety stash and committed to the branch in:
- `6ed8bf5e292f3430300fcb98ce13641885e7a309` — `recovery: version complete validated Unity production assets`

GitHub verification confirms:
- `Assets/Deadreach/Scenes/Bunker_Hub.unity` is versioned
- `Assets/Deadreach/Scenes/DeadCity_VerticalSlice.unity` is versioned
- validated Sam / Shaun / Matt production prefabs are versioned
- validated Rifle / SMG / Pistol / Shotgun production prefabs are versioned
- `ProductionAssetCatalog.asset` is versioned and references the recovered prefab GUIDs
- Unity `.meta` GUIDs and ProjectSettings are now versioned

Build setup was also hardened before the recovery commit:
- 0.5 operator setup reuses validated production prefabs instead of destructively rebuilding them when already present
- 0.6 weapon-family setup reuses validated production prefabs instead of re-downloading/re-importing standalone glTF files when already present

## Gate A — Fresh Unity compile / asset import — PASSED ✅

Current user-confirmed real-Unity result after the recovery commit and 0.5 / 0.6 build-gate hardening:
- recovered production assets loaded
- fresh asset import completed
- fresh script compile completed
- **0 red Unity errors**

## Gate B — Build Production Slice 0.14 — PENDING RETRY

The first attempt failed in the old 0.6 weapon-family import path before scene generation completed. That path has now been hardened to reuse the recovered validated prefabs.

Run:
`DEADREACH > Build Production Slice 0.14`

Require:
- validated operator / weapon prefabs are reused without standalone repair glTF import failures
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
- authored Quaternius Bunker architecture is visible in the rear composition
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
