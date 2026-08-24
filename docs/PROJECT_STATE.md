# DEADREACH — Project State

_Last updated: 2026-08-24_

Canonical handoff for DEADREACH. Update after every major implementation, validation and merge.

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
- active production branch: `production/0.13-premium-bunker-ui`
- Production 0.13 state: **0.13b visually rejected; 0.13c single-system command-center rework implemented; fresh compile PASSED; Build 0.13 next**

Permanent firearm rule:
- use artist-authored firearm geometry already parented to the Quaternius survivor rig
- derive muzzle from that embedded firearm
- never reintroduce the failed external hand-mounted Rifle transform path

## Production 0.13 — Premium Bunker UI / Command Center Overhaul — ACTIVE

### Art-direction target
- finished premium tactical extraction-game menu, not a DEV dashboard
- authored 3D environment must read clearly through the UI
- near-black / steel base with restrained cold cyan and warning amber accents
- strong hierarchy without large colored panel fills
- mobile-landscape readability and touch safety remain mandatory

### 0.13a — superseded
- premium style shell compiled and built successfully
- final visual result rejected because it remained an embellished DEV menu

### 0.13b — real assets added, visual result rejected
Implemented:
- CC0 Quaternius Modular SciFi MegaKit `Door_Frame_A` and `Door_DarkMetal`
- CC0 Kenney UI Pack: Sci-fi runtime textures
- horizontal Operations navigation
- more transparent UI / visible 3D center composition

Real-Unity result:
- fresh compile + imported assets: **PASSED — 0 red compiler errors**
- Build Production Slice 0.13: **PASSED**
- Overview screenshot: **VISUAL FAIL**

Observed in screenshot:
- old Bunker UI + 0.13a + 0.13b were visibly fighting each other
- cyan translucent blocks dominated the presentation
- 3D scene was too dark and visually intersected
- blast-door geometry was doubled / too close to camera
- Kenney chrome increased visual noise
- result still read as a dashboard rather than a premium game screen

0.13b is not acceptable for merge.

### 0.13c — single-system command-center rework — IMPLEMENTED

Runtime ownership:
- `Production13RealAssetCommandCenter` is now the sole 0.13 presentation owner
- existing `Production13PremiumBunkerUI` component is disabled by 0.13c
- active `P13_` / `P13B_` UI decoration is removed
- Kenney graphics remain licensed in the repository but are not active in the 0.13c visual presentation

3D command center:
- primitive `Production_BunkerVisual_0_13` runtime presentation is hidden
- old DEV command-table / workshop / generator / primitive blast-door sightline objects are hidden
- one Quaternius `Door_Frame_A` + one `Door_DarkMetal` form the focal rear blast door
- secondary Quaternius frames form restrained side bulkheads
- all imported decoration colliders are removed
- camera is moved lower and aimed at the authored rear architecture
- fog density and ambient light are reduced/rebalanced so geometry is readable instead of nearly black
- cold and warm point-light accents remain restrained

UI composition:
- no cyan-filled feature panels
- shell uses near-black glass with thin cyan / amber rails
- horizontal Operations navigation remains but is compact and dark
- Overview Bunker Intel block is hidden
- Overview mission console occupies only the left side
- campaign status is a compact right-side console
- large center region remains unobstructed for the authored 3D Bunker
- Deploy remains a dedicated bottom action strip
- dynamic Arsenal / Operators / Campaign / Workshop / Supply panels receive dark-glass treatment without adding a second layout layer
- decorative additions remain raycast-disabled

### Build pipeline
- `DEADREACH > Build Production Slice 0.13`
- accepted Production 0.12 SectorScenePass remains in the pipeline
- accepted Production 0.12 LayoutPolishPass remains in the pipeline
- Production13BunkerScenePass still authors the generated Bunker scene, but its primitive presentation is hidden at runtime by 0.13c in favor of the authored Quaternius focal architecture

### Real-Unity validation state — CURRENT 0.13c
- current 0.13c fresh compile: **PASSED — 0 red compiler errors** ✅ 2026-08-24
- current 0.13c Build Production Slice 0.13: **PENDING**
- 0.13c Overview visual acceptance: **PENDING**
- Overview / Arsenal / Operators / Campaign / Workshop / Supply screen pass: **PENDING**
- mobile safe-area / touch regression: **PENDING**
- full 0.12 expedition regression: **PENDING**
- final Unity Console 0 red runtime errors: **PENDING**

### Stable 0.12 systems that must remain authoritative
- four accepted Sector layouts + 0.12b declutter geometry
- Contamination / Electrical Arc / Fireline hazards
- sector Scrap and BLACK CACHE Item Power bonuses
- 0.11 RECOVERY / PURGE / HOLDOUT / BLACKSITE
- objective-gated extraction and BLACK CACHE risk/reward
- reinforcement waves
- schema-v6 Workshop progression
- fixed lower-left MOVE
- fixed lower-right AIM/FIRE
- independent upper-right Ability
- 0.10 combat-impact VFX
- accepted Arsenal / operator preview / boss / reward behavior

## Next exact gate

Run `DEADREACH > Build Production Slice 0.13`. Require stable 0.12 generation + accepted 0.12 layout polish + Production13BunkerScenePass to complete with no blocking red generation errors. After Bunker reopens, inspect Overview first before any broader regression.

Test plan: `docs/PRODUCTION_13_TEST.md`
