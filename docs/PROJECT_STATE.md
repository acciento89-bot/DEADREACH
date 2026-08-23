# DEADREACH — Project State

_Last updated: 2026-08-23_

This is the canonical handoff file for DEADREACH. Update it after every major development, validation, build, architecture, backend, monetization, store, or release step so another chat can continue without relying on chat history.

## 1. Product identity

- **Game:** DEADREACH
- **Studio / umbrella:** Kamilunavo
- **Repository:** `acciento89-bot/DEADREACH`
- **App Store Connect:** app created
- **iOS Bundle Identifier:** `de.kamilunavo.deadzone`
- **App Store Connect SKU:** `deadzone-001`
- **Platforms:** iOS + Android
- **Monetization:** in-app purchases only
- **Advertising:** none

The store-facing name stays **DEADREACH**. The older technical identifier `de.kamilunavo.deadzone` is intentional and must not trigger a new App Store Connect app or Bundle ID migration.

## 2. Game direction — LOCKED

Premium-feeling mobile 3D survival/extraction roguelite with persistent progression.

Core loop:

**Bunker → Deploy → Expedition → Combat → Loot → Risk decision → Extract / Die / Abandon → Persistent result → Bunker**

Long-term pillars:

- real-time high-angle/isometric-style 3D combat
- extraction risk and deeper-run rewards
- persistent survivor progression
- loot-driven weapon/equipment builds
- visible bunker progression
- bosses and replayable zones
- daily/weekly challenges and seasons
- collections, achievements, streaks and leaderboards
- zero advertising

## 3. Technical direction — LOCKED

- **Engine:** Unity 6.3 LTS
- **Pinned editor:** `6000.3.22f1`
- **Render pipeline:** URP 17.3
- **Language:** C#
- **Input:** Unity Input System 1.17
- **Camera package available:** Cinemachine 3.1.5
- **Current prototype camera:** custom perspective high-angle follow rig
- **Serialization:** Force Text
- **Version control:** GitHub + Git LFS
- **iOS / Android scripting backend:** IL2CPP
- **Orientation:** landscape
- **Color space:** Linear
- **Backend:** not implemented; Supabase remains an option
- **IAP:** Apple + Google storefront integration planned
- **Addressables:** planned
- **Production AI navigation:** planned; current prototype uses direct steering

### Graphics target

Do not convert the primitive prototype into final art. Production visuals need real 3D environments/characters, PBR materials, proper lighting/probes, post-processing, VFX, animation blending, audio, and scalable mobile quality.

Runtime presets already exist:

- Performance
- Balanced
- Ultra

## 4. Current Git state

- `main` — repository initialization only
- `foundation/unity-6.3` — active development branch
- Draft PR: **#1 — foundation: Unity 6.3 vertical slice 0.1**
- PR remains intentionally unmerged until the complete runtime acceptance pass succeeds.

## 5. Foundation implemented

- Unity `.gitignore`
- Git LFS rules
- editor pin `6000.3.22f1`
- URP 17.3.0
- Input System 1.17.0
- Cinemachine 3.1.5
- Unity UI 2.0.0
- `Deadreach.Runtime` assembly
- `Deadreach.Editor` assembly
- automatic project identity/settings bootstrap
- automatic URP asset/bootstrap
- 60 FPS runtime target
- VSync disabled
- device sleep disabled during gameplay
- adaptive Performance/Balanced/Ultra quality service

## 6. Gameplay systems implemented

### Input / player

- WASD
- gamepad movement
- mouse aim/fire
- mobile left-half virtual movement gesture
- mobile right-half aim + auto-fire gesture
- CharacterController movement
- camera-relative motion
- acceleration/deceleration + gravity

### Camera

- true perspective high-angle/isometric-style follow camera
- smoothed position/rotation

### Combat

- Survivor / Infected factions
- health/damage/death events
- friendly-fire rejection
- hitscan shooting
- fire rate/range/damage
- world aim projection

### Enemy prototype

- infected aggro/chase
- melee range/cooldown/damage
- Scrap drop on death
- lightweight direct steering for first slice

### Loot / extraction / persistence

- Scrap pickups and caches
- carried loot belongs to active run only
- extraction hold/progress
- extraction banks Scrap
- death loses carried loot
- abandon loses carried loot
- failed/abandoned run resets extraction streak
- local JSON profile stores secured Scrap, successful extractions, failed runs, current streak, best streak

### Scene flow / UI

- Bunker → Dead City → Bunker flow
- result delay after extraction/death
- Bunker prototype menu
- graphics-preset switching
- pause / resume
- abandon-run flow
- prototype HUD

## 7. Scene generation tooling

Use:

`DEADREACH > Build Complete Vertical Slice 0.1`

It generates:

- `Assets/Deadreach/Scenes/Bunker_Hub.unity`
- `Assets/Deadreach/Scenes/DeadCity_VerticalSlice.unity`

The generated Dead City contains temporary streets, buildings, barricades, lighting/fog, player, camera, six infected, Scrap caches, enemy drops, extraction zone/beacon, HUD and run systems.

The generated Bunker contains a temporary 3D hub shell plus functional deploy/settings menu.

**Primitive geometry and dev materials are scaffolding only, not production art.**

## 8. Real Unity validation status

### PASSED — 2026-08-23

The repository has now been opened in the real pinned Unity editor and package import reached a **clean C# compile with 0 errors**.

Compiler/API issues found and fixed during the real editor pass:

1. `AudioListener.pause` references in `PauseController.cs` caused `CS0103` in the current assembly/editor setup. The explicit audio-pause calls were removed; gameplay pause still uses `Time.timeScale`.
2. `UniversalRenderPipelineAsset.EnsureGlobalSettings()` is inaccessible in Unity 6.3 (`CS0122`). The non-public API call was removed from `DeadreachUrpBootstrap.cs`.

Current compile gate: **0 errors**.

### NOT YET VERIFIED

- complete scene generation
- Bunker runtime
- deploy button
- player movement/aim/fire at runtime
- enemy chase/melee/death
- loot collection
- successful extraction
- failed/death run
- abandon flow
- save persistence across Play Mode
- graphics-preset switching at runtime
- mobile touch behavior on device
- iOS/Android build

Do not claim Vertical Slice 0.1 is accepted until these runtime checks pass.

## 9. Test runbook

Detailed acceptance checklist:

`docs/VERTICAL_SLICE_01_TEST.md`

## 10. Immediate next step

### Runtime Gate 2 — generate and play the slice

In Unity `6000.3.22f1` on `foundation/unity-6.3`:

1. Run **`DEADREACH > Build Complete Vertical Slice 0.1`**.
2. Confirm both generated scenes appear and the Console stays free of blocking errors.
3. Open/start from `Bunker_Hub` and press Play.
4. Verify Deploy loads Dead City.
5. Verify movement, aiming, firing and enemy combat.
6. Collect Scrap and complete one successful extraction.
7. Complete one death/failed run.
8. Verify pause + abandon.
9. Exit/re-enter Play Mode and confirm secured Scrap/streak persistence.
10. Verify all three graphics presets can be switched.
11. Report screenshots/errors/results before merging PR #1.

After this gate passes, merge the foundation branch and immediately begin the first production art/game-feel pass: real character, real infected, weapon/VFX/audio, proper mobile controls and environment art.

## 11. Non-negotiables

- No cheap-looking generic mobile finish.
- Primitive geometry must be replaced rather than treated as final art.
- Mobile performance matters from the first production art/shader decisions.
- Keep gameplay modular/data-driven.
- Do not merge untested Unity runtime behavior.
- Update this file after every major pass.
- Store name stays **DEADREACH**.
- No advertising SDKs or ad-based rewards.

## 12. Handoff protocol

When resuming in another chat:

1. Read this file first.
2. Inspect latest commits and PR #1.
3. Determine whether Runtime Gate 2 has passed.
4. Continue from **Immediate next step**.
5. Update this file before ending the next major pass.

Do not rely on chat history alone.
