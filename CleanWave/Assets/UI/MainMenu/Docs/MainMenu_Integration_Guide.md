# MainMenu 통합 가이드

CleanWave 메인 메뉴 패키지는 **캐릭터/인벤토리 패키지와 독립**으로 동작합니다.  
맵 담당자는 Build Settings에 씬 2개만 넣으면 바로 사용할 수 있습니다.

## 빠른 시작

1. Unity Hub에서 `CleanWave/` 프로젝트를 엽니다.
2. `File > Build Settings`에서 아래 순서를 확인합니다.
3. `MainMenu.unity`를 열고 Play Mode로 버튼을 테스트합니다.

## Build Settings 씬 순서

| Index | 씬 | 역할 |
|-------|-----|------|
| **0** | `Assets/Scenes/MainMenu.unity` | 타이틀 / 게임 시작·종료 |
| **1** | `Assets/Scenes/MainScene.unity` | 실제 게임 플레이 |

`MainMenu.unity`를 **0번**에 두어야 `SceneManager.LoadScene("MainScene")`이 정상 동작합니다.

## MainMenu.unity 사용 방법

`Assets/Scenes/MainMenu.unity`를 열면 다음이 이미 배치되어 있습니다.

- `Main Camera`
- `EventSystem` (버튼 클릭용)
- `MainMenuCanvas` (`MainMenuCanvas.prefab` 인스턴스)

별도 UI 작업 없이 Play Mode에서 바로 테스트할 수 있습니다.

다른 씬에 메뉴를 붙이려면 `Assets/UI/MainMenu/Prefabs/MainMenuCanvas.prefab`을 드래그하고 `EventSystem`이 있는지 확인하세요.

## 프리팹 구성

`Assets/UI/MainMenu/Prefabs/MainMenuCanvas.prefab`

| 오브젝트 | 설명 |
|----------|------|
| `Background` | `mainmenu_hero_full.png` 전체 시안 (1280×720, Raycast Target **OFF**) |
| `GameStartButton` | 투명 클릭 영역 (Raycast Target **ON**) |
| `ExitButton` | 투명 클릭 영역 (Raycast Target **ON**) |
| `MainMenuController` | 버튼 동작 스크립트 |

버튼은 **게임 시작**, **게임 종료** 두 개만 있습니다.  
로고·캐릭터·배경은 **전체 시안 PNG 한 장**에 포함되어 있으며, 별도 Image로 중복 배치하지 않습니다.

## Canvas / EventSystem 설정

- **Canvas**: Screen Space Overlay
- **Canvas Scaler**: Scale With Screen Size, Reference **1280×720**, Match **0.5**
- **Background**: Anchor (0,0)-(1,1), offset 0 → 16:9 전체 채움
- **EventSystem**: `InputSystemUIInputModule` 사용 (**StandaloneInputModule 사용 안 함**)

## 게임 시작 버튼

- `MainMenuController.StartGame()` 호출
- `SceneManager.LoadScene(gameplaySceneName)` 실행
- 기본값: `"MainScene"`

## 게임 종료 버튼

- `MainMenuController.ExitGame()` 호출
- **Editor**: Play Mode 종료
- **빌드**: `Application.Quit()`

## MainScene 이름 바꾸는 방법

메인 게임 씬 파일명이 `MainScene`이 아니라면:

1. `MainMenuCanvas` 프리팹(또는 씬 인스턴스) 선택
2. Inspector에서 `MainMenuController` 컴포넌트 찾기
3. **Gameplay Scene Name** 필드에 실제 씬 이름 입력 (확장자 없이)
4. Build Settings에 해당 씬이 포함되어 있는지 확인

예: 씬 파일이 `CityMap.unity`이면 `gameplaySceneName = "CityMap"`

## 조작 (메인 메뉴)

| 입력 | 동작 |
|------|------|
| 마우스 클릭 | 버튼 선택 |

## 맵 담당자가 건드리지 말아야 할 파일

### 메인 메뉴 패키지 (구조 유지)

- `Assets/UI/MainMenu/Sprites/*.png` 및 `.meta`
- `Assets/UI/MainMenu/Prefabs/MainMenuCanvas.prefab`
- `Assets/UI/MainMenu/Scripts/MainMenuController.cs`
- `Assets/Scenes/MainMenu.unity`

### 캐릭터 패키지 (별도 담당)

메인 메뉴 작업 시 아래는 **수정하지 마세요**.

- `Assets/Characters/Player/` 전체
- `Player.prefab`, `PlayerInventoryCanvas.prefab`
- 캐릭터 PNG, Animator, AnimationClip, Sprite Slice

메인 메뉴의 캐릭터 이미지는 기존 스프라이트 시트의 **표시용 참조**만 사용하며, 원본 에셋을 덮어쓰지 않습니다.

## 캐릭터 패키지와의 관계

| 패키지 | 독립 여부 | 설명 |
|--------|-----------|------|
| MainMenu | 독립 | MainMenu + MainScene만으로 타이틀→게임 진입 가능 |
| Player / Inventory | 독립 | MainScene 안에서 `Player.prefab` + `PlayerInventoryCanvas.prefab` 배치 |

두 패키지는 서로 다른 폴더에 있으며, 메인 메뉴는 인벤토리·이동·줍기 시스템을 포함하지 않습니다.

## 폴더 구조

```
Assets/UI/MainMenu/
├── Sprites/       # 배경, 로고, 버튼 PNG
├── Prefabs/       # MainMenuCanvas.prefab
├── Scripts/       # MainMenuController.cs
└── Docs/          # 이 문서

Assets/Scenes/
└── MainMenu.unity
```

## 문의

버튼이 동작하지 않으면 `EventSystem` 존재 여부, Build Settings 씬 등록, `gameplaySceneName` 철자를 확인한 뒤 GitHub Issues에 남겨 주세요.
