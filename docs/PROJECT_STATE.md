# DEADREACH — Project State

_Last updated: 2026-08-25_

Canonical handoff for DEADREACH. Update after major implementation, validation and merge.

## Product / stable baseline

- Game: DEADREACH
- Studio: Kamilunavo
- Repository: `acciento89-bot/DEADREACH`
- Platforms: iOS + Android
- Unity: `6000.3.22f1`
- URP: 17.3
- Bundle ID: `de.kamilunavo.deadzone`
- Mobile: landscape only
- stable branch: `main`
- stable production level: **0.12**
- Production 0.12 / PR #12 squash merge: `2d328868a6510cb744cff65c7c547cd8148c448e`
- active branch: `production/0.14-premium-command-center`
- active PR: #14 Draft

Permanent firearm rule:
- use artist-authored firearm geometry already parented to the Quaternius survivor rig
- derive muzzle from that embedded firearm
- never reintroduce the failed external hand-mounted Rifle transform path

## Production 0.14 — Premium Command Center Reboot — ACTIVE

### Stable accepted foundation

- branches directly from validated Production 0.12 `main`
- no rejected 0.13 presentation stack
- `Production14CommandCenterUI` owns one command-center presentation
- six native screens: Overview / Arsenal / Operators / Campaign / Workshop / Supply
- user-confirmed six-tab switch test: **PASSED**
- `DEADREACH > Build Production Slice 0.14`: **PASSED after recovery**
- overall layout / composition direction accepted; final visual art is not yet accepted

### Current screen architecture

- premium header with SCRAP / EXTRACTS / BOSS KILLS modules
- six horizontal operations tabs
- mission console left
- campaign console right
- central 3D command-table / holographic city hero
- Ready / Deploy footer
- Quaternius authored rear Bunker architecture
- hologram only shown on Overview

### UI-art decisions

Rejected:
- first procedural industrial plates: too DEV/placeholder-like
- Devdog HUD sprite experiment: **VISUAL FAIL** — wrong radial/partial HUD pieces produced white wireframes, star/hex counters and diagonal footer artifact
- clean graphite fallback: **VISUAL FAIL** — technically clean but user verdict **“Ist ja wie vorher....”**

Current pass:
- **Wenrexa “Assets: UI Minimalism SciFi” CC0** selected as a coherent UI family
- original source: `https://opengameart.org/content/assets-ui-minimalism-scifi`
- individual PNG mirror: `Bamjr/Delivery-Espacio-space-shooter-game/WenrexaAssetsUI_SciFI/PNG`
- editor setup downloads only semantically matching authored components:
  - `MainPanel` for main cards
  - `SelectPanel` for compact cards
  - `TitlePanel` for strips
  - `Button` for tabs / Deploy / tags
- no radial HUD or partial HUD art is mapped into rectangular panels
- runtime `Production14IndustrialSkin` prefers the Wenrexa sprites and uses the graphite generator only as offline fallback
- license/provenance notice is written into the prepared Resources folder

### Hologram visibility bug — FIX IMPLEMENTED

Latest clean-baseline screenshot showed an empty center.

Root cause:
- non-Overview tab disabled `P14_HoloDiorama`
- return path used `GameObject.Find`, which cannot resolve inactive objects

Fix:
- new `Production14HoloVisibilityGuard`
- resolves inactive scene hero roots
- restores the hero when Overview becomes active
- rebuilds the hero if it is unexpectedly missing
- keeps it hidden on other tabs

### Overview hero / room implementation

`Production14HoloDiorama` currently provides:
- projected city with district plates, roads, 18 varied buildings and four objective markers
- layered command table, front rail and side console wings
- projector pod with animated core/rings
- rear command-wall consoles with cyan screens / amber alert rails
- Bunker ambient/fill lighting and cinematic camera framing
- subtle hologram animation

### Recovery / reproducibility checkpoint — COMPLETE ✅

Recovery commit:
- `6ed8bf5e292f3430300fcb98ce13641885e7a309`

Versioned after recovery:
- `Assets/Deadreach/Scenes/Bunker_Hub.unity`
- `Assets/Deadreach/Scenes/DeadCity_VerticalSlice.unity`
- production Sam / Shaun / Matt prefabs
- production Rifle / SMG / Pistol / Shotgun prefabs
- infected prefabs
- materials/controllers/volume profile
- `ProductionAssetCatalog.asset`
- Unity `.meta` GUIDs
- URP / package lock / ProjectSettings

Build hardening:
- 0.5 operator gate reuses validated production prefabs
- 0.6 weapon gate reuses validated production prefabs
- old standalone glTF repair no longer blocks an already-valid production build

### Current real-Unity validation

- recovery committed + verified: **PASSED**
- post-recovery compile: **PASSED — 0 red Unity errors**
- Build Production Slice 0.14: **PASSED**
- Bunker / Play Mode Overview: **PASSED before latest UI changes**
- six-screen navigation: **PASSED**
- Devdog external-HUD art: **VISUAL FAIL**
- clean graphite art: **VISUAL FAIL — “Ist ja wie vorher....”**
- Wenrexa CC0 art integration: **IMPLEMENTED / FRESH COMPILE PENDING**
- Holo visibility guard: **IMPLEMENTED / FRESH COMPILE PENDING**
- new Overview visual screenshot: **PENDING**
- deeper per-screen actions: **PENDING**
- Deploy interaction: **PENDING**
- mobile landscape / safe-area: **PENDING**
- full stable 0.12 expedition regression: **PENDING**
- final Unity Console 0 red runtime errors: **PENDING**

## Production 0.12 — Sector Expansion — STABLE ✅

Final accepted real-Unity state:
- fresh compile: PASSED
- Production Slice 0.12 build: PASSED
- QUARANTINE WARD: PASSED
- TRANSIT COLLAPSE: PASSED
- INDUSTRIAL SPILL: PASSED
- BLACKOUT PLAZA: PASSED
- sector reward / BLACK CACHE behavior: PASSED
- fixed-zone mobile controls: PASSED
- full Bunker -> expedition -> extraction -> Bunker regression: PASSED
- Workshop/progression persistence: PASSED
- combat-impact / boss / reward presentation regression: PASSED
- final Unity Console: 0 red runtime errors

Stable systems remain authoritative:
- four 0.12 sectors + hazards
- 0.11 mission families / objective-gated extraction / BLACK CACHE risk-reward
- reinforcement waves
- schema-v6 Workshop progression
- fixed MOVE / AIM-FIRE / Ability controls
- accepted combat-impact VFX and boss/reward behavior

## Next exact gate

Pull latest `production/0.14-premium-command-center` and let Unity compile. Confirm the Wenrexa setup log and **0 red errors**. No Production Slice rebuild is required for this runtime/editor UI pass. Then enter Play Mode on Overview and capture a screenshot verifying both the new panel/button art and the restored central hologram.

Test plan: `docs/PRODUCTION_14_TEST.md`
