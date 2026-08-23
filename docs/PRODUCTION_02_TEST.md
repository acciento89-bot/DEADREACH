# DEADREACH — Production Pass 0.2 Acceptance Runbook

Use this checklist before merging `production/0.2-gamefeel`.

Editor target: **Unity 6000.3.22f1**

## 1. Branch / compile

- [ ] Local branch is `production/0.2-gamefeel`.
- [ ] Latest remote commits pulled.
- [ ] Unity package refresh/import completes.
- [ ] Console has **0 red C# compiler errors**.

## 2. Generate current production slice

Run:

`DEADREACH > Build Production Slice 0.2`

Confirm:

- [ ] `Bunker_Hub` opens after generation.
- [ ] Build Settings contain Bunker at index 0.
- [ ] Build Settings contain Dead City at index 1.
- [ ] Console has no blocking errors after scene generation.

## 3. Save migration / foundation regression

Before deleting any existing profile, confirm the old validated 0.1 save migrates:

- [ ] Previous secured Scrap still exists.
- [ ] Previous extraction streak/best streak still exists.
- [ ] Existing success/failure counts remain available.
- [ ] Bunker menu opens normally.
- [ ] Graphics preset cycling still works.

## 4. Combat feedback

Deploy to Dead City and fire at environment/enemies.

- [ ] Tracers are visible.
- [ ] Environment impact feedback appears.
- [ ] Enemy-hit impact feedback appears.
- [ ] Critical hits occasionally produce stronger/magenta feedback.
- [ ] No runaway VFX objects or repeating Console errors appear during sustained fire.
- [ ] Existing enemy chase/melee/player-death behavior still works.

## 5. Run weapon inventory

Find one of the generated weapon cases.

- [ ] Mid-zone weapon case exists.
- [ ] Deep-zone weapon case exists.
- [ ] Walking into a weapon case collects it.
- [ ] FIELD HUD changes to `WEAPON LOOT 1/6` or higher.
- [ ] Weapon case disappears after collection.

## 6. Loss rule — weapon must remain unsecured

Collect a weapon case, then die or Pause → Abandon Run.

- [ ] Failed run returns to Bunker.
- [ ] Weapon stash count did **not** increase from the lost weapon.
- [ ] Existing failed-run/streak-reset behavior is still correct.

## 7. Successful weapon extraction

Deploy again, collect a weapon case, then reach the extraction beacon.

- [ ] Extraction accepts weapon loot even if no Scrap was collected.
- [ ] Extraction completes normally.
- [ ] Return to Bunker occurs.
- [ ] `WEAPON STASH` count increases.
- [ ] Extracted weapon shows rarity.
- [ ] Extracted weapon shows item power.
- [ ] Extracted weapon displays generated affixes when rarity permits them.

## 8. Equip / next-run progression

In the Bunker:

- [ ] An equipped primary is shown if an extracted weapon exists.
- [ ] First extracted weapon auto-equips when no previous primary exists.
- [ ] `EQUIP NEXT STASH WEAPON` cycles to another weapon when multiple weapons exist.
- [ ] `[EQUIPPED]` marker follows the chosen weapon.

Deploy again:

- [ ] FIELD HUD displays equipped weapon name.
- [ ] FIELD HUD displays item power.
- [ ] FIELD HUD displays resolved damage.
- [ ] FIELD HUD displays resolved crit chance.
- [ ] Different affixes alter relevant runtime stats compared with BASE DR-7 values.
- [ ] Critical-hit feedback still works with the equipped item.

## 9. Persistence

Exit Play Mode and start again.

- [ ] Weapon stash remains.
- [ ] Equipped primary remains selected.
- [ ] Secured Scrap remains.
- [ ] Extraction/current/best streak data remains.

## 10. Mobile presentation — editor/simulator preliminary

If Unity reports a touchscreen input device:

- [ ] Dynamic MOVE control renders on the left.
- [ ] AIM/FIRE reticle renders on the right.
- [ ] Overlay is hidden in Bunker.
- [ ] Overlay does not block Pause/other game state.

Physical iOS/Android touch/haptic behavior is a separate device gate and is not considered proven by editor validation.

## 11. Merge gate

Production Pass 0.2 may only leave draft state when:

- [ ] clean Unity compile
- [ ] generation works
- [ ] foundation loop has no regression
- [ ] weapon loss-on-death is verified
- [ ] weapon extraction/stash is verified
- [ ] equip → next-run stat modification is verified
- [ ] persistence is verified
- [ ] no blocking Console errors remain

Production art quality is **not** an acceptance criterion for this pass; the temporary primitive characters/environment remain scaffolding. The purpose of 0.2 is to establish the production-ready presentation, loot and equipment architecture that real assets will use next.
