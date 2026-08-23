# DEADREACH — Project State

_Last updated: 2026-08-23_

Canonical handoff for DEADREACH. Update after every major implementation, validation and merge. Do not rely on chat history alone.

## 1. Product identity

- **Game:** DEADREACH
- **Studio:** Kamilunavo
- **Repository:** `acciento89-bot/DEADREACH`
- **Platforms:** iOS + Android
- **Unity:** `6000.3.22f1`
- **Render pipeline:** URP 17.3
- **iOS Bundle ID:** `de.kamilunavo.deadzone`
- **App Store SKU:** `deadzone-001`
- **Monetization:** IAP only, no ads

Core loop:

**Bunker → Deploy → Expedition → Combat → Loot → Risk decision → Extract / Die / Abandon → Bunker → Equip / Upgrade → Deploy stronger**

## 2. Validated / merged baselines

### Vertical Slice 0.1 — MERGED / VALIDATED
Merge `e4d5dbe2c52d3e9aeed52f421fdd99f7c6b01877`.

### Production 0.2 — MERGED / VALIDATED
Merge `fd0dca0ece7d18ca005f2f4b52d65039904fad27`.

### Production 0.3 — MERGED / REAL UNITY VALIDATED
PR #3 merge `924e8ff4ae250da13fd0d198b121802cf80131b0`.

Locked weapon decision from 0.3:
- use Quaternius `SingleWeapon` survivor exports
- use their artist-authored embedded firearm
- derive muzzle from that embedded firearm
- **never reintroduce the failed external Rifle hand-socket transform path**

### Production 0.4 — MERGED / REAL UNITY VALIDATED
PR #4 squash merge `e86c067720f8f6badc6c8a29e41bcd856c29ffe6`.

Validated in real Unity:
- 0 compiler errors
- Dead City real streets / containers / vehicles / barriers / props
- lighting / fog / post-processing / extraction beacon
- DEADREACH-owned environment collision bounds
- Bunker-first Play Mode flow
- extraction traversal safety
- movement / aim / firing / loot / extraction / Bunker-return regression

0.4 remains the stable merged gameplay/environment baseline on `main`.

## 3. Current Git state

- `main`: validated 0.1 + 0.2 + 0.3 + 0.4
- active branch: **`production/0.5-bunker-progression-boss-ui`**
- PR #5 remains **Draft**
- 0.5 first compile gate previously passed with **0 red C# errors** in real Unity
- second Bunker UI polish is visually accepted as a good **Unity Editor/Desktop** direction
- 0.5 has now been expanded once more before runtime acceptance:
  - Arsenal weapon preview auto-orientation fix
  - real distinct operator models
  - automatic Quaternius operator-art bootstrap
  - operator-specific animation controller generation with Sam-controller fallback
  - one consolidated **MEGA Runtime Gate**
- because code changed after the earlier compile pass, require a fresh 0-error compile before the Mega Runtime Gate

## 4. Production 0.5 — Big Update currently on branch

### 4.1 Save / progression

Save schema v4 persists:
- secured Scrap / extractions / streaks
- weapon stash + equipped primary
- highest unlocked level
- selected level
- highest completed level
- boss kills
- selected operator
- unlocked operators
- owned-content entitlement IDs

Campaign cap: **50 levels**.
Successful extraction can unlock the next level. Every tenth level is a boss operation.

### 4.2 Bunker Command Center

`Assets/Deadreach/Runtime/UI/BunkerCommandCenterUI.cs`

Tabs:
- OVERVIEW
- ARSENAL
- OPERATORS
- CAMPAIGN
- STORE

Second UI polish uses anchor-based layout and was accepted from real Unity screenshots as a good desktop/editor direction:
- DEADREACH header visible
- Overview no longer overlaps
- Arsenal has dedicated list + inspector column
- Operators has roster + 3D preview column
- Campaign shows one 10-level sector at a time
- Store cards / navigation / deploy framing coherent

This is **not yet mobile acceptance**.

### 4.3 Arsenal / weapon preview

Arsenal shows:
- persistent stash
- rarity
- item power
- all rolled affixes
- equipped state
- persistent Equip action
- rotating 3D production weapon inspector

Latest fix:
`Assets/Deadreach/Runtime/UI/BunkerWeaponPreviewUI.cs`

The 3D inspector no longer assumes an imported weapon axis. It evaluates orthogonal preview rotations and chooses the orientation that maximizes horizontal screen width while penalizing vertical/depth extent. This is **preview-only** and never changes gameplay weapon transforms or muzzle binding.

Reason: real Unity screenshot showed the rifle standing vertically / effectively on its head in the Arsenal preview.

### 4.4 Real distinct operators

`Assets/Deadreach/Runtime/Progression/OperatorCatalog.cs`
`Assets/Deadreach/Runtime/Presentation/ProductionAssetCatalog.cs`
`Assets/Deadreach/Runtime/Presentation/ProductionVisualBinder.cs`
`Assets/Deadreach/Runtime/UI/BunkerOperatorPreviewUI.cs`

Profiles remain:
- **SAM / Ranger** — balanced
- **RAVEN / Scout** — faster / less durable
- **BRIGGS / Warden** — slower / tougher / harder hitting

They now map to distinct Quaternius `SingleWeapon` characters from the same CC0 Zombie Apocalypse Kit:
- **SAM → Characters_Sam_SingleWeapon**
- **RAVEN → Characters_Lis_SingleWeapon**
- **BRIGGS → Characters_Matt_SingleWeapon**

This replaces the temporary three-recolored-Sam approach.

Runtime selection now changes the actual survivor prefab used by `ProductionVisualBinder`, so the selected model must appear both in the Bunker operator preview and in Dead City gameplay.

Do not tint these authored models in `OperatorRuntimeApplier`; preserve their real appearance. Stats still alter health / movement / weapon damage.

The Operator preview is turned around to face the preview camera rather than permanently showing the back.

### 4.5 Automatic operator-art bootstrap

New editor system:

`Assets/Deadreach/Editor/Production05OperatorArtSetup.cs`

`DEADREACH > Build Production Slice 0.5` now automatically ensures Lis/Matt operator art is present.

If missing, Unity Editor downloads the two known CC0 Quaternius `SingleWeapon` glTF files from the same public mirror already used for the project, normalizes the local `Zombie_Atlas.png` reference, imports them synchronously, builds production wrapper prefabs, and stores them in `ProductionAssetCatalog`.

Animator hardening:
- Lis and Matt get dedicated animator controllers built from their own imported clips when available
- if an export exposes no clips, the already validated Sam controller is used only as a fallback

Therefore the user workflow remains **git pull + Unity build menu**; no manual transform/prefab setup is intended.

Manual recovery menu if needed:

`DEADREACH > Production > Repair 0.5 Operator Art`

### 4.6 Campaign / sectors / enemies

Five 10-level campaign sectors:
1. Dead City
2. Flooded Industrial
3. Ash District
4. Blackout Sector
5. Ground Zero

0.5 still uses the validated Dead City geometry as the common map foundation; sector identity currently changes difficulty + atmosphere. Separate authored sector maps are later content and must not be falsely claimed complete.

Runtime infected archetypes:
- Walker
- Runner
- Brute
- Stalker

They vary movement speed / health / damage / scale while retaining the validated Quaternius infected visual family.

### 4.7 Boss every 10 levels

Boss operations:
- Level 10
- Level 20
- Level 30
- Level 40
- Level 50

Boss:
- enlarged high-health infected
- tier scaling
- mutation/aggression phases around 66% and 33% HP
- boss HUD/health bar
- extraction sealed until boss death

Editor shortcut for acceptance:

`DEADREACH > Dev > 0.5 Select Boss Level 10`

### 4.8 Combat presentation

`Assets/Deadreach/Runtime/Feedback/CombatFeedbackPresenter.cs`

0.5 replaces prototype combat presentation with:
- pooled bright tracer core
- glow trail
- muzzle flash
- directional environment sparks
- directed infected gore/spark streaks
- stronger critical feedback
- no old large red square/billboard impact marker

The artist-authored embedded firearm remains the muzzle source for every operator.

### 4.9 Store surface

Store exposes production-facing cards for:
- operator cosmetics
- Bunker themes
- weapon finishes
- season content

No fake purchase is allowed. StoreKit / Google Play verification remains a later integration gate.

## 5. Mandatory mobile UI release gate

Current accepted screenshots are **Unity Editor/Desktop preview only**.

Before App Store / Play release, a separate landscape-mobile UI gate must cover at minimum:
- `Screen.safeArea`
- notch / Dynamic Island / rounded corners
- representative iPhone + Android landscape aspect ratios
- small physical screen readability
- minimum touch target size/spacing
- responsive reflow rather than only shrinking
- Arsenal preview/list separation on narrow screens
- Operator preview/selection readability
- Campaign sector/grid usability
- Store stacking/scrolling
- gameplay HUD/twin-stick safe area
- real-device validation on at least one notched iPhone and one Android phone

Do **not** mark UI/release final until this mobile gate passes.

## 6. Next action — ONE MEGA Runtime Gate

The user explicitly requested no sequence of tiny runtime approvals.

Canonical test plan:

`docs/PRODUCTION_05_TEST.md`

Workflow:
1. `git pull`
2. fresh Unity compile → require 0 red errors
3. run `DEADREACH > Build Production Slice 0.5` once
4. perform the complete Mega Runtime Gate in one session:
   - Bunker/menu/persistence
   - horizontal Arsenal weapon preview
   - SAM/Lis/Matt distinct Operator previews
   - non-Sam Level 1 gameplay model
   - operator stat differences
   - movement / aim / combat / Combat FX
   - Walker/Runner/Brute/Stalker variety
   - loot / extraction / Level 2 unlock
   - cross-operator runtime swap
   - abandon regression
   - Level 10 boss / phase behavior / extraction seal
   - boss clear / Level 11 unlock
   - final regression sweep
5. only after that full real-Unity pass may PR #5 be marked ready for merge

If a blocker appears, fix the first actual blocker in branch and rerun the affected portion + final regression sweep.

## 7. Handoff protocol

When resuming:
1. read this file first
2. treat 0.1–0.4 as merged/validated baselines
3. never reintroduce external rifle hand/socket transforms on production operators
4. all three 0.5 operators must use artist-authored SingleWeapon rigs
5. active work is `production/0.5-bunker-progression-boss-ui`
6. PR #5 remains Draft
7. desktop/editor Bunker layout direction is accepted; mobile UI acceptance is still pending
8. latest unvalidated additions are distinct Sam/Lis/Matt operators + automatic art bootstrap + weapon preview auto-orientation
9. next step is fresh compile then the single `docs/PRODUCTION_05_TEST.md` Mega Runtime Gate
10. update this file after runtime acceptance/merge
