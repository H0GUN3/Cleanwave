# CleanWave Blockout Tile Selection

This guide limits the first readable terrain blockout to stable representative tiles only. Do not use full atlas randomization for blockout painting.

## Selected Representative Tiles

| Role | Processed sheet | Sprite |
|---|---|---|
| Grass base | `Terrain_Grass_32.png` | `Terrain_Grass_32_00_00` |
| Sand base | `Terrain_Sand_Autotile_32.png` | `Terrain_Sand_Autotile_32_01_01` |
| City pavement base | `Terrain_CitySidewalk_Pavement_Autotile_32.png` | `Terrain_CitySidewalk_Pavement_Autotile_32_00_02` |
| Asphalt road base | `Terrain_CityRoad_Asphalt_Autotile_32.png` | `Terrain_CityRoad_Asphalt_Autotile_32_00_00` |
| River water base | `Terrain_RiverWater_Autotile_32.png` | `Terrain_RiverWater_Autotile_32_03_02` |
| Dirt road horizontal | `Terrain_DirtRoad_Autotile_32.png` | `Terrain_DirtRoad_Autotile_32_00_01` |
| Dirt road vertical | `Terrain_DirtRoad_Autotile_32.png` | `Terrain_DirtRoad_Autotile_32_00_02` |
| Dirt road corner NW/variant | `Terrain_DirtRoad_Autotile_32.png` | `Terrain_DirtRoad_Autotile_32_01_00` |
| Dirt road corner NE/variant | `Terrain_DirtRoad_Autotile_32.png` | `Terrain_DirtRoad_Autotile_32_01_01` |
| Dirt road corner SW/variant | `Terrain_DirtRoad_Autotile_32.png` | `Terrain_DirtRoad_Autotile_32_01_02` |
| Dirt road corner SE/variant | `Terrain_DirtRoad_Autotile_32.png` | `Terrain_DirtRoad_Autotile_32_01_03` |

## Blockout Rules

- Use only the selected sprites above for the first readable map blockout.
- Do not use random decorative variants.
- Do not use edge tiles as base terrain.
- Do not use corner, T-junction, or cross-junction tiles as base fill.
- Leave `Tilemap_Edges` mostly empty until transition/shoreline rules are intentionally configured.
- Leave `Tilemap_Collision` empty during the first blockout pass.
