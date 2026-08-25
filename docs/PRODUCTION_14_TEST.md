# DEADREACH — Production 0.14 Test Gate

Production 0.14 branches directly from the fully validated Production 0.12 `main` baseline. It does not inherit the rejected Production 0.13 presentation stack.

## Pass 1 scope — Premium Command Center reboot

Implemented:
- one runtime presentation owner: `Production14CommandCenterUI`
- no 0.13 UI layers
- premium header with separate SCRAP / EXTRACTS / BOSS KILLS modules
- six horizontal operations tabs
- native Overview / Arsenal / Operators / Campaign / Workshop / Supply screens
- mission console left, campaign console right, hero command-table center
- Quaternius authored rear Bunker architecture
- Ready / Deploy footer
- `DEADREACH > Build Production Slice 0.14` keeps accepted Production 0.12 sector/layout passes

## UI-art history / current direction

Accepted by the user:
- overall screen architecture / placement
- six-screen navigation concept
- command-table / hologram hero concept

Rejected visual attempts:
1. initial procedural industrial plates — too close to DEV/menu placeholder art
2. Devdog external HUD experiment — **VISUAL FAIL**; radial / partial HUD pieces stretched into rectangular panels created white wireframes, star/hex counters and a diagonal footer artifact
3. clean graphite fallback — **VISUAL FAIL**; technically clean but user verdict: **“Ist ja wie vorher....”**

Current implementation:
- coherent **Wenrexa “Assets: UI Minimalism SciFi” CC0** family selected
- source listing: `https://opengameart.org/content/assets-ui-minimalism-scifi`
- individual PNG mirror used by editor setup: `Bamjr/Delivery-Espacio-space-shooter-game/WenrexaAssetsUI_SciFI/PNG`
- only authored semantic families are mapped:
  - `MainPanel` -> main content cards
  - `SelectPanel` -> compact cards
  - `TitlePanel` -> header/footer strips
  - `Button` -> tabs / Deploy / tags
- no radial HUD graphics or partial HUD bars are used
- the pack is explicitly CC0 and its OpenGameArt listing states commercial/free use
- graphite generator remains only as an offline-safe fallback if the external pack cannot be prepared

## Hologram visibility correction

Latest user screenshot also showed an empty Overview center.

Root cause:
- leaving Overview disabled `P14_HoloDiorama`
- returning to Overview used `GameObject.Find`, which cannot find an inactive GameObject, so the hero stayed hidden

Fix implemented:
- `Production14HoloVisibilityGuard`
- resolves the hero including inactive scene objects
- rebuilds it if Overview is active and the root is unexpectedly missing
- keeps it hidden on non-Overview tabs and restores it on Overview

## Recovery checkpoint — COMPLETED ✅

Recovery commit:
- `6ed8bf5e292f3430300fcb98ce13641885e7a309` — `recovery: version complete validated Unity production assets`

Now permanently versioned:
- `Bunker_Hub.unity`
- `DeadCity_VerticalSlice.unity`
- Sam / Shaun / Matt production prefabs
- Rifle / SMG / Pistol / Shotgun production prefabs
- infected production prefabs
- production materials/controllers/volume
- `ProductionAssetCatalog.asset`
- Unity `.meta` GUIDs
- URP / package / ProjectSettings state

Build setup is hardened so validated operator / weapon prefabs are reused instead of destructively re-imported.

## Gate A — Fresh Unity compile / Wenrexa import — PENDING

Previously confirmed:
- post-recovery compile: **PASSED — 0 red Unity errors**
- Build Production Slice 0.14: **PASSED**
- six-screen navigation retest: **PASSED**

Current runtime/editor changes require a fresh compile:
1. pull latest branch
2. let Unity compile
3. editor setup should prepare nine Wenrexa CC0 panel/button sprites
4. expected log: `DEADREACH 0.14 Wenrexa UI pack READY: 9 CC0 panel/button sprites available.`
5. require **0 red Unity errors**

No scene rebuild is required for this runtime/editor UI pass.

## Gate B — Build Production Slice 0.14 — PASSED ✅

User-confirmed after recovery/build hardening. UI-only changes after that do not invalidate the scene-generation gate.

## Gate C — Overview visual acceptance — PENDING NEW WENREXA SCREENSHOT

The clean graphite screenshot is explicitly not accepted.

Next screenshot must show:
- actual Wenrexa panel/button artwork rather than the graphite fallback
- central command table / holographic city visible again
- no Devdog white wireframes / star counters / diagonal footer artifact
- accepted layout preserved
- overall result materially more like finished game UI than the prior DEV-style baseline

## Gate D — Navigation — PASSED ✅ / DEEP ACTIONS PENDING

User-confirmed:
- Overview -> Arsenal -> Operators -> Campaign -> Workshop -> Supply -> Overview all switch correctly

Still to validate later:
- Arsenal equip
- operator selection persistence
- campaign selection / footer refresh
- Workshop upgrades/calibration
- Supply interactions when wired
- Deploy

## Gate E — Stable 0.12 expedition regression — PENDING AFTER VISUAL ACCEPTANCE

After visual acceptance:
1. Deploy.
2. Validate fixed MOVE / AIM-FIRE / Ability controls.
3. Validate 0.12 sector layout/hazards.
4. Complete Primary and optional BLACK CACHE.
5. Extract and return to Bunker.
6. Confirm rewards/progression persistence.
7. Confirm final Unity Console has 0 red runtime errors.

Production 0.14 remains Draft/unmerged until visual direction, deeper interactions and stable expedition regression pass.
