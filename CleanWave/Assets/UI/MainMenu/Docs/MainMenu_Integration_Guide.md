# MainMenu 통합 가이드

CleanWave 메인 메뉴 패키지는 **캐릭터/인벤토리 패키지와 독립**으로 동작합니다.  
맵 담당자는 Build Settings에 씬 2개만 넣으면 바로 사용할 수 있습니다.

## 빠른 시작

1. Unity Hub에서 `CleanWave/` 프로젝트를 엽니다.
2. `File > Build Settings`에서 아래 순서를 확인합니다.
3. `MainMenu.unity`를 열고 Play Mode로 버튼·배경음을 테스트합니다.

## Build Settings 씬 순서

| Index | 씬 | 역할 |
|-------|-----|------|
| **0** | `Assets/Scenes/MainMenu.unity` | 타이틀 / 게임 시작·종료 |
| **1** | `Assets/Scenes/MainScene.unity` | 실제 게임 플레이 |

`MainMenu.unity`를 **0번**에 두어야 `SceneManager.LoadScene("MainScene")`이 정상 동작합니다.

## MainMenu.unity 사용 방법

`Assets/Scenes/MainMenu.unity`를 열면 다음이 이미 배치되어 있습니다.

| 오브젝트 | 설명 |
|----------|------|
| `Main Camera` | 2D 카메라 |
| `EventSystem` | `InputSystemUIInputModule` (버튼 클릭) |
| `MainMenuCanvas` | UI 루트 (`MainMenuCanvas.prefab` 인스턴스) |
| `MainMenuAudio` | `MainMenuBGM` + `AudioSource` (배경음) |

별도 UI 작업 없이 Play Mode에서 바로 테스트할 수 있습니다.

## 프리팹 / 씬 구성

`Assets/UI/MainMenu/Prefabs/MainMenuCanvas.prefab`

| 오브젝트 | 설명 |
|----------|------|
| `BackgroundImage` | `mainmenu_background_final.png` 전체 화면 채움 (Preserve Aspect OFF) |
| `StartButton` | **시작** 버튼 투명 클릭 영역 (Raycast ON) |
| `ExitButton` | **종료** 버튼 투명 클릭 영역 (Raycast ON) |
| `MainMenuController` | 시작·종료 버튼 동작 스크립트 |

버튼 클릭 영역은 배경 이미지 위 좌표에 맞춰 정렬되어 있으며, **시작/종료 클릭이 정상 동작**하는 것을 확인했습니다.

## Canvas / EventSystem 설정

- **Canvas**: Screen Space Overlay
- **Canvas Scaler**: Scale With Screen Size, Reference **1024×576**, Match **0.5**
- **BackgroundImage**: Anchor stretch (0,0)-(1,1), offset 0
- **EventSystem**: `InputSystemUIInputModule` 사용 (**StandaloneInputModule 사용 안 함**)

## 게임 시작 버튼

- `MainMenuController.StartGame()` 호출
- `SceneManager.LoadScene(gameplaySceneName)` 실행
- 기본값: `"MainScene"`

## 게임 종료 버튼

- `MainMenuController.ExitGame()` 호출
- **Editor**: Play Mode 종료
- **빌드**: `Application.Quit()`

## 배경음 — `main-theme.mp3`

- 파일: `Assets/Audio/BGM/main-theme.mp3`
- 씬 오브젝트: `MainMenuAudio`
- 컴포넌트: `MainMenuBGM` + `AudioSource`
- Loop ON, Volume **0.6**, Spatial Blend 0
- MainMenu 씬 진입 시 자동 반복 재생

## MainScene 이름 바꾸는 방법

메인 게임 씬 파일명이 `MainScene`이 아니라면:

1. `MainMenuCanvas` 프리팹(또는 씬 인스턴스) 선택
2. Inspector에서 `MainMenuController` 컴포넌트 찾기
3. **Gameplay Scene Name** 필드에 실제 씬 이름 입력 (확장자 없이)
4. Build Settings에 해당 씬이 포함되어 있는지 확인

예: 씬 파일이 `CityMap.unity`이면 `gameplaySceneName = "CityMap"`

## 맵 담당자가 건드리지 말아야 할 파일

### 메인 메뉴 패키지 (구조 유지)

- `Assets/UI/MainMenu/Sprites/*.png` 및 `.meta`
- `Assets/UI/MainMenu/Prefabs/MainMenuCanvas.prefab`
- `Assets/UI/MainMenu/Scripts/MainMenuController.cs`, `MainMenuBGM.cs`
- `Assets/Scenes/MainMenu.unity`
- `Assets/Audio/BGM/main-theme.mp3`

### 캐릭터 패키지 (별도 담당)

메인 메뉴 작업 시 아래는 **수정하지 마세요**.

- `Assets/Characters/Player/` 전체
- `Player.prefab`, `PlayerInventoryCanvas.prefab`
- 캐릭터 PNG, Animator, AnimationClip, Sprite Slice

## 캐릭터 패키지와의 관계

| 패키지 | 독립 여부 | 설명 |
|--------|-----------|------|
| MainMenu | 독립 | MainMenu + MainScene만으로 타이틀→게임 진입 가능 |
| Player / Inventory | 독립 | MainScene 안에서 `Player.prefab` + `PlayerInventoryCanvas.prefab` 배치 |

## 폴더 구조

```
Assets/UI/MainMenu/
├── Sprites/       # 배경, 버튼 PNG
├── Prefabs/       # MainMenuCanvas.prefab
├── Scripts/       # MainMenuController.cs, MainMenuBGM.cs
└── Docs/          # 이 문서

Assets/Audio/BGM/
└── main-theme.mp3

Assets/Scenes/
└── MainMenu.unity
```

## 문의

버튼이 동작하지 않으면 `EventSystem` 존재 여부, Build Settings 씬 등록, `gameplaySceneName` 철자를 확인한 뒤 GitHub Issues에 남겨 주세요.
