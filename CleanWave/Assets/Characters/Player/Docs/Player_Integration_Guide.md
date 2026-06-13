# Player 통합 가이드

맵 담당자가 **MainScene** 또는 자신의 맵 씬에 플레이어와 인벤토리를 붙일 때 참고하는 문서입니다.

## 핵심: 프리팹 2개만 배치

`SampleScene.unity`를 통째로 머지할 필요가 **없습니다**.  
아래 두 프리팹만 Hierarchy에 드래그하면 캐릭터와 인벤토리가 함께 작동합니다.

| # | 프리팹 경로 | 역할 |
|---|-------------|------|
| 1 | `Assets/Characters/Player/Prefabs/Player.prefab` | 이동, 애니메이션, 가방, 진화 |
| 2 | `Assets/Characters/Player/Prefabs/PlayerInventoryCanvas.prefab` | 하단 Hotbar + Q 전체 인벤토리 UI |

### MainScene 통합 절차

1. Unity Hub에서 `CleanWave/` 프로젝트를 엽니다.
2. `MainScene`(또는 작업 중인 맵 씬)을 엽니다.
3. `Player.prefab`을 씬에 드래그 → 스폰 위치로 Transform 이동
4. `PlayerInventoryCanvas.prefab`을 씬에 드래그 → 위치는 자동(UI Canvas)
5. Play Mode에서 WASD / E / Q / 1·2·3 테스트
6. 쓰레기는 `TrashPickup` 컴포넌트 오브젝트를 맵에 배치

`InventoryUI`의 `playerBag` 필드는 **비워 둬도 됩니다**.  
런타임에 `FindFirstObjectByType<PlayerBag>()`으로 `Player.prefab`의 `PlayerBag`을 자동 연결합니다.

## 조작 키 (New Input System)

| 키 | 동작 |
|----|------|
| **W A S D** | 8방향 이동 |
| **E** | 범위 안 쓰레기 줍기 |
| **Q** | 전체 인벤토리 열기/닫기 |
| **1** | Base 진화 (테스트) |
| **2** | Upgrade 진화 (테스트) |
| **3** | Final 진화 (테스트) |

`1/2/3`은 `PlayerUpgrade` 컴포넌트의 **Enable Debug Keys** 옵션으로 켜고 끌 수 있습니다.  
실제 게임에서는 `PlayerUpgrade.SetEvolutionLevel(int)` 또는 `EvolveNext()`를 호출하세요.

## 진화 단계별 수치

| 단계 | 키 | 이동 속도 | 가방 용량 |
|------|-----|-----------|-----------|
| Base (0) | 1 | **3.0** | **8** |
| Upgrade (1) | 2 | **4.0** | **16** |
| Final (2) | 3 | **5.0** | **24** |

- 하단 Hotbar: 최대 **8칸** 슬롯만 표시 (슬롯 이미지만, dim 없음)
- Q 전체 인벤: **24칸** 고정 표시, 용량 초과 슬롯은 잠금 색
- 전체 인벤 UI는 **귀여운 연녹색 스타일** 유지, **Dim/Overlay 없음**

## Player.prefab 구성

| 컴포넌트 | 설명 |
|----------|------|
| `PlayerMovement` | WASD 이동 (`Rigidbody2D`) |
| `PlayerAnimation8Dir` | 8방향 Walk 애니메이션 |
| `PlayerBag` | 가방 데이터 단일 소스 |
| `PlayerUpgrade` | 진화 단계별 속도/용량/Animator |

씬당 Player 인스턴스는 **1개**만 배치하세요.

## PlayerInventoryCanvas.prefab 구성

| 요소 | 설명 |
|------|------|
| `Canvas` | Screen Space Overlay, Sort Order 100 |
| `InventoryUI` | Q 토글, `PlayerBag` 구독, 슬롯 갱신 |
| `HotbarRoot` | 하단 항상 표시 Hotbar (8칸) |
| `FullInventoryRoot` | Q로 토글되는 24칸 전체 창 |
| 슬롯 스프라이트 | `slot_empty_cute`, `slot_selected_cute`, `slot_trash_cute` |

## Trash 배치 방법

1. 씬에 GameObject 생성 (또는 스프라이트 오브젝트)
2. `TrashPickup` 컴포넌트 추가 (`Assets/Characters/Player/Scripts/TrashPickup.cs`)
3. 맵 위 원하는 위치에 배치
4. 플레이어가 `pickupRange`(기본 1.2) 안에서 **E** → `PlayerBag.TryAddTrash()` 성공 시 제거
5. 가방이 가득 차면 쓰레기는 남아 있음

`SampleScene.unity`의 `Trash_0` ~ `Trash_5`는 **테스트용 예시**입니다. MainScene에는 직접 배치하세요.

## SampleScene (테스트 전용)

`Assets/Scenes/SampleScene.unity`는 로컬 테스트용입니다.  
MainScene 통합 시 이 씬을 머지하지 말고, 위 **프리팹 2개** 방식을 사용하세요.

검증 방법:

1. 빈 씬 또는 SampleScene을 엽니다
2. 기존 Player/Canvas가 있으면 제거합니다
3. `Player.prefab` + `PlayerInventoryCanvas.prefab`만 배치합니다
4. Trash 몇 개 배치 후 Play Mode 테스트

## 패키지 의존성

`Packages/manifest.json`:

```json
"com.unity.inputsystem": "1.19.0"
```

`Project Settings > Player > Active Input Handling`: **Input System Package (New)** 또는 **Both**

## 에셋 경로

```
Assets/Characters/Player/
├── Prefabs/
│   ├── Player.prefab
│   └── PlayerInventoryCanvas.prefab
├── Sprites/          # 진화 3단계 PNG
├── Animations/       # Animator, Walk 클립
├── Scripts/
├── UI/Sprites/       # 인벤토리 슬롯 PNG + .meta
└── Docs/             # 이 문서
```

## 주의사항 (반드시 읽기)

다음 에셋은 **이미 설정이 완료**되어 있습니다. 맵 작업 중 **재생성·덮어쓰기·Slice 초기화·Reimport 후 Slice 변경**을 하지 마세요.

- `Sprites/Evolution_*/player_walk_8dir_*.png` 및 `.meta`
- `Animations/` Animator Controller, Walk AnimationClip
- `UI/Sprites/slot_*_cute.png` 및 `.meta`
- `Prefabs/Player.prefab`, `Prefabs/PlayerInventoryCanvas.prefab` 내부 참조

스프라이트 교체가 필요하면 캐릭터 담당자와 협의 후 `player_sprite_rules.md` 규칙(4×8, 256×224, Bottom Center)을 지켜 주세요.

## 문의

통합 중 오류가 있으면 GitHub Issues에 씬 이름, Console 로그, 재현 단계를 남겨 주세요.
