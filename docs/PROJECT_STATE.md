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
- **Pinned editor:** Unity `6000.3.22f1`
- **Render pipeline:** URP 17.3
- **Language:** C#
- **Target:** mobile-first high-quality 3D
- **Input:** Unity Input System 1.17
- **Camera:** Cinemachine 3.x / perspective high-angle camera
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

1. ✅ Unity 6.3 LTS URP project foundation
2. ◐ Mobile project settings and render configuration
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

### Branches

- `main` — initialized repository / README
- `foundation/unity-6.3` — active development branch

### Completed foundation work

- Repository initialized.
- Canonical handoff file created: `docs/PROJECT_STATE.md`.
- Unity `.gitignore` added.
- `.gitattributes` added with Git LFS rules for large 3D, texture, audio and video assets.
- Unity editor pinned through `ProjectSettings/ProjectVersion.txt` to `6000.3.22f1`.
- Core package manifest added with:
  - URP 17.3
  - Input System 1.17
  - Cinemachine 3.1.5
  - Unity UI
- Runtime assembly created: `Deadreach.Runtime`.
- Editor assembly created: `Deadreach.Editor`.
- Runtime mobile bootstrap created:
  - target 60 FPS
  - VSync disabled to avoid conflicting frame caps
  - device sleep disabled during gameplay
- Editor project bootstrap created to automatically apply production identity/settings:
  - company: Kamilunavo
  - product: DEADREACH
  - iOS bundle ID: `de.kamilunavo.deadzone`
  - Android package ID: `de.kamilunavo.deadzone`
  - initial version: `0.1.0`
  - IL2CPP for iOS and Android
  - landscape-only autorotation
  - Linear color space
  - Force Text serialization

### Not yet implemented

- No playable scene yet.
- No player controller yet.
- No camera rig yet.
- No mobile touch controls yet.
- No weapons or combat yet.
- No enemies yet.
- No loot / inventory / extraction yet.
- No production art assets yet.
- No backend yet.
- No TestFlight / Android build yet.

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
- Keep store-facing name as **DEADREACH** even though the technical bundle identifier remains `de.kamilunavo.deadzone`.
- No advertising SDKs or ad-driven gameplay rewards.

## 8. Immediate next step

Continue Vertical Slice 0.1 on `foundation/unity-6.3`:

1. Add gameplay architecture and state model.
2. Implement perspective high-angle camera rig.
3. Implement player movement abstraction suitable for both touch and desktop testing.
4. Add mobile twin-stick control layer.
5. Add first playable character controller.
6. Establish initial URP/mobile quality profiles.
7. Then begin shooting, damage and enemy AI.

Before merging the foundation branch, open the project once in Unity 6000.3.22f1 and verify package resolution / compilation.

## 9. Handoff protocol

When resuming DEADREACH in another chat:

1. Open this file first.
2. Check the latest commits / open PRs / current working branch.
3. Continue from **Immediate next step** or the newest recorded milestone.
4. Update this file before ending a major development pass.

Do not rely on chat history alone for project continuity.
