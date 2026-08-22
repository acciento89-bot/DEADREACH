# DEADREACH — Project State

_Last updated: 2026-08-22_

This file is the canonical project handoff and progress record for DEADREACH. Update it after every major development, design, build, release, architecture, backend, monetization, or store-related step so work can continue across chat-length limits without losing context.

## 1. Product identity

- **Game name:** DEADREACH
- **Studio / umbrella:** Kamilunavo
- **GitHub repository:** `acciento89-bot/DEADREACH`
- **App Store Connect app:** created
- **iOS Bundle Identifier:** `de.kamilunavo.deadzone`
- **App Store Connect SKU:** `deadzone-001`
- **Primary platforms:** iOS and Android
- **Monetization:** In-app purchases only
- **Advertising:** none

## 2. Core game concept

DEADREACH is a premium-feeling mobile 3D survival-extraction roguelite with persistent progression.

Core loop:

**Bunker → Expedition → Combat → Loot → Risk decision → Extraction → Persistent progression → Bunker upgrades**

Key pillars:

- Real-time isometric / high-angle 3D combat
- Extraction risk: continue deeper for better loot or extract safely
- Persistent survivor progression
- Loot-driven weapon and equipment builds
- Permanent bunker progression and visible base upgrades
- Bosses and replayable zones
- Daily / weekly challenges and later seasonal live content
- Long-term collection, achievements, streaks and leaderboards
- No ad-based revives, rewards or banners

## 3. Technical direction — LOCKED

- **Engine:** Unity 6.3 LTS
- **Render pipeline:** URP
- **Language:** C#
- **Target:** mobile-first high-quality 3D
- **Input:** Unity Input System
- **Camera:** Cinemachine / perspective high-angle camera
- **Content delivery:** Addressables planned
- **AI / navigation:** Unity AI Navigation / NavMesh planned
- **Animation:** Mecanim + Animation Rigging planned
- **Materials:** PBR + Shader Graph where useful
- **Version control:** GitHub
- **Large binary assets:** Git LFS
- **Backend:** not yet implemented; Supabase is an option for accounts, leaderboards, events and cloud state
- **IAP:** Apple + Google storefront integration planned

### Graphics target

The project must not be built as a throwaway low-detail prototype and upgraded later. The visual foundation should support the intended final quality from the start:

- Real 3D environments
- PBR materials
- Proper lighting / baked GI where appropriate
- Reflection and light probes
- Post-processing and color grading
- VFX for weapons, hits, fire, smoke, electricity, particles and atmosphere
- Scalable graphics presets for mobile hardware

Planned quality presets:

- Performance
- Balanced
- Ultra

## 4. First production milestone

### Vertical Slice 0.1

Goal: prove the complete playable loop in one compact but visually representative slice.

Required flow:

**Bunker → Start expedition → Dead City test area → Move → Shoot → Fight enemy → Loot → Extract → Loot persists → Return to bunker**

Initial feature order:

1. Unity 6.3 LTS URP project foundation
2. Mobile project settings and render configuration
3. Perspective isometric / high-angle camera
4. Mobile movement controls
5. Player controller
6. Weapon / shooting system
7. Health and damage framework
8. First enemy AI
9. Loot drop and pickup system
10. Basic inventory
11. Extraction system
12. Persistent run result / save state
13. Bunker hub shell
14. First visually representative Dead City environment

## 5. Current repository state

- Repository exists and is reachable.
- `main` has been initialized with `README.md`.
- Initialization commit: `ca8d0b6b2175ea1c92b2e0fa6c5e37d346573230`
- Working branch created: `foundation/unity-6.3`
- Unity project files have **not yet been added**.
- No gameplay code exists yet.
- No art assets have been committed yet.
- No backend exists yet.
- No TestFlight / Android build exists yet.

## 6. Decisions already made

### Engine choice

Unity 6.3 LTS + URP is the production choice for DEADREACH.

Reasoning:

- Strong mobile deployment workflow for iOS and Android
- Better fit for a mobile-first title than a heavier Unreal pipeline for this project
- C# codebase works well with Git-driven development
- URP gives a better performance/quality balance for a visually strong mobile title
- Easier to maintain scalable quality levels across devices

### Camera / presentation

- True perspective 3D camera
- High-angle / isometric-style presentation
- Not a flat 2D or orthographic presentation by default

### Monetization

- No advertising
- In-app purchases only
- Prefer cosmetics, season content and presentation upgrades over hard pay-to-win
- Candidate purchases include survivor skins, weapon skins, bunker themes and a seasonal pass

## 7. Important non-negotiables

- Do not let the project drift into a cheap-looking mobile prototype.
- Do not build systems that must later be completely replaced merely to reach target visual quality.
- Keep mobile performance in mind from the first environment and shader decisions.
- Keep gameplay systems modular and data-driven where practical.
- Commit meaningful progress regularly.
- Update this file after every major pass.
- Keep store-facing name as **DEADREACH** even though the technical iOS bundle identifier remains `de.kamilunavo.deadzone`.

## 8. Immediate next step

Initialize the actual Unity 6.3 LTS URP project on branch `foundation/unity-6.3`, including:

- Unity project structure
- `.gitignore`
- `.gitattributes` / Git LFS rules
- package manifest
- project version metadata
- base folder architecture
- initial rendering / mobile quality configuration
- first gameplay architecture skeleton

Then begin Vertical Slice 0.1.

## 9. Handoff protocol

When resuming DEADREACH in another chat:

1. Open this file first.
2. Check the latest commits / open PRs / current working branch.
3. Continue from **Immediate next step** or the newest recorded milestone.
4. Update this file before ending a major development pass.

Do not rely on chat history alone for project continuity.
