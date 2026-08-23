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

Weapon lock from 0.3:
- use artist-authored weapon geometry already parented to the Quaternius character rig
- derive muzzle from that embedded firearm
- **never reintroduce the failed external Rifle hand-socket transform path**
- validated Sam weapon sits on the left hand and is accepted

### Production 0.4 — MERGED / REAL UNITY VALIDATED
PR #4 squash merge `e86c067720f8f6badc6c8a29e41bcd856c29ffe6`.

Validated:
- 0 compiler errors
- Dead City streets / containers / vehicles / barriers / props
- lighting / fog / post-processing / extraction beacon
- environment collision bounds
- Bunker-first Play Mode flow
- extraction traversal safety
- movement / aim / fire / loot / extraction / Bunker return regression

`main` remains the stable 0.4 baseline.

## 3. Current Git state

- active branch: **`production/0.5-bunker-progression-boss-ui`**
- PR #5 remains **Draft**
- earlier 0.5 foundation code passed real Unity compile with **0 red C# errors**
- second Bunker UI polish is accepted as a good **Unity Editor/Desktop** direction
- Atomic `Production05OperatorArtSetupV2` has now passed the real Unity `Build Production Slice 0.5` step with **no blocking error**
- the previous `Missing Nested Prefab Asset` + glTFast import blocker is therefore **resolved in real Unity**
- full Production 0.5 is **not yet accepted**; the consolidated MEGA Runtime Gate remains outstanding

## 4. Production 0.5 scope on branch

### 4.1 Persistence / campaign

Save schema v4 persists:
- secured Scrap / extractions / streaks
- stash + equipped primary
- highest unlocked / selected / highest cleared level
- boss kills
- selected operator
- unlocked operators
- owned-content entitlement IDs

Campaign cap: **50 levels** across five 10-level sectors.
Every tenth level is a boss operation.

### 4.2 Bunker Command Center

Tabs:
- OVERVIEW
- ARSENAL
- OPERATORS
- CAMPAIGN
- STORE

Accepted Editor/Desktop direction after second polish:
- DEADREACH header visible
- Overview no longer overlaps
- Arsenal list + dedicated 3D inspector column
- Operator roster + dedicated 3D preview column
- Campaign shows one sector / ten levels at a time
- coherent navigation + deploy framing

This is **not mobile acceptance**.

### 4.3 Arsenal

Shows persistent stash, rarity, item power, affix rolls, equipped state and Equip action.

`BunkerWeaponPreviewUI` now uses preview-only canonical orientation scoring so imported weapons should appear horizontally instead of standing vertically/on their head. This must never alter gameplay weapon transforms or muzzle binding.

### 4.4 Distinct operator plan — corrected after inspecting original Quaternius exports

Display profiles remain:
- **SAM / Ranger** — balanced
- **RAVEN / Scout** — faster / less durable
- **BRIGGS / Warden** — slower / tougher / harder hitting

The originally attempted Lis/Matt `SingleWeapon` mapping was rejected after inspecting the original files:
- `Characters_Lis_SingleWeapon.gltf` carries a **Guitar**
- `Characters_Matt_SingleWeapon.gltf` carries **Knife / WoodenBat_Saw**

Those are not valid shooter production operators.

Corrected production mapping:
- **SAM → Quaternius Sam SingleWeapon / artist-rigged Pistol** — already validated baseline
- **RAVEN → Quaternius Shaun SingleWeapon / artist-rigged SMG**
- **BRIGGS → Quaternius Matt full export / artist-rigged Rifle only**

The Matt full export contains Axe, Guitar, Knife, Pistol, Rifle, Shotgun, SMG, Spear and bat variants already parented to the artist hand rig. The 0.5 wrapper hides every embedded weapon renderer except Rifle. No external weapon mount is introduced.

`ProductionVisualBinder` now chooses an **enabled** embedded firearm first. This preserves the wrapper's explicit Matt/Rifle choice; only if no enabled firearm exists does it fall back to a disabled firearm for compatibility with the historical validated Sam wrapper.

Selected operator model is used both in Bunker preview and actual Dead City gameplay. Stats still modify health / mobility / damage.

### 4.5 Atomic operator glTF import V2 — REAL UNITY BUILD VALIDATED

Public entry point:
`Assets/Deadreach/Editor/Production05OperatorArtSetup.cs`

Implementation:
`Assets/Deadreach/Editor/Production05OperatorArtSetupV2.cs`

Real-Unity failures that forced the redesign:
- first auto-downloaded Lis/Matt sources failed glTFast import
- first repair corrected the source mapping to Shaun/Matt but cleanup used `AssetDatabase.DeleteAsset` on obsolete sources while legacy wrapper prefabs still referenced their source GUIDs
- Unity therefore imported a half-deleted graph and reported `Missing Nested Prefab Asset` for old Lis/Matt wrappers
- Shaun/Matt were also refreshed/imported source-by-source while dependency/importer state was still being mutated, causing continued glTFast failures and duplicate import attempts

Atomic V2:
- leaves `Production05OperatorArtSetup` as a thin stable API/menu wrapper so existing builder calls do not change
- removes generated/legacy wrapper prefabs + controllers and obsolete Lis/Matt sources through filesystem operations **before any AssetDatabase refresh**
- removes `.meta` for Shaun/Matt only when the previous glTFast import produced no GameObject, clearing stale failed ScriptedImporter state
- prepares **both** `Survivor_Shaun.gltf` and `Survivor_Matt_Full.gltf` completely before Unity sees either source
- discovers every non-`data:` `"uri"`, resolves it against the original remote glTF URL, ensures the external dependency exists locally and rewrites the URI to its stable local basename
- performs one synchronous `AssetDatabase.Refresh(...ForceSynchronousImport | ForceUpdate)` after the complete filesystem graph is coherent
- does not perform repeated source-by-source `Refresh + ImportAsset + retry` loops
- validates both imported GameObjects only after that one pass
- instantiates then `UnpackPrefabInstance(...Completely...)` before saving wrapper prefabs, eliminating nested glTF prefab-parent links in generated production wrappers
- Shaun keeps its artist-rigged SMG
- Matt full keeps its artist-rigged Rifle and hides all other embedded weapon renderers
- dedicated animator controllers are built from each imported operator's own clips where available, with validated Sam controller only as fallback
- no external hand-mounted weapon path is introduced

**Real Unity result on 2026-08-23:** `DEADREACH > Build Production Slice 0.5` completed with no blocking error after pulling Atomic V2. The prior nested-prefab/glTFast blocker is accepted as fixed. Runtime/visual behavior of the full 0.5 scope still requires the MEGA Runtime Gate below.

### 4.6 Enemies / boss / runtime progression

Runtime infected archetypes:
- Walker
- Runner
- Brute
- Stalker

Boss operations: 10 / 20 / 30 / 40 / 50.
Boss has increased size/HP, tier scaling, mutation phases around 66% and 33% HP, boss HUD and extraction lock until death.

Editor boss shortcut:
`DEADREACH > Dev > 0.5 Select Boss Level 10`

### 4.7 Combat presentation

0.5 combat FX:
- pooled tracer core
- glow trail
- muzzle flash
- environment sparks
- infected gore/spark streaks
- stronger critical feedback
- old large red square/billboard impact marker removed

The artist-rigged embedded firearm remains the muzzle source.

### 4.8 Store

Store surface includes cosmetics, Bunker themes, weapon finishes and season content.
No fake purchases. StoreKit / Google Play verification remains a later integration gate.

## 5. Mandatory mobile UI release gate

Current accepted UI screenshots are **Unity Editor/Desktop preview only**.

Before release, landscape mobile validation must cover:
- `Screen.safeArea`
- notch / Dynamic Island / rounded corners
- representative iPhone + Android aspect ratios including wide 19.5:9 / 20:9
- small physical screen readability
- minimum touch target size/spacing
- responsive reflow instead of simple shrink
- Arsenal preview/list separation
- Operator preview/selection readability
- Campaign usability
- Store stacking/scrolling
- gameplay HUD/twin-stick safe area
- at least one real notched iPhone and one representative Android phone

Do **not** mark UI/release final until this gate passes.

## 6. Next action — Production 0.5 MEGA Runtime Gate

The import/build blocker is resolved. Do not spend another turn on importer repair unless a new reproducible import error appears.

Run the consolidated `docs/PRODUCTION_05_TEST.md` acceptance in one end-to-end pass:
- Bunker/menu/persistence
- horizontal Arsenal preview
- distinct Sam/Pistol, Raven/Shaun/SMG, Briggs/Matt/Rifle previews and runtime swaps
- Level 1 gameplay / operator stats
- combat VFX
- infected variety
- loot / extraction / Level 2 unlock
- abandon regression
- Level 10 boss / extraction seal / boss clear / progression
- final 0.4 regression sweep

PR #5 stays Draft until that entire real-Unity gate passes.

## 7. Handoff protocol

When resuming:
1. read this file first
2. treat 0.1–0.4 as merged/validated
3. never reintroduce an external hand-mounted Rifle path
4. active branch is `production/0.5-bunker-progression-boss-ui`
5. Atomic `Production05OperatorArtSetupV2` import/build blocker is **REAL UNITY VALIDATED FIXED**
6. next immediate action is the single `docs/PRODUCTION_05_TEST.md` MEGA Runtime Gate
7. do not repeat importer mini-tests unless a new import error appears
8. keep PR #5 Draft until full 0.5 acceptance
