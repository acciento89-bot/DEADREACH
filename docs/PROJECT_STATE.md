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

**Bunker → Deploy → Expedition → Mission / Combat → Loot → Risk decision → Extract / Die / Abandon → Bunker → Equip / Upgrade → Deploy stronger**

## 2. Validated / merged baselines

### Vertical Slice 0.1 — MERGED / VALIDATED
Merge `e4d5dbe2c52d3e9aeed52f421fdd99f7c6b01877`.

### Production 0.2 — MERGED / VALIDATED
Merge `fd0dca0ece7d18ca005f2f4b52d65039904fad27`.

### Production 0.3 — MERGED / REAL UNITY VALIDATED
PR #3 merge `924e8ff4ae250da13fd0d198b121802cf80131b0`.

Permanent firearm rule:
- use artist-authored firearm geometry already parented to the Quaternius survivor rig
- derive muzzle from that embedded firearm
- **never reintroduce the failed external hand-mounted Rifle transform path**

### Production 0.4 — MERGED / REAL UNITY VALIDATED
PR #4 squash merge `e86c067720f8f6badc6c8a29e41bcd856c29ffe6`.

### Production 0.5 — MERGED / REAL UNITY VALIDATED
PR #5 squash merge `a066386f05c6593f1840ef6902f62c808cbdf319`.

### Production 0.6 + 0.7 — MERGED / REAL UNITY VALIDATED
- PR #7 merge `9633aabd903251a09c8475b5b8672a03988a92bc`
- PR #6 promotion to `main` `b69f5270a6e3e26780ccaa0445e4e6764808f753`

### Production 0.8 — MERGED / REAL UNITY VALIDATED
PR #8 squash merge `876127fb9997951afcca738cd7251acd2f662014`.

### Production 0.9 — MERGED / REAL UNITY VALIDATED
PR #9 squash merge `2f15df3b5ca7b15eeacea39928b63118700e2432`.

### Production 0.10 — MERGED / REAL UNITY VALIDATED
PR #10 squash merge `f48368cd46799afa230c8bc52f475300d8f68761`.

### Production 0.11 — MERGED / REAL UNITY VALIDATED
PR #11 squash merge `5b1b40322e305b1546a9ca5a37c1f6b89eabea72`.
- RECOVERY / PURGE / HOLDOUT / BLACKSITE mission system
- objective-gated extraction
- optional BLACK CACHE risk/reward path
- runtime reinforcement waves
- mission HUD / objective markers / alerts
- extraction north-edge geometry hardening
- fixed-zone mobile regression passed
- full Bunker → mission → extraction → Bunker regression passed
- Unity Console: **0 red runtime errors**

## 3. Current Git state

- stable branch: **`main`**
- stable production level: **0.11**
- stable merge: **`5b1b40322e305b1546a9ca5a37c1f6b89eabea72`**
- PR #11: merged / closed
- no production branch is authoritative after the 0.11 merge
- next production work must branch from current `main`

## 4. Stable Production 0.11 baseline

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
- full 360° movement
- direction-based aiming only
- landscape only

### Combat impact / presentation
- operator ability VFX
- infected special VFX
- world hit marker / critical marker
- subtle camera-lens impact
- tracer / muzzle / sparks / gore preserved
- responsive mobile-readable FIELD OPS HUD

### Expedition Director
- **RECOVERY** — secure data core
- **PURGE** — eliminate bounded infected target count
- **HOLDOUT** — activate uplink and hold defense radius under reinforcement pressure
- **BLACKSITE** — breach terminal → eliminate response / mutation boss → secure vault core
- boss levels force BLACKSITE
- Primary objective gates extraction
- Primary completion unlocks extraction immediately
- optional BLACK CACHE creates an extract-now vs risk-more decision
- BLACK CACHE can trigger reinforcement response
- optional mission reward banks only after successful extraction
- death / abandon clears pending mission rewards

### Extraction egress hardening
- accepted extraction remains centered at `z=20`
- `World_Ground` and `Road_Main` support extend beyond the north extraction trigger
- extraction-owned colliders are trigger-only
- sealed-zone enter → exit → re-entry passed real runtime validation

### Presentation baseline preserved
- accepted Arsenal orientation/framing
- responsive Bunker layouts
- boss/reward presentation
- sector atmosphere FX
- artist-rigged embedded firearms only

## 5. Production 0.11 validation record

- fresh compile: **0 red compiler errors** ✅
- `DEADREACH > Build Production Slice 0.11` ✅
- Mission HUD / objective marker ✅
- `EXTRACTION SEALED` before Primary ✅
- Primary completion / extraction unlock ✅
- BLACK CACHE / reinforcement path ✅
- extraction egress fix ✅
- fixed-zone MOVE / AIM-FIRE / Ability ✅
- Bunker → Workshop / Arsenal → Deploy → mission / combat / loot → extraction → Bunker ✅
- Workshop / progression persistence ✅
- optional cache reward banks after successful extraction ✅
- Production 0.10 combat / boss / reward / sector presentation intact ✅
- Unity Console: **0 red runtime errors** ✅

Test plan: `docs/PRODUCTION_11_TEST.md`

## 6. Next development entry point

1. `git switch main`
2. `git pull`
3. branch the next production pass from current `main`
4. preserve Production 0.11 Expedition Director / extraction / risk-reward systems
5. preserve schema-v6 Workshop progression
6. preserve fixed-zone mobile controls
7. preserve Production 0.10 combat-impact presentation
8. never reintroduce external gameplay hand-mounted Rifle transforms
9. keep mobile landscape-only
