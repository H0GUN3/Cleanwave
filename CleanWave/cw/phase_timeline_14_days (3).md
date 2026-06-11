# Draft: 14-Day Phase Timeline

## 목적
- `phase_system.md`, `phase_map_scene.md`, `phase_asset_ui.md`를 14일 일정에 배치한다.
- 각 팀원이 매일 무엇을 해야 하는지, 어떤 산출물을 넘겨야 하는지, 어떤 연결 지점을 확인해야 하는지 명확히 한다.

## 운영 원칙
- 목표: 2주 안에 버그 없이 플레이 가능한 MVP.
- 우선순위: 전체 플레이 루프 완성 > 안정성 > 그래픽 완성도 > 추가 기능.
- 협업: 매일 짧은 점검 + 하루 1회 통합.
- Scene 수정: 맵/Scene 담당 1명만 `MainScene` 수정.
- 통합: 사용자가 담당하며, `main` merge 전 Unity Play Mode 실행 확인.
- Day 12 이후 새 기능 추가 금지.

## 역할 약칭
- **SYSTEM**: 시스템 담당.
- **MAP**: 맵/Scene 담당.
- **ASSET**: 에셋/UI 담당.
- **LEAD**: 사용자, Git 통합/검수 담당.

## Day 1 — 프로젝트 기준 고정
### SYSTEM
- `phase_system.md` Phase 0 진행.
- 필요한 스크립트 목록 작성.
- 필요한 Prefab 목록 작성: Player, Trash, Bin, ZoneGate, Shop.

### MAP
- `phase_map_scene.md` Phase 0 진행.
- MainScene 생성.
- Grid/Tilemap 32x32 기준 설정.
- 도심/하천/해안 20x15 블록아웃 시작.

### ASSET
- `phase_asset_ui.md` Phase 0 진행.
- 무료 맵 에셋팩 후보 1~2개 선정.
- 직접 제작 에셋 스타일 기준 정리.

### LEAD
- GitHub repo/branch 세팅.
- 역할별 브랜치 생성:
  - `feature/player-system`
  - `feature/map-scene`
  - `feature/assets-ui`
- 폴더 구조 생성 기준 확인.

### 연결 확인
- SYSTEM → MAP: 필요한 Prefab 이름 전달.
- ASSET → MAP: 사용할 무료 타일팩 후보 전달.
- MAP → SYSTEM: MainScene 이름, Player 시작 위치 후보 공유.

### 완료 기준
- Unity 프로젝트가 열리고 빈 MainScene이 실행된다.
- 세 역할이 각자 작업할 브랜치를 가진다.

## Day 2 — 더미 통합 테스트
### SYSTEM
- 더미 Player 이동 코드 시작.
- Trash/Bin 타입 구조 초안 작성.

### MAP
- 도심 20x15 구역 블록아웃.
- 시작 위치, 수거함 위치, 출구 위치 더미 배치.

### ASSET
- 더미 캐릭터 32x48 1장.
- 더미 쓰레기 16x16 1장.
- 더미 수거함 32x48 1장.

### LEAD
- 더미 에셋이 Unity에서 크기/Pivot 문제 없는지 확인.
- 첫 통합 테스트 진행.

### 연결 확인
- ASSET → SYSTEM: 더미 Sprite 전달.
- SYSTEM → MAP: Player Prefab 임시본 전달.
- MAP → SYSTEM: 충돌 Layer/Tilemap 구조 공유.

### 완료 기준
- Player가 도심 더미 맵에서 움직인다.
- 더미 쓰레기/수거함이 Scene에 보인다.

## Day 3 — 이동/도심 맵 기준 완성
### SYSTEM
- `phase_system.md` Phase 1 완료 목표.
- 상하좌우 이동, 방향 상태값 처리.
- Rigidbody2D/Collider2D 기준 설정.

### MAP
- `phase_map_scene.md` Phase 1 진행.
- 도심 구역 통로/장애물/충돌 기준 구성.
- 하천 진입 잠금 위치 배치.

### ASSET
- 플레이어 Idle down/up/side 제작 시작.
- 무료 타일팩 최종 후보 적용 테스트.

### LEAD
- 이동/충돌 Play Mode 확인.

### 연결 확인
- SYSTEM ↔ ASSET: Animator 방향명 `up/down/side` 확인.
- SYSTEM ↔ MAP: 충돌 Layer 이름 확인.

### 완료 기준
- 플레이어가 도심에서 벽을 통과하지 않고 이동한다.
- 방향 상태가 구분된다.

## Day 4 — 수거/가방 1차
### SYSTEM
- `phase_system.md` Phase 2 진행.
- 수거 범위 감지.
- F키 수거.
- 가방 카운트/용량 5개 제한.

### MAP
- 도심 쓰레기 배치 위치 20개 후보 지정.
- Trash Prefab 배치 테스트.

### ASSET
- 쓰레기 7종 제작 시작.
- 우선 paper/can/plastic 3종 완료 목표.

### LEAD
- 수거 기능 통합 확인.

### 연결 확인
- SYSTEM → MAP: Trash Prefab 필수 필드 전달.
- ASSET → SYSTEM: 쓰레기 Sprite 파일명 전달.

### 완료 기준
- 도심에서 쓰레기를 주울 수 있다.
- 가방 5개 제한이 작동한다.

## Day 5 — 분리수거 1차
### SYSTEM
- `phase_system.md` Phase 3 진행.
- BinType 5종 구현.
- TrashType 7종 구현.
- 정답/오답 판정 1차.
- 오답 점수 -5 적용.

### MAP
- 도심 수거함 1세트 배치.
- 상점 위치 더미 배치.

### ASSET
- 수거함 5종 제작 시작.
- 쓰레기 7종 1차 완료.

### LEAD
- 분리수거 루프 Play Mode 확인.

### 연결 확인
- SYSTEM ↔ ASSET: TrashType/BinType 이름 일치 확인.
- MAP ↔ SYSTEM: Bin Prefab 타입 설정 확인.

### 완료 기준
- 쓰레기를 수거함에 넣을 수 있다.
- 정답/오답이 구분된다.

## Day 6 — 정화율/코인/HUD 1차
### SYSTEM
- `phase_system.md` Phase 4 일부 진행.
- 오염도/코인 계산.
- 정화율 계산.
- HUD 데이터 전달.

### MAP
- 도심 쓰레기 20개 실제 배치.
- 도심 플레이 동선 조정.

### ASSET
- HUD 아이콘 1차: 점수, 코인, 정화율, 가방.
- 수거함 5종 1차 완료.

### LEAD
- 도심 1구역 루프 통합 확인.

### 연결 확인
- SYSTEM ↔ MAP: 도심 전체 오염도 계산에 필요한 Trash 수량/타입 확인.
- SYSTEM ↔ ASSET: HUD 아이콘 연결 방식 확인.

### 완료 기준
- 도심에서 수거→분리수거→코인/정화율 증가가 작동한다.

## Day 7 — 도심 완성/중간 통합
### SYSTEM
- 도심 기준 루프 버그 수정.
- 가방/수거함/정화율 오류 수정.

### MAP
- 도심 구역 최종 정리.
- 하천 진입 Gate 더미 유지.

### ASSET
- 플레이어 Walk down/up/side 각 4프레임 완료 목표.
- 쓰레기/수거함 크기 보정.

### LEAD
- 중간 통합.
- main에서 도심 루프 실행 확인.
- 문제 목록 작성.

### 연결 확인
- 세 담당 모두 도심 루프를 직접 실행해 본다.

### 완료 기준
- 도심 1구역은 발표 가능한 수준으로 플레이된다.

## Day 8 — 하천/해안 간소 구역 확장
### SYSTEM
- 구역 데이터 구조 확장.
- Zone ID 또는 Zone Manager 구조 정리.

### MAP
- `phase_map_scene.md` Phase 2 진행.
- 하천 20x15, 해안 20x15 간소 구역 구성.
- 도심→하천→해안 연결 통로 구성.

### ASSET
- 하천/해안용 무료 타일/장식 적용 지원.
- 비닐/어망/기름 가독성 보정.

### LEAD
- 3구역 Scene 이동 가능 여부 확인.

### 연결 확인
- SYSTEM ↔ MAP: 구역 경계 이름, Gate 이름, Zone ID 확인.
- MAP ↔ ASSET: 하천/해안 타일 스타일 확인.

### 완료 기준
- 3구역이 한 Scene 안에 존재한다.
- 플레이어가 잠금 전/후 이동할 위치가 정해진다.

## Day 9 — 구역 개방/특수 쓰레기
### SYSTEM
- 구역 정화율 100% 시 다음 Gate 개방.
- 어망/기름 특수 수거함 처리.

### MAP
- 하천 쓰레기 25개, 해안 쓰레기 30개 배치.
- 특수 쓰레기 위치 배치.

### ASSET
- Pickup down/up/side 각 3프레임 제작 또는 대체 이펙트 준비.
- 특수 수거함 시각 보정.

### LEAD
- 도심→하천 개방 테스트.
- 하천→해안 개방 테스트.

### 연결 확인
- SYSTEM ↔ MAP: 각 구역 쓰레기 수량/타입 실제 배치와 데이터 일치 확인.
- SYSTEM ↔ ASSET: 특수 타입 Sprite/아이콘 연결 확인.

### 완료 기준
- 100% 정화 시 다음 구역으로 진행 가능하다.
- 특수 쓰레기를 특수 수거함에서 처리할 수 있다.

## Day 10 — 업그레이드/결과화면 1차
### SYSTEM
- `phase_system.md` Phase 5 진행.
- 집게/가방/신발 업그레이드 적용.
- 비용 50/100코인 적용.
- 결과화면 1차 표시.

### MAP
- 구역별 수거함 옆 상점 위치 배치.
- 상점 상호작용 위치 확인.

### ASSET
- 상점/업그레이드 UI 최소 리소스.
- 결과화면 별 아이콘 제작.

### LEAD
- 업그레이드가 기존 루프를 깨지 않는지 확인.

### 연결 확인
- SYSTEM ↔ MAP: 상점 오브젝트 이름/위치 확인.
- SYSTEM ↔ ASSET: 결과화면 UI 리소스 확인.

### 완료 기준
- 코인으로 업그레이드 가능.
- 결과화면이 최소 형태로 표시된다.

## Day 11 — 전체 루프 연결
### SYSTEM
- 전체 루프 버그 수정.
- 결과화면 표시 타이밍 확정.

### MAP
- 발표용 플레이 경로 조정.
- 막히는 통로/불필요한 장식 제거.

### ASSET
- 누락 Sprite/UI 보완.
- 크기/Pivot 문제 수정.

### LEAD
- 시작부터 결과화면까지 전체 플레이 테스트.

### 연결 확인
- 세 담당 모두 자기 파트가 전체 루프에서 정상 작동하는지 확인.

### 완료 기준
- 시작 → 도심 → 하천 → 해안 → 결과화면 흐름이 한 번 이상 성공한다.

## Day 12 — 안정화 1일차
### 전체 원칙
- 새 기능 추가 금지.
- 버그 수정, 누락 리소스, 플레이 루트 안정화만 진행.

### SYSTEM
- 치명 버그 수정.
- 예외 상황 처리: 가방 가득 참, 오답, 코인 부족.

### MAP
- 플레이어가 끼는 위치 제거.
- 시연 동선 정리.

### ASSET
- 잘 안 보이는 Sprite 수정.
- UI 가독성 수정.

### LEAD
- 버그 리스트 우선순위 관리.

### 완료 기준
- 전체 루프가 2회 연속 성공한다.

## Day 13 — QA/빌드 확인
### 전체 원칙
- 새 기능 추가 금지.
- 빌드/시연 안정성만 확인.

### SYSTEM
- 남은 버그 수정.

### MAP
- 시연 루트 최종 고정.

### ASSET
- 발표 화면에서 보이는 리소스만 최종 보정.

### LEAD
- 빌드 테스트.
- 백업 빌드 생성.
- 발표 시연 체크리스트 작성.

### 완료 기준
- Unity Editor Play Mode 성공.
- 빌드 실행 성공.
- 발표용 백업본 존재.

## Day 14 — 발표 준비/리허설
### 전체 원칙
- 개발 변경 최소화.
- 리허설과 백업에 집중.

### SYSTEM
- 치명 버그만 수정.

### MAP
- 시연 시작 위치/동선 확인.

### ASSET
- 발표 자료에 들어갈 스크린샷/아이콘 지원.

### LEAD
- 발표 리허설.
- 시연 실패 시 대체 설명 준비.
- 최종 빌드/프로젝트 백업.

### 완료 기준
- 발표자가 정해진 루트로 시연 가능.
- 실패 시 대체 시나리오가 있다.

## 매일 점검 질문
- 오늘 각자 완료한 것은 무엇인가?
- Unity에서 직접 실행해 봤는가?
- 다른 담당자에게 넘겨야 할 파일/Prefab/정보가 있는가?
- 내일 작업을 막는 의존성이 있는가?
- main 통합 가능한 상태인가?

## 파트 간 핵심 인터페이스

### SYSTEM ↔ MAP
- Player 시작 위치.
- Trash Prefab 필드.
- Bin Prefab 타입.
- Zone ID/Gate 이름.
- 구역별 쓰레기 수량.

### SYSTEM ↔ ASSET
- Animator 방향명: `up`, `down`, `side`.
- TrashType 이름.
- BinType 이름.
- HUD 아이콘 연결 이름.
- Pickup 애니메이션 사용 여부.

### MAP ↔ ASSET
- Tile 크기 32x32.
- 캐릭터 32x48이 맵에서 어색하지 않은지.
- 쓰레기 16x16 가독성.
- 수거함 32x48 크기.
- 무료 에셋팩 스타일 통일.

## 최종 대체 전략
- 캐릭터 애니메이션 실패: 무료 캐릭터 수정 또는 Pickup 이펙트 대체.
- 3구역 완성 실패: 도심 완성 + 하천/해안 간소 구역 유지.
- 업그레이드 실패: 가방 용량 업그레이드만 남기고 집게/신발은 발표 설명으로 처리.
- 결과화면 실패: 게임 종료 지점에서 HUD 수치로 발표.
