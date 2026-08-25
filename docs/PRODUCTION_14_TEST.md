# DEADREACH — Production 0.14 Test Gate

Production 0.14 branches directly from the fully validated Production 0.12 `main` baseline. It does not inherit the rejected Production 0.13 presentation stack.

## Pass 1 scope — Premium Command Center reboot

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
- post-screenshot navigation pass adds native 0.14 Arsenal / Operators / Campaign / Workshop / Supply content without resurrecting the legacy DEV dashboard

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

## Gate A — Fresh Unity compile / asset import — RETEST REQUIRED

The post-recovery compile was user-confirmed with **0 red Unity errors**. That result is now stale only for the newest runtime navigation/screen code added after the successful 0.14 build screenshot.

Current requirement:
- pull the latest branch
- allow Unity to compile the new 0.14 screen code
- require **0 red Unity errors**

No scene rebuild is required solely for this runtime UI navigation change.

## Gate B — Build Production Slice 0.14 — PASSED ✅

User-confirmed real-Unity result after recovery/build-gate hardening:
- validated production assets loaded
- old standalone weapon glTF repair path did not block generation
- `DEADREACH > Build Production Slice 0.14` completed
- Bunker scene reopened
- Play Mode reached the new 0.14 Overview

The later navigation code change is runtime UI only and does not invalidate the successful scene-generation gate.

## Gate C — Overview visual acceptance — NOT ACCEPTED YET

User screenshot verdict: **“nah dran aber nicht ganz”**.

Current positive direction:
- physical segmented header/nav language is substantially closer to the approved reference
- resource counters are separate modules
- left mission and right campaign consoles read more like game UI than the old DEV dashboard
- central hologram/command-table composition is visible

Still requires another visual polish pass before acceptance:
- center composition needs more authored richness / less sparse technical-block appearance
- overall screen still needs the final premium material/detail density of the reference

Pass only when the screen clearly reads as a finished premium command center rather than a stylized prototype.

## Gate D — Command-center interaction — FIX IMPLEMENTED / RETEST PENDING

Observed failure on the successful build screenshot:
- Arsenal / Operators / Campaign / Workshop / Supply buttons received clicks
- only the header title changed
- Overview content remained visible

Root cause confirmed in code: `HandleNav` intentionally treated Production 0.14 Pass 1 as Overview-only and never built another screen.

Fix now implemented on branch:
- dedicated `ScreenContent` layer is swapped per tab
- active nav visual state follows the selected tab
- hologram is visible only on Overview
- Arsenal opens a secured-inventory/loadout screen and can equip weapons
- Operators opens roster/details and can select the active operator
- Campaign opens sector/level selection and persists selected level
- Workshop opens bunker-system upgrades + equipped-weapon calibration
- Supply opens its own content screen
- legacy DEV dashboard is not restored

Retest after fresh compile:
1. click all six tabs
2. verify content actually changes on every tab
3. return to Overview and verify hologram returns
4. change Operator or Campaign level and verify footer updates
5. Arsenal equip / Workshop actions must remain clickable

## Gate E — Stable 0.12 regression — PENDING AFTER VISUAL ACCEPTANCE

Only after Overview and command-center interaction are accepted:
1. Deploy from the new command center.
2. Validate fixed MOVE / AIM-FIRE / Ability controls.
3. Validate selected 0.12 sector layout and hazards.
4. Complete Primary and optional BLACK CACHE.
5. Extract.
6. Return to Bunker.
7. Confirm rewards/progression persistence.
8. Confirm final Unity Console has 0 red runtime errors.

Production 0.14 remains Draft/unmerged until the visual direction, command-center interactions and stable expedition regression are accepted.
