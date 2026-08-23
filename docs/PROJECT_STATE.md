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

The store-facing name stays **DEADREACH**. The older technical identifier `de.kamilunavo.deadzone` is intentional.

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

Graphics target: real 3D environments/characters, PBR, proper lighting/probes, post-processing, VFX, animation, audio and scalable mobile quality. Primitive prototype art must be replaced, not polished into final art.

Runtime presets already exist: Performance / Balanced / Ultra.

## 4. Current Git state

- `main` — repository initialization only
- `foundation/unity-6.3` — active development branch
- Draft PR: **#1 — foundation: Unity 6.3 vertical slice 0.1**
- PR remains unmerged until the complete runtime acceptance pass succeeds.

## 5. Foundation + gameplay implemented

Foundation:

- Unity `.gitignore`
- Git LFS rules
- editor pin `6000.3.22f1`
- URP 17.3.0
- Input System 1.17.0
- Cinemachine 3.1.5
- Unity UI 2.0.0
- Runtime / Editor asmdefs
- automatic project settings/bootstrap
- automatic URP bootstrap
- 60 FPS target, VSync off, sleep disabled
- adaptive Performance/Balanced/Ultra quality service

Gameplay:

- WASD / gamepad / mouse / touch input
- CharacterController survivor movement
- perspective high-angle follow camera
- factions + health/damage/death
- hitscan shooting
- infected chase + melee AI
- enemy Scrap drops + placed Scrap caches
- extraction hold/progress
- death/abandon loses carried loot
- extraction banks Scrap
- persistent JSON profile for Scrap, successes, failures and streaks
- Bunker menu
- pause/resume/abandon
- Bunker ↔ Dead City scene flow

## 6. Scene generation tooling

Primary command:

`DEADREACH > Build Complete Vertical Slice 0.1`

Dev-only generators are now moved under:

`DEADREACH > Dev > ...`

Generated scenes:

- `Assets/Deadreach/Scenes/Bunker_Hub.unity`
- `Assets/Deadreach/Scenes/DeadCity_VerticalSlice.unity`

A centralized editor utility now owns scene registration:

`DeadreachBuildSettings.cs`

It guarantees the complete slice Build Settings order:

1. `Bunker_Hub`
2. `DeadCity_VerticalSlice`

Repair command:

`DEADREACH > Project > Repair Scene Build Settings`

## 7. Real Unity validation status

### Compile gate — PASSED

The project was opened in Unity `6000.3.22f1` and reached **0 C# errors** after fixing two Unity-6.3 API issues:

- removed `AudioListener.pause` calls that produced `CS0103`
- removed inaccessible `UniversalRenderPipelineAsset.EnsureGlobalSettings()` call that produced `CS0122`

### Runtime gate — PARTIALLY PASSED

Confirmed in the real Unity editor:

- Dead City generated and entered Play Mode
- gameplay actually started
- player could be killed by the prototype enemies
- failed-run logic triggered after death

Runtime issue found:

- after death, `RunSession` attempted to return to `Bunker_Hub`
- Unity reported that `Bunker_Hub` was not in Build Settings
- root cause: the old menu exposed two similarly named generators; the Dead-City-only builder could create a playable expedition without generating/registering the Bunker

Fix committed on `foundation/unity-6.3`:

- `DeadreachBuildSettings.cs` centralizes scene registration
- complete generator explicitly writes Bunker at build index 0 and Dead City at index 1
- complete generator reopens `Bunker_Hub` after generation
- old single-scene menu entries moved to `DEADREACH > Dev`
- repair command added for already-generated scenes

### Still needs runtime verification

- Bunker runtime/menu
- Deploy Bunker → Dead City
- death → automatic return to Bunker after the build-settings fix
- successful extraction → return to Bunker
- Scrap persistence
- failed/abandon persistence and streak reset
- pause + abandon
- graphics presets
- mobile touch on device
- iOS / Android builds

## 8. Immediate next step

On the local machine:

1. `git pull` on `foundation/unity-6.3`.
2. Wait for Unity to compile; require **0 errors**.
3. Run **`DEADREACH > Build Complete Vertical Slice 0.1`**.
4. Confirm Unity leaves `Bunker_Hub` open.
5. Press Play in the Bunker.
6. Deploy to Dead City.
7. Die once and verify automatic return to Bunker.
8. Then complete one successful extraction and verify automatic return + banked Scrap.
9. Test pause/abandon and persistence.
10. Only after these pass should PR #1 be merged.

## 9. Test runbook

Detailed acceptance checklist:

`docs/VERTICAL_SLICE_01_TEST.md`

## 10. After foundation validation

Priority:

1. production mobile joystick/aim UI + haptics
2. real survivor character + animation
3. real infected models + animation
4. weapon model + muzzle/tracer/impact VFX
5. audio
6. Dead City environment art kit
7. post-processing/color grading/lighting
8. data-driven weapons, rarity and affixes
9. real inventory/equipment
10. stronger risk-vs-reward depth choices
11. bunker room upgrades
12. first boss
13. Addressables/content organization
14. backend/accounts/leaderboards/events
15. IAP cosmetics/season structure
16. iOS/Android build pipeline + TestFlight/device profiling

## 11. Non-negotiables

- No cheap generic mobile finish.
- Primitive geometry is temporary scaffolding.
- Mobile performance matters from the first production art/shader decisions.
- Keep gameplay modular/data-driven.
- Do not merge untested runtime behavior.
- Update this file after every major pass.
- Store name stays **DEADREACH**.
- No advertising SDKs or ad-based rewards.

## 12. Handoff protocol

When resuming in another chat:

1. Read this file first.
2. Inspect latest commits and PR #1.
3. Check whether the post-fix Bunker/death/extraction runtime gate has passed.
4. Continue from **Immediate next step**.
5. Update this file before ending the next major pass.

Do not rely on chat history alone.
