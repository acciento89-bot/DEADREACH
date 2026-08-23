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
- **Mobile orientation:** landscape only

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

### Production 0.5 — MERGED / REAL UNITY VALIDATED
PR #5 squash merge `a066386f05c6593f1840ef6902f62c808cbdf319`.

### Production 0.6 + 0.7 — MERGED / REAL UNITY VALIDATED
- PR #7 merged Production 0.7 presentation polish into the 0.6 integration branch at `9633aabd903251a09c8475b5b8672a03988a92bc`
- PR #6 promoted the full 0.6 + 0.7 stack to `main` at `b69f5270a6e3e26780ccaa0445e4e6764808f753`

### Production 0.8 — MERGED / REAL UNITY VALIDATED
- PR #8 squash merged to `main` at `876127fb9997951afcca738cd7251acd2f662014`
- Workshop / Calibration / Salvage / permanent Bunker upgrades validated

### Production 0.9 — MERGED / REAL UNITY VALIDATED
- PR #9 squash merged to `main` at `2f15df3b5ca7b15eeacea39928b63118700e2432`
- combat roles + operator abilities validated
- final fixed-zone mobile twin-stick controls validated
- mobile HUD readability validated
- final full regression passed with **0 red runtime errors**

### Production 0.10 — MERGED / REAL UNITY VALIDATED
- PR #10 squash merged to `main` at `f48368cd46799afa230c8bc52f475300d8f68761`
- compile and `Build Production Slice 0.10` gates passed
- operator and infected special VFX accepted
- hit / critical / camera-impact presentation accepted
- fixed-zone mobile controls revalidated
- full Bunker → Workshop → Deploy → combat/loot → extraction → Bunker regression passed
- final Unity Console: **0 red runtime errors**

## 3. Current Git state

- stable branch: `main`
- stable production level: **0.10**
- stable merge: `f48368cd46799afa230c8bc52f475300d8f68761`
- no active production branch is authoritative after the 0.10 merge
- next production work must branch from current `main`

## 4. Stable Production 0.10 baseline

### Progression
- save schema v6
- Item Power / Calibration / Salvage
- Workbench / Medbay / Cargo Rig / Scavenger Network
- Workshop survives expedition → Bunker reload

### Combat identities
- WALKER baseline
- RUNNER burst
- BRUTE slam
- STALKER flank
- SAM Field Patch
- RAVEN Vector Dash
- BRIGGS Shockwave

### Mobile
- fixed lower-left MOVE
- fixed lower-right AIM/FIRE
- independent upper-right Ability
- full 360-degree movement
- direction-based aiming only
- phone HUD readable

### Combat impact / presentation
- SAM heal rings / motes
- RAVEN dash trails / endpoint pulse
- BRIGGS expanding Shockwave ground pulse / radial impact
- Runner burst trail
- Brute slam rings / particles
- Stalker flank trail / pulses
- world hit marker on successful damage hits
- distinct critical marker + critical pulse
- subtle camera-lens impact for damage / heavy abilities / specials
- existing tracer / muzzle / sparks / gore pipeline preserved
- runtime URP-safe presentation with no external art dependency

### Presentation baseline preserved
- accepted Arsenal orientation/framing
- responsive Bunker layouts
- boss/reward presentation
- sector atmosphere FX
- landscape-only mobile orientation

## 5. Next development entry point

1. `git switch main`
2. `git pull`
3. branch the next production pass from current `main`
4. preserve schema-v6 Workshop progression
5. preserve fixed-zone mobile controls
6. preserve the accepted Production 0.10 combat-impact layer
7. never reintroduce external gameplay hand-mounted Rifle transforms
8. keep mobile landscape-only

## 6. Handoff protocol

When resuming:
1. read this file first
2. stable baseline is Production 0.10 on `main`
3. stable merge is `f48368cd46799afa230c8bc52f475300d8f68761`
4. next production work starts from current `main`
5. preserve all validated 0.10 progression, controls and combat presentation
