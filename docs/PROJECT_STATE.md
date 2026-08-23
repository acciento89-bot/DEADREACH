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

Production 0.1–0.5 are merged / real-Unity validated. Stable main baseline remains Production 0.5 merge `a066386f05c6593f1840ef6902f62c808cbdf319`.

Permanent gameplay-art rule:
- use artist-authored firearm geometry already parented to the Quaternius survivor rig for gameplay
- derive muzzle from that embedded firearm
- never reintroduce the failed external hand-mounted Rifle transform path

## 3. Current stacked workflow

- Production 0.6 branch: `production/0.6-content-rewards-mobile`
- Production 0.7 branch: **`production/0.7-presentation-polish`**
- Production 0.7 Draft PR: **#7**
- 0.7 is stacked on the functionally tested 0.6 draft and is not merge-ready until all real-Unity presentation gates pass.

## 4. Production 0.6 functional status

Functionally validated in real Unity:
- compile gate: 0 errors
- `DEADREACH > Build Production Slice 0.6`
- Rifle / SMG / Pistol / Shotgun persistence and Arsenal use
- five sector identities visibly differ
- Level 10 `THE BREAKER` boss identity / phase presentation
- `MUTATION RELIC SECURED` combat popup
- successful extraction → persistent Bunker relic debrief
- `TRANSFER TO ARSENAL` → same reward present and equipable

Presentation debt exposed by the gate became the scope of Production 0.7.

## 5. Production 0.7 implemented scope

### Layout / safe area
- Bunker header/navigation/content/deploy use separate safe-area zones with gutters
- ultrawide, 16:9, 16:10 and 4:3-ish landscape breakpoints
- compact landscape now reclaims width from the navigation rail for split content views

### Arsenal
- DR-7 baseline orientation preserved
- SMG / Pistol / Shotgun incorrect inversion removed
- family-aware yaw, centering and camera framing
- real Unity Arsenal orientation gate accepted for all four weapon families

### Operators
- standalone operator 3D preview no longer relies on fixed global screen anchors
- preview frame now follows the live `OperatorInspector` panel screen bounds
- compact landscape reserves a larger lower information zone for operator name / role / stats
- square preview aspect is preserved to avoid character stretching
- portrait is intentionally not a supported Bunker layout; mobile target remains landscape

### Boss reward / debrief
- reward card reflows inside safe area and lower than persistent boss/HUD regions
- Bunker recovery debrief uses safe-area modal layout and blocks underlying interaction
- human-readable affix labels

### Sector FX
- particle systems receive real runtime materials with URP / Standard / Sprite shader fallback
- Flooded Industrial: cool rain
- Ash District: drifting ash
- Blackout: sparse dust with reduced purple intensity
- Ground Zero: rising red contamination motes

### Build gate
- `DEADREACH > Build Production Slice 0.7`

## 6. Real Unity validation progress

- 0 compiler errors ✅
- Production 0.7 build gate ✅
- Arsenal four-family orientation/framing ✅
- Bunker first multi-aspect pass: 16:9 / 19:9 acceptable, but 4:3 / 16:10 exposed compact-layout drift ⚠️
- compact-layout follow-up implemented in commits `9dacdef131a579aae3c9f9c4f01bb606dadc36e1` and `f56b7b71006b296b8532bb7c326ef370454296d6`; **retest required**
- boss reward/debrief 0.7 regression pending
- sector FX 0.7 regression pending

## 7. Next action

1. pull `production/0.7-presentation-polish`
2. require 0 compiler errors after the compact-layout follow-up
3. retest Operators at 4:3 / 16:10 / 16:9 / 19:9 landscape
4. if clean, continue Level 10 reward/debrief regression
5. then Levels 11 / 21 / 31 / 41 FX regression
6. keep PR #7 Draft until all real Unity gates pass

## 8. Handoff protocol

When resuming:
1. read this file first
2. treat 0.1–0.5 as merged / real-Unity validated baseline
3. treat 0.6 gameplay/reward behavior as functionally tested but still stacked below 0.7
4. active working branch is `production/0.7-presentation-polish`
5. never reintroduce external gameplay hand-mounted Rifle transforms
6. do not call Bunker responsive UI final until the compact landscape retest passes
7. mobile target is landscape; portrait is not part of the accepted release layout
