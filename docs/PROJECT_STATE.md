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

Real Unity validation passed:

- 0 C# compiler errors
- Bunker + Dead City generation
- deterministic Build Settings
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

Validated foundation loop:

**Bunker → Deploy → Combat/Loot → Extraction / Death / Abandon → Bunker → persistent result**

## 5. Current Git state

- `main` — accepted Vertical Slice 0.1
- `production/0.2-gamefeel` — active Production Pass 0.2
- PR #2 — `production: game feel, weapon loot and equipment loop 0.2`
- PR #2 remains **Draft** until runtime validation passes

## 6. Production Pass 0.2 — IMPLEMENTED

### Mobile input / feedback

- visible dynamic twin-stick overlay over the existing proven touch input
- Safe Area handling
- overlay hidden in Bunker
- gamepad shot/damage/death rumble
- stronger critical-hit feedback
- coarse iOS/Android vibration fallback

### Combat feedback / VFX architecture

- decoupled `CombatFeedback` event bus
- runtime tracers
- impact particles
- damageable/non-damageable differentiation
- critical-hit differentiation
- temporary runtime VFX only; final production VFX still pending

### Audio architecture

- data-driven `AudioCue`
- pooled `AudioService`
- weapon shot / impact cue references
- final audio content not yet supplied

### Character presentation hooks

- `PlayerAnimationDriver`
- `InfectedAnimationDriver`
- infected attack presentation event
- gameplay remains decoupled from future Animator/model assets

### Data-driven weapon / loot progression

- ScriptableObject weapon definitions
- rarities: Common / Uncommon / Rare / Epic / Legendary
- serializable individual weapon instances
- item power
- random affixes:
  - Damage %
  - Fire Rate %
  - Range %
  - Crit Chance %
  - Crit Damage %
- deeper weapon cases improve rarity odds
- rarity controls affix count

### Run inventory / stash / equipment

- run-only weapon inventory capacity 6
- two generated weapon cases in Dead City
- death / abandon loses unsecured weapon loot
- successful extraction banks weapon instances into persistent stash
- extraction accepts Scrap OR weapon loot
- save schema v3 persists stash + equipped primary
- old foundation profile migrates instead of intentionally resetting
- first extracted weapon auto-equips when no primary exists
- Bunker can cycle equipped stash weapon
- equipped weapon modifies next-run damage / fire rate / range / crit stats

Established progression loop:

**Find weapon → survive → extract → stash → equip → next run becomes stronger/different**

### UI additions

Bunker displays:

- stash count
- equipped primary
- rarity / item power
- up to three affixes
- recent extracted weapons
- `EQUIP NEXT STASH WEAPON`

Field HUD displays:

- equipped weapon / BASE state
- item power
- resolved damage
- resolved crit chance
- carried Scrap
- weapon-loot inventory count

## 7. Production 0.2 real Unity validation

### Compile gate — PASSED

Confirmed by the user in real Unity `6000.3.22f1` on 2026-08-23:

- **0 red compiler errors**
- **0 yellow compiler warnings**

Compatibility fixes required during this gate:

- enabled built-in Unity Audio module: `com.unity.modules.audio` `1.0.0`
- enabled built-in Unity Animation module: `com.unity.modules.animation` `1.0.0`
- enabled built-in Unity Particle System module: `com.unity.modules.particlesystem` `1.0.0`
- scoped `_lastMobileVibrationTime` to mobile platforms to eliminate Windows-editor `CS0414`

This means Production 0.2 now **compiles cleanly**, but the new runtime/progression features are not yet fully validated.

### Runtime gate — NEXT

Primary editor command:

**`DEADREACH > Build Production Slice 0.2`**

Required runtime checks:

1. run `Build Production Slice 0.2`
2. confirm generator finishes without red Console errors
3. Play from Bunker
4. confirm previous Scrap / streak data still loads
5. Deploy to Dead City
6. confirm movement / aiming / shooting still work
7. confirm tracer + impact feedback appears
8. collect a weapon case and verify HUD shows `WEAPON LOOT 1/6`
9. intentionally die or abandon; confirm unsecured weapon does **not** enter stash
10. run again, collect weapon case, successfully extract
11. confirm `WEAPON STASH` increases in Bunker
12. confirm rarity / item power / affixes display
13. confirm extracted primary auto-equips if no previous weapon was equipped
14. use `EQUIP NEXT STASH WEAPON` when multiple weapons exist
15. Deploy again and confirm FIELD HUD shows equipped weapon / power / changed damage/crit values
16. confirm equipped affixes actually affect combat stats
17. verify critical-hit feedback appears during combat
18. re-check death / extraction / pause / abandon foundation behavior for regressions
19. leave and re-enter Play Mode; confirm stash + equipped weapon persist
20. require no blocking Console errors

Detailed runbook:

`docs/PRODUCTION_02_TEST.md`

## 8. Deliberate limitations of 0.2

- primitive survivor / infected / environment geometry is still not production art
- Animator hooks exist but real models/controllers are not yet integrated
- VFX are placeholders
- audio framework exists but final clips are missing
- weapon-case geometry is temporary
- tracer spawning should later be pooled
- rarity presentation should later use shared materials / MaterialPropertyBlock
- platform haptics should later use higher-quality native implementation
- production NavMesh navigation still pending
- current UI is still prototype IMGUI rather than final production UI

## 9. After 0.2 runtime validation

Priority:

1. real survivor 3D model + animation controller
2. real infected models + animation sets
3. real weapon models / attachment presentation
4. proper pooled muzzle / tracer / impact VFX
5. first real audio-content pass
6. Dead City production environment-art replacement
7. URP post-processing / color grading / atmosphere
8. proper stash/loadout UI
9. deeper run choices / multiple extraction decisions
10. bunker upgrades
11. first boss
12. Addressables/content organization
13. iOS/Android build pipeline + physical-device profiling
14. backend/accounts/leaderboards/events
15. IAP cosmetics / season structure

## 10. Non-negotiables

- No cheap generic mobile finish.
- Primitive geometry remains temporary scaffolding.
- Mobile performance matters from the first production-art decisions.
- Keep gameplay modular and data-driven.
- Do not merge PR #2 until real runtime validation passes.
- Update this file after every major pass.
- Store name stays **DEADREACH**.
- No advertising SDKs or ad-based rewards.

## 11. Handoff protocol

When resuming in another chat:

1. read this file first
2. inspect PR #2 / current production branch
3. note that Production 0.2 compile gate has passed with 0 errors / 0 warnings
4. continue with the Production 0.2 runtime gate if it has not yet passed
5. update this file before merge / next major production pass

Do not rely on chat history alone.
