# 에셋 제작 가이드

이 문서는 그래픽이 따로 노는 현상과 Unity 적용 오류를 줄이기 위한 최소 에셋 제작 규칙입니다.

## 스타일 방향

- 2D 탑다운 픽셀아트 느낌을 기준으로 한다.
- 무료 에셋과 직접 제작 픽셀아트를 혼합하되, 스타일은 1~2개 기준으로 제한한다.
- 직접 제작 우선 대상은 플레이어, 쓰레기 7종, 수거함 5종이다.
- 무료 에셋 우선 대상은 맵 타일, 배경 장식, 자연물, 건물류다.
- 예쁜 에셋보다 Unity에 바로 넣기 쉬운 에셋을 우선한다.

## 해상도 규격

| 대상 | 기준 크기 |
|---|---:|
| 타일 | 32x32 |
| 플레이어 캐릭터 | 32x48 |
| 쓰레기 | 16x16 |
| 수거함 | 32x48 |
| 작은 장식 | 32x32 |
| 큰 장식/건물 | 필요 시 64x64 이상 |

## 캐릭터 애니메이션

- 방향은 `down`, `up`, `side` 기준으로 만든다.
- 좌우는 `side` 하나를 만들고 Unity Flip으로 처리한다.
- Idle: down/up/side, 1~2프레임
- Walk: down/up/side, 각 4프레임
- Pickup: down/up/side, 각 3프레임
- Pickup 제작이 어렵다면 집게 아이콘 또는 반짝 이펙트로 대체한다.

## 쓰레기와 수거함

- 쓰레기 7종은 색과 실루엣으로 구분한다.
- 쓰레기 종류: paper, can, plastic, vinyl, food, net, oil
- 수거함 5종: paper, can, plastic, general, special
- 특수 수거함은 net/oil 처리에 사용한다.
- 작은 크기에서는 디테일보다 가독성을 우선한다.

## 파일/폴더 규칙

- 소문자 영어만 사용한다.
- 띄어쓰기와 한글 파일명은 사용하지 않는다.
- 단어 구분은 언더스코어 `_`를 사용한다.
- 번호는 `01`, `02` 형식으로 작성한다.
- 파일을 Unity에 추가한 뒤 `.meta` 파일도 함께 확인한다.

## 파일명 예시

```text
chr_player_idle_down_01.png
chr_player_walk_side_01.png
chr_player_pickup_down_01.png
trash_paper_01.png
trash_can_01.png
trash_plastic_01.png
bin_paper_01.png
bin_can_01.png
bin_plastic_01.png
bin_general_01.png
bin_special_01.png
tile_city_grass_01.png
tile_river_water_01.png
tile_beach_sand_01.png
ui_coin_01.png
```

## Unity 적용 체크리스트

- [ ] 크기가 기준 규격과 맞는다.
- [ ] Pivot이 이상해서 떠 보이거나 묻히지 않는다.
- [ ] PPU 기준이 섞이지 않는다.
- [ ] 같은 화면에서 스타일이 크게 튀지 않는다.
- [ ] 파일명이 규칙에 맞다.
- [ ] `.meta` 파일이 같이 존재한다.

## 실패 시 대체안

- 캐릭터 제작 실패: 무료 캐릭터 에셋 수정
- 수거 모션 실패: Pickup 애니메이션 대신 집게 아이콘/반짝 이펙트
- 맵 에셋 통일 실패: 단일 무료 에셋팩만 사용
- 3구역 제작 실패: 도심 완성 + 하천/해안 간소 구역 유지
