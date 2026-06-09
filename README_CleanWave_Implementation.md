# CleanWave Implementation Notes

This repository now contains the full script architecture for the attached 4-week plan.

## Implemented Scope

- Core loop: movement, pickup, sorting, score/combo, battery, stage clear/fail
- Meta systems: upgrade data model, save/load, pollution penalty, charging station
- Stage expansion systems: stage catalog/flow, map clean-visual blend, tutorial flow, result UI
- Stabilization tools: object pool, pooled auto return, runtime build validation
- Editor bootstrap: generates required scenes (`MainMenu`, `Tutorial`, `Stage1_Campus`, `Stage2_River`, `Stage3_Beach`, `Result`)

## Key Folders

- `Assets/Scripts/Core`
- `Assets/Scripts/Player`
- `Assets/Scripts/Trash`
- `Assets/Scripts/System`
- `Assets/Scripts/UI`
- `Assets/Scripts/Data`
- `Assets/Scripts/Audio`
- `Assets/Scripts/Editor`

## Quick Start In Unity

1. Open the project in Unity (2D template recommended).
2. From menu: `CleanWave/Generate Base Scenes`.
3. Create ScriptableObjects:
   - `CleanWave/Stage Config` (one per stage)
   - `CleanWave/Stage Catalog`
   - `CleanWave/Upgrade Data`
4. In each stage scene, assign references:
   - `StageManager` -> `StageConfig`, `ScoreManager`, `InventoryManager`
   - `PlayerController` -> managers and trash layer
   - `UIManager` -> HUD text/slider components
5. Add sorting bins with `SortingBin` and matching accepted `TrashType`.
6. Add trash prefabs with `TrashObject`, and spawner with `TrashSpawner`.
7. Run play mode and tune balancing values in `StageConfig`.

## Build Checklist (Windows)

- Add all six scenes to Build Settings.
- Confirm console has no compile/runtime errors.
- Verify clear/fail conditions on all 3 stages.
- Verify save persistence after restart.
