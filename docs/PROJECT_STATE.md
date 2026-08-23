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

**Bunker → Deploy → Expedition → Combat → Loot → Risk decision → Extract / Die / Abandon → Persistent result → Bunker → Equip / Upgrade → Deploy stronger**

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
- **Current camera:** custom perspective high-angle follow rig
- **Serialization:** Force Text
- **Version control:** GitHub + Git LFS
- **iOS / Android scripting backend:** IL2CPP
- **Orientation:** landscape
- **Color space:** Linear
- **Backend:** not implemented; Supabase remains an option
- **IAP:** Apple + Google storefront integration planned
- **Addressables:** planned
- **Production AI navigation:** planned; current prototype uses direct steering

Graphics target: real 3D environments/characters, PBR, proper lighting/probes, post-processing, VFX, animation, audio and scalable mobile quality. Primitive prototype art remains temporary scaffolding.

## 4. Milestone status

### Vertical Slice 0.1 — ACCEPTED / MERGED

PR #1 **`foundation: Unity 6.3 vertical slice 0.1 — validated`** was merged to `main` on 2026-08-23.

Squash merge commit:

`e4d5dbe2c52d3e9aeed52f421fdd99f7c6b01877`

All editor-foundation gates passed in real Unity `6000.3.22f1`:

- 0 C# compiler errors
- Bunker + Dead City generation
- Bunker → Deploy → gameplay
- movement / aiming / firing
- infected chase / melee / death
- player death → failed run → Bunker
- Scrap collection
- extraction progress
- successful extraction → Bunker
- secured Scrap + extraction streak persistence
- Pause → Resume
- Pause → Abandon → failed-run accounting
- Performance / Balanced / Ultra switching

Validated foundation loop:

**Bunker → Deploy → Combat/Loot → Extraction / Death / Abandon → Bunker → persistent result**

## 5. Current Git state

- `main` — contains accepted Vertical Slice 0.1 foundation
- `production/0.2-gamefeel` — active Production Pass 0.2 branch
- PR #1 — merged
- Production Pass 0.2 is **NOT YET Unity-compiled or runtime-validated** after the changes listed below

Do not claim Production Pass 0.2 is stable until the next real Unity validation gate passes.

## 6. Production Pass 0.2 — IMPLEMENTED ON BRANCH, AWAITING UNITY VALIDATION

### A. Mobile controls / feedback

`DeadreachInput.cs` now exposes active touch-stick state without replacing the proven input path.

`MobileTwinStickOverlay.cs` adds a visible dynamic mobile overlay:

- left MOVE zone / dynamic movement stick
- right AIM/FIRE reticle
- Safe Area handling
- hidden in Bunker
- automatically shown for mobile/touchscreen targets
- presentation-only layer over the existing input system

`HapticsService.cs` adds:

- gamepad shot rumble
- stronger critical-hit rumble
- damage/death feedback
- coarse iOS/Android vibration fallback

### B. Combat feedback / VFX architecture

`CombatFeedback.cs` is now the decoupled gameplay-feedback event bus.

Events/payloads cover:

- shot origin/end point
- damageable hit
- critical hit
- impact point/normal
- player damage
- player death

`CombatFeedbackPresenter.cs` currently provides temporary runtime presentation:

- visible tracers
- hit impacts
- different damageable/non-damageable feedback
- stronger magenta critical-hit feedback

This runtime VFX is an architecture/proof layer, not final production VFX art.

### C. Audio architecture

`AudioCue.cs` provides data-driven audio assets with:

- clip variations
- volume
- random pitch range
- spatial blend
- max distance

`AudioService.cs` provides a small pooled runtime AudioSource system.

`WeaponDefinition` now supports shot and impact AudioCue references.

No final audio clips have been added yet.

### D. Real character/infected integration hooks

`PlayerAnimationDriver.cs` exposes Animator parameters/triggers for:

- Speed
- IsMoving
- IsAiming
- IsDead
- Hit

`InfectedAnimationDriver.cs` exposes:

- Speed
- Attack
- Hit
- IsDead

`InfectedChaser` now exposes an Attack event for presentation without moving animation logic into AI.

The generated slice attaches these drivers even when no real Animator/model is present, allowing future real assets to be plugged in without rewriting gameplay.

### E. Data-driven weapons

`WeaponDefinition.cs` introduces ScriptableObject weapon definitions.

Current weapon metadata/stat support:

- weapon ID / display name
- archetype
- rarity
- base damage
- rounds per second
- range
- aim turn speed
- haptic strength
- tracer duration/width
- shot/impact AudioCues

Weapon rarities:

- Common
- Uncommon
- Rare
- Epic
- Legendary

### F. Weapon instances, rarity and affixes

`WeaponInstanceData.cs` adds serializable individual loot instances.

Each extracted weapon can contain:

- unique instance ID
- definition ID
- display-name snapshot
- rarity
- item power
- random affixes

Current affix pool:

- Damage %
- Fire Rate %
- Range %
- Crit Chance %
- Crit Damage %

`WeaponLootFactory.cs` provides deterministic prototype rolls by seed + normalized zone depth.

Deeper weapon cases improve rarity odds. Affix count scales by rarity:

- Common: 0
- Uncommon: 1
- Rare: 2
- Epic: 3
- Legendary: 4

### G. Run inventory → extraction → permanent stash

`RunInventory.cs` adds a run-only weapon inventory with current capacity 6.

`WeaponLootPickup.cs` creates extractable weapon loot with rarity presentation.

Production Slice 0.2 adds two prototype weapon cases:

- mid-zone weapon case
- deeper/higher-value weapon case

Rules now implemented:

- picked-up weapons remain unsecured during the run
- death/abandon clears run weapon loot
- successful extraction snapshots the run inventory
- extracted weapons are cloned into the persistent profile stash
- extraction can be completed with either Scrap or weapon loot

### H. Save schema / persistent equipment

`SaveService` schema is now version 3.

It persists:

- secured Scrap
- successful extractions
- failed runs
- current/best extraction streak
- weapon stash
- equipped primary weapon ID

Old foundation save data is migrated rather than intentionally reset.

First extracted weapon auto-equips if no primary weapon was previously selected.

### I. Equipped loot actually changes the next run

`WeaponStatResolver.cs` converts the equipped weapon's affixes into runtime combat stats.

`HitscanWeapon.cs` now:

- loads the equipped persistent primary weapon
- resolves damage/fire-rate/range bonuses
- applies Crit Chance + Crit Damage
- produces stronger critical-hit VFX/haptic feedback
- remains compatible with base/fallback weapon stats if the stash is empty

This establishes the first real progression loop:

**Find weapon → survive → extract → weapon enters stash → equip → next run is stronger/different**

### J. Bunker + HUD additions

Bunker UI now displays:

- weapon stash count
- equipped primary weapon
- rarity / item power
- up to three affixes
- recent extracted weapons
- equipped marker
- `EQUIP NEXT STASH WEAPON` control

Field HUD now displays:

- equipped primary name
- item power or BASE state
- resolved damage
- resolved critical chance
- carried Scrap
- weapon-loot inventory count

### K. Production slice generation

The main editor command for this branch is now:

**`DEADREACH > Build Production Slice 0.2`**

It:

1. regenerates Dead City
2. attaches presentation/animation hooks
3. adds RunInventory
4. adds mid/deep weapon cases
5. regenerates the Bunker
6. verifies Bunker index 0 / Dead City index 1
7. opens Bunker ready for Play Mode

Dev-only individual builders remain under:

`DEADREACH > Dev > ...`

## 7. Production 0.2 validation gate — NEXT

The code above has been committed but has **not yet been compiled in the user's real Unity editor**.

Required next validation on Windows / Unity `6000.3.22f1`:

1. switch local repo to `production/0.2-gamefeel`
2. pull latest branch
3. wait for Unity compilation
4. require **0 red compiler errors**
5. run **`DEADREACH > Build Production Slice 0.2`**
6. start from Bunker
7. verify old Scrap/streak save still loads
8. Deploy to Dead City
9. verify tracers/impact feedback while firing
10. collect a weapon case and confirm `WEAPON LOOT 1/6`
11. intentionally die/abandon once and confirm that unsecured weapon does NOT enter stash
12. next run collect a weapon case and successfully extract
13. confirm Bunker `WEAPON STASH` increases
14. confirm weapon rarity / power / affixes are displayed
15. use `EQUIP NEXT STASH WEAPON`
16. Deploy again and confirm FIELD HUD shows equipped name/power and changed runtime damage/crit values
17. confirm normal extraction/death/pause foundation behavior still works
18. physical-device mobile twin-stick/haptic validation comes after clean editor validation

## 8. Known deliberate limitations of 0.2

- primitive survivor/infected/environment geometry is still not production art
- Animator hooks exist, but real animated character assets/controllers are not yet supplied
- VFX are runtime placeholders, not final particles/shaders
- audio framework exists, but no final audio content is supplied
- weapon cases use temporary geometry
- tracer creation should later move to pooling for mobile optimization
- weapon pickup rarity coloring should later use shared materials / MaterialPropertyBlock instead of per-instance material access
- mobile vibration is currently coarse and should later move to better platform-specific haptics
- production NavMesh enemy navigation is still pending

## 9. After 0.2 validation

Priority:

1. real survivor 3D model + animation controller
2. real infected models + animation sets
3. real weapon model / hands/attachment presentation
4. proper pooled muzzle/tracer/impact VFX
5. first real audio content pass
6. Dead City environment art replacement
7. URP post-processing / color grading / atmosphere
8. better stash/loadout UI instead of prototype IMGUI
9. deeper run choices / multiple extraction decisions
10. bunker upgrade rooms
11. first boss
12. Addressables/content organization
13. iOS/Android build pipeline + physical-device profiling
14. backend/accounts/leaderboards/events
15. IAP cosmetics / season structure

## 10. Non-negotiables

- No cheap generic mobile finish.
- Primitive geometry is temporary scaffolding.
- Mobile performance matters from the first production art/shader decisions.
- Keep gameplay modular/data-driven.
- Do not merge Production Pass 0.2 until real Unity compilation/runtime validation passes.
- Update this file after every major pass.
- Store name stays **DEADREACH**.
- No advertising SDKs or ad-based rewards.

## 11. Handoff protocol

When resuming in another chat:

1. read this file first
2. inspect current branch / newest PR
3. determine whether Production 0.2 Unity validation has happened
4. continue from **Production 0.2 validation gate** if not
5. after validation, update this file before merge and begin the next art/content pass

Do not rely on chat history alone.
