# CleanWave 팀 가이드

이 문서는 CleanWave 팀원이 같은 기준으로 작업하기 위한 공개 협업 가이드입니다. 세부 기획 수치와 MVP 범위는 항상 [`CleanWave_프로젝트_계획서.md`](../CleanWave_프로젝트_계획서.md)를 우선합니다.

## 최종 목표

- 2주 안에 처음부터 끝까지 플레이 가능한 MVP를 만든다.
- 도심 -> 하천 -> 해안 3구역 루프를 간소하게라도 연결한다.
- 우선순위는 `전체 플레이 루프 완성 > 안정성 > 그래픽 완성도 > 추가 기능`이다.
- 발표 전에는 새 기능보다 버그 수정, 통합, 검증을 우선한다.

## 역할 분담

### 시스템 담당

- 플레이어 이동
- 쓰레기 수거
- 가방 용량
- 수거함 분리수거
- 점수, 코인, 정화율
- 구역 개방 로직
- 업그레이드와 결과화면 연결

### 맵/Scene 담당

- `MainScene` 수정 권한 보유
- Unity Tilemap과 구역 배치
- 도심, 하천, 해안 20x15 Tile 구역 구성
- 수거함, 상점, 쓰레기 위치 배치
- 팀원이 만든 Prefab, Sprite, Tile을 Scene에 통합

### 에셋/UI 담당

- 플레이어 스프라이트 제작
- 쓰레기 7종 제작
- 수거함 5종 제작
- HUD와 결과화면 리소스 준비
- 무료 맵 에셋 수집과 스타일 통일

### 통합 담당

- Git merge 관리
- `main` 실행 상태 확인
- 매일 통합/점검 진행
- 충돌, 범위 증가, 일정 위험 판단

## 협업 원칙

- `main` 브랜치는 항상 Unity에서 열리고 실행 가능한 상태로 유지한다.
- `MainScene`과 Tilemap 배치는 맵/Scene 담당 1명만 수정한다.
- 다른 팀원은 Scripts, Prefabs, Sprites, Animations, UI 리소스를 중심으로 작업한다.
- 통합 전 각자 브랜치에서 Unity 실행을 확인한다.
- 통합 후 `main`에서 Unity Play Mode 실행을 확인한다.
- 커밋은 작은 작업 단위로 나눈다.
- 막힌 점과 다른 파트 요청사항은 매일 공유한다.

## 역할별 브랜치 예시

- `feature/player-system`
- `feature/map-scene`
- `feature/assets-ui`

## 일일 공유 형식

```text
오늘 완료한 것:
막힌 점:
다른 파트에 요청할 것:
내일 할 일:
Unity 실행 확인 여부:
```

## 금지 범위

현재 MVP에는 아래 기능을 넣지 않습니다.

- 배터리
- 미니맵
- 랭킹
- 콤보
- 튜토리얼
- Clean Cart

## 참고 문서

- [`DEVELOPMENT_RULES.md`](DEVELOPMENT_RULES.md): Unity/Git 협업 규칙
- [`CODE_GUIDE.md`](CODE_GUIDE.md): C# 코드 작성 규칙
- [`ASSET_GUIDE.md`](ASSET_GUIDE.md): 에셋 제작 규칙
- [`WORK_BREAKDOWN.md`](WORK_BREAKDOWN.md): 역할별 작업 단위
- [`SCHEDULE.md`](SCHEDULE.md): 14일 일정
