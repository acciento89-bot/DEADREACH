# DEADREACH — Production 0.4 Unity Acceptance

Production 0.4 is an environment / lighting / atmosphere pass. It must not regress the validated 0.3 character/combat presentation.

## A. Local update + environment assets

```powershell
cd <your DEADREACH repo>
git fetch
git switch production/0.4-environment-atmosphere
git pull
powershell -ExecutionPolicy Bypass -File .\tools\install-quaternius-deadcity-set.ps1 -CommitAndPush
```

Wait for Unity / glTFast to finish importing the new Environment + Vehicles assets.

## B. Compile gate

Required before runtime testing:

- **0 red compiler errors**
- no new blocking package/import errors

If compile fails, stop here and fix the actual compiler error before generating the slice.

## C. Generate Production Slice 0.4

Run:

**`DEADREACH > Build Production Slice 0.4`**

Required:

- generator completes without red exception/error
- Bunker reopens at the end
- Build Settings still contain Bunker first / Dead City second

## D. Dead City visual gate

Play → Deploy.

Expected 0.4 additions:

- real modular street surfaces appear over the retained prototype road underlay
- cracked street pieces / intersection are visible
- real traffic/plastic barriers replace some of the visual emptiness
- green/red containers dress side areas
- WaterTower creates a distant landmark
- pickup / sports car / truck create wrecked-traffic silhouettes
- barrels / broken pallet / pipes / trash / wheel stack / blood props dress the route
- real streetlight / traffic-light geometry is present
- fog is denser but gameplay remains readable
- warm street-light pools contrast with colder moon/fill lighting
- post-processing is active (ACES + modest bloom + grading + vignette)
- extraction beacon is visibly stronger / easier to read

Missing environment asset warnings are **not** an accepted final state after the installer has run.

## E. 0.3 regression lock — critical

The accepted 0.3 Sam presentation must remain untouched:

- colored Survivor still appears
- colored Infected variants still appear
- embedded artist-rigged weapon remains on the current left-hand mount
- weapon stays aligned while moving / aiming / firing
- muzzle / tracer still originate from the embedded weapon
- do **not** reintroduce an external Rifle mount or transform hack

## F. Gameplay regression gate

Required:

- movement works
- aim / fire works
- enemies chase / attack / die
- player damage / death works
- Scrap + weapon loot pickup works
- environment colliders do not trap the player at spawn
- environment colliders do not block the extraction zone entirely
- successful extraction returns to Bunker
- death / abandon still loses unsecured run weapons
- Pause / Resume / Abandon still work
- no blocking Console errors

## G. Visual quality notes to report

For the first 0.4 screenshot pass, specifically report if any of these are wrong:

- environment models are gigantic / tiny
- roads are rotated or floating
- vehicles float / sink / block the route too aggressively
- shared atlas colors are wrong
- scene is too dark or fog too strong
- bloom is excessive
- extraction beacon is distracting
- any 0.3 character/weapon regression

## H. Merge gate

Do **not** merge Production 0.4 until:

1. clean compile
2. generator passes
3. real environment assets visible in Play Mode
4. atmosphere readable
5. 0.3 character/weapon presentation preserved
6. gameplay/extraction regression gate passes
7. no blocking Console errors
