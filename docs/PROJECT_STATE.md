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
- Production 0.13 state: first large premium-UI / Bunker-art block implemented; **fresh Unity compile passed, build/runtime validation pending**

Permanent firearm rule:
- use artist-authored firearm geometry already parented to the Quaternius survivor rig
- derive muzzle from that embedded firearm
- never reintroduce the failed external hand-mounted Rifle transform path

## Production 0.13 — Premium Bunker UI / Command Center Overhaul — ACTIVE

### Art-direction target
- DEADREACH menu must read as a premium tactical extraction-game command center rather than a flat prototype UI
- graphite / cold cyan / warning amber identity
- strong hierarchy, panel depth, tactical framing and restrained animation
- visual effects must never steal touch input or reduce mobile-landscape readability

### Implemented — premium UI system
- new `Production13PremiumBunkerUI`
- runtime auto-install on Bunker scene load
- animated tactical backdrop with grid / scan sweep / cold edge glow / vignette bands
- premium corner-frame graphic for shell and feature panels
- header / navigation / content / deploy chrome
- animated tactical-link telemetry
- pulsing deploy rail
- numbered premium navigation labels including dynamically installed Workshop
- dynamic rescanning/restyling so Arsenal / Operators / Campaign / Store / Workshop content created after tab changes receives the same design language
- button hover / pressed / disabled palette hardening
- text shadow / headline emphasis pass
- semantic rarity / danger / success information preserved
- decorative Production 0.13 graphics are raycast-disabled

### Implemented — cinematic 3D Bunker pass
- new `Production13BunkerScenePass`
- structural command-center ribs
- emissive cyan / amber architectural accents
- rotating holographic command-table array
- dual server banks with status LEDs
- three-screen tactical wall
- floor guidance markers
- blast-door warning array
- animated emergency / monitor / holo lighting through `Production13BunkerAtmosphere`
- darker fog / ambient treatment and command-center camera tuning

### Build pipeline
- new `DEADREACH > Build Production Slice 0.13`
- Build 0.13 first regenerates the existing baseline
- accepted Production 0.12 SectorScenePass remains in the pipeline
- accepted Production 0.12 LayoutPolishPass remains in the pipeline
- Production13BunkerScenePass runs only after stable 0.12 world generation succeeds

### Real-Unity validation state
- fresh compile: **PASSED — 0 red compiler errors**
- Build Production Slice 0.13: **PENDING**
- premium shell visual acceptance: **PENDING**
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

Run `DEADREACH > Build Production Slice 0.13` in the freshly compiled branch. Require stable 0.12 world generation + layout polish + Production13BunkerScenePass to complete with no blocking red generation errors. After that, first visual acceptance starts on Bunker Overview.

Test plan: `docs/PRODUCTION_13_TEST.md`
