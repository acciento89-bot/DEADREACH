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
- PR remains unmerged until the last small runtime gates pass.

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
- HP bar / damage flash / extraction messaging / progress HUD

## 6. Scene generation tooling

Primary command:

`DEADREACH > Build Complete Vertical Slice 0.1`

Dev-only generators are under:

`DEADREACH > Dev > ...`

Generated scenes:

- `Assets/Deadreach/Scenes/Bunker_Hub.unity`
- `Assets/Deadreach/Scenes/DeadCity_VerticalSlice.unity`

Central scene registration:

`DeadreachBuildSettings.cs`

Guaranteed build order:

1. `Bunker_Hub`
2. `DeadCity_VerticalSlice`

Repair command:

`DEADREACH > Project > Repair Scene Build Settings`

## 7. Real Unity validation status

### Compile gate — PASSED

The project was opened in Unity `6000.3.22f1` and reached **0 C# errors** after fixing two Unity-6.3 API issues:

- removed `AudioListener.pause` calls that produced `CS0103`
- removed inaccessible `UniversalRenderPipelineAsset.EnsureGlobalSettings()` call that produced `CS0122`

### Core runtime loop — PASSED

Confirmed in the real Unity editor on 2026-08-23:

- complete slice generation works
- `Bunker_Hub` starts successfully
- Deploy loads `DeadCity_VerticalSlice`
- gameplay starts successfully
- prototype enemies chase/attack and can kill the player
- failed-run logic triggers after death
- death → automatic return to `Bunker_Hub` works
- player can collect Scrap
- extraction-zone progress works
- successful extraction completes
- successful extraction → automatic return to `Bunker_Hub` works
- secured Scrap increases after extraction
- extraction streak increments
- secured Scrap and streak remain after leaving and re-entering Play Mode
- latest HUD/extraction hardening is therefore confirmed compiling/running in the real editor

This validates the first complete DEADREACH gameplay loop:

**Bunker → Deploy → Combat/Loot → Extraction or Death → Bunker → persistent result**

### Remaining foundation merge gates

Still to verify manually before PR #1 is merged:

1. Pause → Resume during expedition.
2. Pause → Abandon Run → return to Bunker, carried loot lost, failed-run accounting/streak reset correct.
3. Switch Performance / Balanced / Ultra from the Bunker and confirm no blocking errors.

Mobile touch on physical device and iOS/Android builds are important next-stage validation, but are no longer blockers for merging the editor foundation PR.

## 8. Immediate next step

### Finish Foundation Gate

In the current generated slice:

1. Deploy to Dead City.
2. Open Pause and verify Resume.
3. Open Pause again and choose Abandon Run; confirm return to Bunker and failed-run state.
4. In Bunker switch Performance → Balanced → Ultra and confirm each selection works without Console errors.
5. Report result.
6. If all pass: mark PR #1 ready, merge to `main`, and create the first production/game-feel branch.

### Production Pass 0.2 immediately after merge

Priority order:

1. production-grade mobile HUD / twin-stick visuals + haptics architecture
2. real survivor character integration + animation controller
3. real infected character integration + animation states
4. weapon model and weapon presentation architecture
5. muzzle flash / tracer / impact / hit feedback VFX
6. audio framework and first combat audio pass
7. Dead City environment-art replacement pass
8. URP post-processing / color grading / atmosphere
9. data-driven weapon definitions, rarity and affixes
10. real run inventory/equipment
11. deeper risk/reward choices and multiple extraction decisions
12. bunker room upgrade architecture
13. first boss encounter
14. Addressables/content organization
15. backend/accounts/leaderboards/events
16. IAP cosmetics/season structure
17. iOS/Android build pipeline + TestFlight/device profiling

## 9. Test runbook

Detailed acceptance checklist:

`docs/VERTICAL_SLICE_01_TEST.md`

## 10. Non-negotiables

- No cheap generic mobile finish.
- Primitive geometry is temporary scaffolding.
- Mobile performance matters from the first production art/shader decisions.
- Keep gameplay modular/data-driven.
- Do not merge untested critical runtime behavior.
- Update this file after every major pass.
- Store name stays **DEADREACH**.
- No advertising SDKs or ad-based rewards.

## 11. Handoff protocol

When resuming in another chat:

1. Read this file first.
2. Inspect latest commits and PR #1.
3. Check whether Pause/Abandon + graphics presets have passed.
4. If yes, merge PR #1 and begin Production Pass 0.2.
5. Update this file before ending the next major pass.

Do not rely on chat history alone.
