# DEADREACH — Project State

_Last updated: 2026-08-23_

Canonical handoff for DEADREACH. Update after every major implementation/validation/merge.

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

Locked presentation decision:
- Quaternius `Characters_Sam_SingleWeapon.gltf` is the visible Survivor source
- artist-authored embedded weapon is accepted on the **left hand**
- muzzle/tracer is derived from that embedded weapon
- never reintroduce the failed external Rifle hand-socket transform approach

### Production 0.4 — MERGED / REAL UNITY VALIDATED
PR #4 squash merge:

`e86c067720f8f6badc6c8a29e41bcd856c29ffe6`

Validated in real Unity:
- 0 compiler errors after URP Core/import hardening
- required Quaternius Dead City imports healthy
- real street surfaces
- green/red containers
- pickup / sports / truck vehicle wrecks
- barriers / barrels / pipes / trash / road props
- lighting / fog / post-processing / extraction beacon
- DEADREACH-owned collision bounds on imported environment art
- Play Mode starts from Bunker
- extraction approach traversal safety gate
- movement / aim / firing / loot / extraction / Bunker return regression passed

0.4 is the current stable visual/gameplay baseline on `main`.

## 3. Current Git state

- `main`: validated 0.1 + 0.2 + 0.3 + 0.4
- active branch: **`production/0.5-bunker-progression-boss-ui`**
- Production 0.5 is a large systems/presentation pass
- Production 0.5 **REAL-UNITY COMPILE GATE PASSED: 0 C# compiler errors confirmed by user on 2026-08-23**
- Production 0.5 Bunker desktop/editor visual polish is now accepted as a good direction from real Unity screenshots
- Production 0.5 runtime / progression / boss / combat acceptance is still pending
- 0.5 must remain Draft until `docs/PRODUCTION_05_TEST.md` passes

## 4. Production 0.5 — Big Update implemented on branch

### 4.1 Persistent progression — Save schema v4

`Assets/Deadreach/Runtime/Persistence/SaveService.cs`

Added persistent:
- highest unlocked campaign level
- selected campaign level
- highest completed level
- boss kill count
- selected operator
- unlocked operator IDs
- owned-content entitlement IDs
- migration from schema v3 without dropping existing stash/equipped weapon data

Campaign cap: **50 levels**.

Successful extraction records the completed level and unlocks the next level. Every tenth completed level counts as a boss clear.

### 4.2 Post-apocalyptic Bunker Command Center UI

New:

`Assets/Deadreach/Runtime/UI/BunkerCommandCenterUI.cs`

The old Bunker prototype IMGUI panel is replaced in the 0.5 generator by a runtime uGUI command center with:
- **OVERVIEW**
- **ARSENAL**
- **OPERATORS**
- **CAMPAIGN**
- **STORE**

Visual direction: dark industrial bunker, rust/hazard accents, green readiness status, command-center framing.

Overview shows next deployment, current operator/weapon, progress and Bunker intel.

### 4.3 Arsenal / affix inspection

Arsenal uses the existing real persistent weapon stash and shows:
- rarity
- display name
- item power
- all rolled affixes and values
- equipped state
- Equip action persisted through `SaveService`
- rotating 3D production weapon inspector while the Arsenal tab is active

### 4.4 Operator menu / character profiles

New:

`Assets/Deadreach/Runtime/Progression/OperatorCatalog.cs`

Three unlocked base profiles:
- **SAM / Ranger** — balanced
- **RAVEN / Scout** — faster, less durable
- **BRIGGS / Warden** — slower, tougher, slightly harder hitting

Selection persists. `OperatorRuntimeApplier` changes real health/mobility/damage values and applies mild body tint variation to the current validated production character while deliberately leaving the embedded weapon hierarchy alone.

Current limitation: 0.5 uses the validated Sam production mesh as the base visual for all three profiles with tint/stat variation. Separate authored character meshes can replace these profiles later without changing the persistence/UI architecture.

### 4.5 50-level campaign

New:

`Assets/Deadreach/Runtime/Progression/RunDifficultyDirector.cs`

Five 10-level sectors:
1. Dead City
2. Flooded Industrial
3. Ash District
4. Blackout Sector
5. Ground Zero

Current 0.5 uses the validated Dead City geometry as the common map foundation while each sector changes runtime atmosphere/fog/key-light color and difficulty. Distinct authored sector maps are future environment content, not falsely claimed as complete here.

### 4.6 Infected variety

Existing validated Quaternius infected visuals are retained. Runtime archetypes now create materially different combat profiles:
- Walker
- Runner
- Brute
- Stalker

Profiles vary speed, health, damage and scale and continue using the existing production infected visual variants.

### 4.7 Boss every 10 levels

Levels **10 / 20 / 30 / 40 / 50** are boss runs.

The final infected is promoted to a large mutation-class boss with tier scaling and two mutation phases:
- around 66% HP
- around 33% HP

Boss phases increase speed/damage/attack rate and visual scale.

Extraction on boss levels is sealed until the boss is dead. `RunSession`, `ExtractionZone` and the field HUD expose the boss lock state.

Editor validation shortcuts:
- `DEADREACH > Dev > 0.5 Unlock Through Boss Level 10`
- `DEADREACH > Dev > 0.5 Select Boss Level 10`
- `DEADREACH > Dev > 0.5 Unlock Full Campaign 50`

### 4.8 Bunker environment redesign

`Assets/Deadreach/Editor/BunkerHubSceneBuilder.cs`

Main generator is now:

**`DEADREACH > Build Production Slice 0.5`**

Bunker shell now includes:
- heavier industrial dark-metal palette
- sealed blast door + frame + hazard strip
- ceiling beams
- exposed pipes
- supply crate stacks
- warning floor strips
- cold command light
- warm workshop light
- generator green light
- blast-door red emergency light

The 0.5 generator preserves all required 0.4 environment/import/traversal gates and attaches the 0.5 campaign/operator runtime systems to Dead City.

### 4.9 Combat presentation upgrade

`Assets/Deadreach/Runtime/Feedback/CombatFeedbackPresenter.cs`

Old prototype presentation removed/replaced:
- no single plain white tracer presentation
- no old large red billboard/square impact marker path

New pooled combat VFX:
- bright tracer core
- wider transparent glow trail
- damage/crit color response
- muzzle flash particles
- directional world impact sparks
- directed infected gore streak particles
- stronger critical feedback

The accepted 0.3/0.4 embedded-weapon muzzle origin remains the source of shot feedback.

### 4.10 Field HUD

`Assets/Deadreach/Runtime/UI/PrototypeHud.cs`

Field HUD now shows:
- campaign level
- sector name
- boss objective messaging
- mutation boss HP bar
- explicit `EXTRACTION SEALED` state while boss is alive
- level-clear result messaging

This field HUD is still IMGUI and is not yet the final production mobile HUD. The Bunker Command Center is the main UI replacement in 0.5.

### 4.11 Store surface

The Bunker STORE tab includes production-facing cards for:
- operator cosmetics
- Bunker themes
- weapon finishes
- season content

**No fake purchase is implemented.** Store buttons are non-purchasing placeholders in 0.5. `ownedContentIds` / `GrantContent` are entitlement hooks for a later verified StoreKit / Google Play integration.

## 5. Mandatory mobile UI release gate

The accepted 0.5 Bunker screenshots are **Unity Editor/Desktop preview only**. They must not be treated as final mobile acceptance.

Before release, DEADREACH requires a dedicated landscape-mobile UI pass covering at minimum:
- `Screen.safeArea` / notch / Dynamic Island / rounded-corner protection
- iPhone and Android landscape aspect ratios, including very wide 19.5:9 / 20:9 devices
- UI scaling at small physical screen sizes, not only Game View pixels
- minimum touch-target sizing and spacing for all navigation / equip / operator / campaign / store / deploy controls
- responsive Bunker layouts: panels may collapse/reflow rather than simply shrink
- Arsenal 3D preview must not overlap the weapon list on narrow screens
- Operator 3D preview and stats must remain readable without covering selection controls
- Campaign sector + 10-level grid must fit through responsive sizing/scrolling rather than tiny buttons
- Store cards must stack or scroll safely on narrow devices
- all gameplay HUD/twin-stick controls must obey safe area independently of Bunker UI
- real-device validation on at least one notched iPhone and one representative Android phone

Current anchor-based 0.5 layout is a better foundation for responsiveness, but **mobile layout is not validated yet**.

Do not mark UI/release as final until this mobile gate passes.

## 6. Immediate validation gate

### Compile gate — PASSED
User confirmed **0 red C# compiler errors** in real Unity `6000.3.22f1` on 2026-08-23.

### Bunker visual direction — EDITOR PASS
Real Unity screenshots after the second layout-polish pass confirm the Bunker Command Center is now visually acceptable as a desktop/editor presentation direction:
- DEADREACH header visible
- Overview no longer overlaps
- Arsenal uses a dedicated weapon list + 3D inspector column
- Operators use a 3D character preview
- Campaign shows one 10-level sector grid at a time
- navigation / deploy framing is coherent

This is **not mobile acceptance**; see mandatory mobile gate above.

### Next runtime gate
Run `docs/PRODUCTION_05_TEST.md`:
1. deploy Level 1 and verify movement/combat/loot/extraction
2. validate new muzzle/tracer/impact FX
3. validate infected archetype variety
4. use the Level 10 Editor shortcut to validate boss + extraction seal
5. re-run 0.4 regression locks

Do not claim 0.5 runtime works until the real Unity gameplay gate passes.

## 7. Likely follow-up after 0.5 validation

- separate authored operator meshes/animations
- distinct authored sector maps and encounters beyond shared Dead City foundation
- real StoreKit / Google Play products and receipt verification
- dedicated mobile-responsive Bunker UI + production mobile field HUD
- production NavMesh / more advanced infected behaviors
- combat audio-content pass
- physical-device profiling

## 8. Handoff protocol

When resuming:
1. read this file first
2. treat 0.1–0.4 as merged/validated baselines
3. never reintroduce the external Rifle hand-socket transform path
4. left-hand artist-rigged embedded weapon remains accepted
5. active work is `production/0.5-bunker-progression-boss-ui`
6. 0.5 compile gate is PASSED in real Unity
7. current Bunker visual direction is accepted in Unity Editor, but mobile UI acceptance remains explicitly pending
8. next gate is Level 1 gameplay/combat FX, then Level 10 boss validation
9. keep PR draft until real-Unity acceptance passes

Do not rely on chat history alone.
