# Draft: Development Rules

## 목적
- 2주 안에 버그 없이 플레이 가능한 MVP를 만들기 위한 개발/협업 원칙.

## 핵심 원칙
- `main` 브랜치는 항상 Unity에서 실행 가능한 상태로 유지한다.
- 기능 수보다 안정성을 우선한다.
- 발표 전 2~3일은 새 기능 추가보다 버그 수정/통합/검증을 우선한다.
- Scene 충돌 방지를 최우선으로 한다.

## Unity 협업 규칙
- `MainScene`과 Tilemap 배치는 맵 담당 1명만 수정한다.
- 다른 팀원은 Scripts, Prefabs, Sprites, Animations, UI 리소스를 작업한다.
- 맵 요소는 PNG/Tile/Prefab 단위로 제작해 맵 담당에게 전달한다.
- 최종 Scene 배치는 맵 담당이 한다.

## Git 규칙
- 역할별 브랜치를 사용한다.
  - `feature/player-system`
  - `feature/map-scene`
  - `feature/assets-ui`
- 통합 담당은 사용자 1명이 맡는다.
- 커밋은 작은 작업 단위마다 한다.
- 커밋 메시지는 간단 규칙을 사용한다.
  - `add: player movement`
  - `fix: trash pickup range`
  - `update: city map layout`
  - `docs: update asset rules`
  - `art: add trash sprites`
- `main` merge 전 Unity Play Mode 실행을 확인한다.

## 코드 규칙
- 자세한 C# 코드 규칙은 `code_rules.md`를 따른다.
- 기능은 작게 나눠 구현한다: 이동 → 수거 → 가방 → 수거함 → 정화율 → 구역 개방.
- 임시 구현은 허용하되 이름에 `Temp`를 붙이거나 TODO 주석을 남긴다.
- 발표에 필요 없는 확장 기능은 2주 MVP에 넣지 않는다.

## 미정
- Prefab 생성/수정 권한.
- 일일 통합 시간.
