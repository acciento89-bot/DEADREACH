# DEADREACH

Premium mobile 3D survival-extraction roguelite by Kamilunavo.

## Product identity

- **Store name:** DEADREACH
- **iOS Bundle ID:** `de.kamilunavo.deadzone`
- **App Store Connect SKU:** `deadzone-001`
- **Engine:** Unity 6.3 LTS
- **Render pipeline:** Universal Render Pipeline (URP)
- **Primary platforms:** iOS and Android
- **Monetization:** In-app purchases only; no advertising

## Game direction

DEADREACH combines real-time high-angle 3D combat, extraction risk, loot-driven builds, persistent bunker progression and seasonal live content.

Current core loop:

**Bunker → Deploy → Expedition → Combat → Loot → Extract / Die / Abandon → Bunker → Equip / Upgrade → Deploy stronger**

## Current state

Validated and merged:

- Vertical Slice 0.1 foundation
- Production Pass 0.2 game-feel / weapon-loot / equipment loop

Active:

- `production/0.3-art-presentation`
- production asset-binding pipeline passed real Unity compile, generator and empty-catalog fallback runtime validation
- Quaternius Zombie Apocalypse Kit selected as the first CC0 production-art source
- actual selected Survivor / Infected / Rifle binaries still need to be imported and validated

## Repository policy

Large binary assets belong in Git LFS. Unity-generated folders (`Library`, `Temp`, `Logs`, build output) must never be committed. Project decisions and handoff state are maintained in `docs/PROJECT_STATE.md`.
