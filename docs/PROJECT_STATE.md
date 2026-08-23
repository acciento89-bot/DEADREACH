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
PR #5 squash merge **`a066386f05c6593f1840ef6902f62c808cbdf319`**.

Production 0.5 remains the current `main` baseline until PR #6 is merged.

## 3. Current Git state

- integration branch: **`production/0.6-content-rewards-mobile`**
- PR #6 targets `main`
- Production 0.7 PR #7 has been merged into the 0.6 integration branch
- PR #7 merge commit: **`9633aabd903251a09c8475b5b8672a03988a92bc`**
- all Production 0.6 + 0.7 real Unity acceptance gates passed on 2026-08-23
- PR #6 is ready for final promotion to `main`

## 4. Production 0.6 functional scope

- visible boss reward acquisition
- persistent post-extraction Bunker reward debrief
- save schema v5 for secured boss reward state
- persistent weapon families Rifle / SMG / Pistol / Shotgun
- family-specific combat profiles
- family-aware field loot and boss relics
- standalone Quaternius Arsenal models
- glTF import staging/validation hardening
- five sector identities
- five named mutation boss identities
- safe-area-aware Bunker implementation
- `DEADREACH > Build Production Slice 0.6`

## 5. Production 0.7 presentation polish merged into integration

### Bunker / responsive layout
- separate header / navigation / content / deploy anchor zones
- dedicated ultrawide / 16:9 / 16:10 / 4:3 landscape behavior
- compact landscape navigation rail tightened to return space to content
- Operator 3D preview binds to the real `OperatorInspector` panel instead of fixed screen percentages
- Operator preview preserves square framing and avoids compact-layout drift
- portrait intentionally excluded; project bootstrap disables portrait autorotation

### Arsenal 3D inspector
- DR-7 baseline orientation preserved
- SMG / Pistol / Shotgun no longer inherit the incorrect Rifle inversion
- family-aware yaw
- combined-bounds recentering
- automatic camera framing

### Sector atmosphere
- real particle materials replace magenta missing-material rendering
- Flooded Industrial: cool rain
- Ash District: drifting ash
- Blackout: sparse dark dust
- Ground Zero: red contamination motes

### Reward / debrief presentation
- Bunker debrief uses safe-area-aware modal sizing and blocks underlying interaction
- human-readable affix names
- combat reward is a compact lower-right **RELIC SECURED** toast
- detailed post-extraction reward review remains in the Bunker

### Gameplay HUD polish
- compact Field Ops HUD with reduced left-side footprint
- carried Scrap / weapon loot condensed
- compact objective status row
- desktop control hint shortened and omitted on mobile builds
- boss health presented as a slim top-center strip
- boss name integrated into boss health presentation
- mutation phase reduced to a lightweight status chip
- extraction and run-result panels reduced in size

## 6. Final real Unity acceptance — COMPLETE

Passed on 2026-08-23:
1. **0 red compiler errors**
2. `DEADREACH > Build Production Slice 0.7`
3. Arsenal Rifle / SMG / Pistol / Shotgun orientation and framing
4. Bunker layout at 4:3 / 16:10 / 16:9 / ~19:9 landscape
5. Level 10 gameplay HUD / boss bar / phase presentation
6. boss kill → compact Relic toast
7. extraction → Bunker relic debrief → transfer to Arsenal
8. Levels 11 / 21 / 31 / 41 sector FX presentation
9. final Bunker → expedition → combat → loot → return regression
10. **0 red runtime Console errors** after final regression

No remaining blocker was reported in real Unity.

## 7. Handoff protocol

When resuming:
1. read this file first
2. treat 0.1–0.5 as already merged / real-Unity validated
3. treat the full 0.6 + 0.7 stack as real-Unity validated and ready for `main`
4. never reintroduce external gameplay hand-mounted Rifle transforms
5. keep mobile landscape-only
6. after PR #6 merge, advance `main` as the canonical stable baseline before starting the next production pass
