# 에셋 제작 가이드

이 문서는 그래픽이 따로 노는 현상과 Unity 적용 오류를 줄이기 위한 최소 에셋 제작 규칙입니다.

## 스타일 방향

- 2D 탑다운 픽셀아트 느낌을 기준으로 한다.
- 무료 에셋과 직접 제작 픽셀아트를 혼합하되, 스타일은 1~2개 기준으로 제한한다.
- 직접 제작 우선 대상은 플레이어, 쓰레기 7종, 수거함 5종이다.
- 무료 에셋 우선 대상은 맵 타일, 배경 장식, 자연물, 건물류다.
- 예쁜 에셋보다 Unity에 바로 넣기 쉬운 에셋을 우선한다.
- 오분류 퀴즈형 피드백 팝업은 낮은 채도의 민트, 모래색, 연한 하늘색을 중심으로 만든다.
- 오답 강조는 강한 빨강보다 산호색 포인트 정도로 부드럽게 처리한다.

## 해상도 규격

| 대상 | 기준 크기 |
|---|---:|
| 타일 | 32x32 |
| 플레이어 캐릭터 | 32x48 |
| 쓰레기 | 16x16 |
| 수거함 | 32x48 |
| 작은 장식 | 32x32 |
| 큰 장식/건물 | 필요 시 64x64 이상 |
| UI 아이콘 | 16x16 또는 32x32 |
| UI 버튼/패널 | 9-slice 적용 가능한 픽셀 패널 |

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

## 오분류 퀴즈형 피드백 UI

이 UI는 오분류 직후 1회 표시되는 팝업이다. HUD 상시 요소가 아니며, 튜토리얼이나 상시 퀴즈 모드로 확장하지 않는다.

### 필수 UI 에셋

| 에셋 | 용도 | 파일명 예시 |
|---|---|---|
| 팝업 배경 패널 | 문제와 해설을 담는 둥근 사각형 패널 | `ui_quiz_panel_01.png` |
| 패널 장식 프레임 | 잎, 물결, 작은 자연 장식 | `ui_quiz_frame_leaf_01.png` |
| 질문 아이콘 | `?` 말풍선 또는 작은 환경 아이콘 | `ui_quiz_icon_question_01.png` |
| 선택지 버튼 기본 | A/B 선택지 기본 상태 | `ui_quiz_button_normal_01.png` |
| 선택지 버튼 정답 | 정답 선택지 강조 상태 | `ui_quiz_button_correct_01.png` |
| 선택지 버튼 오답 | 오답 선택지 강조 상태 | `ui_quiz_button_wrong_01.png` |
| 확인 버튼 | 해설 확인 후 닫기 | `ui_button_confirm_01.png` |

쓰레기 7종과 수거함 5종 아이콘은 기존 에셋을 작게 재사용한다. 예를 들어 `trash_food_01.png`, `bin_general_01.png`처럼 연결한다.

### 디자인 양식

- 팝업은 화면 중앙에 배치하고, 필요하면 반투명 어두운 배경을 깔아 시선을 모은다.
- 패널은 9-slice가 가능한 픽셀 패널로 만들어 Unity UI에서 크기 조절이 가능하게 한다.
- 제목은 `앗, 다시 생각해볼까요?`처럼 부드러운 문장으로 둔다.
- 문제, 선택지, 해설 텍스트는 이미지가 아니라 TextMeshPro 텍스트로 출력한다.
- 선택지는 2개만 사용한다.
- 해설 영역에는 정답 수거함 아이콘과 1~2문장 설명을 함께 보여준다.
- 팝업 등장 애니메이션은 0.15~0.25초 정도의 짧은 스케일 인으로 제한한다.

### UI 오브젝트 구조 예시

```text
QuizFeedbackPanel
├── DimBackground
├── PanelBackground
├── QuestionIcon
├── QuestionText
├── WrongTrashIcon
├── ChoiceButtonA
│   └── ChoiceTextA
├── ChoiceButtonB
│   └── ChoiceTextB
├── ExplanationBox
│   ├── CorrectBinIcon
│   └── ExplanationText
└── ConfirmButton
    └── ConfirmText
```

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
ui_quiz_panel_01.png
ui_quiz_frame_leaf_01.png
ui_quiz_icon_question_01.png
ui_quiz_button_normal_01.png
ui_quiz_button_correct_01.png
ui_quiz_button_wrong_01.png
ui_button_confirm_01.png
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
