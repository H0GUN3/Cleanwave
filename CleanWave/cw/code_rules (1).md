# Draft: Code Rules

## 목적
- Unity 초보 팀이 2주 안에 안정적으로 MVP를 만들기 위한 C# 코드 작성 규칙.
- 복잡한 구조보다 읽기 쉽고 연결 실수가 적은 구조를 우선한다.

## 기본 원칙
- 기능은 작게 나눈다.
- 한 스크립트는 한 가지 책임만 가진다.
- `main`에 merge하기 전 Unity Play Mode에서 직접 실행한다.
- 발표에 필요 없는 확장 기능은 만들지 않는다.
- 임시 구현은 허용하지만 `TODO` 또는 `Temp` 표시를 남긴다.

## 네이밍 규칙

### C# 파일/클래스
- 파일명과 클래스명은 동일하게 쓴다.
- PascalCase 사용.

예시:
```txt
PlayerController.cs
TrashItem.cs
TrashBin.cs
BagInventory.cs
ZoneManager.cs
UpgradeShop.cs
HudController.cs
ResultScreen.cs
```

### 변수/필드
- private 필드: camelCase.
- Inspector에서 연결해야 하는 값은 `[SerializeField] private` 사용.

예시:
```csharp
[SerializeField] private float moveSpeed = 3f;
[SerializeField] private int bagCapacity = 5;
```

### 메서드
- PascalCase 사용.
- 동작이 드러나는 이름 사용.

예시:
```csharp
TryPickupTrash()
TryDepositTrash()
UpdatePurificationRate()
OpenNextZone()
```

## 폴더 규칙
```txt
Assets/
  Scripts/
    Player/
    Trash/
    Inventory/
    UI/
    Map/
    Upgrade/
    Core/
```

## 추천 스크립트 책임

### PlayerController
- 플레이어 이동.
- 방향 상태 계산.
- 상호작용 키 입력 전달.
- 직접 점수/정화율 계산하지 않는다.

### PlayerInteractor
- 플레이어 주변 상호작용 대상 감지.
- F키 입력 시 현재 대상에게 상호작용 요청.
- 수거함/쓰레기/상점의 세부 로직은 각 오브젝트에 맡긴다.

### BagInventory
- 가방 슬롯 관리.
- Q/E 선택 변경.
- 마우스 슬롯 선택.
- 현재 선택된 쓰레기 반환.
- 가방 용량 5/8/12 관리.

### TrashItem
- 쓰레기 타입 보유.
- 오염도/코인 값 보유.
- 수거될 때 BagInventory에 전달.

### TrashBin
- 수거함 타입 보유.
- 선택된 쓰레기가 올바른지 판정.
- 정답/오답 결과를 GameManager 또는 ScoreManager에 전달.

### ZoneManager
- 구역별 전체 오염도/처리 오염도 관리.
- 구역 정화율 100% 시 다음 Gate 열기.

### UpgradeShop
- F키 즉시 업그레이드 처리.
- 집게/가방/신발 단계 관리.
- 코인 부족 시 피드백.

### HudController
- 점수/코인/정화율/가방 UI 표시.
- 게임 로직 계산은 하지 않는다.

### ResultScreen
- 최종 점수/정화율/별등급 표시.

## Enum 규칙

### TrashType
```csharp
public enum TrashType
{
    Paper,
    Can,
    Plastic,
    Vinyl,
    Food,
    Net,
    Oil
}
```

### BinType
```csharp
public enum BinType
{
    Paper,
    Can,
    Plastic,
    General,
    Special
}
```

### ZoneType
```csharp
public enum ZoneType
{
    City,
    River,
    Beach
}
```

## 분리수거 매핑
- Paper → Paper
- Can → Can
- Plastic → Plastic
- Vinyl → General
- Food → General
- Net → Special
- Oil → Special

## Inspector 연결 규칙
- Prefab에 필요한 참조는 Inspector에서 빠짐없이 연결한다.
- 연결이 필요한 필드는 `[SerializeField] private`로 둔다.
- `FindObjectOfType` 남발 금지.
- 연결 누락 시 `Debug.LogWarning`으로 명확히 표시한다.

## 입력 규칙
- 이동: WASD 또는 방향키.
- 상호작용/버리기: F.
- 가방 선택 이동: Q/E.
- 가방 슬롯 선택: 마우스 클릭.
- 드래그앤드롭은 구현하지 않는다.

## UI 규칙
- TextMeshPro 중심.
- HUD 필수 표시: 점수, 코인, 정화율, 가방 슬롯.
- 가방 슬롯은 선택된 슬롯을 강조한다.
- 빈 슬롯에서 F를 누르면 “버릴 쓰레기가 없음” 피드백.
- 코인 부족 시 “코인이 부족합니다” 피드백.

## 금지 사항
- Scene 안 오브젝트 이름에 의존하는 하드코딩.
- 매직넘버 남발.
- 발표에 필요 없는 복잡한 인벤토리 UI.
- 드래그앤드롭 인벤토리.
- 자동 분류 업그레이드.
- 새 기능을 Day 12 이후 추가.

## 테스트 체크리스트
- [ ] 이동이 된다.
- [ ] 벽을 통과하지 않는다.
- [ ] 쓰레기를 수거할 수 있다.
- [ ] 가방이 5개에서 막힌다.
- [ ] Q/E로 슬롯 선택이 바뀐다.
- [ ] 마우스로 슬롯 선택이 된다.
- [ ] F로 선택 쓰레기를 버린다.
- [ ] 잘못 버리면 점수 -5.
- [ ] 올바르게 버리면 코인/정화율 증가.
- [ ] 구역 100% 시 Gate가 사라진다.
- [ ] 업그레이드가 적용된다.
- [ ] 결과화면이 표시된다.

## 미정
- 실제 이동 속도 수치.
- 기본 수거 범위 수치.
- Orthographic Size.
- 일일 통합 시간.
