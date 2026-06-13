# Player 통합 가이드

맵 담당자가 CleanWave 프로젝트에 플레이어 캐릭터, 쓰레기 줍기, 인벤토리 UI를 붙일 때 참고하는 문서입니다.

## 빠른 시작

1. Unity Hub에서 `CleanWave/` 프로젝트를 엽니다.
2. 맵 씬(또는 `Assets/Scenes/SampleScene.unity`)을 엽니다.
3. `Assets/Characters/Player/Prefabs/Player.prefab`을 Hierarchy로 드래그합니다.
4. 씬에 `PlayerInventoryCanvas`가 없다면 `SampleScene.unity`의 UI 구성을 참고해 동일하게 배치합니다.
5. 쓰레기는 `TrashPickup` 컴포넌트가 붙은 오브젝트를 맵에 배치합니다. (`SampleScene` 예시 참고)
6. Play Mode에서 WASD 이동, E 줍기, Q 인벤토리, 1/2/3 진화를 확인합니다.

## Player.prefab 사용 방법

경로: `Assets/Characters/Player/Prefabs/Player.prefab`

프리팹에 포함된 컴포넌트:

| 컴포넌트 | 역할 |
|----------|------|
| `SpriteRenderer` | 8방향 스프라이트 표시 |
| `Animator` | 진화 단계별 Walk 애니메이션 |
| `Rigidbody2D` | 2D 물리 이동 |
| `CapsuleCollider2D` | 충돌 |
| `PlayerMovement` | WASD 이동 |
| `PlayerAnimation8Dir` | 이동 방향에 따른 8방향 애니메이션 |
| `PlayerBag` | 가방 데이터 (단일 소스) |
| `PlayerUpgrade` | 진화 단계별 속도/용량/외형 |

**맵 씬에 넣는 방법**

- Hierarchy에 `Player.prefab`을 드래그 앤 드롭하면 됩니다.
- 씬당 Player 인스턴스는 **1개**만 두세요.
- 시작 위치는 맵 스폰 지점에 맞게 Transform만 조정하면 됩니다.
- 프리팹 내부 설정(속도, 용량, Animator)은 이미 구성되어 있으므로 별도 수정 없이 사용할 수 있습니다.

## 조작 키 (New Input System)

프로젝트는 **Input System Package** (`Keyboard.current`)를 사용합니다.  
`Edit > Project Settings > Player > Active Input Handling`이 **Input System Package (New)** 또는 **Both**여야 합니다.

| 키 | 동작 |
|----|------|
| **W A S D** | 8방향 이동 |
| **E** | 범위 안의 쓰레기 줍기 |
| **Q** | 전체 인벤토리 열기/닫기 |
| **1** | Base 진화 단계 (테스트용) |
| **2** | Upgrade 진화 단계 (테스트용) |
| **3** | Final 진화 단계 (테스트용) |

`1/2/3` 키는 `PlayerUpgrade`의 `enableDebugKeys`가 켜져 있을 때 Play Mode에서만 동작하는 **진화 테스트 키**입니다. 실제 게임 업그레이드 연동 시에는 `PlayerUpgrade.SetEvolutionLevel(int)`를 호출하세요.

## 진화 단계별 수치

| 단계 | 키 | 이동 속도 | 가방 용량 |
|------|-----|-----------|-----------|
| Base (0) | 1 | 3.0 | 8 |
| Upgrade (1) | 2 | 4.0 | 16 |
| Final (2) | 3 | 5.0 | 24 |

- `PlayerBag.MaxCapacity`가 현재 단계의 사용 가능 칸 수입니다.
- 전체 인벤토리 UI는 항상 **24칸**을 표시하고, 용량을 초과한 슬롯은 잠금 색으로 표시됩니다.
- 하단 Hotbar는 최대 **8칸**만 표시합니다.

## TrashPickup 오브젝트 배치 방법

`TrashPickup` 스크립트: `Assets/Characters/Player/Scripts/TrashPickup.cs`

1. 씬에 빈 GameObject 또는 스프라이트 오브젝트를 만듭니다.
2. `TrashPickup` 컴포넌트를 추가합니다.
3. (선택) `SpriteRenderer`로 쓰레기 비주얼을 지정합니다.
4. 맵 위 원하는 위치에 배치합니다.

동작 방식:

- 플레이어가 `pickupRange`(기본 1.2) 안에 있을 때 **E**를 누르면 줍기를 시도합니다.
- `PlayerBag.TryAddTrash()`가 성공하면 해당 오브젝트가 제거됩니다.
- 가방이 가득 차면 쓰레기는 **제거되지 않습니다**.

`SampleScene.unity`에는 `Trash_0` ~ `Trash_5` 예시가 배치되어 있습니다. Gizmos로 줍기 범위(노란 원)를 확인할 수 있습니다.

## Inventory UI 연결 (SampleScene 기준)

씬 오브젝트: `PlayerInventoryCanvas`  
스크립트: `Assets/Characters/Player/Scripts/InventoryUI.cs`

### 구조

```
PlayerInventoryCanvas
├── HotbarRoot          ← 항상 표시 (하단 8칸)
│   └── Slot_01 ~ Slot_08 (+ Slot_09는 비활성)
└── FullInventoryRoot   ← Q 키로 토글 (24칸 전체)
    └── Slot_01 ~ Slot_24
```

### Inspector 연결

`InventoryUI` 컴포넌트 필드:

| 필드 | 연결 대상 |
|------|-----------|
| `playerBag` | 비어 있어도 됨 — 런타임에 `FindFirstObjectByType<PlayerBag>()`로 자동 탐색 |
| `hotbarRoot` | `HotbarRoot` GameObject |
| `hotbarSlots` | Hotbar의 `Image` 슬롯 8~9개 |
| `hotbarCountText` | Hotbar 카운트 Text |
| `fullInventoryRoot` | `FullInventoryRoot` GameObject |
| `fullInventorySlots` | Full Inventory의 `Image` 슬롯 24개 |
| `fullInventoryCountText` | Full Inventory 카운트 Text |
| `emptySlotSprite` | `UI/Sprites/slot_empty_cute.png` |
| `selectedSlotSprite` | `UI/Sprites/slot_selected_cute.png` |
| `trashSlotSprite` | `UI/Sprites/slot_trash_cute.png` |

### 맵 씬에 UI 붙이는 방법

**방법 A (권장):** `SampleScene.unity`에서 `PlayerInventoryCanvas`를 복사해 맵 씬에 붙여넣기  
**방법 B:** `SampleScene.unity`를 열어 연결 상태를 참고한 뒤 동일한 Canvas 구조를 직접 구성

주의: dim/overlay 없이 게임 화면 위에 슬롯만 표시됩니다. `PlayerBag.OnBagChanged` 이벤트로 슬롯이 자동 갱신됩니다.

## SampleScene 테스트 방법

경로: `Assets/Scenes/SampleScene.unity`

1. `SampleScene.unity`를 엽니다.
2. Play Mode 진입
3. WASD로 이동, Trash 근처에서 E로 줍기
4. Q로 24칸 인벤토리 열기/닫기
5. 1/2/3으로 진화 단계 전환 후 속도·용량·슬롯 변화 확인

## 패키지 의존성

`Packages/manifest.json`에 다음이 포함되어 있어야 합니다:

```json
"com.unity.inputsystem": "1.19.0"
```

## 에셋 경로 요약

```
Assets/Characters/Player/
├── Sprites/          # 진화 3단계 PNG (Base / Upgrade / Final)
├── Animations/       # 8방향 Walk 클립, Animator Controller
├── Prefabs/          # Player.prefab
├── Scripts/          # Movement, Bag, Upgrade, TrashPickup, InventoryUI
├── UI/Sprites/       # 인벤토리 슬롯 스프라이트
└── Docs/             # 이 문서
```

## 주의사항 (반드시 읽기)

다음 파일은 **이미 Slice·Animator·AnimationClip 설정이 완료**되어 있습니다.  
맵 작업 중 아래 항목을 **재생성, 덮어쓰기, Reimport 후 Slice 초기화**하지 마세요.

- `Sprites/Evolution_0_Base/player_walk_8dir_base.png`
- `Sprites/Evolution_1_Upgrade/player_walk_8dir_upgrade.png`
- `Sprites/Evolution_2_Final/player_walk_8dir_final.png`
- 각 PNG의 `.meta` (Sprite Mode: Multiple, 4×8, 32 sprites)
- `Animations/` 아래 Animator Controller 및 Walk 클립
- `Prefabs/Player.prefab`의 Animator·스프라이트 참조

스프라이트를 교체해야 할 때는 담당자와 협의 후, 동일한 그리드 규칙(4열×8행, 256×224, Bottom Center)을 유지한 채 진행하세요. 자세한 규칙은 `player_sprite_rules.md`를 참고하세요.

## 문의

통합 중 오류가 있으면 GitHub Issues에 씬 이름, Console 로그, 재현 단계를 함께 남겨 주세요.
