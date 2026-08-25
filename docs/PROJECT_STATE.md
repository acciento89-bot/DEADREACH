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
- Production 0.14 state: **scene build PASSED; first visual screenshot close but not accepted; non-Overview tab failure fixed in code; fresh compile + interaction retest next**

Permanent firearm rule:
- use artist-authored firearm geometry already parented to the Quaternius survivor rig
- derive muzzle from that embedded firearm
- never reintroduce the failed external hand-mounted Rifle transform path

## Production 0.14 — Premium Command Center Reboot — ACTIVE

### Why 0.14 is a clean reboot
- branches directly from validated Production 0.12 `main`
- rejected Production 0.13 presentation layers are not part of this branch
- no stacked 0.13 shell / Kenney / real-asset overlay system
- command-center presentation is rebuilt as one runtime system

### Command-center implementation
- `Production14CommandCenterUI` is the sole new presentation owner
- legacy `BunkerCommandCenterUI` may initialize its stable gameplay state, then its visual canvas is removed when 0.14 starts
- new screen-space command-center shell is built as one system
- new `Production14IndustrialSkin` generates sliced industrial UI plates at runtime
- brushed gunmetal treatment, clipped corners, bevel edges, rivet details and controlled cyan/amber accents
- premium header with separate SCRAP / EXTRACTS / BOSS KILLS counter modules
- six segmented horizontal Operations tabs
- compact physical-style deployment console on Overview
- compact campaign status console on Overview
- center remains the main hero composition
- `Production14HoloDiorama` builds an animated tactical command table / projected city with objective markers, rings and cyan/amber lighting
- authored Quaternius `Door_Frame_A` / `Door_DarkMetal` geometry is loaded from `Resources/Production14/Quaternius` for rear Bunker architecture
- Bunker camera is lowered and reframed around the central command-table presentation
- old primitive sightline props and primitive Blastdoor geometry are hidden by the 0.14 hero pass
- premium bottom Ready / Deploy console
- decorative UI has raycast disabled
- holographic decorative objects have colliders removed

### Functional screen navigation pass
The first screenshot exposed that `HandleNav` intentionally changed only the header because Pass 1 had been hard-coded as Overview-only. That behavior is removed.

Current branch now includes native 0.14 screens:
- Overview — mission / campaign / hero hologram
- Arsenal — secured weapon inventory + equip actions + active-loadout inspector
- Operators — roster + active operator details + selection
- Campaign — sector navigation + unlocked level selection
- Workshop — permanent Bunker upgrades + equipped weapon calibration
- Supply — dedicated supply/cosmetic content screen

Navigation behavior:
- one `ScreenContent` layer is rebuilt per selected tab
- header/nav/footer remain stable during screen swaps
- active nav styling follows the selected tab
- hologram is active only on Overview and returns when Overview is selected again
- Operator/Campaign changes refresh the footer deployment state
- no legacy DEV dashboard is restored for unfinished screens

### Recovery / reproducibility checkpoint — COMPLETE ✅

A local-clean operation exposed that critical Unity-generated production assets were previously only present in the local working tree. They were recovered from the pre-0.14 safety stash and permanently versioned in:
- recovery commit `6ed8bf5e292f3430300fcb98ce13641885e7a309`

Now versioned on the branch:
- `Assets/Deadreach/Scenes/Bunker_Hub.unity`
- `Assets/Deadreach/Scenes/DeadCity_VerticalSlice.unity`
- DevPalette materials used by generated scenes
- full `Assets/Deadreach/Art/Production` controllers / materials / prefabs / volume profile
- Sam / Shaun / Matt production prefabs
- Rifle / SMG / Pistol / Shotgun production prefabs
- four infected production prefabs
- `Assets/Deadreach/Resources/Deadreach/ProductionAssetCatalog.asset`
- Unity GUID `.meta` files
- URP project assets
- `Packages/packages-lock.json`
- `ProjectSettings`

The recovered `ProductionAssetCatalog.asset` references the versioned production prefab GUIDs, including Rifle GUID `af8e76922930adf43856af69b33a808c`.

### Build pipeline hardening
- menu item: `DEADREACH > Build Production Slice 0.14`
- accepted Production 0.12 SectorScenePass remains authoritative
- accepted Production 0.12 LayoutPolishPass remains authoritative
- 0.14 command center bootstraps at runtime in the Bunker
- 0.5 operator art gate reuses validated Sam / Shaun / Matt prefabs before any repair/import path
- 0.6 weapon-family gate reuses validated Rifle / SMG / Pistol / Shotgun prefabs before any standalone glTF repair/import path

### Current real-Unity validation
- complete production asset recovery: **COMMITTED + VERIFIED ON GITHUB**
- post-recovery Unity compile before navigation pass: **PASSED — 0 red Unity errors**
- `DEADREACH > Build Production Slice 0.14` after recovery: **PASSED**
- Bunker reopened and Play Mode reached new Overview: **PASSED**
- Overview visual verdict: **NOT FINAL — user: “nah dran aber nicht ganz”**
- non-Overview tabs on that build: **FAILED — clicks changed header only**
- root cause: Overview-only guard in `HandleNav`
- functional six-screen navigation fix: **IMPLEMENTED**
- fresh Unity compile after navigation fix: **PENDING**
- six-tab interaction retest: **PENDING**
- next visual polish pass: **PENDING**
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

Pull latest `production/0.14-premium-command-center`, let Unity compile and require **0 red errors**. No scene rebuild is required for the runtime-only navigation fix. Enter Play Mode in the already generated Bunker and test Overview / Arsenal / Operators / Campaign / Workshop / Supply. If all six switch correctly, continue directly with the next visual polish pass on the Overview center composition.

Test plan: `docs/PRODUCTION_14_TEST.md`
