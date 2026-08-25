# DEADREACH — Project State

_Last updated: 2026-08-25_

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
- active production branch: `production/0.14-premium-command-center`
- Production 0.14 state: **Pass 1 premium Overview / Command Center reboot implemented; fresh Unity compile + asset import PASSED; build gate next**

Permanent firearm rule:
- use artist-authored firearm geometry already parented to the Quaternius survivor rig
- derive muzzle from that embedded firearm
- never reintroduce the failed external hand-mounted Rifle transform path

## Production 0.14 — Premium Command Center Reboot — ACTIVE

### Why 0.14 is a clean reboot
- branches directly from validated Production 0.12 `main`
- rejected Production 0.13 presentation layers are not part of this branch
- no stacked 0.13 shell / Kenney / real-asset overlay system
- Overview is rebuilt first and must pass visual acceptance before the other Bunker tabs are redesigned

### Pass 1 implementation
- `Production14CommandCenterUI` is the sole new presentation owner
- legacy `BunkerCommandCenterUI` may initialize its stable gameplay state, then its visual canvas is removed when 0.14 starts
- new screen-space command-center shell is built as one system
- new `Production14IndustrialSkin` generates sliced industrial UI plates at runtime
- brushed gunmetal treatment, clipped corners, bevel edges, rivet details and controlled cyan/amber accents
- premium header with separate SCRAP / EXTRACTS / BOSS KILLS counter modules
- six segmented horizontal Operations tabs
- compact physical-style deployment console on the left
- compact campaign status console on the right
- center remains open as the main hero composition
- `Production14HoloDiorama` builds an animated tactical command table / projected city with objective markers, rings and cyan/amber lighting
- authored Quaternius `Door_Frame_A` / `Door_DarkMetal` geometry is loaded from `Resources/Production14/Quaternius` for rear Bunker architecture
- Bunker camera is lowered and reframed around the central command-table presentation
- old primitive sightline props and primitive Blastdoor geometry are hidden by the 0.14 hero pass
- premium bottom Ready / Deploy console
- decorative UI has raycast disabled
- holographic decorative objects have colliders removed
- non-Overview tabs are intentionally visually pending in Pass 1 and do not resurrect the legacy DEV dashboard

### Build pipeline
- new menu item: `DEADREACH > Build Production Slice 0.14`
- reuses accepted base generation path
- accepted Production 0.12 SectorScenePass remains authoritative
- accepted Production 0.12 LayoutPolishPass remains authoritative
- 0.14 command center bootstraps at runtime in the Bunker

### Current real-Unity validation
- fresh Production 0.14 compile + asset import after Git LFS recovery: **PASSED — 0 red Unity errors**
- Build Production Slice 0.14: **PENDING**
- Overview visual acceptance against premium command-center reference: **PENDING**
- Deploy interaction: **PENDING**
- mobile landscape / safe-area pass: **PENDING**
- full stable 0.12 expedition regression: **PENDING**
- final Unity Console 0 red runtime errors: **PENDING**

## Production 0.12 — Sector Expansion — STABLE ✅

### Final real-Unity validation
- fresh compile after 0.12b: **PASSED — 0 red compiler errors**
- `DEADREACH > Build Production Slice 0.12`: **PASSED**
- QUARANTINE WARD: **PASSED**
- TRANSIT COLLAPSE after declutter: **PASSED**
- INDUSTRIAL SPILL after declutter: **PASSED**
- BLACKOUT PLAZA after declutter: **PASSED**
- sector reward / BLACK CACHE Item Power behavior: **PASSED**
- fixed-zone mobile controls: **PASSED**
- full Bunker → Workshop/Arsenal → Deploy → mission/risk-reward → extraction → Bunker regression: **PASSED**
- Workshop/progression persistence: **PASSED**
- 0.10 combat-impact / boss / reward presentation regression: **PASSED**
- final Unity Console: **0 red runtime errors**

### Stable systems that remain authoritative
- four accepted sector layouts + 0.12b declutter geometry
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
- accepted Arsenal / operator / boss / reward behavior

## Next exact gate

Run `DEADREACH > Build Production Slice 0.14`. Require accepted base generation + Production 0.12 sector/layout passes to complete and the Bunker scene to reopen with no blocking red generation error. Then enter Play Mode and capture the Overview for visual acceptance.

Test plan: `docs/PRODUCTION_14_TEST.md`
