# DEADREACH — Project State

_Last updated: 2026-08-23_

This is the canonical handoff file for DEADREACH. Update it after every major development, validation, build, architecture, backend, monetization, store, or release step.

## 1. Product identity

- **Game:** DEADREACH
- **Studio:** Kamilunavo
- **Repository:** `acciento89-bot/DEADREACH`
- **Platforms:** iOS + Android
- **Unity:** `6000.3.22f1`
- **Render pipeline:** URP 17.3
- **iOS Bundle ID:** `de.kamilunavo.deadzone`
- **App Store SKU:** `deadzone-001`
- **Monetization:** IAP only
- **Advertising:** none

Store-facing name remains **DEADREACH**. The older internal `deadzone` identifier is intentional.

## 2. Locked game direction

Premium-feeling mobile 3D survival / extraction roguelite with persistent progression.

Core loop:

**Bunker → Deploy → Expedition → Combat → Loot → Risk decision → Extract / Die / Abandon → Persistent result → Bunker → Equip / Upgrade → Deploy stronger**

Long-term pillars:

- high-angle real-time 3D combat
- extraction risk/reward
- loot-driven weapon/equipment builds
- persistent survivor progression
- visible bunker progression
- bosses and replayable zones
- dailies/weeklies/seasons
- collections, achievements, streaks, leaderboards
- zero ads

## 3. Technical direction

- Unity 6.3 LTS, pinned `6000.3.22f1`
- C#
- URP 17.3
- Unity Input System 1.17
- Cinemachine available
- custom perspective high-angle camera currently active
- IL2CPP for iOS / Android
- landscape
- Linear color space
- Force Text serialization
- GitHub + Git LFS
- Addressables planned
- backend/accounts/leaderboards/events planned
- Supabase remains an option
- Apple / Google IAP planned
- production NavMesh AI planned; current AI is direct steering

Graphics target remains real 3D characters/environments, PBR, proper lighting/probes, post-processing, animation, audio and scalable mobile quality. Primitive geometry is temporary scaffolding only.

## 4. Vertical Slice 0.1 — ACCEPTED / MERGED

PR #1 `foundation: Unity 6.3 vertical slice 0.1 — validated` was merged to `main` on 2026-08-23.

Merge commit:

`e4d5dbe2c52d3e9aeed52f421fdd99f7c6b01877`

Validated in real Unity:

- 0 C# errors
- Bunker + Dead City generation
- Bunker → Deploy → gameplay
- movement / aiming / firing
- infected chase / melee
- death → failed run → Bunker
- Scrap collection
- successful extraction → Bunker
- Scrap + streak persistence
- Pause → Resume
- Pause → Abandon
- Performance / Balanced / Ultra switching

## 5. Production Pass 0.2 — ACCEPTED / READY TO MERGE

Branch: `production/0.2-gamefeel`

PR #2: `production: game feel, weapon loot and equipment loop 0.2`

### Implemented

- visible dynamic mobile twin-stick overlay
- gamepad/mobile haptics architecture
- decoupled combat feedback bus
- runtime tracer / impact / critical-hit feedback
- pooled audio service + data-driven AudioCue assets
- survivor/infected Animator integration hooks
- ScriptableObject weapon definitions
- Common / Uncommon / Rare / Epic / Legendary weapon rarities
- individual serializable weapon instances
- item power
- Damage / Fire Rate / Range / Crit Chance / Crit Damage affixes
- depth-influenced weapon loot rolls
- run-only weapon inventory, capacity 6
- two generated weapon cases in Dead City
- death/abandon loses unsecured weapon loot
- extraction banks weapon loot into persistent stash
- extraction accepts Scrap OR weapon loot
- save schema v3 with stash + equipped primary + migration
- Bunker stash/equipment presentation
- `EQUIP NEXT STASH WEAPON`
- equipped affixes modify next-run combat stats
- critical-hit feedback
- FIELD HUD shows equipped weapon / power / damage / crit / run weapon-loot count

Established progression loop:

**Find weapon → survive → extract → stash → equip → next run becomes stronger/different**

## 6. Production 0.2 real Unity validation — PASSED

Confirmed by the user in real Unity `6000.3.22f1` on 2026-08-23.

### Compile gate

- **0 red compiler errors**
- **0 yellow compiler warnings**

Unity-6.3 compatibility fixes during the gate:

- enabled `com.unity.modules.audio` `1.0.0`
- enabled `com.unity.modules.animation` `1.0.0`
- enabled `com.unity.modules.particlesystem` `1.0.0`
- scoped the mobile-only haptics timestamp field to mobile compilation

### Runtime gate

User confirmed the complete Production 0.2 test pass works as expected, including:

- `DEADREACH > Build Production Slice 0.2` generation
- Bunker start + previous profile loading
- Deploy to Dead City
- movement / aiming / shooting without regressions
- tracer / impact feedback
- weapon-case pickup + run weapon-loot HUD
- unsecured weapon loss on death/abandon
- successful weapon extraction into persistent stash
- rarity / item power / affix display
- equipped-primary persistence
- equipped affixes reflected in next-run combat stats
- critical-hit feedback
- stash/equipment persistence across Play Mode restart
- foundation death / extraction / pause / abandon flows still working
- no blocking Console errors

Production 0.2 is therefore **runtime-accepted and may be merged to `main`**.

Physical-device touch/haptics and iOS/Android builds remain separate device/build validation tasks; they were not part of this Windows Editor runtime gate.

## 7. Deliberate limitations after 0.2

- survivor/infected/environment geometry is still temporary prototype art
- Animator hooks exist but real production character assets/controllers are not yet integrated
- VFX are functional placeholders
- audio framework exists but final clips are missing
- weapon-case geometry is temporary
- production NavMesh navigation is pending
- current Bunker/HUD presentation still uses prototype IMGUI
- physical-device touch/haptics still needs device validation

## 8. Next milestone — Production Art / Presentation 0.3

Start from updated `main` after PR #2 merge.

Priority:

1. real survivor model/prefab integration path + production Animator controller
2. real infected model/prefab integration path + animation sets
3. real weapon model / muzzle / attachment presentation
4. pooled muzzle / tracer / impact VFX
5. first real combat audio-content pass
6. Dead City production environment-art replacement framework
7. URP post-processing / color grading / atmosphere
8. replace prototype IMGUI with production HUD/loadout UI
9. NavMesh production enemy navigation
10. physical-device mobile input/haptics validation
11. iOS/Android build pipeline + profiling
12. deeper run choices / multiple extraction decisions
13. bunker upgrades
14. first boss
15. Addressables/content organization
16. backend/accounts/leaderboards/events
17. IAP cosmetics / season structure

## 9. Non-negotiables

- No cheap generic mobile finish.
- Primitive geometry remains temporary scaffolding.
- Mobile performance matters from the first production-art decisions.
- Keep gameplay modular and data-driven.
- Update this file after every major pass.
- Store name stays **DEADREACH**.
- No advertising SDKs or ad-based rewards.

## 10. Handoff protocol

When resuming in another chat:

1. read this file first
2. verify PR #2 merge status
3. if merged, continue on the Production Art / Presentation 0.3 branch
4. distinguish clearly between implemented, editor-validated, device-validated, and production-art-complete states
5. update this file before ending the next major pass

Do not rely on chat history alone.
