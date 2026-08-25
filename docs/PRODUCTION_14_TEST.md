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
- native 0.14 Overview / Arsenal / Operators / Campaign / Workshop / Supply screens
- compact physical-style mission console on Overview
- compact glass campaign console on Overview
- central command-table / holographic city hero composition
- authored Quaternius `Door_Frame_A` / `Door_DarkMetal` rear Bunker architecture
- premium footer / Deploy action strip
- decorative UI is raycast-disabled
- hologram decoration has no gameplay colliders
- `DEADREACH > Build Production Slice 0.14` keeps the accepted 0.12 sector and layout passes in the build pipeline

Latest hero polish implemented after the first screenshot:
- denser holographic city with district plates, roads, more varied buildings and four objective markers
- wider layered command table with front rail, side console wings and illuminated table edges
- separate projector pod with animated projector core
- rear command-wall consoles, emissive cyan displays and amber alert rails
- stronger Bunker ambient/fill lighting and a tighter cinematic camera
- slower subtler hologram animation so the center reads like a tactical display rather than a spinning prototype

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

## Gate A — Fresh Unity compile / asset import — RETEST REQUIRED AFTER HERO POLISH

Confirmed before the newest hero-presentation change:
- post-recovery compile: **PASSED — 0 red Unity errors**
- navigation/screen runtime change compiled and entered Play Mode successfully because all six screens were user-tested

The latest `Production14HoloDiorama` visual-polish commit changes runtime C# again, so require one fresh compile before visual judgment:
- pull latest branch
- allow Unity to compile
- require **0 red Unity errors**

No scene rebuild is required solely for this runtime hero-polish change.

## Gate B — Build Production Slice 0.14 — PASSED ✅

User-confirmed real-Unity result after recovery/build-gate hardening:
- validated production assets loaded
- old standalone weapon glTF repair path did not block generation
- `DEADREACH > Build Production Slice 0.14` completed
- Bunker scene reopened
- Play Mode reached the new 0.14 Overview

Later command-center changes are runtime UI/presentation only and do not invalidate the successful scene-generation gate.

## Gate C — Overview visual acceptance — NOT ACCEPTED YET / NEW POLISH PENDING SCREENSHOT

First 0.14 screenshot verdict: **“nah dran aber nicht ganz”**.

Accepted direction from that screenshot:
- physical segmented header/nav language is substantially closer to the approved reference
- resource counters are separate modules
- left mission and right campaign consoles read more like game UI than the old DEV dashboard
- central hologram/command-table composition is visible

Still missing in that screenshot:
- center composition was too sparse / technical
- overall 3D scene lacked the material/detail density of the approved reference

A new center/room polish pass is now implemented as listed above. Visual acceptance remains pending until the user sees the new Overview in Play Mode.

Pass only when the screen clearly reads as a finished premium command center rather than a stylized prototype.

## Gate D — Command-center navigation — PASSED ✅ / DEEP ACTIONS STILL PENDING

Initial failure:
- Arsenal / Operators / Campaign / Workshop / Supply buttons received clicks
- only the header title changed because `HandleNav` was intentionally Overview-only

Fix implemented:
- dedicated `ScreenContent` layer swaps per tab
- active nav visual state follows selected tab
- hologram is visible only on Overview
- native 0.14 Arsenal / Operators / Campaign / Workshop / Supply screens are built without restoring the legacy DEV dashboard

User-confirmed retest:
- **Overview → Arsenal → Operators → Campaign → Workshop → Supply → Overview all switch correctly**

Still to validate later:
- Arsenal equip action
- Operator selection persistence
- Campaign level selection + footer refresh
- Workshop upgrade/calibration actions
- Supply interactions when commerce is wired
- Deploy button

## Gate E — Stable 0.12 regression — PENDING AFTER VISUAL ACCEPTANCE

Only after Overview visual direction is accepted:
1. Deploy from the new command center.
2. Validate fixed MOVE / AIM-FIRE / Ability controls.
3. Validate selected 0.12 sector layout and hazards.
4. Complete Primary and optional BLACK CACHE.
5. Extract.
6. Return to Bunker.
7. Confirm rewards/progression persistence.
8. Confirm final Unity Console has 0 red runtime errors.

Production 0.14 remains Draft/unmerged until the visual direction, deeper command-center actions and stable expedition regression are accepted.
