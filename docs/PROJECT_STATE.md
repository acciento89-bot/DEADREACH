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

Runtime presets: Performance / Balanced / Ultra.

## 4. Milestone status

### Vertical Slice 0.1 — ACCEPTED

All editor-foundation gates passed in real Unity `6000.3.22f1` on 2026-08-23.

Confirmed:

- clean compile with **0 C# errors**
- complete Bunker + Dead City generation
- deterministic Build Settings: Bunker index 0, Dead City index 1
- Bunker starts
- Deploy loads Dead City
- movement / aiming / firing work
- infected chase, melee and player death work
- death registers failed run and returns to Bunker
- Scrap collection works
- extraction progress works
- successful extraction returns to Bunker
- secured Scrap increases
- extraction streak increments
- secured Scrap + streak persist after leaving/re-entering Play Mode
- Pause → Resume works
- Pause → Abandon Run returns to Bunker with failed-run accounting
- Performance / Balanced / Ultra switching works without blocking Console errors

Validated core loop:

**Bunker → Deploy → Combat/Loot → Extraction / Death / Abandon → Bunker → persistent result**

Unity-6.3 compatibility fixes found during live validation:

- removed unsupported `AudioListener.pause` calls
- removed inaccessible `UniversalRenderPipelineAsset.EnsureGlobalSettings()` call
- centralized scene Build Settings registration to prevent Dead-City-only return failures

Physical-device touch and iOS/Android builds are deferred to Production Pass 0.2/device validation and were not blockers for the foundation merge.

## 5. Foundation implemented

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

Gameplay currently implemented:

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

Dev-only generators:

`DEADREACH > Dev > ...`

Generated scenes:

- `Assets/Deadreach/Scenes/Bunker_Hub.unity`
- `Assets/Deadreach/Scenes/DeadCity_VerticalSlice.unity`

Repair command:

`DEADREACH > Project > Repair Scene Build Settings`

## 7. Git / release state

- `foundation/unity-6.3` — Vertical Slice 0.1 foundation, fully validated and ready for merge
- PR #1 — **foundation: Unity 6.3 vertical slice 0.1 — validated**
- next branch after merge: `production/0.2-gamefeel`

## 8. Production Pass 0.2 — NEXT

Goal: stop looking like a systems prototype and establish the first production-quality gameplay presentation while preserving mobile performance.

Priority:

1. production mobile HUD / visible twin-stick controls + haptics architecture
2. character presentation hooks + animation-state architecture for real survivor asset integration
3. infected presentation/animation-state architecture
4. data-driven weapon definitions and runtime weapon configuration
5. muzzle flash / tracer / impact / hit-feedback VFX architecture
6. audio event framework and first combat audio pass
7. Dead City production environment replacement path
8. URP post-processing / color grading / atmosphere
9. rarity / affix model for weapons
10. real run inventory/equipment
11. deeper risk/reward decisions and multiple extraction choices
12. bunker room upgrade architecture
13. first boss encounter
14. Addressables/content organization
15. backend/accounts/leaderboards/events
16. IAP cosmetics/season structure
17. iOS/Android build pipeline + physical-device profiling

## 9. Immediate next step

After PR #1 merge:

1. create `production/0.2-gamefeel` from updated `main`
2. add production gameplay/presentation architecture in a large pass
3. keep generated primitive geometry only as fallback scaffolding
4. require another clean Unity compile before merging Production Pass 0.2
5. begin physical-device touch validation and mobile build preparation

## 10. Non-negotiables

- No cheap generic mobile finish.
- Primitive geometry is temporary scaffolding.
- Mobile performance matters from the first production art/shader decisions.
- Keep gameplay modular/data-driven.
- Update this file after every major pass.
- Store name stays **DEADREACH**.
- No advertising SDKs or ad-based rewards.

## 11. Handoff protocol

When resuming in another chat:

1. Read this file first.
2. Inspect latest commits / open PRs / current production branch.
3. Continue from **Immediate next step** or the newest recorded milestone.
4. Update this file before ending a major pass.

Do not rely on chat history alone.
