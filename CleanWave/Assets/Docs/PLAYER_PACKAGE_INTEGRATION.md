# Player Package Integration

이 문서는 `character` 브랜치에서 가져온 플레이어 캐릭터 패키지를 `MainScene`에 안전하게 연결하기 위한 작업 메모다.

## 반입 범위

- 반입한 폴더: `Assets/Characters/Player/`
- 씬에 배치할 프리팹:
  - `Assets/Characters/Player/Prefabs/Player.prefab`
  - `Assets/Characters/Player/Prefabs/PlayerInventoryCanvas.prefab`
- 제외 대상: `Assets/Scenes/SampleScene.unity`

`SampleScene`은 테스트 참고용이며, 현재 맵/에셋 정리 작업과 충돌하지 않도록 `MainScene`에 머지하지 않는다.

## 절대 건드리지 말 것

아래 파일들은 GUID와 import 설정이 깨지면 프리팹, 애니메이션, UI 참조가 망가질 수 있다.

- 캐릭터 PNG와 Sprite Slice 설정
- Animator Controller / Animator Override Controller
- AnimationClip
- `Player.prefab`
- `PlayerInventoryCanvas.prefab`
- `Assets/Characters/Player/` 아래 `.meta` 파일

이름 변경, 재생성, reimport 설정 초기화, `.meta` 삭제를 하지 않는다.

## MainScene 연결 절차

1. `MainScene`을 연다.
2. `Player.prefab`을 시작 위치에 배치한다.
3. `PlayerInventoryCanvas.prefab`을 씬 루트에 배치한다.
4. `InventoryUI.playerBag` 필드는 비워둔다. 런타임에 `PlayerBag`을 자동으로 찾는다.
5. 카메라가 플레이어를 보여주도록 위치 또는 추적 설정을 별도로 맞춘다.
6. 쓰레기 줍기 테스트가 필요하면 맵의 쓰레기 오브젝트에 `TrashPickup`과 Collider 설정을 별도로 연결한다.
7. Play Mode에서 `WASD`, `E`, `Q`, `1/2/3` 입력을 확인한다.

## 현재 기능 범위

- `WASD`: 8방향 이동
- `E`: 근처 쓰레기 줍기
- `Q`: 인벤토리 열기/닫기
- `1/2/3`: 캐릭터 단계 테스트

이 패키지는 현재 플레이어 이동, 픽업, 가방, 인벤토리 UI 공유용이다. 점수, 코인, 정화율, 분리배출 판정, 구역 해금과의 최종 연동은 별도 통합 작업으로 처리한다.

## 통합 시 주의할 문제 상황

- 두 프리팹만 복사하면 안 된다. `Assets/Characters/Player/` 전체와 `.meta`가 함께 있어야 한다.
- 맵/타일맵/카메라/쓰레기 배치 작업 중인 씬 파일은 직접 수정하지 말고, 씬 담당자와 배치 타이밍을 맞춘다.
- `SampleScene`에 있는 테스트 쓰레기 오브젝트는 예시일 뿐이며 `MainScene`으로 복사하지 않는다.
- 현재 `PlayerUpgrade`의 테스트 수치는 최종 장비 업그레이드 수치가 아니다. 최종 밸런스 작업 때 별도 조정한다.
