# DEADREACH — Project State

_Last updated: 2026-08-22_

This file is the canonical handoff and progress record for DEADREACH. Update it after every major development, design, build, architecture, backend, monetization, store, or release step so work can continue across chat-length limits without relying on chat history.

## 1. Product identity

- **Game name:** DEADREACH
- **Studio / umbrella:** Kamilunavo
- **GitHub:** `acciento89-bot/DEADREACH`
- **App Store Connect:** app created
- **iOS Bundle Identifier:** `de.kamilunavo.deadzone`
- **App Store Connect SKU:** `deadzone-001`
- **Primary platforms:** iOS + Android
- **Monetization:** in-app purchases only
- **Advertising:** none

The store-facing brand remains **DEADREACH**. The older technical identifier `de.kamilunavo.deadzone` is intentional and must not trigger a new App Store Connect app or Bundle ID migration.

## 2. Core game concept

DEADREACH is a premium-feeling mobile 3D survival-extraction roguelite with persistent progression.

Core loop:

**Bunker → Deploy → Expedition → Combat → Loot → Risk decision → Extraction / Death → Persistent result → Bunker**

Long-term pillars:

- real-time high-angle / isometric-style 3D combat
- extraction risk: continue deeper for stronger rewards or leave safely
- persistent survivor progression
- loot-driven weapon/equipment builds
- visible bunker progression and upgrades
- bosses and replayable zones
- daily / weekly challenges
- seasons and later live content
- collections, achievements, streaks and leaderboards
- no ad rewards, banners or forced advertising

## 3. Technical direction — LOCKED

- **Engine:** Unity 6.3 LTS
- **Pinned editor:** `6000.3.22f1`
- **Render pipeline:** URP 17.3
- **Language:** C#
- **Target:** mobile-first high-quality 3D
- **Input package:** Unity Input System 1.17
- **Camera package available:** Cinemachine 3.1.5
- **Current prototype camera:** custom perspective high-angle follow rig
- **Version control:** GitHub
- **Large assets:** Git LFS
- **Serialization:** Force Text
- **iOS / Android scripting backend:** IL2CPP
- **Orientation:** landscape
- **Color space:** Linear
- **Backend:** not implemented yet; Supabase remains an option for accounts / cloud state / leaderboards / events
- **IAP:** Apple + Google storefront integration planned
- **Addressables:** planned, not installed/configured yet
- **AI Navigation/NavMesh:** planned for production enemies; current slice uses lightweight direct steering

### Graphics target

The final game must not inherit the temporary primitive art used to validate systems. Production visuals should use:

- real 3D environment kits and characters
- PBR materials
- baked / mixed lighting where appropriate
- light + reflection probes
- color grading and post-processing
- proper VFX for muzzle flash, tracers, impacts, blood, electricity, smoke, fire and atmosphere
- animation blending and rigging
- scalable mobile quality settings

Runtime quality presets already exist:

- Performance
- Balanced
- Ultra

## 4. Vertical Slice 0.1 goal

Target flow:

**Bunker → Deploy to Dead City → Move → Aim/fire → Fight infected → Collect Scrap → Extract → Scrap persists → Return to bunker**

Failure flow:

**Bunker → Expedition → Death or Abandon → carried loot lost → extraction streak reset → Return to bunker**

## 5. Current branch / repository state

### Branches

- `main` — repository initialization only
- `foundation/unity-6.3` — active development branch

At the last comparison before this status update, `foundation/unity-6.3` was **36 commits ahead of `main`** with no divergence.

### Foundation completed

- Unity `.gitignore`
- Git LFS `.gitattributes` for large models/textures/audio/video
- Unity editor pin: `6000.3.22f1`
- package manifest with:
  - URP 17.3.0
  - Input System 1.17.0
  - Cinemachine 3.1.5
  - Unity UI 2.0.0
- `Deadreach.Runtime` assembly
- `Deadreach.Editor` assembly
- automatic project configuration:
  - company `Kamilunavo`
  - product `DEADREACH`
  - bundle/package ID `de.kamilunavo.deadzone`
  - version `0.1.0`
  - iOS build number `1`
  - Android version code `1`
  - IL2CPP
  - landscape
  - Linear color space
  - Force Text serialization
- runtime bootstrap:
  - 60 FPS target
  - VSync disabled
  - device sleep disabled during gameplay

### URP hardening completed

`DeadreachUrpBootstrap.cs` now creates/assigns a DEADREACH URP asset on first project open if it does not already exist:

`Assets/Deadreach/Settings/Deadreach_URP.asset`

This prevents the project from silently remaining on the Built-in Render Pipeline merely because the URP package is installed.

### Gameplay systems implemented

#### Input

`DeadreachInput.cs`

- desktop WASD
- gamepad left stick movement
- mouse aim
- left mouse automatic fire
- mobile left-half virtual movement gesture
- mobile right-half aim + automatic fire gesture
- Input System / Enhanced Touch based

#### Player

`PlayerMotor.cs`

- CharacterController-based movement
- camera-relative movement
- acceleration / deceleration
- gravity
- facing from movement when not actively aiming

#### Camera

`HighAngleCameraRig.cs`

- perspective camera
- high-angle/isometric-style composition
- smooth position follow
- smooth rotation follow
- target auto-discovery or explicit assignment

#### Combat

`Damageable.cs`

- health
- factions: Neutral / Survivor / Infected
- same-faction damage rejection
- damage events
- death events

`HitscanWeapon.cs`

- screen/touch aim projection into world
- survivor rotation toward aim point
- automatic fire rate
- range
- hitscan raycast
- faction-aware damage

#### Enemy AI

`InfectedChaser.cs`

- aggro range
- chase behavior
- melee range
- melee cooldown / damage
- per-enemy configuration
- Scrap drop on death

Current AI is intentionally lightweight for the first slice. Production enemy pathfinding/navmesh is still pending.

#### Loot

`LootPickup.cs`

- collectible Scrap
- rotating/bobbing pickup presentation
- enemy death drops
- placed caches supported
- collected loot goes into the active run, not directly into permanent progression

#### Run state / extraction

`RunSession.cs`

- carried Scrap
- extraction progress
- successful-run state
- failed-run state
- successful extraction banks loot
- death loses carried loot
- abandoning a run loses carried loot
- death/abandon resets current extraction streak
- result is shown briefly before returning to bunker

`ExtractionZone.cs`

- trigger-based extraction zone
- hold duration
- optional requirement that the player carries loot
- normalized extraction progress

#### Persistence

`SaveService.cs`

Local JSON profile at:

`Application.persistentDataPath/deadreach-profile.json`

Currently persisted:

- secured Scrap
- successful extractions
- failed runs
- current extraction streak
- best extraction streak

#### Scene flow

`SceneFlowService.cs`

- Bunker scene
- Dead City expedition scene
- guarded scene loading
- missing Build Settings scenes produce an explicit error rather than a blind load failure

#### Bunker shell / menu

`BunkerPrototypeMenu.cs`

- secured Scrap display
- extraction count
- current/best streak
- Deploy to Dead City
- switch Performance / Balanced / Ultra graphics preset

This is a functional prototype menu, not final UI art.

#### Pause

`PauseController.cs`

- mobile-visible pause button during expedition
- Escape / controller Start support
- resume
- abandon run and return to bunker
- abandon routes through failed-run accounting

### Adaptive graphics implemented

`MobileQualityService.cs`

- Performance / Balanced / Ultra presets
- automatic first-run recommendation from available system/GPU memory
- player selection persisted with PlayerPrefs
- controls frame target, shadow distance/resolution, LOD bias, AA and anisotropic filtering

## 6. Scene generation tooling

No `.unity` production scenes are hand-authored in Git yet. Instead the branch contains deterministic editor builders so the first playable slice is reproducible after a fresh clone.

### Complete slice command

In Unity:

`DEADREACH > Build Complete Vertical Slice 0.1`

This generates:

- `Assets/Deadreach/Scenes/Bunker_Hub.unity`
- `Assets/Deadreach/Scenes/DeadCity_VerticalSlice.unity`

and configures Build Settings with the Bunker first.

### Bunker generator

`BunkerHubSceneBuilder.cs`

Creates a temporary 3D bunker shell with:

- floor / walls
- command table
- workshop/storage/generator blocks
- multiple colored lights
- perspective bunker camera
- functional deploy/settings menu

### Dead City generator

`VerticalSliceSceneBuilder.cs`

Creates a temporary atmospheric gameplay test map with:

- streets / cross street
- block buildings
- barricades
- street lights
- fog / ambient lighting
- player
- perspective follow camera
- six infected enemies
- placed Scrap caches
- enemy Scrap drops
- extraction zone + beacon
- HUD
- run/session systems

The generator also creates reusable development materials under:

`Assets/Deadreach/Art/DevPalette`

### Important visual status

The generated Bunker and Dead City use primitive geometry and a deliberate dev palette. They validate systems, scale, camera, lighting direction and composition only.

**They are not the target production graphics and must not be mistaken for a finished art pass.**

## 7. Test documentation

Canonical first-run checklist:

`docs/VERTICAL_SLICE_01_TEST.md`

It covers:

- Unity version
- first-open bootstrap
- URP verification
- complete scene generation
- desktop controls
- touch controls
- combat
- enemy death + loot
- extraction
- persistence
- failed-run behavior
- acceptance checks before merge

## 8. What has NOT been verified yet

This is critical:

- The branch has **not yet been opened/compiled in an actual Unity 6000.3.22f1 editor during this development pass**.
- The generated scenes have therefore not yet been physically produced and played.
- Unity package resolution still needs a real editor pass.
- Any Unity-API differences surfaced by real compilation must be fixed before merging.
- No iOS build exists yet.
- No Android build exists yet.
- No TestFlight build exists yet.

Do not claim Vertical Slice 0.1 is tested until the acceptance runbook has actually passed.

## 9. Remaining major work

### Before foundation merge

1. Open repo in Unity `6000.3.22f1`.
2. Wait for package import/compilation.
3. Fix any compiler/API errors.
4. Run `DEADREACH > Build Complete Vertical Slice 0.1`.
5. Play from `Bunker_Hub`.
6. Complete one successful extraction.
7. Complete one failed/death run.
8. Verify persisted Scrap/streak after leaving and re-entering Play Mode.
9. Verify pause + abandon.
10. Verify graphics preset switching.
11. Verify Console is free of blocking errors.
12. Only then merge foundation branch.

### After foundation validation

Priority order:

1. production mobile joystick / aim UI and haptics
2. real player character + animations
3. real infected character + animations
4. weapon model + muzzle flash / tracer / impact VFX
5. audio pass
6. environment art kit for Dead City
7. post-processing / color grading / lighting pass
8. data-driven weapon definitions and rarity/affix system
9. real run inventory / equipment
10. multiple extraction/depth choices to strengthen risk-vs-reward
11. bunker room upgrade architecture
12. first boss encounter
13. Addressables/content organization
14. backend/accounts/leaderboards/events
15. IAP catalog / cosmetics / season structure
16. iOS/Android build pipeline
17. TestFlight / device performance profiling

## 10. Decisions already made

### Engine

Unity 6.3 LTS + URP remains the production choice.

### Presentation

- true perspective 3D
- high-angle/isometric-style view
- not flat 2D
- not orthographic by default

### Monetization

- zero advertising
- IAP only
- preference for cosmetics, presentation and season content over hard pay-to-win
- candidate products: survivor skins, weapon skins, bunker themes, seasonal pass

## 11. Non-negotiables

- Do not let DEADREACH become a cheap-looking generic mobile prototype.
- Temporary primitive geometry must be replaced, not polished into final art.
- Keep mobile performance in mind from the first production art/shader decisions.
- Keep gameplay systems modular and data-driven where practical.
- Do not merge untested Unity code merely because repository structure looks correct.
- Commit meaningful progress regularly.
- Update this file after every major pass.
- Keep store name **DEADREACH** even though the technical ID remains `de.kamilunavo.deadzone`.
- No advertising SDKs or ad-based rewards.

## 12. Immediate next step

**Real Unity validation pass.**

Open `foundation/unity-6.3` in Unity `6000.3.22f1`, resolve compilation/package issues if any, generate the complete Vertical Slice 0.1, and execute `docs/VERTICAL_SLICE_01_TEST.md`.

After the slice passes, merge the foundation PR and start the first production art/game-feel pass rather than adding more unverified systems.

## 13. Handoff protocol

When resuming DEADREACH in another chat:

1. Read this file first.
2. Inspect the latest commits and open PRs.
3. Check whether the Unity validation run has happened since this update.
4. Continue from **Immediate next step**.
5. Update this file before ending the next major pass.

Do not rely on chat history alone for project continuity.
