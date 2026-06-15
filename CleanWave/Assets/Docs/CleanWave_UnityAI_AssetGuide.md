# CleanWave Unity AI Asset Guide

> **권장 위치**: Unity 프로젝트 루트에 `AGENTS.md`로 복사하거나, `Assets/Docs/CleanWave_UnityAI_AssetGuide.md`로 보관한다.  
> **목적**: Unity AI가 CleanWave의 현재 에셋 체계, 맵 자동 구성 규칙, 쓰레기 분류 규칙을 먼저 이해하고 불필요한 전체 프로젝트 스캔/무분별한 파일 생성을 줄이기 위한 기준 문서이다.

---

## 0. Unity AI 작업 원칙

Unity AI는 이 문서를 먼저 읽고 아래 원칙을 따른다.

1. **새 아트를 생성하지 않는다.** 현재 등록된 에셋만 사용한다.
2. **AI 자동 맵 구성용**이므로 예쁜 배치보다 **규칙적 분류 가능성**과 **Unity Rule Tile 사용 가능성**을 우선한다.
3. 타일셋 평가는 아래 우선순위를 따른다.
   - 1순위: **오토타일 / Unity Rule Tile 가능성**
   - 2순위: **AI 자동 분류성**
   - 3순위: **결합성**
   - 4순위: **CleanWave 적합성**
   - 5순위: 시각적 완성도
4. 한 파일에 서로 다른 역할을 과하게 섞지 않는다.
   - 예: 도로 + 보도 + 횡단보도 혼합 금지
   - 예: 경계용 바위 + 장식 바위 혼합 금지
   - 예: 나무 + 덤불 + 바위 + 통나무 혼합은 가능하나, AI 자동 배치용이면 별도 분류가 필요하다.
5. 파일 수정, 씬 수정, 프리팹 생성, 스크립트 생성 전에는 사용자 승인 요청을 우선한다.
6. 맵 생성 시 전체 프로젝트를 무작정 분석하지 말고, 이 문서의 에셋 목록과 규칙을 기준으로 필요한 폴더만 조회한다.

---

## 1. 프로젝트 개요

CleanWave는 **2D 탑다운 환경 정화 / 플로깅 교육 게임**이다.

### 기본 월드 구조

```text
1단계: 도심
2단계: 하천
3단계: 해안
```

### 진행 구조

- 플레이어는 도심에서 시작한다.
- 도심 정화 후 하천 구역으로 이동한다.
- 하천 정화 후 해안 구역으로 이동한다.
- 전체 동선은 위에서 아래로 이어지는 **S자형 메인 루트**를 기준으로 한다.

### 학습 구조

각 구역은 쓰레기 종류가 누적되는 방식이다.

```text
도심: 일반 생활 쓰레기 학습
하천: 도심 쓰레기 + 수질 오염 쓰레기 학습
해안: 도심/하천 쓰레기 일부 + 해안 오염 쓰레기 학습
```

---

## 2. 에셋 제작 및 사용 핵심 기준

CleanWave 에셋은 사람이 수동으로 예쁘게 배치하는 용도가 아니라, **AI가 맵을 자동 구성할 수 있도록 규칙적으로 분류 가능한 구조**여야 한다.

### 타일셋 필수 조건

```text
- 32x32 tile 기준
- 동일한 셀 크기
- 일정한 grid atlas
- 잘린 타일 없음
- 역할이 명확한 타일
- 중복/유사 타일 최소화
- Unity Rule Tile 구성 가능
```

### 오토타일 평가 체크리스트

오토타일 계열은 반드시 아래 요소를 기준으로 본다.

```text
- 직선 horizontal / vertical
- 내부 코너 inside corners
- 외부 코너 outside corners
- convex corners
- concave corners
- T-junctions
- 4-way cross junctions
- end caps / dead ends
- curved variants
- 일정한 폭 / 두께 / 중심 정렬
```

### Prop 평가 체크리스트

Prop 계열은 Rule Tile보다는 자동 배치 가능성을 본다.

```text
- 개별 오브젝트로 분리되어 있는가
- 실루엣이 명확한가
- 카테고리 구분이 쉬운가
- 텍스트/라벨 없이 아이콘과 형태만으로 이해 가능한가
- 상호작용 오브젝트라면 개별 Collider 설정이 가능한가
```

---

## 3. CleanWave 통합 컬러 팔레트

새 에셋을 만들거나 기존 에셋을 보정할 때 아래 색상 범위를 따른다. 정확한 단일값보다 **범위**가 중요하다.

| 지형 | Base | Light | Dark | 느낌 |
|---|---|---|---|---|
| Grass | `#5CAF20 ~ #78C40F` | `#95D31C ~ #A8DA3A` | `#248B23 ~ #3AA020` | 밝은 노랑빛 잔디 |
| Dirt | `#B88444 ~ #C2924C` | `#CDAA70 ~ #D8B57A` | `#78552A ~ #956A38` | 따뜻한 황토색 |
| Water | `#214F68 ~ #2E617A` | `#3F7F95 ~ #56A0AD` | `#1B3F55 ~ #233F4B` | 어두운 청록 물 |
| Rock | `#7E7060 ~ #A18F72` | `#BBA57C ~ #C7B28D` | `#58402C ~ #75512E` | 회갈색/따뜻한 바위 |
| Sand | `#E0C16F ~ #EBD48C` | `#F2DFA4 ~ #F6E7B5` | `#B88E45 ~ #C49B55` | 연한 베이지 모래 |
| City Asphalt | `#3A3A3A ~ #505050` | `#6B6B6B ~ #777777` | `#222222 ~ #2E2E2E` | 어둡지만 너무 검지 않은 도로 |
| City Pavement | `#AFA998 ~ #C5BFAA` | `#D4CEBA ~ #E0D8C0` | `#777064 ~ #8C8373` | 밝은 회베이지 보도 |

### 팔레트 사용 규칙

```text
- 물은 네온 블루 금지
- 모래는 너무 노랗거나 주황색 금지
- 잔디는 과도하게 형광 녹색 금지
- 바위는 너무 검거나 대비 강한 블록 느낌 금지
- 전체 지형의 명암 깊이와 채도는 통일한다
```

---

## 4. 파일명 규칙

### Terrain

```text
Terrain_[Type]_[Role].png
```

예:

```text
Terrain_Grass.png
Terrain_DirtRoad_Autotile.png
Terrain_GrassDirtEdges.png
Terrain_RiverWater_Autotile.png
Terrain_Sand_Autotile.png
Terrain_GrassSandEdges_Autotile.png
Terrain_SandWaterEdges_Autotile.png
Terrain_CityRoad_Asphalt_Autotile.png
Terrain_CitySidewalk_Pavement_Autotile.png
```

### Props

```text
Props_[Category]_[Role].png
```

예:

```text
Props_Tree.png
Props_RockBorder_Autotile.png
Props_RockCluster_Decoration.png
Props_RecycleBins_4Type.png
Props_Trash_Urban.png
Props_Trash_River.png
Props_Trash_Coast.png
```

### 사용하지 않는 명명 방식

```text
New Image.png
grass edge tileset.png
Dirt.png
GLASS.png
imagegen.png
```

---

## 5. 권장 폴더 구조

```text
Assets/
  Art/
    Terrain/
      Grass/
        Terrain_Grass.png
        Terrain_GrassDirtEdges.png

      DirtRoad/
        Terrain_DirtRoad_Autotile.png

      River/
        Terrain_RiverWater_Autotile.png

      Beach/
        Terrain_Sand_Autotile.png
        Terrain_GrassSandEdges_Autotile.png
        Terrain_SandWaterEdges_Autotile.png

      City/
        Terrain_CityRoad_Asphalt_Autotile.png
        Terrain_CitySidewalk_Pavement_Autotile.png

      Rock/
        Props_RockBorder_Autotile.png
        Props_RockCluster_Decoration.png

    Props/
      Nature/
        Props_Tree.png

      Gameplay/
        Props_RecycleBins_4Type.png
        Props_Trash_Urban.png
        Props_Trash_River.png
        Props_Trash_Coast.png
```

---

## 6. 현재 에셋 목록과 역할

### Terrain 계열

| 파일명 | 역할 | 사용 방식 | 주의점 |
|---|---|---|---|
| `Terrain_Grass.png` | 잔디 기본 바닥/변형 | BaseGround Tilemap | 경계용이 아니라 기본 바닥용이다. |
| `Terrain_GrassDirtEdges.png` | 잔디↔흙길 연결 | Transition/Edge Tilemap 또는 Rule Tile | CleanWave 메인 S자 길 연결 핵심. |
| `Terrain_DirtRoad_Autotile.png` | 흙길 오토타일 | Road/Path Tilemap Rule Tile | 기존 `Terrain_DirtRoad.png`보다 우선 사용. |
| `Terrain_RiverWater_Autotile.png` | 하천 물/잔디↔물 연결 | Water Tilemap Rule Tile | 하천 구역 핵심. 물 색은 청록 계열 유지. |
| `Terrain_CityRoad_Asphalt_Autotile.png` | 도심 아스팔트 도로 | CityRoad Tilemap Rule Tile | 보도/횡단보도와 섞지 않는다. |
| `Terrain_CitySidewalk_Pavement_Autotile.png` | 도심 보도/포장도로 | CityPavement Tilemap Rule Tile | 도로와 별도 레이어로 운용 가능. |
| `Terrain_Sand_Autotile.png` | 해안 모래 기본 바닥 | BeachGround Tilemap Rule Tile | Grass/Sand, Sand/Water 연결은 별도 에셋 사용. |
| `Terrain_GrassSandEdges_Autotile.png` | 잔디↔모래 연결 | Beach Transition Tilemap | 해안 진입부, 모래길 연결에 사용. |
| `Terrain_SandWaterEdges_Autotile.png` | 모래↔물 연결 | Shoreline Tilemap Rule Tile | 해안선 핵심 에셋. |

### Prop 계열

| 파일명 | 역할 | 사용 방식 | 주의점 |
|---|---|---|---|
| `Props_RockBorder_Autotile.png` | 바위 경계/장벽 | RockBorder Tilemap 또는 Props | 자연 바위보다는 돌담/경계 느낌. AI 배치용으로 좋음. |
| `Props_RockCluster_Decoration.png` | 장식 바위더미 | Props layer | Rule Tile이 아니라 장식 배치용. |
| `Props_Tree.png` | 나무/덤불/풀/돌/통나무 | Props layer | AI 자동 배치용이면 Tree/Bush/Plant/Rock로 분리 권장. |
| `Props_RecycleBins_4Type.png` | 4종 수거함 | Gameplay Props | 개별 상호작용 오브젝트로 사용. |
| `Props_RecycleBins_Station.png` | 묶음형 수거함 | Reference/Decorative | 상호작용이 애매하므로 메인 사용 비추천. |
| `Props_Trash_Urban.png` | 도심 쓰레기 4종 | Collectible Props | 도심 구역 기본 쓰레기. |
| `Props_Trash_River.png` | 하천 쓰레기 4종 | Collectible Props | 하천 구역부터 추가 등장. |
| `Props_Trash_Coast.png` | 해안 쓰레기 4종 | Collectible Props | 해안 구역부터 추가 등장. |

---

## 7. 삭제/비활성 추천 에셋

아래 에셋은 AI 자동 맵 구성용 메인 에셋으로 쓰지 않는다.

```text
Terrain_DirtRoad.png
Terrain_CityRoadPavement.png
Markings_Crosswalk.png
TrafficLight-related assets
RoadSigns-related assets
```

### 이유

- `Terrain_DirtRoad.png`: 새 `Terrain_DirtRoad_Autotile.png`가 더 규칙적이다.
- `Terrain_CityRoadPavement.png`: 도로/보도/횡단보도가 섞여 AI 분류에 불리하다.
- `Markings_Crosswalk.png`: MVP에서 제외. 횡단보도는 도로 구조와 강하게 묶여 AI 생성이 흔들린다.
- 신호등/도로표지판: MVP에 불필요하며 배치 복잡도를 올린다.

---

## 8. Tilemap 레이어 권장 구조

Unity 씬에서는 아래 순서로 Tilemap을 구성한다.

```text
Grid
  Tilemap_BaseGround
  Tilemap_RoadPath
  Tilemap_Water
  Tilemap_TerrainEdges
  Tilemap_RockBorder
  Tilemap_CityRoad
  Tilemap_CityPavement
  Tilemap_PropsDecor
  Tilemap_GameplayProps
  Tilemap_Collision
```

### 역할

- `Tilemap_BaseGround`: Grass, Sand, CityPavement 같은 넓은 바닥.
- `Tilemap_RoadPath`: DirtRoad, 메인 S자 동선.
- `Tilemap_Water`: RiverWater.
- `Tilemap_TerrainEdges`: GrassDirt, GrassSand, SandWater 경계.
- `Tilemap_RockBorder`: 바위 경계.
- `Tilemap_PropsDecor`: 나무, 바위더미, 풀 등 장식.
- `Tilemap_GameplayProps`: 수거함, 쓰레기, 상호작용 오브젝트.
- `Tilemap_Collision`: 보이지 않는 충돌 타일.

---

## 9. 맵 자동 구성 순서

AI가 맵을 만들 때는 아래 순서로 배치한다.

### 1단계: 구역 레이아웃

```text
상단: 도심
중단: 하천
하단: 해안
```

### 2단계: 메인 동선

- 위에서 아래로 이어지는 S자형 길을 만든다.
- 도심에서는 CityRoad/Sidewalk를 사용한다.
- 자연 구역에서는 DirtRoad와 GrassDirtEdges를 사용한다.
- 해안 구역에서는 Sand, GrassSandEdges, SandWaterEdges를 사용한다.

### 3단계: 물/해안 배치

- 하천: `Terrain_RiverWater_Autotile.png`
- 해안: `Terrain_SandWaterEdges_Autotile.png`

### 4단계: 경계/장식

- 하천/해안 경계에 `Props_RockBorder_Autotile.png`를 사용한다.
- 자연 장식에는 `Props_Tree.png`와 `Props_RockCluster_Decoration.png`를 사용한다.

### 5단계: 게임플레이 오브젝트

- 모든 구역에 4종 수거함을 배치한다.
- 구역별 쓰레기 배치는 아래 쓰레기 분류 규칙을 따른다.

---

## 10. 구역별 쓰레기 학습 구조

### 수거함 4종

`Props_RecycleBins_4Type.png`를 사용한다.

| 색상 | 아이콘 | 분류 | 사용 구역 |
|---|---|---|---|
| Gray | 쓰레기봉투 | 일반 쓰레기 | 모든 구역 |
| Yellow | 플라스틱 병 | 플라스틱 | 모든 구역 |
| Green | 캔 | 캔/금속 | 모든 구역 |
| Red | 경고 아이콘 | 특수 쓰레기 | 모든 구역 |

> 모든 구역에서 같은 4종 수거함을 사용한다. 학습 효과는 “새 구역으로 갈수록 새로운 쓰레기 유형이 추가되는 구조”에서 나온다.

### 도심 쓰레기: `Props_Trash_Urban.png`

| 오브젝트 | 수거함 분류 | 의도 |
|---|---|---|
| 종이컵 | 일반 쓰레기 | 기본 생활 쓰레기 |
| 비닐봉지 | 플라스틱 | 플라스틱 분류 학습 |
| 찌그러진 캔 | 캔/금속 | 캔/금속 분류 학습 |
| 음식물 남은 용기 | 특수 쓰레기 | 음식물/오염 쓰레기 분류 학습 |

### 하천 쓰레기: `Props_Trash_River.png`

| 오브젝트 | 수거함 분류 | 의도 |
|---|---|---|
| 페트병 | 플라스틱 | 물가 플라스틱 오염 |
| 세제통 | 특수 쓰레기 | 화학/수질 오염 |
| 오염된 플라스틱 통 | 플라스틱 | 오염된 플라스틱 용기 |
| 오염 쓰레기 뭉치 | 특수 쓰레기 | 하천 오염 덩어리 |

### 해안 쓰레기: `Props_Trash_Coast.png`

| 오브젝트 | 수거함 분류 | 의도 |
|---|---|---|
| 어망 | 특수 쓰레기 | 해양/어업 폐기물 |
| 로프 | 특수 쓰레기 | 해양 폐기물 |
| 부표 | 플라스틱 | 해안 플라스틱 오염 |
| 기름통/드럼통 | 특수 쓰레기 | 기름/위험 오염 |

### 구역별 등장 규칙

```text
도심: Urban 4종만 사용
하천: Urban 일부 + River 4종 사용
해안: Urban 일부 + River 일부 + Coast 4종 사용
```

단, MVP에서는 단순하게 아래처럼 해도 된다.

```text
도심: Props_Trash_Urban
하천: Props_Trash_River
해안: Props_Trash_Coast
```

---

## 11. 쓰레기 배치 규칙

AI 자동 맵 생성 시 쓰레기는 아래 규칙으로 배치한다.

```text
- 길 중앙을 막지 않는다.
- 플레이어 이동 경로 옆, 풀밭, 물가, 해안가에 배치한다.
- 수거함 근처에 너무 많이 몰아두지 않는다.
- 한 화면에서 같은 쓰레기를 과도하게 반복하지 않는다.
- 같은 구역 안에서도 4종이 고르게 등장하도록 한다.
- 각 구역에서 플레이어가 해당 구역의 학습 목표를 이해할 수 있게 배치한다.
```

### 추천 배치 수

```text
도심: 12~20개
하천: 16~24개
해안: 20~30개
```

### 쓰레기 밀도

```text
도심: 건물 주변, 길가, 공원 주변
하천: 물가, 갈대 주변, 다리 근처
해안: 모래사장, 선착장, 등대 주변, 해안선 근처
```

---

## 12. 수거함 배치 규칙

`Props_RecycleBins_4Type.png`는 개별 상호작용 오브젝트로 사용한다.

```text
- 4개 수거함은 서로 붙여서 하나의 station처럼 보이게 배치할 수 있지만, 실제 GameObject는 개별로 유지한다.
- 각 수거함은 Collider2D Trigger를 가진다.
- 각 수거함은 BinCategory를 가진다.
- 플레이어가 쓰레기를 들고 해당 수거함에 접근하면 분류 판정을 수행한다.
```

### 권장 수거함 GameObject 이름

```text
Bin_General
Bin_Plastic
Bin_CanMetal
Bin_Special
```

### 권장 쓰레기 GameObject 이름

```text
Trash_Urban_PaperCup
Trash_Urban_PlasticBag
Trash_Urban_CrushedCan
Trash_Urban_FoodContainer

Trash_River_PETBottle
Trash_River_DetergentBottle
Trash_River_ContaminatedPlasticContainer
Trash_River_PollutedTrashClump

Trash_Coast_FishingNet
Trash_Coast_RopeBundle
Trash_Coast_Buoy
Trash_Coast_OilDrum
```

---

## 13. Unity Import 설정

### Terrain Tileset

```text
Texture Type: Sprite (2D and UI)
Sprite Mode: Multiple
Pixels Per Unit: 32
Filter Mode: Point (no filter)
Compression: None
Mesh Type: Full Rect
```

### Props

```text
Texture Type: Sprite (2D and UI)
Sprite Mode: Multiple
Pixels Per Unit: 32 또는 64
Filter Mode: Point (no filter)
Compression: None
Mesh Type: Full Rect
```

### Slice 규칙

- Terrain Autotile은 가능한 한 `Grid by Cell Size = 32 x 32`로 자른다.
- Props는 개별 오브젝트가 32x32를 넘는 경우 수동 Slice를 허용한다.
- 수거함은 64x64 이상일 수 있으므로 Sprite Editor에서 개별로 자른 뒤 동일 스케일을 맞춘다.
- 쓰레기 오브젝트는 가능한 한 32x32 또는 64x64 셀 기준으로 정리한다.

---

## 14. 권장 컴포넌트 설계

### TrashCategory

```csharp
public enum TrashCategory
{
    General,
    Plastic,
    CanMetal,
    Special
}
```

### ZoneType

```csharp
public enum ZoneType
{
    Urban,
    River,
    Coast
}
```

### TrashData 개념

```text
TrashData
- id
- displayName
- category
- zone
- sprite
- scoreValue
```

### BinData 개념

```text
BinData
- id
- category
- sprite
- acceptedCategories
```

### 판정 규칙

```text
if trash.category == bin.category:
    correct
else:
    incorrect
```

특수 쓰레기는 구역별 위험/오염 쓰레기를 포괄한다.

---

## 15. AI 맵 생성 시 금지 사항

Unity AI는 아래 작업을 하지 않는다.

```text
- 없는 에셋을 임의로 만들기
- 텍스트 라벨이 있는 쓰레기/수거함 만들기
- 도로와 보도를 한 타일셋에 다시 섞기
- 횡단보도/신호등/표지판을 MVP에 강제로 추가하기
- 장식 오브젝트를 이동 경로 중앙에 과하게 배치하기
- 쓰레기를 수거함 바로 옆에 몰아넣기
- 물 타일 위에 도심 쓰레기를 무작위로 배치하기
- 한 구역의 학습 목표와 맞지 않는 쓰레기를 과도하게 배치하기
```

---

## 16. AI에게 줄 기본 작업 지시문

Unity AI에게 맵 제작을 요청할 때는 아래 지시문을 먼저 준다.

```text
Read CleanWave_UnityAI_AssetGuide.md first.
Use only the listed assets.
Do not generate new art.
Do not create unnecessary files.
Do not scan unrelated folders.
Do not modify scripts unless explicitly requested.
Use the asset roles, naming rules, tilemap layers, and trash classification rules from the guide.
Prioritize Unity Rule Tile usability, AI classification clarity, connectivity, and CleanWave gameplay suitability.
Ask before creating or modifying scene objects.
```

---

## 17. AI 맵 생성 요청 예시

```text
Create a CleanWave prototype map using the existing assets only.

Requirements:
- Use a 2D top-down Tilemap scene.
- Create three connected zones: Urban, River, Coast.
- Use an S-shaped main path from top to bottom.
- Use Terrain_CityRoad_Asphalt_Autotile and Terrain_CitySidewalk_Pavement_Autotile for Urban.
- Use Terrain_DirtRoad_Autotile and Terrain_GrassDirtEdges for natural paths.
- Use Terrain_RiverWater_Autotile for River.
- Use Terrain_Sand_Autotile, Terrain_GrassSandEdges_Autotile, and Terrain_SandWaterEdges_Autotile for Coast.
- Place Props_RecycleBins_4Type in every zone as four separate interactable bins.
- Place trash according to the zone rules:
  Urban: Props_Trash_Urban
  River: Props_Trash_River
  Coast: Props_Trash_Coast
- Do not use crosswalks, traffic lights, or road signs.
- Ask before modifying the scene.
```

---

## 18. 현재 기준 최종 에셋 상태

### 사용 확정

```text
Terrain_Grass.png
Terrain_GrassDirtEdges.png
Terrain_DirtRoad_Autotile.png
Terrain_RiverWater_Autotile.png
Terrain_CityRoad_Asphalt_Autotile.png
Terrain_CitySidewalk_Pavement_Autotile.png
Terrain_Sand_Autotile.png
Terrain_GrassSandEdges_Autotile.png
Terrain_SandWaterEdges_Autotile.png
Props_RockBorder_Autotile.png
Props_RockCluster_Decoration.png
Props_Tree.png
Props_RecycleBins_4Type.png
Props_Trash_Urban.png
Props_Trash_River.png
Props_Trash_Coast.png
```

### 참고용 또는 비추천

```text
Props_RecycleBins_Station.png
Terrain_CityRoadPavement.png
Terrain_DirtRoad.png
Markings_Crosswalk.png
```

---

## 19. 최종 요약

CleanWave 에셋 체계는 아래 원칙을 따른다.

```text
타일셋은 Rule Tile 가능성이 최우선이다.
Prop은 AI가 자동 분류/배치할 수 있어야 한다.
수거함은 4종만 사용한다.
쓰레기는 도심/하천/해안 4종씩만 사용한다.
구역이 진행될수록 새로운 오염 유형이 추가된다.
모든 에셋은 텍스트 없이 아이콘/형태/색상으로 구분한다.
MVP에서는 횡단보도, 신호등, 도로표지판은 제외한다.
```
