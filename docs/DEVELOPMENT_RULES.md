# 개발 및 협업 규칙

이 문서는 Unity 협업 중 Scene 충돌과 통합 오류를 줄이기 위한 규칙입니다.

## 핵심 원칙

- 기능 수보다 안정성을 우선한다.
- `main`은 항상 Unity에서 열리고 실행 가능한 상태로 유지한다.
- 발표 전 2~3일은 새 기능 추가보다 버그 수정, 통합, 검증을 우선한다.
- Scene 충돌 방지를 최우선으로 한다.

## Unity 협업 규칙

- `MainScene`과 Tilemap 배치는 맵/Scene 담당 1명만 수정한다.
- Scene 배치가 필요한 경우 다른 담당자는 맵/Scene 담당에게 요청한다.
- 다른 팀원은 Scripts, Prefabs, Sprites, Animations, UI 리소스를 작업한다.
- 맵 요소는 PNG, Tile, Prefab 단위로 제작해 맵/Scene 담당에게 전달한다.
- Unity가 생성한 `.meta` 파일은 삭제하지 않고 함께 커밋한다.
- `Library`, `Temp`, `Obj`, `Logs`, `UserSettings` 폴더는 커밋하지 않는다.
- `.unity`, `.prefab`, import settings, Inspector 연결은 가능하면 Unity Editor에서 수정한다.

## Git 규칙

- 역할별 브랜치를 사용한다.
- 커밋은 작은 작업 단위마다 한다.
- 커밋 메시지는 작업 내용을 짧게 설명한다.
- `main` merge 전에는 본인 브랜치에서 Unity Console 에러와 Play Mode를 확인한다.
- 통합 담당은 merge 후 `main`에서 다시 Unity Play Mode를 확인한다.

## 권장 커밋 메시지 예시

```text
플레이어 이동 추가
쓰레기 수거 범위 수정
도심 맵 배치 정리
에셋 파일명 규칙 문서화
수거함 스프라이트 추가
```

## 작업 전 체크리스트

- [ ] 최신 `main` 기준에서 작업을 시작했다.
- [ ] 내가 수정할 파일이 내 역할 범위에 속한다.
- [ ] `MainScene` 수정이 필요한 경우 맵/Scene 담당과 합의했다.
- [ ] 새 Unity 에셋을 추가했다면 `.meta` 파일도 함께 확인했다.

## Merge 전 체크리스트

- [ ] Unity Console에 빨간 에러가 없다.
- [ ] Play Mode에 진입할 수 있다.
- [ ] 변경 파일에 `Library`, `Temp`, `Obj`, `Logs`, `UserSettings`가 없다.
- [ ] 불필요한 `.csproj`, `.sln`, `.slnx` 파일을 올리지 않는다.
- [ ] MVP 제외 기능을 새로 추가하지 않았다.
