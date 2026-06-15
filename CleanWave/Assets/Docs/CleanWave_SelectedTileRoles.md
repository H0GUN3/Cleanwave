# CleanWave — Selected Tile Roles (Blockout Guide)

> Purpose: Controlled, representative tile selection for the FIRST map blockout.
> Goal: avoid placing random curve / corner / T-junction / cross / shoreline sprites
> as base terrain, which produces disconnected, visually broken terrain.
>
> Source set (ONLY): `Assets/Art/Processed/Terrain32/`
> All tiles are 32x32 px, PPU 32 → 1 tile = exactly 1 Unity Tilemap cell.
>
> Naming convention: each sprite is named `{Sheet}_{row:00}_{col:00}`
> (row 0 = top row of the sheet, col 0 = left column). Linear index = `row * cols + col`.

---

## Critical findings (read before painting)

1. **These are AUTOTILE / transition sheets, not flat fill palettes.**
   Most sprites in each sheet are edges, corners, T-junctions, crosses, or shoreline
   transitions meant for edge-matching. Only a small subset are valid "base fill" tiles.

2. **City Road & Dirt Road have NO terrain — they are road pieces on transparency.**
   They are meant to be painted ON TOP of a base ground layer (grass / sand / pavement),
   not used as ground themselves.

3. **River Water is a shoreline-transition sheet.** Only ONE tile (`r0c0`) is clean
   open water. Every other tile contains a grass/shore edge and will look broken if
   used as a water base.

4. **All processed tiles have a slight feathered/transparent inset border (~1–2px).**
   The row-0 base tiles still read as solid, but seamless edge-to-edge tiling is not
   guaranteed. For final polish, converting each fill sheet to a RuleTile is recommended
   instead of hand-picking single sprites.

---

## 1. Terrain_Grass_32  (grid 6x6, 36 sprites)

| Role | Sprite name | row,col | index |
| --- | --- | --- | --- |
| Base fill (grass) | `Terrain_Grass_32_00_00` | r0,c0 | 0 |
| Variation 1 | `Terrain_Grass_32_00_01` | r0,c1 | 1 |
| Variation 2 | `Terrain_Grass_32_00_02` | r0,c2 | 2 |

- **Use for:** general grass ground fill across City / River outskirts.
- **DO NOT use as base fill:** flower/decorative patch tiles (r0c3–c5, r1c3–c5, r2 flowers),
  grass-to-dirt EDGE and CORNER tiles (rows 3–5, indices ~18–35), bushes/dark blobs (row 5).

## 2. Terrain_Sand_Autotile_32  (grid 8x8, 64 sprites)

| Role | Sprite name | row,col | index |
| --- | --- | --- | --- |
| Base fill (sand) | `Terrain_Sand_Autotile_32_00_00` | r0,c0 | 0 |
| Variation 1 | `Terrain_Sand_Autotile_32_00_01` | r0,c1 | 1 |
| Variation 2 | `Terrain_Sand_Autotile_32_00_02` | r0,c2 | 2 |

- **Use for:** Coast beach ground fill.
- **DO NOT use as base fill:** half/strip tiles (r1 c4–c7, indices 12–15) and ALL
  corner / edge / T-junction / cross / slope tiles (rows 2–7, indices 16–63).

## 3. Terrain_CitySidewalk_Pavement_32  (grid 9x8, 72 sprites)

| Role | Sprite name | row,col | index |
| --- | --- | --- | --- |
| Base fill (pavement) | `Terrain_CitySidewalk_Pavement_Autotile_32_00_00` | r0,c0 | 0 |
| Variation 1 | `Terrain_CitySidewalk_Pavement_Autotile_32_00_01` | r0,c1 | 1 |

- **Use for:** City sidewalk / plaza ground fill.
- **DO NOT use as base fill:** curb / top-edge tiles (row 1, indices 9–17),
  and all corner / edge / junction tiles (rows 2–7, indices 18–71).

## 4. Terrain_CityRoad_Asphalt_Autotile_32  (grid 8x8, 64 sprites) — OVERLAY layer

| Role | Sprite name | row,col | index |
| --- | --- | --- | --- |
| Base asphalt fill | `Terrain_CityRoad_Asphalt_Autotile_32_00_00` | r0,c0 | 0 |
| Horizontal road | `Terrain_CityRoad_Asphalt_Autotile_32_00_01` | r0,c1 | 1 |
| Vertical road | `Terrain_CityRoad_Asphalt_Autotile_32_00_04` | r0,c4 | 4 |

- **Use for:** straight City roads only, painted on the Road overlay layer above pavement.
- **DO NOT use during blockout:** curves / rounded corners (rows 1–4),
  T-junctions and the cross/plus tile (row 5), Y-junctions / end-caps / decorative dots (rows 6–7).

## 5. Terrain_RiverWater_Autotile_32  (grid 9x6, 54 sprites)

| Role | Sprite name | row,col | index |
| --- | --- | --- | --- |
| Clean water base (ONLY) | `Terrain_RiverWater_Autotile_32_00_00` | r0,c0 | 0 |

- **Use for:** open river / water body fill — this is the ONLY tile to use for the blockout.
- **DO NOT use during blockout:** every other tile (indices 1–53). They all contain a
  shore / edge / corner / river-bend / T-junction / island transition and will break the fill.

## 6. Terrain_DirtRoad_Autotile_32  (grid 8x8, 64 sprites) — OVERLAY layer

| Role | Sprite name | row,col | index |
| --- | --- | --- | --- |
| Horizontal dirt road | `Terrain_DirtRoad_Autotile_32_00_01` | r0,c1 | 1 |
| Vertical dirt road | `Terrain_DirtRoad_Autotile_32_00_02` | r0,c2 | 2 |
| Corner A | `Terrain_DirtRoad_Autotile_32_01_00` | r1,c0 | 8 |
| Corner B | `Terrain_DirtRoad_Autotile_32_01_01` | r1,c1 | 9 |
| Corner C | `Terrain_DirtRoad_Autotile_32_01_02` | r1,c2 | 10 |
| Corner D | `Terrain_DirtRoad_Autotile_32_01_03` | r1,c3 | 11 |
| (optional) Base dirt fill | `Terrain_DirtRoad_Autotile_32_00_00` | r0,c0 | 0 |

- **Use for:** River-zone dirt paths on the Road overlay layer above grass.
- **Corners note:** indices 8–11 are the four corner orientations (r1, c0–c3). Confirm exact
  orientation in the Sprite Editor before painting each turn.
- **DO NOT use during blockout:** the cross/plus tiles (r0 c3–c7, indices 3–7),
  all T-junctions, Y-junctions, X-cross, end-caps and decorative road variants (rows 3–7).

## 7. Terrain_GrassSandEdges_Autotile_32 — RESERVED
- Do NOT use during first blockout. Reserve for later grass↔sand edge polish.

## 8. Terrain_SandWaterEdges_Autotile_32 — RESERVED
- Do NOT use during first blockout. Reserve for later shoreline polish.

## 9. Props_RockBorder_Autotile_32 — RESERVED
- Do NOT use during first blockout. Reserve for later map boundary polish.

---

## Layering summary for the blockout

- **Ground layer (base fill):** Grass (i0), Sand (i0), City Pavement (i0) — pick the
  variation tiles (i1/i2) sparsely for natural noise.
- **Water layer:** River Water i0 ONLY.
- **Road overlay layer (above ground):** City Asphalt (i0 fill / i1 H / i4 V),
  Dirt Road (i1 H / i2 V / i8–i11 corners).
- **Reserved for polish:** GrassSandEdges, SandWaterEdges, RockBorder.

## Rules
- Do NOT use all sprites randomly.
- Do NOT use edge / corner / T / cross / shoreline tiles as base terrain.
- Use ONLY the indices listed above for the first blockout.
