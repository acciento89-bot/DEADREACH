# DEADREACH — Production 0.13 Test Gate

Production 0.13 branches from the fully real-Unity-validated Production 0.12 `main` baseline.

## Scope

Production 0.13 is the premium Bunker / menu art-direction pass.

Implemented first large block:
- asset-free animated tactical UI backdrop with grid, scan sweep, cold edge glow and vignette bands
- premium corner-frame chrome for shell and feature panels
- cyan / graphite / amber tactical palette layered over existing semantic rarity / danger / success colors
- upgraded header, navigation, content and deploy framing
- numbered premium navigation labels including dynamically installed Workshop
- animated tactical-link telemetry and deploy pulse
- dynamic restyling for content created after tab changes
- premium treatment covers Overview / Arsenal / Operators / Campaign / Supply Network / Workshop
- Arsenal / Operator inspector panels receive stronger command-screen treatment without changing their validated preview ownership
- dedicated cinematic 3D Bunker scene pass
- structural command-center ribs and emissive accents
- rotating holographic command-table array
- left/right server banks with status LEDs
- three-screen tactical wall with animated monitor lighting
- floor guidance markers
- blast-door warning array and emergency lights
- runtime atmosphere animation for holo / monitor / emergency lighting
- `DEADREACH > Build Production Slice 0.13`
- Build 0.13 preserves the accepted 0.12 sector scene + declutter passes before authoring the new Bunker

## Gate A — Fresh Unity compile — PASSED ✅

User-confirmed real Unity result on 2026-08-24:
- **0 red compiler errors**
- no blocking missing-type / UI / editor API errors observed

## Gate B — Build Production Slice 0.13 — PENDING

Run:
`DEADREACH > Build Production Slice 0.13`

Require:
- stable 0.12 sector generation still completes
- accepted 0.12 layout polish still completes
- Production13BunkerScenePass completes
- Bunker scene reopens
- no blocking red generation errors

## Gate C — Premium shell visual acceptance — PENDING

At Bunker Overview validate:
- menu no longer reads as flat prototype / dev UI
- dark graphite / cyan / amber DEADREACH identity is obvious
- animated tactical grid / scanline remains subtle and does not reduce readability
- header / navigation / content / deploy bar feel like one command-center system
- active navigation state remains obvious
- all navigation remains clickable
- deploy CTA is visually dominant but not oversized
- 3D Bunker presentation is visible as atmosphere rather than fighting the UI
- holo table rotates without distracting from menu text
- server / tactical-wall / emergency lighting reads as premium background detail

## Gate D — Screen-by-screen visual acceptance — PENDING

Validate every screen:
1. Overview
2. Arsenal
3. Operators
4. Campaign
5. Workshop
6. Supply Network

Require:
- no legacy panel visually dominates the new design language
- text hierarchy is clear on phone landscape
- panel chrome never covers buttons or content
- scroll areas still scroll
- Arsenal equip actions still work
- Operator selection still works
- Campaign level selection still works
- Workshop upgrade / calibration / salvage interactions still work
- Supply content remains non-pay-to-win presentation

## Gate E — Mobile / responsive — PENDING

Require:
- landscape safe area remains correct
- header, nav, content and deploy zones do not overlap
- touch targets remain usable
- premium decorative graphics have raycast disabled
- navigation / deploy / Workshop / Arsenal interactions receive touches normally

## Gate F — Full regression — PENDING

1. Bunker opens with 0.13 premium shell.
2. Workshop and Arsenal state remain present.
3. Select operator / weapon / campaign level.
4. Deploy.
5. 0.12 sector selection / hazards / missions remain intact.
6. Complete Primary and optional BLACK CACHE.
7. Extract successfully.
8. Return to premium Bunker.
9. Secured rewards / progression persist.
10. 0.10 combat-impact / boss / reward presentation remains intact.
11. Unity Console ends with **0 red runtime errors**.

Production 0.13 remains Draft/unmerged until all real-Unity gates pass.
