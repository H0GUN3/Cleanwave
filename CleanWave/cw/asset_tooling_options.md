# Draft: Asset Tooling Options

## 목적
- CleanWave의 2주 MVP 제작에 사용할 에셋 제작/정리/자동화 도구 후보를 정리한다.
- 캐릭터, 스프라이트, 타일, UI, OMO/MCP/CLI 보조 도구를 구분한다.

## 핵심 결론
- 최종 픽셀 에셋 제작은 Aseprite 중심이 가장 안전하다.
- ComfyUI/GPT Image 등 AI 도구는 컨셉/초안/레퍼런스 생성용으로 제한한다.
- OMO/MCP는 에셋 직접 제작보다 파일명 검사, 폴더 정리, 문서화, 규격 검사 자동화에 쓰는 것이 안전하다.

## 추천 최소 스택
1. Aseprite 또는 LibreSprite
2. Kenney / itch.io / OpenGameArt 무료 에셋
3. Lospec 팔레트
4. ImageMagick CLI
5. Git/GitHub
6. Filesystem MCP 또는 OMO 파일 검사 작업
7. 필요 시 ComfyUI

## 도구별 판단

| 도구 | 용도 | 장점 | 리스크 | 추천 |
|---|---|---|---|---|
| Aseprite | 최종 픽셀/애니메이션/스프라이트시트 | 픽셀아트와 애니메이션에 최적 | 유료 | 채택 |
| LibreSprite | Aseprite 무료 대체 | 무료, 기본 기능 충분 | 완성도/지원 약함 | 예산 없으면 채택 |
| Piskel | 브라우저 픽셀 편집 | 설치 없음, 빠름 | 고급 작업 약함 | 보조/초안 |
| Tiled | 맵/타일 배치 | 레벨 설계에 좋음 | Unity Tilemap과 이중 관리 위험 | 필요 시 보류 |
| Lospec | 팔레트 | 스타일 통일에 좋음 | 제작 도구 아님 | 채택 |
| Kenney | 무료 에셋 | UI/타일/오브젝트 빠름 | 스타일 불일치 가능 | 채택 |
| OpenGameArt/itch.io | 무료 에셋 검색 | 선택지 많음 | 라이선스/품질 편차 | 선택 |
| ComfyUI | AI 이미지 생성 | 컨셉/기준 이미지 생성 | 세팅/일관성 리스크 | 제한 채택 |
| GPT Image | 빠른 아이디어 이미지 | 접근 쉬움 | 최종 픽셀 규격 약함 | 참고용 |
| ImageMagick | 이미지 일괄 처리 | 리사이즈/투명도/검사 자동화 | CLI 학습 필요 | 채택 |
| Aseprite CLI | export 자동화 | 시트 export 반복 작업에 좋음 | Aseprite 필요 | 선택 채택 |
| Unity batchmode | 빌드/검사 자동화 | import/build 자동화 가능 | 세팅 과하면 위험 | 후순위 |

## OMO/MCP에 붙이기 좋은 도구

### Filesystem MCP / OMO 파일 검사
- 파일명 규칙 검사.
- 폴더 구조 검사.
- 누락 에셋 목록 작성.
- 한글/공백 파일명 탐지.

### Git/GitHub CLI 또는 MCP
- 역할별 브랜치 관리.
- 변경 파일 확인.
- 에셋 출처/라이선스 기록.

### ImageMagick CLI
- 16x16/32x48/32x32 규격 검사.
- 이미지 크기 일괄 확인.
- 투명 배경 확인.
- 썸네일/contact sheet 생성.

### Aseprite CLI
- `.aseprite` 파일에서 PNG sequence/sprite sheet export.
- 반복 export 자동화.

### ComfyUI API/CLI 래핑
- prompt 파일 기반 생성 요청.
- 결과물을 raw 폴더에 저장.
- 단, 2주 프로젝트에서는 직접 MCP 서버 제작은 비추천.

### Playwright MCP
- 웹 에셋 사이트 탐색 보조.
- 라이선스/다운로드 페이지 확인.
- 대량 스크래핑은 금지.

## 피해야 할 것
- 커스텀 MCP 서버를 여러 개 만드는 것.
- AI 파이프라인에 과투자하는 것.
- TexturePacker 등 유료/복잡한 툴을 초반부터 도입하는 것.
- Blender 기반 3D 렌더 파이프라인.
- 라이선스 확인 없이 외부 에셋을 섞는 것.

## CleanWave 추천 조합
- 캐릭터: ComfyUI/GPT Image로 기준 이미지 → Aseprite로 32x48 정리.
- 쓰레기 21개: Aseprite 직접 제작.
- 수거함 5개: Aseprite 직접 제작.
- 맵/타일: Kenney/itch.io/OpenGameArt 무료 에셋 중 하나의 스타일로 제한.
- UI: TextMeshPro + 간단 아이콘.
- 자동화: ImageMagick으로 크기/파일명 검사.

## 결정 필요
- Aseprite를 사용할지, 무료 대체로 갈지.
- Tiled를 사용할지, Unity Tilemap만 사용할지.
- ImageMagick을 설치해 검사 자동화에 쓸지.
- ComfyUI를 누가 관리할지.
