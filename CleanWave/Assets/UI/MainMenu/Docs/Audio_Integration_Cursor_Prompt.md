# Cursor 작업 지시: 메인메뉴 BGM / 쓰레기 줍기 효과음 연결

오디오 파일은 이미 프로젝트에 들어와 있다.

## 오디오 파일 위치

```text
Assets/Audio/BGM/main-theme.mp3
Assets/Audio/SFX/pop.mp3
Assets/Audio/SFX/inventory.mp3
```

## 목표

1. MainMenu 씬에서 `main-theme.mp3`가 반복 재생되어야 한다.
2. 플레이어가 `E` 키로 쓰레기를 성공적으로 주웠을 때 `pop.mp3`가 한 번 재생되어야 한다.
3. 플레이어가 `Q` 키로 전체 인벤토리 창을 열 때 `inventory.mp3`가 한 번 재생되어야 한다.

## 이미 추가된 스크립트

```text
Assets/UI/MainMenu/Scripts/MainMenuBGM.cs
Assets/Characters/Player/Scripts/TrashPickup.cs
```

`TrashPickup.cs`에는 아래 필드가 추가되어 있다.

```text
pickupSfx
pickupSfxVolume
```

수거 성공 시 `AudioSource.PlayClipAtPoint()`로 효과음을 재생한다.

## MainMenu BGM 연결

MainMenu 씬에서 다음 작업을 해라.

1. `MainMenuCanvas` 또는 별도 빈 오브젝트 `MainMenuAudio`를 만든다.
2. `AudioSource` 컴포넌트를 붙인다.
3. `MainMenuBGM` 컴포넌트를 붙인다.
4. `MainMenuBGM.mainTheme`에 아래 파일을 연결한다.

```text
Assets/Audio/BGM/main-theme.mp3
```

5. AudioSource 설정:

```text
Play On Awake: false
Loop: true
Volume: 0.6
Spatial Blend: 0 (2D)
```

## 쓰레기 줍기 효과음 연결

씬에 있는 모든 `TrashPickup` 컴포넌트에 아래 파일을 연결한다.

```text
Assets/Audio/SFX/pop.mp3
```

연결할 필드:

```text
TrashPickup.pickupSfx
```

볼륨:

```text
pickupSfxVolume = 0.8
```

## 인벤토리 열기 효과음 연결

`PlayerInventoryCanvas.prefab` 또는 씬의 `PlayerInventoryCanvas` 안에 있는 `InventoryUI` 컴포넌트에 아래 파일을 연결한다.

```text
Assets/Audio/SFX/inventory.mp3
```

연결할 필드:

```text
InventoryUI.inventoryOpenSfx
```

볼륨:

```text
inventoryOpenSfxVolume = 0.8
```

주의:

```text
Q를 눌러 전체 인벤토리 창이 열릴 때만 소리가 나야 한다.
Q를 다시 눌러 닫을 때는 소리가 나지 않아도 된다.
```

## 주의사항

1. `TrashPickup`의 수거 로직은 바꾸지 마라.
2. `PlayerBag.TryAddTrash()`가 성공했을 때만 효과음이 나야 한다.
3. 가방이 가득 차서 수거 실패하면 효과음이 나면 안 된다.
4. Player.prefab, PlayerInventoryCanvas.prefab, 캐릭터 PNG, Animator, InventoryUI는 건드리지 마라.
5. 메인메뉴 버튼 구조는 오디오 연결 외에는 건드리지 마라.

## 검증

Play Mode에서 확인:

```text
[ ] MainMenu 씬 진입 시 main-theme.mp3 반복 재생
[ ] 게임 시작 버튼 클릭 후 MainScene 이동
[ ] 쓰레기 근처에서 E 입력 시 pop.mp3 1회 재생
[ ] 가방이 가득 찼을 때는 pop.mp3가 재생되지 않음
[ ] Q 입력으로 전체 인벤토리 창이 열릴 때 inventory.mp3 1회 재생
[ ] Q 입력으로 전체 인벤토리 창을 닫을 때는 inventory.mp3가 반복 재생되지 않음
[ ] Console 에러 0건
```
