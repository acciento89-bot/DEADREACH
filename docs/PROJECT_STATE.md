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
- Production 0.13 state: **0.13b real-asset command-center implementation complete; fresh compile/import validation pending**

Permanent firearm rule:
- use artist-authored firearm geometry already parented to the Quaternius survivor rig
- derive muzzle from that embedded firearm
- never reintroduce the failed external hand-mounted Rifle transform path

## Production 0.13 — Premium Bunker UI / Command Center Overhaul — ACTIVE

### Art-direction target
- DEADREACH menu must read as a premium tactical extraction-game command center rather than a flat prototype / DEV UI
- authored 3D environment should be a visible part of the menu composition
- graphite / cold cyan / warning amber identity
- strong hierarchy and real game-art silhouette rather than flat full-screen panels
- visual effects must never steal touch input or reduce mobile-landscape readability

### 0.13a foundation — implemented / superseded visually
- `Production13PremiumBunkerUI` tactical shell / dynamic style layer
- header / navigation / content / deploy chrome
- dynamic styling for Overview / Arsenal / Operators / Campaign / Store / Workshop
- first `Production13BunkerScenePass` with command table / server banks / tactical wall / lighting
- fresh compile passed with 0 red errors
- Build Production Slice 0.13 passed in real Unity
- final visual acceptance was rejected by user because the result still read too much like an embellished DEV menu

### 0.13b real-asset pass — implemented
- new `Production13RealAssetCommandCenter`
- runtime auto-install on Bunker scene load after the existing premium shell
- real CC0 Quaternius Modular SciFi MegaKit authored meshes added to project
- `Door_Frame_A` used as central and secondary command-deck bulkhead architecture
- `Door_DarkMetal` used to build the central blast-door assembly
- imported geometry is re-materialed to DEADREACH steel / graphite / cyan / amber
- imported mesh colliders are removed so command-center decoration cannot affect validated gameplay/navigation
- selected CC0 Kenney UI Pack: Sci-fi graphics added as runtime textures
- Kenney plates are applied behind header / content / navigation / Deploy elements
- previous near-opaque tactical backdrop is disabled by the 0.13b runtime layer
- Backdrop / ContentFrame / feature panels are opened up substantially so the real 3D Bunker remains visible
- decorative UI remains raycast-disabled
- third-party license / provenance records are committed under `Assets/Deadreach/Art/Production13`

### Existing cinematic 3D support retained
- structural command-center ribs
- emissive cyan / amber architectural accents
- rotating holographic command-table array
- dual server banks with status LEDs
- three-screen tactical wall
- floor guidance markers
- blast-door warning array
- animated emergency / monitor / holo lighting through `Production13BunkerAtmosphere`
- 0.13b layers genuine authored meshes over this supporting scene work instead of relying on primitives alone

### Build pipeline
- `DEADREACH > Build Production Slice 0.13`
- Build 0.13 first regenerates the existing baseline
- accepted Production 0.12 SectorScenePass remains in the pipeline
- accepted Production 0.12 LayoutPolishPass remains in the pipeline
- Production13BunkerScenePass runs only after stable 0.12 world generation succeeds
- 0.13b real-asset command-center layer loads its imported Resources when the generated Bunker runs

### Real-Unity validation state — CURRENT 0.13b
- 0.13a fresh compile: **PASSED, NOW STALE AFTER 0.13b CODE/ASSET CHANGES**
- 0.13a Build Production Slice 0.13: **PASSED, NOW STALE AFTER 0.13b CODE/ASSET CHANGES**
- current 0.13b fresh compile + asset import: **PENDING**
- current 0.13b Build Production Slice 0.13: **PENDING**
- 0.13b real-asset shell visual acceptance: **PENDING**
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

1. pull `production/0.13-premium-bunker-ui`
2. let Unity import the new Quaternius OBJ/MTL and Kenney PNG assets
3. fresh Unity compile → require **0 red compiler errors** and no blocking model-import errors
4. only after that, rerun `DEADREACH > Build Production Slice 0.13`
5. visual acceptance starts on Overview with explicit check that genuine Quaternius geometry and Kenney chrome are visible and that the 3D Bunker is no longer hidden behind the UI

Test plan: `docs/PRODUCTION_13_TEST.md`
