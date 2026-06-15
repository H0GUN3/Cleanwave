# Unity AI Rules Mirror

이 문서는 Unity AI가 CleanWave 프로젝트 내부에서 바로 읽기 위한 규칙 미러 문서다. 최종 기준은 프로젝트 루트의 `AGENTS.md`와 `CleanWave_프로젝트_계획서.md`를 따른다.

## Source Priority

1. 프로젝트 루트 `CleanWave_프로젝트_계획서.md`
2. 프로젝트 루트 `PROJECT_CONTEXT.md`
3. 프로젝트 루트 `AGENTS.md`
4. Unity 내부 미러 문서 `Assets/Docs/*`

## Scope Guard

- CleanWave는 Unity 2D 힐링형 환경 정화 MVP다.
- 작업은 MVP 범위 안에서만 제안하거나 수행한다.
- 연결형 1맵 구조를 유지한다.
- 구역 순서는 `도심 -> 하천 -> 해안`이다.
- 각 구역은 `32x24 Tile` 기준이다.
- 다음 구역은 현재 구역 정화율 `100%` 완료 후 열린다.
- 쓰레기 수는 도심 `20개`, 하천 `25개`, 해안 `30개`다.
- HUD는 `점수 / 코인 / 정화율 / 가방`만 표시한다.
- 결과화면은 `점수 / 정화율 / 별등급`만 표시한다.
- 별등급 기준은 `70/80/95%`다.

## Wrong Sorting Quiz Popup Rules

- 오분류 퀴즈형 피드백 팝업은 오분류 직후 1회만 표시한다.
- 문제, 선택지 2개, 정답 해설을 포함한다.
- 점수 감점은 기존 규칙대로 `-5점`만 적용한다.
- 별등급에는 영향을 주지 않는다.
- 상시 퀴즈 모드나 튜토리얼 시스템으로 확장하지 않는다.
- HUD 상시 요소로 만들지 않는다.

## Unity Edit Safety

- 씬, 프리팹, Inspector 값을 수정하기 전에는 어떤 오브젝트와 어떤 필드를 바꿀지 먼저 목록으로 알려준다.
- 같은 씬이나 프리팹을 Coplay MCP와 Unity AI가 동시에 수정하지 않는다.
- `Assets/Scenes/MainScene.unity`는 메인 씬 기준으로 유지한다.
- `.unity`, `.prefab`, `.asset` YAML을 직접 손으로 수정하지 않는다.
- `Library/`, `Temp/`, `Obj/`, `Logs/`, `UserSettings/`는 읽거나 수정하지 않는다.

## Must Not Add

아래 기능은 UI, 코드, 프리팹, WBS, 향후 구현 노트에 추가하지 않는다.

- 배터리
- 미니맵
- 랭킹
- 콤보
- 튜토리얼
- Clean Cart
- 제한시간 기반 게임오버
- 자동 분류 업그레이드
- 드래그앤드롭 인벤토리

## Implementation Guidance

- 복잡한 새 시스템보다 현재 MVP 루프 완성을 우선한다.
- 플레이어 장비 업그레이드는 집게, 가방, 신발만 사용한다.
- Clean Cart나 자동 분류 장비를 만들지 않는다.
- 오분류 피드백은 `Assets/Docs/QUIZ_BANK.md` 문항을 사용한다.
- UI 텍스트는 TextMeshPro 사용을 우선한다.
