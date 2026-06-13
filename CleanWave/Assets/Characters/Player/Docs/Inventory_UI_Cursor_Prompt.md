# Cursor 작업 지시: CleanWave 인벤토리 UI 적용

아래 조건대로 인벤토리 UI를 Unity 프로젝트에 적용해줘.

## 절대 건드리지 말 것

1. 캐릭터 PNG 3장 재생성 금지
2. 캐릭터 PNG 덮어쓰기 금지
3. Animator Controller / AnimationClip / Sprite Slice 연결 초기화 금지
4. 구버전 입력 API 사용 금지
   - `UnityEngine.Input.GetKey`
   - `UnityEngine.Input.GetKeyDown`
   - `Input.GetAxis`
5. placeholder 이미지 생성 금지

입력은 반드시 새 Input System의 `UnityEngine.InputSystem.Keyboard.current`를 사용해라.

## 참고 파일 위치

목업:

- `Assets/Characters/Player/UI/Mockups/Inventory_UI_Mockup_KO_Bright.png`

UI 스프라이트:

- `Assets/Characters/Player/UI/Sprites/slot_empty.png`
- `Assets/Characters/Player/UI/Sprites/slot_selected.png`
- `Assets/Characters/Player/UI/Sprites/slot_trash.png`
- `Assets/Characters/Player/UI/Sprites/hotbar_9slots_preview_ko.png`

스크립트 초안:

- `Assets/Characters/Player/Docs/InventoryDrafts/PlayerInventory_Draft.cs.txt`
- `Assets/Characters/Player/Docs/InventoryDrafts/InventoryUI_Draft.cs.txt`

초안은 참고용이다. 실제 적용할 때는 필요한 경우 `.cs` 파일로 정리해서 `Assets/Characters/Player/Scripts/` 아래에 넣어라.

## 원하는 UI 동작

1. 하단 기본 인벤토리 바는 항상 화면 아래 중앙에 표시한다.
2. `Q` 키를 누르면 전체 인벤토리 창을 열고, 다시 누르면 닫는다.
3. 전체 인벤토리 창을 열어도 화면 전체를 어둡게 덮는 Dim/Overlay는 만들지 마라.
4. 전체 인벤토리 창 바깥쪽은 게임 화면이 그대로 보이게 해라.
5. `E` 키로 쓰레기를 주우면 인벤토리 쓰레기 수량이 1 증가한다.
6. 가방이 가득 차면 쓰레기를 더 줍지 못하고, 쓰레기 오브젝트도 사라지면 안 된다.
7. UI는 현재 쓰레기 수량과 최대 가방 용량을 표시한다.

## 업그레이드 단계별 값

`PlayerUpgrade`의 Inspector에서 쉽게 수정 가능해야 한다.
코드에 숫자를 고정하지 말고 단계별 데이터로 관리해라.

- Base: 이동속도 3.0, 가방 용량 5
- Upgrade: 이동속도 4.0, 가방 용량 8
- Final: 이동속도 5.0, 가방 용량 12

진화 단계가 바뀌면 다음이 동시에 바뀌어야 한다.

1. Animator Controller
2. 이동속도
3. 가방 용량
4. 인벤토리 UI 슬롯 표시

## 권장 구조

Canvas:

- `PlayerInventoryCanvas`
  - `HotbarRoot`
  - `FullInventoryRoot`

Canvas 설정:

- Render Mode: Screen Space - Overlay
- Canvas Scaler: Scale With Screen Size
- Reference Resolution: 1920 x 1080
- Match: 0.5

HotbarRoot:

- Anchor: Bottom Center
- 항상 활성화
- 9칸 표시

FullInventoryRoot:

- Anchor: Middle Center
- 기본 비활성화
- `Q` 키로 활성/비활성 전환
- 24칸 정도 표시
- 화면 전체 Dim 배경 만들지 말 것

## 기존 코드와 통합

현재 프로젝트에는 `PlayerBag`이 있을 수 있다.
둘 중 하나로 정리해라.

추천:

1. `PlayerBag`을 유지하되, 용량/수량 이벤트를 UI가 구독하게 수정한다.
2. 또는 `PlayerBag` 역할을 `PlayerInventory`로 교체한다.

중요한 것은 `TrashPickup`, `PlayerUpgrade`, `InventoryUI`가 같은 수량/용량 데이터를 보게 하는 것이다.
수량 시스템이 두 개로 나뉘면 안 된다.

## Sprite Import 설정

UI 스프라이트들은 다음 설정을 권장한다.

- Texture Type: Sprite (2D and UI)
- Sprite Mode: Single
- Filter Mode: Point
- Compression: None
- Pixels Per Unit: 100

## 테스트 항목

Play Mode에서 아래를 확인해라.

1. Console 에러 0건
2. WASD 8방향 이동 정상
3. E 키로 쓰레기 줍기 정상
4. 하단 인벤토리 수량 증가 정상
5. Q 키로 전체 인벤토리 열기/닫기 정상
6. 전체 인벤토리 열어도 바깥 게임 화면이 어두워지지 않음
7. 1/2/3 키로 진화 단계 전환 정상
8. 진화할수록 이동속도 증가
9. 진화할수록 가방 용량 증가
10. 가방이 가득 차면 쓰레기를 더 줍지 못함

## 완료 보고 형식

완료 후 다음을 보고해라.

1. 생성/수정한 파일 목록
2. Inspector에서 연결한 컴포넌트 목록
3. 각 진화 단계의 이동속도/가방용량 값
4. Play Mode 테스트 결과
5. Console 에러 여부
