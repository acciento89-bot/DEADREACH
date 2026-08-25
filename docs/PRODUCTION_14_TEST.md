# DEADREACH — Production 0.14 Test Gate

Production 0.14 branches directly from the fully validated Production 0.12 `main` baseline. It does not inherit the rejected Production 0.13 presentation stack.

## Pass 1 scope — Premium Command Center reboot

Implemented:
- one runtime presentation owner: `Production14CommandCenterUI`
- stable legacy Bunker canvas is allowed to initialize, then its presentation is removed when 0.14 takes ownership
- no 0.13 UI layers are used
- premium header with separate SCRAP / EXTRACTS / BOSS KILLS counter modules
- six segmented horizontal operations tabs
- native 0.14 Overview / Arsenal / Operators / Campaign / Workshop / Supply screens
- compact physical-style mission console on Overview
- compact campaign console on Overview
- central command-table / holographic city hero composition
- authored Quaternius `Door_Frame_A` / `Door_DarkMetal` rear Bunker architecture
- premium footer / Deploy action strip
- decorative UI is raycast-disabled
- hologram decoration has no gameplay colliders
- `DEADREACH > Build Production Slice 0.14` keeps the accepted 0.12 sector and layout passes in the build pipeline

Current UI-art direction after the latest screenshot fail:
- the temporary Devdog external HUD sprite experiment is **REJECTED**
- thin white wireframe / hexagonal frames are not part of the accepted direction
- `Production14IndustrialSkin` now owns a clean dark graphite baseline with restrained cyan / amber rails
- no external HUD sprite dependency is used by the runtime skin
- the editor cleanup removes the locally downloaded rejected Devdog `Resources/Production14/UI/External` folder after compile
- command-center layout and navigation geometry remain unchanged

Latest hero polish remains implemented:
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

## Gate A — Fresh Unity compile / asset cleanup — PENDING

Previously confirmed:
- post-recovery compile: **PASSED — 0 red Unity errors**
- six-screen navigation runtime compiled and entered Play Mode successfully

The latest UI-art correction changes runtime/editor C# again, so require one fresh compile:
- pull latest branch
- allow Unity to compile
- rejected external HUD folder should be removed automatically if it exists
- require **0 red Unity errors**

No scene rebuild is required solely for this runtime/editor UI correction.

## Gate B — Build Production Slice 0.14 — PASSED ✅

User-confirmed real-Unity result after recovery/build-gate hardening:
- validated production assets loaded
- old standalone weapon glTF repair path did not block generation
- `DEADREACH > Build Production Slice 0.14` completed
- Bunker scene reopened
- Play Mode reached the new 0.14 Overview

Later command-center UI/presentation-only changes do not invalidate the successful scene-generation gate.

## Gate C — Overview visual acceptance — FAILED AGAIN / RETEST PENDING

First 0.14 screenshot verdict: **“nah dran aber nicht ganz”**.

Accepted from the first screenshot:
- overall screen architecture and command-center composition
- separate resource counters
- left mission / right campaign console placement
- central hologram / command-table concept
- six-screen horizontal operations navigation

Latest external-asset experiment verdict from the user screenshot: **VISUAL FAIL**.
Observed failure:
- thin white HUD outlines dominated the interface
- counter cards became star/hex-like shapes
- footer produced a large diagonal white line
- mission / campaign frames read as wireframe placeholders
- the result looked less premium than the prior 0.14 baseline

Cause:
- unrelated Devdog HUD shapes were incorrectly repurposed as rectangular nine-slice panels
- a radial hexagon frame was used for resource counters
- a partial lower HUD bar was stretched across the footer

Correction implemented:
- external Devdog sprite lookup removed from the runtime skin
- auto-download bootstrap removed and replaced by local cleanup
- new clean dark graphite / cyan / amber panel baseline implemented without wireframe art, rivets or fake brushed-metal decoration
- layout itself was not changed

Visual acceptance remains pending until the user sees the corrected Overview in Play Mode.

Pass only when the screen clearly reads as a finished game command center rather than a DEV / wireframe UI.

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
