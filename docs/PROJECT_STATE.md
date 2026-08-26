# DEADREACH — Project State

_Last updated: 2026-08-26_

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

## Production 0.14 — Premium Command Center Reboot — FINAL QA ACTIVE

### Accepted foundation
- branches directly from validated Production 0.12 `main`
- no rejected 0.13 presentation stack
- one `Production14CommandCenterUI` presentation owner
- six native screens: Overview / Arsenal / Operators / Campaign / Workshop / Supply
- six-tab switching: **PASSED**
- Production Slice 0.14 build after recovery: **PASSED**
- current Wenrexa + restored-hologram Overview: **VISUALLY ACCEPTED**
- visual iteration is frozen unless a blocking defect appears

### Current UI/presentation
- Wenrexa `UI Minimalism SciFi` CC0 authored panel/button family
- graphite generator retained only as offline fallback
- mission console left, campaign status right, command-table/hologram center
- Quaternius rear Bunker architecture
- `Production14HoloVisibilityGuard` restores inactive/missing hero on Overview
- Ready / Deploy footer

Rejected art history remains documented:
- procedural plate baseline — too DEV-like
- Devdog HUD experiment — visual fail
- graphite fallback — visual fail / effectively unchanged

### Recovery / reproducibility — COMPLETE ✅
Recovery commit:
- `6ed8bf5e292f3430300fcb98ce13641885e7a309`

Versioned production scenes, operator/weapon/infected prefabs, materials/controllers/volume, catalog, GUID metas, URP/package/project settings. Build hardening reuses validated production prefabs before legacy import/repair paths.

### Release-hardening mega block — IMPLEMENTED
New runtime partial:
- `Assets/Deadreach/Runtime/UI/Production14ReleaseBlock.cs`

Implemented:
- `Screen.safeArea` normalized root handling
- adaptive landscape `CanvasScaler`
- mobile minimum touch targets
- hardened Arsenal scroll behavior
- guarded Deploy / double-tap protection
- deployment validation for selected level/operator
- save-before-deploy
- save on pause/focus loss/quit
- live profile-state detection and command-center action feedback
- live header/footer refresh after profile changes
- Arsenal salvage buttons for unequipped secured weapons
- Scrap update after salvage

Orientation hardening:
- Portrait and Portrait Upside Down are disabled
- Landscape Left/Right remain enabled
- editor bootstrap repairs these PlayerSettings automatically on fresh checkout
- release validator rechecks the result

Release validator:
- `DEADREACH > Validate Production 0.14 Release Readiness`
- validates required scenes, Build Settings, Quaternius resources, Wenrexa preparation/fallback and landscape orientation compatibility

One-pass final QA document:
- `docs/PRODUCTION_14_RELEASE_GATE.md`

### Current real-Unity validation
PASSED:
- recovered assets committed/verified
- post-recovery compile
- Production Slice 0.14 build
- Bunker / Play Mode
- six-screen navigation
- current Overview visual acceptance
- latest release-hardening/orientation compile gate
- `DEADREACH 0.14 RELEASE STATIC CHECK: PASS`

Remaining final QA only:
- deep Arsenal/operator/campaign/workshop interactions
- mobile safe-area/touch sweep
- guarded Deploy interaction
- full stable 0.12 expedition regression
- return-to-Bunker persistence
- final Unity Console **0 red runtime errors**

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
Execute sections C–E of `docs/PRODUCTION_14_RELEASE_GATE.md` as one consolidated final run. If all pass, mark PR #14 ready and proceed to merge/release packaging.
