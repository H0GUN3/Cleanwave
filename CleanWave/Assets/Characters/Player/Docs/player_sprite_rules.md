# Player Sprite Rules

## Sprite Sheet
- 4 columns x 8 rows
- Cell size: 256 x 224
- Pivot: Bottom Center
- 4 walk frames per direction
- Transparent RGBA PNG

## Direction Row Order
0 = UP
1 = UP_RIGHT
2 = RIGHT
3 = DOWN_RIGHT
4 = DOWN
5 = DOWN_LEFT
6 = LEFT
7 = UP_LEFT

## Evolution Levels
0 = Base
1 = Upgrade
2 = Final

## Upgrade Effects
- Move speed increases
- Bag capacity increases
- Animator/visual set changes

## Suggested Values
- Base: moveSpeed 3.0, bagCapacity 5
- Upgrade: moveSpeed 3.8, bagCapacity 8
- Final: moveSpeed 4.6, bagCapacity 12

## Unity Import
- Texture Type: Sprite (2D and UI)
- Sprite Mode: Multiple
- Slice: Grid By Cell Size
- Cell X: 256
- Cell Y: 224
- Pivot: Bottom Center
- Filter Mode: Point (no filter)
- Compression: None
