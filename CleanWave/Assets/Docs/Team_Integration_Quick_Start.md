# CleanWave 팀 통합 빠른 시작

`character` 브랜치를 받은 뒤 **MainScene** 또는 자신의 맵 씬에서 바로 플레이할 수 있도록 하는 체크리스트입니다.

## 1. 저장소 받기

```bash
git clone https://github.com/H0GUN3/Cleanwave.git
cd Cleanwave
git checkout character
```

이후 업데이트:

```bash
git pull origin character
```

## 2. Unity 프로젝트 열기

Unity Hub에서 `Cleanwave/CleanWave/` 폴더를 프로젝트로 추가하고 엽니다.

## 3. Build Settings 확인

`File > Build Settings`에서 아래 순서를 확인합니다.

| Index | 씬 |
|-------|-----|
| 0 | `Assets/Scenes/MainMenu.unity` |
| 1 | `Assets/Scenes/MainScene.unity` |

자신의 맵 씬을 쓰는 경우, MainMenu의 `gameplaySceneName`을 해당 씬 이름으로 바꾸고 Build Settings에 등록하세요.  
자세한 내용: `Assets/UI/MainMenu/Docs/MainMenu_Integration_Guide.md`

## 4. MainScene(또는 맵 씬)에 프리팹 배치

Hierarchy에 아래 **2개만** 드래그합니다.

| 프리팹 | 경로 |
|--------|------|
| Player | `Assets/Characters/Player/Prefabs/Player.prefab` |
| 인벤토리 UI | `Assets/Characters/Player/Prefabs/PlayerInventoryCanvas.prefab` |

`InventoryUI`의 `playerBag`은 비워 둬도 런타임에 자동 연결됩니다.

## 5. 쓰레기 오브젝트 배치

1. 씬에 GameObject 생성
2. `TrashPickup` 컴포넌트 추가
3. 원하는 위치에 배치
4. Inspector에서 `pickupSfx` = `Assets/Audio/SFX/pop.mp3` (이미 연결된 예시는 `MainScene`의 `Trash_0`~`Trash_5` 참고)

## 6. Play 테스트

`MainMenu.unity`에서 Play 후 아래를 확인합니다.

| 항목 | 확인 |
|------|------|
| 시작 버튼 | `MainScene`으로 이동 |
| 종료 버튼 | Play Mode 종료 (빌드 시 앱 종료) |
| 배경음 | `main-theme.mp3` 반복 재생 |
| WASD | 8방향 이동 |
| E | 쓰레기 줍기 + `pop.mp3` |
| Q | 전체 인벤토리 열기/닫기 + 열 때 `inventory.mp3` |
| 1 / 2 / 3 | 진화 단계 테스트 (속도·용량 변경) |

## 포함된 오디오

```
Assets/Audio/
├── BGM/main-theme.mp3      # 메인 메뉴 배경음
└── SFX/
    ├── pop.mp3             # 쓰레기 줍기
    └── inventory.mp3       # 인벤토리 열기
```

## 상세 문서

| 문서 | 내용 |
|------|------|
| `Assets/Characters/Player/Docs/Player_Integration_Guide.md` | 플레이어·인벤토리·쓰레기·오디오 |
| `Assets/UI/MainMenu/Docs/MainMenu_Integration_Guide.md` | 메인 메뉴·Build Settings·BGM |

## 주의

- `Player.prefab`, `PlayerInventoryCanvas.prefab`, 캐릭터 PNG, Animator, Sprite Slice는 **재생성·덮어쓰기 금지**
- `SampleScene.unity`는 로컬 테스트용이며 통합 시 사용하지 않아도 됩니다
- 문제 발생 시 Console 로그와 함께 GitHub Issues에 남겨 주세요
