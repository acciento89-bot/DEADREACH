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

Permanent weapon rule:
- use artist-authored firearm geometry already parented to the Quaternius survivor rig for gameplay
- derive muzzle from that embedded firearm
- **never reintroduce the failed external hand-mounted Rifle transform path**

### Production 0.4 — MERGED / REAL UNITY VALIDATED
PR #4 squash merge `e86c067720f8f6badc6c8a29e41bcd856c29ffe6`.

Validated Dead City streets / vehicles / containers / props / atmosphere / collision / Bunker-first flow / extraction traversal.

### Production 0.5 — MERGED / REAL UNITY VALIDATED
PR #5 squash merge **`a066386f05c6593f1840ef6902f62c808cbdf319`**.

Real Unity acceptance included:
- post-apocalyptic Bunker Command Center
- Overview / Arsenal / Operators / Campaign / Store
- three distinct operator models and runtime swapping
- persistent 50-level campaign / five sectors
- Walker / Runner / Brute / Stalker combat profiles
- Level 10 mutation boss, boss HUD and extraction seal
- upgraded tracer / muzzle / impact feedback
- weapon finishes
- automatic next-level selection after extraction
- boss no longer drops ordinary Scrap; dedicated Mutation Relic is actually granted
- Arsenal weapon preview is upright
- Dead City world boundaries / fall safety prevent void-running

Known UX carry-over accepted into 0.6:
- boss reward was functionally granted and visible in Console, but 0.5 did not visibly celebrate/confirm it to the player.

`main` stable baseline is now Production 0.5.

## 3. Current Git state

- active branch: **`production/0.6-content-rewards-mobile`**
- base: Production 0.5 merge `a066386f05c6593f1840ef6902f62c808cbdf319`
- Production 0.6 implementation is in progress / not yet real-Unity validated
- next PR should remain Draft until 0.6 compile/build/runtime acceptance

## 4. Production 0.6 implemented scope

### 4.1 Boss reward presentation — two-stage confirmation

The 0.5 functional reward path is preserved.

0.6 adds:
1. **immediate combat popup** on `RunSession.BossRewardGranted`
   - `MUTATION RELIC SECURED`
   - reward name
   - rarity / family / item power
   - finish
   - affixes
2. **persistent Bunker recovery debrief** after successful extraction
   - save schema v5 stores `lastBossReward` + `bossRewardDebriefPending`
   - the debrief appears after returning to Bunker
   - `TRANSFER TO ARSENAL` acknowledges it
   - reward remains risk-based: death/abandon before extraction does not persist the debrief/reward

### 4.2 True weapon families

Persistent weapon family enum:
- Rifle
- SMG
- Pistol
- Shotgun

Legacy 0.5 weapon JSON naturally migrates to Rifle because Rifle is enum value 0.

Loot now rolls multiple families instead of always generating `dr7-rifle`.

Family identities:
- **DR-7 Rifle** — balanced baseline
- **RV-9 SMG** — lower damage / much higher fire rate / shorter range
- **PX-4 Sidearm** — lower sustained output / higher crit identity
- **SG-12 Shotgun** — heavy slow short-range slug profile in 0.6; pellet spread is a later combat feature

Boss relic family varies by boss tier.

### 4.3 Real Arsenal family models

`ProductionAssetCatalog` now supports standalone Rifle / SMG / Pistol / Shotgun production prefabs.

`Production06WeaponArtSetup` automatically downloads the self-contained Quaternius standalone Pistol / SMG / Shotgun glTFs, builds unpacked production preview prefabs and configures them into the catalog. Existing validated Rifle prefab remains the Rifle source.

`BunkerWeaponPreviewUI` now chooses the actual model from `WeaponInstanceData.family` and still applies the validated upright preview normalization + finish styling.

Important gameplay rule remains unchanged: runtime survivor gameplay continues to use the stable artist-rigged operator firearm. 0.6 does **not** reintroduce an external hand-mounted weapon just to force family visuals onto every operator.

### 4.4 Five stronger sector identities

`Production06SectorIdentity` layers runtime presentation over the validated 0.5 Dead City geometry:
- **Sector 01 / Dead City** — cold emergency blue/red lights
- **Sector 02 / Flooded Industrial** — teal flood patches / cyan industrial lighting / wet mist
- **Sector 03 / Ash District** — scorch zones / warm fire lighting / ash fall
- **Sector 04 / Blackout Sector** — global light reduction / purple-blue flicker lights / sparse particles
- **Sector 05 / Ground Zero** — mutation pools / aggressive red lighting / contamination atmosphere

The validated streets, collision and extraction layout remain untouched.

### 4.5 Boss identity presentation

Each mutation tier receives a distinct identity:
- Tier 1 — **THE BREAKER**
- Tier 2 — **FLOOD MAW**
- Tier 3 — **ASH TITAN**
- Tier 4 — **BLACKOUT WRAITH**
- Tier 5 — **GROUND ZERO PRIME**

0.6 adds:
- tier color tint via `MaterialPropertyBlock`
- mutation aura light
- mutation particles
- dedicated boss-name overlay
- phase text updates on the existing ~66% / ~33% mutation phases
- target-eliminated state

### 4.6 First serious mobile-landscape responsive pass

`BunkerMobileResponsiveUI` applies runtime layout adaptation without destroying the accepted desktop source layout:
- `Screen.safeArea` applied to the Bunker backdrop
- ultrawide/notched phone breakpoint
- compact landscape/tablet breakpoint
- CanvasScaler changes by aspect ratio
- navigation/content/header/deploy proportions reflow per breakpoint
- minimum touch-target hints via `LayoutElement`

Gameplay HUD already uses `Screen.safeArea` from 0.5.

**This is not final mobile acceptance.** Real-device iPhone + Android validation remains mandatory before release.

### 4.7 Runtime bootstrap

`Production06RuntimeBootstrap` attaches 0.6 presentation systems on scene load:
- expedition: SectorIdentity / BossPresentation / BossRewardPresentationUI
- Bunker: MobileResponsiveUI / BossRewardDebriefUI

No manual scene-component wiring is required.

### 4.8 Production 0.6 build entry

Use:

`DEADREACH > Build Production Slice 0.6`

It:
1. prepares standalone weapon-family art
2. reuses the real-Unity-validated Production 0.5 scene generation pipeline
3. relies on 0.6 runtime bootstrap for the new presentation systems

## 5. Mandatory mobile release gate

Production 0.6 introduces the first responsive implementation, but release still requires:
- representative notched iPhone landscape
- representative Android landscape
- safe-area verification
- readable Bunker tabs / Arsenal / Operators / Campaign / Store
- touch targets usable with fingers
- gameplay twin-stick + HUD safe-area verification
- no clipping at ultrawide and compact landscape ratios

Do **not** call mobile UI final until real-device checks pass.

## 6. Next action — Production 0.6 acceptance

1. switch/pull `production/0.6-content-rewards-mobile`
2. require 0 red compiler errors
3. run `DEADREACH > Build Production Slice 0.6`
4. verify weapon-family import gate passes
5. run `docs/PRODUCTION_06_TEST.md`
6. keep PR Draft until real Unity acceptance

## 7. Handoff protocol

When resuming:
1. read this file first
2. treat 0.1–0.5 as merged / real-Unity validated baseline
3. never reintroduce external gameplay hand-mounted Rifle transforms
4. active branch is `production/0.6-content-rewards-mobile`
5. 0.6 is not yet real-Unity validated
6. use `DEADREACH > Build Production Slice 0.6`
7. mobile responsive work is implementation-only until real-device validation passes
