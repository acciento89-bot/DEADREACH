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

### Production 0.1–0.5
Production 0.1 through 0.5 are merged / validated. Stable main baseline remains Production 0.5 at `a066386f05c6593f1840ef6902f62c808cbdf319`.

Permanent gameplay weapon rule:
- use artist-authored firearm geometry already parented to the Quaternius survivor rig for gameplay
- derive muzzle from that embedded firearm
- never reintroduce the failed external hand-mounted weapon transform path just to force Arsenal visuals onto operators

## 3. Production 0.6 functional validation

Branch: `production/0.6-content-rewards-mobile` / PR #6.

Functionally validated in real Unity on 2026-08-23:
- 0 compiler errors
- `DEADREACH > Build Production Slice 0.6` passed
- Bunker / Arsenal weapon-family flow passed
- Levels 1 / 11 / 21 / 31 / 41 are visibly distinct
- Level 10 `THE BREAKER` boss presentation works
- `MUTATION RELIC SECURED` appears on boss kill
- extraction Bunker relic debrief appears
- `TRANSFER TO ARSENAL` transfers the reward and reward is equipable

Presentation debt exposed during the real Unity gate:
- Bunker/modal/header regions can visually overlap or feel cramped
- new SMG / Pistol / Shotgun Arsenal previews can be inverted/wrongly oriented while DR-7 is acceptable
- sector atmospheric ParticleSystems render magenta/purple missing-material streaks
- sector-specific atmosphere needs stronger production identity

0.6 PR remains Draft and is the base for 0.7.

## 4. Active work — Production 0.7

- active branch: **`production/0.7-presentation-polish`**
- PR: **#7**
- base: `production/0.6-content-rewards-mobile`
- goal: presentation/layout/Arsenal/sector-FX polish without regressing 0.6 gameplay/reward behavior

Implemented:
- separated Bunker header / navigation / content / deploy anchor zones with breakpoint-specific gutters
- safe-area-aware boss reward card placed below persistent top HUD zones
- safe-area-aware modal Bunker relic debrief with blocked underlying interaction
- human-readable reward/debrief affix labels
- family-aware Arsenal preview orientation
- DR-7 Rifle keeps its validated baseline flip
- SMG / Pistol / Shotgun no longer inherit the wrong DR-7 inversion
- combined-bounds recentering and automatic camera framing
- actual runtime particle material assignment with URP / Standard / Sprite fallback
- distinct cool rain / drifting ash / blackout dust / red contamination atmosphere styles
- toned-down Blackout purple lighting
- new `DEADREACH > Build Production Slice 0.7` menu entry
- dedicated `docs/PRODUCTION_07_TEST.md`

## 5. Production 0.7 real Unity validation

Passed on 2026-08-23:
- **compile gate:** 0 compiler errors ✅
- **build gate:** `DEADREACH > Build Production Slice 0.7` completed with no blocking red build/import error ✅

Still pending:
1. Arsenal orientation runtime gate — Rifle / SMG / Pistol / Shotgun
2. Bunker responsive runtime gate — 16:9 / ~19.5:9 / compact landscape
3. Level 10 boss reward + relic debrief presentation regression
4. Levels 11 / 21 / 31 / 41 sector FX regression
5. final gameplay/persistence regression

PR #7 remains Draft until those real Unity presentation gates are clean.

## 6. Next action

Open Play → Bunker → Arsenal and equip one weapon from each family.

Required first:
- DR-7 / Rifle remains correct
- SMG grip/magazine points downward
- Pistol grip points downward and silhouette is readable
- Shotgun horizontal/readable and not inverted
- preview remains centered while rotating
- finish tint remains visible

Then continue with `docs/PRODUCTION_07_TEST.md`.

## 7. Handoff protocol

When resuming:
1. read this file first
2. main stable baseline is Production 0.5
3. 0.6 is functionally validated but remains a Draft presentation base
4. active work is PR #7 / `production/0.7-presentation-polish`
5. 0.7 compile + build gates are already green
6. next real Unity gate is Arsenal weapon orientation
7. never reintroduce arbitrary external gameplay hand-mounted weapon transforms
