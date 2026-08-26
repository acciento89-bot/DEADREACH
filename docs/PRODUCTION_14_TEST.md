# DEADREACH — Production 0.14 Test Gate

Production 0.14 branches directly from the fully validated Production 0.12 `main` baseline and does not inherit the rejected Production 0.13 presentation stack.

## Implemented command-center scope
- one runtime presentation owner: `Production14CommandCenterUI`
- six native screens: Overview / Arsenal / Operators / Campaign / Workshop / Supply
- Wenrexa CC0 `UI Minimalism SciFi` panel/button family with graphite offline fallback
- Quaternius authored rear Bunker architecture
- central tactical hologram / command-table presentation
- six-tab navigation
- Ready / Deploy footer
- Holo visibility guard restores the hero after returning to Overview
- `DEADREACH > Build Production Slice 0.14` retains accepted Production 0.12 sector/layout passes

## Visual history
Rejected:
1. initial procedural plates — too DEV/placeholder-like
2. Devdog HUD experiment — wrong radial/partial HUD pieces caused white wireframes, star/hex counters and footer artifact
3. clean graphite fallback — user verdict: `Ist ja wie vorher....`

Current Wenrexa + restored-hologram Overview has now been visually accepted by the user with the explicit decision to stop spending time on further visual redesign and continue toward completion.

## Recovery checkpoint — COMPLETED ✅
Recovery commit:
- `6ed8bf5e292f3430300fcb98ce13641885e7a309`

Recovered/versioned production assets include both production scenes, operator/weapon/infected prefabs, production materials/controllers/volume, `ProductionAssetCatalog.asset`, Unity GUID metas, URP/package/project settings.

## Gate A — Fresh compile after release-hardening mega block — PENDING
Latest implementation adds `Production14ReleaseBlock` and release validator, so require one fresh compile with **0 red Unity errors**.

## Gate B — Build Production Slice 0.14 — PASSED ✅
User-confirmed after recovery/build hardening. Current runtime/editor release hardening does not require rebuilding the generated scenes unless a real scene-generation defect appears.

## Gate C — Overview visual acceptance — PASSED ✅
User accepted the current Wenrexa + hologram Overview and explicitly requested moving on rather than continuing visual redesign.

## Gate D — Navigation — PASSED ✅
User-confirmed Overview -> Arsenal -> Operators -> Campaign -> Workshop -> Supply -> Overview switching.

## Release-hardening mega block — IMPLEMENTED / VALIDATION PENDING
New `Production14ReleaseBlock` adds:
- normalized `Screen.safeArea` ownership for the 0.14 root
- adaptive `CanvasScaler` behavior for compact/ultrawide landscape
- enforced minimum touch targets
- hardened scroll inertia/sensitivity
- guarded Deploy action to prevent double activation
- invalid-level/operator deployment checks
- save before deployment
- save on mobile pause/focus loss/application quit
- live profile-change detection with header/footer refresh
- transient command-center action feedback
- Arsenal salvage controls for unequipped secured weapons
- Scrap refresh after salvage
- profile persistence snapshots across Bunker interaction

New editor command:
`DEADREACH > Validate Production 0.14 Release Readiness`

Expected PASS log:
`DEADREACH 0.14 RELEASE STATIC CHECK: PASS`

The validator checks:
- Bunker + expedition scene assets
- enabled Build Settings entries
- required Quaternius command-center Resources
- Wenrexa preparation/fallback availability
- landscape orientation compatibility

## Final consolidated gate
Use `docs/PRODUCTION_14_RELEASE_GATE.md` and validate in one run:
1. fresh compile
2. static release validator
3. Arsenal equip + salvage
4. operator selection persistence
5. campaign selection/footer refresh
6. Workshop upgrade + weapon calibration where affordable
7. Supply open/return
8. Holo return after tab switching
9. mobile landscape safe-area/touch sweep
10. Deploy
11. stable 0.12 sector/hazard/objective/BLACK CACHE/extraction regression
12. return to Bunker + persistence
13. final Unity Console **0 red runtime errors**

Production 0.14 remains Draft/unmerged until this consolidated release gate passes.
