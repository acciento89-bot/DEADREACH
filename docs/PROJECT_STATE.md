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
- Atomic operator importer V2 passed real Unity `Build Production Slice 0.5` with no blocking error
- Bunker desktop/editor visual direction is accepted
- the 0.5 MEGA Runtime Gate was executed in real Unity and **most core systems passed**
- user screenshots confirm distinct Sam/Raven/Briggs previews and runtime switching, Level 1 gameplay, upgraded tracer/combat presentation, Level 10 mutation boss, boss HUD/extraction gate, successful boss clear and campaign progression through Level 11
- five finalization issues were found during that gate and are now fixed in branch but **not yet revalidated**:
  1. successful extraction unlocked the next level but did not automatically select it
  2. boss still used ordinary Scrap as its configured drop instead of a dedicated reward
  3. Arsenal weapon preview was horizontal but still upside-down
  4. stash weapons all looked like the same finish
  5. player could leave the authored road at the map end, fall into the void and continue running below the level

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

**Finalization change:** successful extraction now automatically moves `selectedLevel` to `completedLevel + 1` whenever that next level is unlocked. Replaying an older level advances to its next already-unlocked mission; Level 50 remains selected at campaign end.

### 4.2 Bunker Command Center

Tabs:
- OVERVIEW
- ARSENAL
- OPERATORS
- CAMPAIGN
- STORE

Accepted Editor/Desktop direction:
- DEADREACH header visible
- Overview no overlap
- Arsenal list + dedicated 3D inspector
- Operator roster + dedicated 3D preview
- Campaign shows one 10-level sector at a time
- coherent navigation + deploy framing

This is **not mobile acceptance**.

### 4.3 Arsenal / weapon finishes

Arsenal shows persistent stash, rarity, item power, affix rolls, equipped state and Equip action.

Finalization adds persistent `visualSkinId` support and `WeaponVisualStyle`:
- Factory Issue
- Rustwalker
- Hazard Stripe
- Nightwatch
- Toxic Salvage
- Bloodline
- unique Mutation Core finishes for boss tiers 1–5

Old stash entries without a stored finish derive a deterministic finish from their existing instance ID/item power, so the current save also gains visible variety without reset.

The selected finish is applied:
- to the rotating Arsenal production-weapon preview
- to the artist-rigged embedded firearm used by the selected runtime operator

No gameplay hand/muzzle transform is modified by finish styling.

`BunkerWeaponPreviewUI` keeps automatic horizontal axis normalization, then applies the observed Quaternius-specific preview-only **180° X correction** so the grip/magazine should render below the receiver instead of above it. The inspector also displays `FINISH // <name>`.

### 4.4 Distinct operators — REAL UNITY PARTIAL ACCEPTANCE

Profiles:
- **SAM / Ranger** — balanced
- **RAVEN / Scout** — faster / less durable
- **BRIGGS / Warden** — slower / tougher / harder hitting

Production mapping:
- **SAM → Quaternius Sam SingleWeapon / artist-rigged Pistol**
- **RAVEN → Quaternius Shaun SingleWeapon / artist-rigged SMG**
- **BRIGGS → Quaternius Matt full export / artist-rigged Rifle only**

Real Unity screenshots confirm:
- three visibly distinct operator models
- selection changes correctly in Bunker
- selected operator changes in gameplay
- operator stats display different profiles

Do not reintroduce external weapon hand sockets.

### 4.5 Atomic operator glTF import V2 — REAL UNITY VALIDATED

`Production05OperatorArtSetupV2` prepares the full filesystem/dependency graph before Unity import, performs one synchronous import pass, fully unpacks generated wrappers and preserves Shaun/SMG + Matt/Rifle.

Real Unity result: `DEADREACH > Build Production Slice 0.5` completed with no blocking error after Atomic V2.

### 4.6 Enemies / boss / dedicated boss reward

Runtime infected archetypes: Walker, Runner, Brute, Stalker.

Boss operations: 10 / 20 / 30 / 40 / 50, with tier scaling, mutation phases around 66% and 33% HP, boss HUD and extraction lock until death.

Real Unity screenshots confirm Level 10 mutation boss, boss HP UI and successful progression through Level 10 into Sector 02 / Level 11.

**Finalization change:** boss `scrapDrop` is now configured to **0**. On boss death `WeaponLootFactory.CreateBossReward()` grants a guaranteed Epic/Legendary `MUTATION T# // DR-7 RELIC` with tier-specific Mutation Core finish and strong affixes. The reward enters run weapon inventory immediately when capacity is available; otherwise it is reserved by `RunSession` and guaranteed into the extraction snapshot. It remains unsecured until successful extraction, preserving the extraction-risk loop.

A reserved boss reward counts as extraction loot, so a boss run cannot become stuck on the normal `requireLoot` gate after the boss dies.

### 4.7 Combat presentation — REAL UNITY PARTIAL ACCEPTANCE

0.5 combat FX:
- pooled tracer core + glow
- muzzle flash
- environment sparks
- infected gore/spark streaks
- stronger critical feedback
- old large red square/billboard impact marker removed

User runtime screenshots show the upgraded blue tracer path in live combat. Artist-rigged embedded firearm remains the muzzle source.

### 4.8 Dead City world/fall safety

Finalization adds two layers of protection:
1. generated `DeadCity_WorldSafety_0_5` with invisible West/East/South/North BoxCollider boundaries just outside the authored pavement plus a deep emergency catch floor
2. `PlayerFallSafety` runtime backstop on the player, storing the last valid in-bounds position and recovering if physics/spawn ever gets below the map or outside the playable rectangle

Goal: the player cannot simply walk off the road into the void, and even a physics edge case cannot leave the game running below the level.

This requires rebuilding Production Slice 0.5 once so the new scene colliders/component are authored into Dead City.

### 4.9 Store

Store surface includes cosmetics, Bunker themes, weapon finishes and season content. No fake purchases. StoreKit / Google Play verification remains a later integration gate.

## 5. Mandatory mobile UI release gate

Current accepted UI screenshots are **Unity Editor/Desktop preview only**. Before release, landscape mobile validation must cover safe areas/notches, representative iPhone + Android aspect ratios, touch targets, responsive reflow, gameplay HUD/twin-stick layout, and at least one real notched iPhone plus representative Android phone.

Do **not** mark UI/release final until this gate passes.

## 6. Next action — targeted 0.5 finalization re-test

Do **not** rerun the entire MEGA gate. The already-passed sections remain accepted.

After pulling latest branch:
1. require 0 compiler errors
2. run **`DEADREACH > Build Production Slice 0.5`** once to author new world-safety colliders/guard
3. Arsenal: equip two or three existing stash weapons and verify different `FINISH // ...` colors plus upright weapon preview
4. clear a standard mission and verify Bunker returns with the next level automatically selected
5. run Level 10 boss shortcut, kill boss and confirm boss itself drops no Scrap and a `MUTATION T# // DR-7 RELIC` weapon reward is carried/secured after extraction
6. walk deliberately into all four map edges/end of the road and verify player cannot fall into the void; if an edge case gets below bounds, recovery must occur immediately
7. final Console: 0 blocking red errors

The detailed targeted checklist is in `docs/PRODUCTION_05_TEST.md`.

If these five targeted fixes pass, mark PR #5 Ready and merge to `main`.

## 7. Handoff protocol

When resuming:
1. read this file first
2. treat 0.1–0.4 as merged/validated
3. never reintroduce an external hand-mounted Rifle path
4. active branch is `production/0.5-bunker-progression-boss-ui`
5. operator import blocker is real-Unity validated fixed
6. most of the 0.5 MEGA runtime gate is already real-Unity accepted from screenshots
7. only the five finalization fixes in section 6 require targeted revalidation
8. keep PR #5 Draft until those targeted checks pass
