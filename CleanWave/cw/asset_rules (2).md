# Draft: Asset Rules

## 목적
- 그래픽이 따로 노는 현상과 Unity 적용 오류를 줄이기 위한 최소 에셋 제작 규칙.

## 스타일 방향
- 스타듀밸리식 32x32 탑다운 감성.
- 무료 에셋 + 직접 제작 픽셀아트 혼합.
- 직접 제작: 플레이어, 쓰레기 7종, 수거함 5종.
- 무료 에셋 중심: 맵 타일, 배경 장식, 자연물, 건물류.

## 해상도 규격
- 타일: `32x32`
- 플레이어 캐릭터 캔버스: `32x48`
- 쓰레기: `16x16`
- 수거함: `32x48`
- 작은 장식: `32x32`
- 큰 장식/건물: 필요 시 `64x64` 이상 허용.

## 캐릭터 애니메이션
- 방향: `down`, `up`, `side`
- 좌우: `side` 하나 제작 후 Unity Flip 사용.
- Idle: down/up/side, 1~2프레임.
- Walk: down/up/side, 각 4프레임.
- Pickup: down/up/side, 각 3프레임.
- 원본 제작 파이프라인: GPT 이미지 → volidless.dev 스프라이트화 → Aseprite 정리 → Unity 테스트.

## 쓰레기/수거함 규칙
- 쓰레기 7종은 색 + 실루엣으로 구분한다.
- 수거함 5종: 종이, 캔, 플라스틱, 일반, 특수.
- 특수 수거함은 어망/기름 처리에 사용한다.

## 파일/폴더 규칙
- 소문자 영어만 사용한다.
- 띄어쓰기와 한글 파일명은 금지한다.
- 단어 구분은 언더스코어 `_`를 사용한다.
- 번호는 `01`, `02` 형식으로 작성한다.

## 파일명 예시
- `chr_player_idle_down_01.png`
- `chr_player_walk_side_01.png`
- `chr_player_pickup_down_01.png`
- `trash_paper_01.png`
- `trash_can_01.png`
- `trash_plastic_01.png`
- `bin_paper_01.png`
- `bin_can_01.png`
- `bin_plastic_01.png`
- `bin_general_01.png`
- `bin_special_01.png`
- `tile_city_grass_01.png`
- `tile_river_water_01.png`
- `tile_beach_sand_01.png`

## 실패 시 대체안
- 캐릭터 제작 실패: 무료 캐릭터 에셋 수정.
- 수거 모션 실패: Pickup 애니메이션 대신 집게 아이콘/반짝 이펙트.
- 맵 에셋 통일 실패: 단일 무료 에셋팩만 사용.
- 3구역 제작 실패: 도심 완성 + 하천/해안 간소 구역 유지.
