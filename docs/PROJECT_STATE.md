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

Permanent gameplay-art rule:
- use artist-authored firearm geometry already parented to the Quaternius survivor rig for gameplay
- derive muzzle from that embedded firearm
- **never reintroduce the failed external hand-mounted Rifle transform path**

### Production 0.4 — MERGED / REAL UNITY VALIDATED
PR #4 squash merge `e86c067720f8f6badc6c8a29e41bcd856c29ffe6`.

Validated Dead City streets / vehicles / containers / props / atmosphere / collision / Bunker-first flow / extraction traversal.

### Production 0.5 — MERGED / REAL UNITY VALIDATED
PR #5 squash merge **`a066386f05c6593f1840ef6902f62c808cbdf319`**.

Validated:
- Bunker Command Center
- Overview / Arsenal / Operators / Campaign / Store
- Sam / Raven / Briggs operator swapping
- persistent 50-level campaign / five sectors
- Walker / Runner / Brute / Stalker combat profiles
- Level 10 mutation boss and extraction seal
- tracer / muzzle / impact feedback
- weapon finishes
- automatic next-level selection
- dedicated Mutation Relic grant
- upright DR-7 Arsenal preview
- world boundaries / fall safety

`main` stable baseline remains Production 0.5 until the stacked 0.6/0.7 work is promoted.

## 3. Production 0.6 — FUNCTIONALLY REAL-UNITY VALIDATED / STILL DRAFT

Branch: `production/0.6-content-rewards-mobile`
PR: #6
Head before 0.7 stack: `a585237a9519a53960762ab137d744e20b04548b`

Real Unity gates completed on 2026-08-23:
- 0 compiler errors after glTF import hardening
- `DEADREACH > Build Production Slice 0.6` completes
- Bunker / Arsenal family runtime path works
- Levels 1 / 11 / 21 / 31 / 41 are visibly differentiated and traversable
- Level 10 `THE BREAKER` overlay / mutation state works
- `MUTATION RELIC SECURED` appears on boss kill
- post-extraction Bunker relic debrief appears
- `TRANSFER TO ARSENAL` transfers the same reward
- reward exists and is equipable in Arsenal

0.6 functional scope:
- persistent Rifle / SMG / Pistol / Shotgun families
- family-specific stat profiles
- multi-family field loot
- tier-varying boss relic families
- standalone Quaternius Pistol / SMG / Shotgun Arsenal models
- visible boss reward popup
- persistent Bunker reward debrief / save schema v5
- five sector identities
- five named mutation bosses
- first mobile-landscape / safe-area pass

0.6 was intentionally **not promoted yet** because the real Unity visual gate exposed presentation debt:
- sector particles appeared as bright magenta/purple missing-material streaks
- Bunker / reward / debrief layout still felt cramped or overlapping
- new non-DR-7 weapon-family 3D previews were inverted / incorrectly oriented
- final sector materials/texturing still need art polish

## 4. Current Git state — Production 0.7

- active branch: **`production/0.7-presentation-polish`**
- stacked base: `production/0.6-content-rewards-mobile`
- Draft PR: **#7** `production: presentation layout arsenal and sector polish 0.7`
- 0.7 gate: `docs/PRODUCTION_07_TEST.md`
- 0.7 is implemented repo-side but **not yet real-Unity compiled/built/runtime validated**

## 5. Production 0.7 implemented scope

### 5.1 Arsenal 3D inspector orientation / framing

`BunkerWeaponPreviewUI` now:
- preserves the validated historical X-flip only for Rifle / DR-7
- stops applying that Rifle-specific inversion to SMG / Pistol / Shotgun
- keeps automatic horizontal-orientation candidate selection
- applies small family-specific presentation yaw
- recenters from combined renderer bounds
- auto-frames the preview camera from final bounds so the weapon is less likely to clip
- slows preview rotation slightly for readability

Acceptance target:
- DR-7 remains correct
- SMG magazine/grip points downward
- Pistol grip points downward
- Shotgun is horizontal/readable

### 5.2 Sector atmosphere material / style pass

`Production06SectorIdentity` is upgraded by the 0.7 branch:
- creates a real runtime particle material using shader fallback order:
  - `Universal Render Pipeline/Particles/Unlit`
  - `Particles/Standard Unlit`
  - `Sprites/Default`
- removes dependency on materialless default ParticleSystem rendering that produced magenta streaks
- gives sectors distinct particle modes:
  - Flooded Industrial — stretched cool rain
  - Ash District — billboard drifting ash + noise
  - Blackout Sector — sparse dark dust + noise
  - Ground Zero — rising red contamination motes + noise
- slightly tones down Blackout purple light values

### 5.3 Bunker responsive layout hardening

`BunkerMobileResponsiveUI` now uses non-overlapping vertical zones with explicit gutters for:
- header
- navigation
- main content
- deploy bar

Separate anchor sets exist for:
- ultrawide / notched landscape
- compact/tablet landscape
- 16:9 desktop/mobile baseline

`Screen.safeArea` remains authoritative.

### 5.4 Boss reward popup polish

`BossRewardPresentationUI` now:
- owns a dedicated safe-area root
- reflows when safe area / resolution changes during Play
- sits lower than the persistent top HUD/boss identity region
- uses aspect-specific panel anchors
- keeps all reward fields and readable affix labels

### 5.5 Bunker relic debrief polish

`BunkerBossDebriefUI` now:
- owns a dedicated safe-area root
- uses a stronger dim layer and blocks underlying interaction
- has aspect-specific modal sizing
- keeps the transfer button inside the safe area
- converts raw enum-style affix text such as `DAMAGEPERCENT` to readable labels like `DAMAGE`, `CRIT DAMAGE`, `FIRE RATE`

### 5.6 Production 0.7 build entry

Use:

`DEADREACH > Build Production Slice 0.7`

It preserves the validated scene-generation path and adds the 0.7 presentation/runtime code on top.

## 6. Mandatory Production 0.7 acceptance

Follow `docs/PRODUCTION_07_TEST.md`.

Minimum gate:
1. `git fetch`
2. `git switch production/0.7-presentation-polish`
3. `git pull`
4. require 0 red compiler errors
5. run `DEADREACH > Build Production Slice 0.7`
6. verify Arsenal orientation for all four families
7. inspect Bunker at 16:9 / ultrawide / compact landscape
8. run Level 10 reward/debrief flow
9. inspect Levels 11 / 21 / 31 / 41 sector FX
10. final 0.6 gameplay/reward regression

PR #7 stays Draft until this passes in real Unity.

## 7. Merge strategy

Because 0.7 is stacked on the still-Draft 0.6 branch:
1. finish real Unity 0.7 acceptance
2. promote / merge 0.6 to `main`
3. retarget 0.7 PR from `production/0.6-content-rewards-mobile` to `main`
4. re-check diff / CI / Unity smoke gate
5. merge 0.7 only after the stacked base is stable

Do not merge 0.7 directly while its 0.6 base is still unpromoted.

## 8. Mandatory mobile release gate

Even after editor acceptance, release still requires:
- representative notched iPhone landscape
- representative Android landscape
- real safe-area verification
- readable Bunker tabs / Arsenal / Operators / Campaign / Store
- touch targets usable with fingers
- gameplay twin-stick + HUD safe-area verification
- no clipping at ultrawide and compact landscape ratios

Do **not** call mobile UI final until real-device checks pass.

## 9. Handoff protocol

When resuming:
1. read this file first
2. treat 0.1–0.5 as merged / real-Unity validated baseline
3. treat 0.6 as functionally real-Unity validated but still Draft due presentation debt
4. active work is 0.7 on `production/0.7-presentation-polish`
5. never reintroduce external gameplay hand-mounted Rifle transforms
6. use `DEADREACH > Build Production Slice 0.7`
7. PR #7 remains Draft until real Unity 0.7 acceptance passes
8. real-device mobile validation remains later mandatory release work
