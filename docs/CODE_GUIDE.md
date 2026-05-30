# C# 코드 가이드

이 문서는 Unity 초보 팀이 2주 안에 안정적으로 MVP를 만들기 위한 C# 코드 작성 규칙입니다. 복잡한 구조보다 읽기 쉽고 연결 실수가 적은 구조를 우선합니다.

## 기본 원칙

- 기능은 작게 나눈다.
- 한 스크립트는 한 가지 책임만 가진다.
- `main`에 merge하기 전 Unity Play Mode에서 직접 실행한다.
- 발표에 필요 없는 확장 기능은 만들지 않는다.
- 임시 구현은 허용하지만 `TODO` 또는 `Temp` 표시를 남긴다.

## 폴더 규칙

현재 Unity 프로젝트 기준 스크립트 위치는 아래 구조를 사용합니다.

```text
CleanWave/Assets/Scripts/
├── Core/
├── Player/
├── Trash/
├── Inventory/
├── UI/
├── Map/
└── Upgrade/
```

## 네이밍 규칙

- C# 파일명과 클래스명은 동일하게 쓴다.
- 파일명과 클래스명은 PascalCase를 사용한다.
- private 필드는 camelCase를 사용한다.
- Inspector에서 연결해야 하는 값은 `[SerializeField] private`로 둔다.
- 메서드는 PascalCase를 사용하고 동작이 드러나는 이름을 쓴다.

```csharp
[SerializeField] private float moveSpeed = 3f;
[SerializeField] private int bagCapacity = 5;

private void TryPickupTrash()
{
}
```

## 추천 스크립트 책임

| 스크립트 | 책임 |
|---|---|
| `PlayerController` | 플레이어 이동, 방향 상태 계산, 상호작용 입력 전달 |
| `PlayerInteractor` | 주변 상호작용 대상 감지, `F` 입력 전달 |
| `BagInventory` | 가방 슬롯, 용량, 선택 쓰레기 관리 |
| `TrashItem` | 쓰레기 타입, 오염도, 코인 값 보유 |
| `TrashBin` | 수거함 타입, 정답/오답 판정 |
| `ZoneManager` | 구역별 오염도, 정화율, Gate 개방 관리 |
| `UpgradeShop` | 집게, 가방, 신발 업그레이드 처리 |
| `HudController` | 점수, 코인, 정화율, 가방 UI 표시 |
| `ResultScreen` | 최종 점수, 정화율, 별등급 표시 |

## Enum 기준

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

public enum BinType
{
    Paper,
    Can,
    Plastic,
    General,
    Special
}

public enum ZoneType
{
    City,
    River,
    Beach
}
```

## 분리수거 매핑

| 쓰레기 | 수거함 |
|---|---|
| Paper | Paper |
| Can | Can |
| Plastic | Plastic |
| Vinyl | General |
| Food | General |
| Net | Special |
| Oil | Special |

## 입력 규칙

- 이동: WASD 또는 방향키
- 상호작용/수거/투입: `F`
- 가방 안의 쓰레기를 선택하는 방식은 구현 단계에서 확정한다.
- 드래그앤드롭 인벤토리는 구현하지 않는다.

## UI 규칙

- HUD 필수 표시: 점수, 코인, 정화율, 가방
- 결과화면 필수 표시: 점수, 정화율, 별등급
- HUD는 TextMeshPro 텍스트 중심으로 구성한다.
- 가방 슬롯은 선택된 슬롯을 강조한다.
- 빈 슬롯에서 `F`를 누르면 간단한 피드백을 표시한다.
- 코인 부족 시 간단한 피드백을 표시한다.

## 금지 사항

- Scene 안 오브젝트 이름에 의존하는 하드코딩
- 매직넘버 남발
- `FindObjectOfType` 남발
- 발표에 필요 없는 복잡한 인벤토리 UI
- 드래그앤드롭 인벤토리
- 자동 분류 업그레이드
- Day 12 이후 새 기능 추가

## 테스트 체크리스트

- [ ] 이동이 된다.
- [ ] 벽을 통과하지 않는다.
- [ ] 쓰레기를 수거할 수 있다.
- [ ] 가방이 5개에서 막힌다.
- [ ] `F`로 선택 쓰레기를 버린다.
- [ ] 잘못 버리면 점수 `-5`가 적용된다.
- [ ] 올바르게 버리면 코인과 정화율이 증가한다.
- [ ] 구역 100% 시 다음 구역이 열린다.
- [ ] 업그레이드가 적용된다.
- [ ] 결과화면이 표시된다.
