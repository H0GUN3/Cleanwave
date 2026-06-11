#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;
using System.IO;
using CleanWave;

/// <summary>
/// 메뉴: CleanWave → ★ 맵 전체 재구축 ★
/// 배경 이미지(1448×1086, PPU=22 → 66×49 world units) 기준으로 씬 재구성.
/// - 투명 벽 콜라이더 (건물 영역)
/// - 도로에만 쓰레기 배치
/// - BinSelectionUI / BagGaugeUI 포함 HUD
/// </summary>
public class MapRebuildSetup : EditorWindow
{
    const string BG_PATH = "Assets/Sprites/Background/bg_map_01.png";

    // 맵 크기 (PPU=22 기준)
    const float MAP_W = 66f;
    const float MAP_H = 49f;

    [MenuItem("CleanWave/★ 맵 전체 재구축 ★")]
    public static void Run()
    {
        if (!EditorUtility.DisplayDialog("맵 재구축",
            "씬 전체를 이미지 기반으로 재구성합니다.\n계속하시겠습니까?", "예", "취소")) return;

        EditorUtility.DisplayProgressBar("CleanWave", "초기화...", 0.05f);
        ClearScene();

        EditorUtility.DisplayProgressBar("CleanWave", "배경 이미지 설정...", 0.10f);
        SetupBackground();

        EditorUtility.DisplayProgressBar("CleanWave", "카메라/매니저...", 0.15f);
        SetupManagers();

        EditorUtility.DisplayProgressBar("CleanWave", "ZoneManager...", 0.35f);
        CreateZones();

        EditorUtility.DisplayProgressBar("CleanWave", "수거함 배치...", 0.45f);
        PlaceBins();

        EditorUtility.DisplayProgressBar("CleanWave", "쓰레기 배치...", 0.55f);
        PlaceTrash();

        EditorUtility.DisplayProgressBar("CleanWave", "상점 배치...", 0.62f);
        PlaceShops();

        EditorUtility.DisplayProgressBar("CleanWave", "플레이어 생성...", 0.70f);
        var player = CreatePlayer();

        EditorUtility.DisplayProgressBar("CleanWave", "애니메이터...", 0.78f);
        AttachAnimator(player);

        EditorUtility.DisplayProgressBar("CleanWave", "HUD 생성...", 0.88f);
        CreateHUD(player);

        EditorUtility.DisplayProgressBar("CleanWave", "씬 저장...", 0.97f);
        SaveScene();

        EditorUtility.ClearProgressBar();
        EditorUtility.DisplayDialog("완료", "맵 재구축 완료!\nPlay 버튼으로 확인하세요.", "확인");
    }

    [MenuItem("CleanWave/Remove All Walls")]
    public static void RemoveWalls()
    {
        int removed = 0;
        // "Walls" 루트 오브젝트 제거
        var wallsRoot = GameObject.Find("Walls");
        if (wallsRoot != null) { Object.DestroyImmediate(wallsRoot); removed++; }

        // 이름에 "Border" 또는 "Wall" 포함된 루트 오브젝트도 정리
        foreach (var go in UnityEngine.SceneManagement.SceneManager
            .GetActiveScene().GetRootGameObjects())
        {
            if (go.name.Contains("Border") || go.name.Contains("Wall"))
            { Object.DestroyImmediate(go); removed++; }
        }

        EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        EditorUtility.DisplayDialog("Remove Walls",
            $"완료! {removed}개 오브젝트 삭제됨.", "OK");
    }

    // ──────────────────────────────────────────────────
    // 씬 초기화
    // ──────────────────────────────────────────────────
    static void ClearScene()
    {
        foreach (var go in UnityEngine.SceneManagement.SceneManager
            .GetActiveScene().GetRootGameObjects())
        {
            if (go.name != "Main Camera") Object.DestroyImmediate(go);
        }
        // 기존 Tile 에셋 삭제
        if (Directory.Exists("Assets/Tiles"))
        {
            FileUtil.DeleteFileOrDirectory("Assets/Tiles");
            AssetDatabase.Refresh();
        }
    }

    // ──────────────────────────────────────────────────
    // 배경 이미지 임포트 & 배치
    // ──────────────────────────────────────────────────
    static void SetupBackground()
    {
        var imp = AssetImporter.GetAtPath(BG_PATH) as TextureImporter;
        if (imp == null) { Debug.LogError("배경 이미지 없음: " + BG_PATH); return; }
        imp.textureType         = TextureImporterType.Sprite;
        imp.spriteImportMode    = SpriteImportMode.Single;
        imp.filterMode          = FilterMode.Point;
        imp.textureCompression  = TextureImporterCompression.Uncompressed;
        imp.spritePixelsPerUnit = 22;          // 1448/22 ≈ 66, 1086/22 ≈ 49
        imp.mipmapEnabled       = false;
        imp.maxTextureSize      = 2048;
        imp.spritePivot         = new Vector2(0.5f, 0.5f);
        imp.SaveAndReimport();
        AssetDatabase.Refresh();

        var spr = AssetDatabase.LoadAssetAtPath<Sprite>(BG_PATH);
        var go  = new GameObject("Background_Map");
        var sr  = go.AddComponent<SpriteRenderer>();
        sr.sprite       = spr;
        sr.sortingOrder = -10;
        // 이미지 자연 크기가 ≈66×49 이므로 중앙에 배치
        go.transform.position = new Vector3(MAP_W / 2f, MAP_H / 2f, 1f);
    }

    // ──────────────────────────────────────────────────
    // 카메라 / 매니저
    // ──────────────────────────────────────────────────
    static void SetupManagers()
    {
        var camGo = GameObject.Find("Main Camera") ?? new GameObject("Main Camera");
        camGo.tag = "MainCamera";
        var cam   = camGo.GetComponent<Camera>() ?? camGo.AddComponent<Camera>();
        cam.orthographic      = true;
        cam.orthographicSize  = 7f;
        cam.backgroundColor   = new Color(0.05f, 0.05f, 0.1f);
        cam.transform.position = new Vector3(MAP_W / 2f, MAP_H / 2f, -10f);
        // 재실행 시 기존 CameraFollow 제거 후 재추가
        var oldCf = camGo.GetComponent<CameraFollow>();
        if (oldCf != null) Object.DestroyImmediate(oldCf);
        var cf = camGo.AddComponent<CameraFollow>();
        // 맵 경계 클램핑 설정
        var soCf = new SerializedObject(cf);
        soCf.FindProperty("smoothSpeed").floatValue  = 6f;
        soCf.FindProperty("useBounds").boolValue     = true;
        soCf.FindProperty("minBounds").vector2Value  = new Vector2(12.5f, 7f);
        soCf.FindProperty("maxBounds").vector2Value  = new Vector2(53.5f, 42f);
        soCf.ApplyModifiedProperties();
        if (!camGo.GetComponent<AudioListener>()) camGo.AddComponent<AudioListener>();

        var mgr = new GameObject("GameManager");
        mgr.AddComponent<GameManager>();
        mgr.AddComponent<ScoreManager>();

        // UI 클릭에 필수: EventSystem + StandaloneInputModule
        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var esGo = new GameObject("EventSystem");
            esGo.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGo.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
    }

    // ──────────────────────────────────────────────────
    // 투명 벽 콜라이더 (건물 영역 + 강물 + 바다)
    // 이미지 분석 기준 world units (PPU=22, 1448×1086 이미지)
    // worldX = px * 66/1448, worldY = (1086-py) * 49/1086
    // ──────────────────────────────────────────────────
    static void PlaceWalls()
    {
        var root = new GameObject("Walls");

        // ── 도심 건물 블록 ──
        // 4행 × 3열 구조, 도로폭 ≈ 2 world unit
        // 수직 도로: x≈7.5~9.5, x≈15~17
        // 수평 도로: y≈22~24, y≈29~31, y≈36~38

        // 상단 행 (y=40~49)
        Wall(root, 0.5f, 40f, 7f,   9f,  "City_T1");   // 좌 건물
        Wall(root, 9.5f, 40f, 5f,   9f,  "City_T2");   // 중 건물
        Wall(root, 17f,  40f, 5.5f, 9f,  "City_T3");   // 우 건물
        Wall(root, 23f,  40f, 7f,   9f,  "City_T4");   // 최우 건물

        // 2번째 행 (y=32~35)
        Wall(root, 0.5f, 32f, 7f,   3.5f, "City_M1");
        Wall(root, 9.5f, 32f, 5f,   3.5f, "City_M2");
        Wall(root, 17f,  32f, 5.5f, 3.5f, "City_M3");
        Wall(root, 23f,  32f, 7f,   3.5f, "City_M4");

        // 3번째 행 (y=25~28)
        Wall(root, 0.5f, 25f, 7f,   3.5f, "City_L1");
        Wall(root, 9.5f, 25f, 5f,   3.5f, "City_L2");
        Wall(root, 17f,  25f, 5.5f, 3.5f, "City_L3");
        Wall(root, 23f,  25f, 7f,   3.5f, "City_L4");

        // 하단 행: 공원/광장으로 일부 개방 (y=13~21)
        Wall(root, 0.5f,  13f, 5.5f, 8.5f, "City_B1");  // 좌 건물
        Wall(root, 20.5f, 13f, 9f,   8.5f, "City_B2");  // 우 건물
        // 중앙 x≈7~20은 공원이라 개방

        // 최하단 (y=0~12): 건물 일부
        Wall(root, 0.5f,  0f, 5.5f, 12.5f, "City_Bot1");
        Wall(root, 20.5f, 0f, 9f,   12.5f, "City_Bot2");

        // ── 하천 강물 (x=33~46) ──
        // 다리 2개: y≈13~16 (남쪽), y≈31~34 (북쪽)
        // 강물 구간 분할 (다리 구간 제외)
        Wall(root, 33f, 0f,  13f, 12.5f, "River_Water_S");   // 하단 강물
        Wall(root, 33f, 17f, 13f, 13.5f, "River_Water_M");   // 중간 강물
        Wall(root, 33f, 35f, 13f, 14f,   "River_Water_N");   // 상단 강물

        // ── 해안 바다 (x=61~66) ──
        Wall(root, 61f, 0f, 5f, 49f, "Ocean");

        // ── 전체 맵 외벽 ──
        Wall(root, 0f,    -0.5f, MAP_W, 0.5f, "Border_Bottom");
        Wall(root, 0f,    MAP_H, MAP_W, 0.5f, "Border_Top");
        Wall(root, -0.5f, 0f,    0.5f, MAP_H, "Border_Left");
        Wall(root, MAP_W, 0f,    0.5f, MAP_H, "Border_Right");

        Debug.Log("[Setup] 투명 벽 배치 완료");
    }

    static void Wall(GameObject parent, float x, float y, float w, float h, string name)
    {
        var go  = new GameObject(name);
        go.transform.SetParent(parent.transform);
        go.transform.position = new Vector3(x + w / 2f, y + h / 2f, 0f);
        var col  = go.AddComponent<BoxCollider2D>();
        col.size = new Vector2(w, h);
        // 완전 투명 — 비주얼 없음
    }

    // ──────────────────────────────────────────────────
    // ZoneManager & ZoneGate
    // ──────────────────────────────────────────────────
    static ZoneManager cityZone, riverZone, beachZone;

    static void CreateZones()
    {
        cityZone  = MakeZone("ZoneManager_City",  ZoneType.City);
        riverZone = MakeZone("ZoneManager_River", ZoneType.River);
        beachZone = MakeZone("ZoneManager_Beach", ZoneType.Beach);

        // Gate 1: 도심→하천 (x=30 통로 남쪽 다리)
        var g1 = MakeGate("Gate_CityToRiver",  new Vector3(31.5f, 14.5f, 0), new Vector2(1.5f, 3f));
        // Gate 2: 하천→해안
        var g2 = MakeGate("Gate_RiverToBeach", new Vector3(49f,   14.5f, 0), new Vector2(1.5f, 3f));

        Link(cityZone,  "gateToNextZone", g1);
        Link(riverZone, "gateToNextZone", g2);
    }

    static ZoneManager MakeZone(string name, ZoneType zt)
    {
        var go = new GameObject(name);
        var zm = go.AddComponent<ZoneManager>();
        var so = new SerializedObject(zm);
        so.FindProperty("zoneType").enumValueIndex = (int)zt;
        so.ApplyModifiedProperties();
        return zm;
    }

    static ZoneGate MakeGate(string name, Vector3 pos, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.position = pos;

        // 반투명 붉은 블록 (개방 전 시각적 표시)
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = CreateSolidSprite(new Color(0.9f, 0.2f, 0.1f, 0.75f));
        sr.drawMode = SpriteDrawMode.Simple;
        go.transform.localScale = new Vector3(size.x, size.y, 1f);
        sr.sortingOrder = 3;

        var col = go.AddComponent<BoxCollider2D>();
        col.size = Vector2.one;   // localScale이 size이므로 실제 크기 = size

        var gate = go.AddComponent<ZoneGate>();
        var so   = new SerializedObject(gate);
        so.FindProperty("gateVisual").objectReferenceValue   = go;
        so.FindProperty("gateCollider").objectReferenceValue = col;
        so.ApplyModifiedProperties();
        return gate;
    }

    // ──────────────────────────────────────────────────
    // 수거함 배치
    // 위치: 공원 입구 근처 (도로에서 접근 가능)
    // ──────────────────────────────────────────────────
    static readonly BinType[]   BIN_TYPES   = { BinType.Paper, BinType.Can, BinType.Plastic, BinType.General, BinType.Special };
    static readonly string[]    BIN_SPRITES = { "bin_paper_01","bin_can_01","bin_plastic_01","bin_general_01","bin_special_01" };
    static readonly Color[]     BIN_COLORS  = {
        new Color(0.95f, 0.95f, 0.1f), new Color(0.2f, 0.5f, 1f),
        new Color(0.1f, 0.9f, 0.9f),   new Color(0.55f, 0.55f, 0.55f),
        new Color(0.9f, 0.2f, 0.2f)
    };

    static void PlaceBins()
    {
        // 도심 수거함: 공원 앞 y≈22 도로
        PlaceBinSet(ZoneType.City,  new Vector3(7f,  22.3f, 0));
        // 하천 수거함: 좌측 강변 y≈22
        PlaceBinSet(ZoneType.River, new Vector3(31f, 22.3f, 0));
        // 해안 수거함: 해안 입구 y≈22
        PlaceBinSet(ZoneType.Beach, new Vector3(51f, 22.3f, 0));
    }

    static void PlaceBinSet(ZoneType zone, Vector3 origin)
    {
        var parent = new GameObject($"Bins_{zone}");
        for (int i = 0; i < 5; i++)
        {
            var go = new GameObject($"Bin_{BIN_TYPES[i]}");
            go.transform.SetParent(parent.transform);
            go.transform.position = origin + new Vector3(i * 1.3f, 0, 0);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Sprites/Bin/{BIN_SPRITES[i]}.png");
            sr.color  = BIN_COLORS[i];
            sr.sortingOrder = 2;

            var col  = go.AddComponent<BoxCollider2D>();
            col.size    = new Vector2(0.9f, 1.3f);
            col.offset  = new Vector2(0f, 0.65f);
            col.isTrigger = true;

            // 수거함 이름 라벨
            CreateWorldLabel(go, BinName(BIN_TYPES[i]), new Vector3(0, 1.6f, 0));

            var bin  = go.AddComponent<TrashBin>();
            var so   = new SerializedObject(bin);
            so.FindProperty("binType").enumValueIndex = (int)BIN_TYPES[i];
            so.FindProperty("zoneType").enumValueIndex = (int)zone;
            so.ApplyModifiedProperties();
        }
    }

    // ──────────────────────────────────────────────────
    // 쓰레기 배치 — 도로 위치만 수동 지정
    // ──────────────────────────────────────────────────
    static readonly (float x, float y, TrashType type, ZoneType zone)[] TRASH_POINTS =
    {
        // ── 도심 도로 (y≈22, y≈29, y≈36, y≈43, 수직x≈7.5, x≈16) ──
        // 수평 도로 y=22 (main road)
        (1.5f, 22f, TrashType.Paper,   ZoneType.City),
        (3.5f, 22f, TrashType.Can,     ZoneType.City),
        (5.5f, 22f, TrashType.Plastic, ZoneType.City),
        (10.5f,22f, TrashType.Vinyl,   ZoneType.City),
        (12.5f,22f, TrashType.Food,    ZoneType.City),
        (18.5f,22f, TrashType.Paper,   ZoneType.City),
        (20.5f,22f, TrashType.Can,     ZoneType.City),
        (24.5f,22f, TrashType.Plastic, ZoneType.City),
        // 수평 도로 y=29
        (1.5f, 29f, TrashType.Can,     ZoneType.City),
        (5.5f, 29f, TrashType.Paper,   ZoneType.City),
        (10.5f,29f, TrashType.Plastic, ZoneType.City),
        (18.5f,29f, TrashType.Vinyl,   ZoneType.City),
        (24.5f,29f, TrashType.Food,    ZoneType.City),
        // 수평 도로 y=36
        (3.5f, 36f, TrashType.Paper,   ZoneType.City),
        (12f,  36f, TrashType.Can,     ZoneType.City),
        (20f,  36f, TrashType.Plastic, ZoneType.City),
        (26f,  36f, TrashType.Food,    ZoneType.City),
        // 수직 도로 x=7.5
        (7.5f, 25f, TrashType.Vinyl,   ZoneType.City),
        (7.5f, 32f, TrashType.Paper,   ZoneType.City),
        (7.5f, 39f, TrashType.Can,     ZoneType.City),
        // 공원 & 수직 x=16
        (16f,  25f, TrashType.Plastic, ZoneType.City),
        (9f,   18f, TrashType.Food,    ZoneType.City),
        (12f,  16f, TrashType.Paper,   ZoneType.City),
        (15f,  18f, TrashType.Can,     ZoneType.City),
        (11f,  10f, TrashType.Vinyl,   ZoneType.City),
        (13f,  8f,  TrashType.Food,    ZoneType.City),

        // ── 하천 강변 + 다리 ──
        (30.5f, 5f,  TrashType.Vinyl,   ZoneType.River),
        (30.5f, 9f,  TrashType.Plastic, ZoneType.River),
        (30.5f, 18f, TrashType.Net,     ZoneType.River),
        (30.5f, 25f, TrashType.Vinyl,   ZoneType.River),
        (30.5f, 32f, TrashType.Food,    ZoneType.River),
        (30.5f, 39f, TrashType.Can,     ZoneType.River),
        (30.5f, 44f, TrashType.Plastic, ZoneType.River),
        // 다리 (남쪽 y=13~16)
        (35f, 14f, TrashType.Net,     ZoneType.River),
        (38f, 14f, TrashType.Oil,     ZoneType.River),
        (41f, 14f, TrashType.Vinyl,   ZoneType.River),
        (44f, 14f, TrashType.Plastic, ZoneType.River),
        // 다리 (북쪽 y=31~34)
        (35f, 32f, TrashType.Net,     ZoneType.River),
        (38f, 32f, TrashType.Oil,     ZoneType.River),
        (41f, 32f, TrashType.Vinyl,   ZoneType.River),
        // 우측 강변
        (47f, 5f,  TrashType.Can,     ZoneType.River),
        (47f, 10f, TrashType.Plastic, ZoneType.River),
        (47f, 18f, TrashType.Net,     ZoneType.River),
        (47f, 25f, TrashType.Oil,     ZoneType.River),
        (47f, 36f, TrashType.Vinyl,   ZoneType.River),
        (47f, 43f, TrashType.Food,    ZoneType.River),

        // ── 해안 모래사장 ──
        (51f,  5f,  TrashType.Net,     ZoneType.Beach),
        (53f,  5f,  TrashType.Oil,     ZoneType.Beach),
        (55f,  5f,  TrashType.Net,     ZoneType.Beach),
        (58f,  5f,  TrashType.Plastic, ZoneType.Beach),
        (51f,  9f,  TrashType.Oil,     ZoneType.Beach),
        (54f,  9f,  TrashType.Net,     ZoneType.Beach),
        (57f,  9f,  TrashType.Vinyl,   ZoneType.Beach),
        (52f, 14f,  TrashType.Net,     ZoneType.Beach),
        (55f, 14f,  TrashType.Oil,     ZoneType.Beach),
        (58f, 14f,  TrashType.Plastic, ZoneType.Beach),
        (51f, 19f,  TrashType.Net,     ZoneType.Beach),
        (54f, 19f,  TrashType.Oil,     ZoneType.Beach),
        (57f, 19f,  TrashType.Can,     ZoneType.Beach),
        (52f, 24f,  TrashType.Net,     ZoneType.Beach),
        (55f, 24f,  TrashType.Plastic, ZoneType.Beach),
        (58f, 24f,  TrashType.Oil,     ZoneType.Beach),
        (51f, 30f,  TrashType.Net,     ZoneType.Beach),
        (54f, 30f,  TrashType.Oil,     ZoneType.Beach),
        (57f, 30f,  TrashType.Vinyl,   ZoneType.Beach),
        (52f, 36f,  TrashType.Net,     ZoneType.Beach),
        (55f, 36f,  TrashType.Oil,     ZoneType.Beach),
        (58f, 36f,  TrashType.Plastic, ZoneType.Beach),
        (51f, 42f,  TrashType.Net,     ZoneType.Beach),
        (54f, 42f,  TrashType.Oil,     ZoneType.Beach),
        (57f, 42f,  TrashType.Can,     ZoneType.Beach),
        (52f, 46f,  TrashType.Net,     ZoneType.Beach),
        (55f, 46f,  TrashType.Plastic, ZoneType.Beach),
        (58f, 46f,  TrashType.Oil,     ZoneType.Beach),
        (60f, 20f,  TrashType.Net,     ZoneType.Beach),
        (60f, 32f,  TrashType.Oil,     ZoneType.Beach),
    };

    // 파일명: Assets/Sprites/Trash/Trash/Trash_XXX.png (사용자 업로드 기준)
    // 순서: TrashType enum 값(0=Paper,1=Can,2=Plastic,3=Vinyl,4=Food,5=Net,6=Oil)
    static readonly string[] TRASH_SPRITES = {
        "Trash_Paper", "Trash_Can",  "Trash_Plastic",
        "Trash_Vinyl",  "Trash_Food", "Trash_Net",    "Trash_Oil"
    };

    static void PlaceTrash()
    {
        var parents = new System.Collections.Generic.Dictionary<ZoneType, GameObject>();
        foreach (ZoneType zt in System.Enum.GetValues(typeof(ZoneType)))
            parents[zt] = new GameObject($"Trash_{zt}");

        foreach (var (x, y, type, zone) in TRASH_POINTS)
        {
            var go = new GameObject($"Trash_{type}_{x}_{y}");
            go.transform.SetParent(parents[zone].transform);
            go.transform.position = new Vector3(x, y, 0);

            var sr = go.AddComponent<SpriteRenderer>();
            var spr = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Sprites/Trash/Trash/{TRASH_SPRITES[(int)type]}.png");
            sr.sprite = spr;
            sr.sortingOrder = 3;

            var col = go.AddComponent<CircleCollider2D>();
            col.radius    = 0.3f;
            col.isTrigger = true;

            var item = go.AddComponent<TrashItem>();
            var so   = new SerializedObject(item);
            so.FindProperty("trashType").enumValueIndex = (int)type;
            so.FindProperty("zoneType").enumValueIndex  = (int)zone;
            so.ApplyModifiedProperties();
        }
        Debug.Log($"[Setup] 쓰레기 {TRASH_POINTS.Length}개 배치");
    }

    // ──────────────────────────────────────────────────
    // 업그레이드 상점
    // ──────────────────────────────────────────────────
    static readonly UpgradeType[] SHOP_TYPES = { UpgradeType.Tongs, UpgradeType.Bag, UpgradeType.Shoes };
    static readonly string[]      SHOP_NAMES = { "Tongs", "Bag", "Shoes" };

    static void PlaceShops()
    {
        PlaceShopSet(ZoneType.City,  new Vector3(7f,  12f, 0));
        PlaceShopSet(ZoneType.River, new Vector3(31f, 6f,  0));
        PlaceShopSet(ZoneType.Beach, new Vector3(51f, 6f,  0));
    }

    static void PlaceShopSet(ZoneType zone, Vector3 origin)
    {
        var parent = new GameObject($"Shops_{zone}");
        for (int i = 0; i < 3; i++)
        {
            var go = new GameObject($"Shop_{SHOP_TYPES[i]}");
            go.transform.SetParent(parent.transform);
            go.transform.position = origin + new Vector3(i * 1.8f, 0, 0);

            var sr = go.AddComponent<SpriteRenderer>();
            // 더미 상점 스프라이트 (UI 가방 아이콘 재활용 또는 null 허용)
            var shopSpr = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/ui_bag_01.png");
            sr.sprite = shopSpr;
            float h = (float)i / 2f;
            sr.color = Color.HSVToRGB(h, 0.7f, 0.9f);
            sr.sortingOrder = 2;

            var col = go.AddComponent<BoxCollider2D>();
            col.size = new Vector2(1.2f, 1.5f); col.offset = new Vector2(0, 0.75f); col.isTrigger = true;

            CreateWorldLabel(go, SHOP_NAMES[i], new Vector3(0, 1.8f, 0));

            var shop = go.AddComponent<UpgradeShop>();
            var so   = new SerializedObject(shop);
            so.FindProperty("upgradeType").enumValueIndex = (int)SHOP_TYPES[i];
            so.ApplyModifiedProperties();
        }
    }

    // ──────────────────────────────────────────────────
    // 플레이어
    // ──────────────────────────────────────────────────
    static GameObject CreatePlayer()
    {
        var go = new GameObject("Player");
        go.tag = "Player";
        go.transform.position = new Vector3(14f, 22.5f, 0); // 도심 메인 도로

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Player/chr_player_idle_down_01.png");
        sr.sortingOrder = 5;

        var rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f; rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        var col = go.AddComponent<CapsuleCollider2D>();
        col.size = new Vector2(0.5f, 0.8f); col.offset = new Vector2(0f, 0.4f);

        go.AddComponent<PlayerController>();
        go.AddComponent<BagInventory>();

        var cf = Object.FindFirstObjectByType<CameraFollow>();
        cf?.SetTarget(go.transform);
        return go;
    }

    // ──────────────────────────────────────────────────
    // Animator
    // ──────────────────────────────────────────────────
    static void AttachAnimator(GameObject player)
    {
        string dir = "Assets/Animations/Controllers";
        Directory.CreateDirectory(dir);
        string path = $"{dir}/PlayerAnimator.controller";

        var ctrl = AnimatorController.CreateAnimatorControllerAtPath(path);
        ctrl.AddParameter("IsMoving",  AnimatorControllerParameterType.Bool);
        ctrl.AddParameter("Direction", AnimatorControllerParameterType.Int);
        ctrl.AddParameter("FaceX",     AnimatorControllerParameterType.Float);
        ctrl.AddParameter("Pickup",    AnimatorControllerParameterType.Trigger);

        var sm = ctrl.layers[0].stateMachine;
        var idle = AddState(sm, "Idle",      MakeClip("idle",       8f, "Assets/Sprites/Player/chr_player_idle_down_01.png"));
        var walkD = AddState(sm, "Walk_Down", MakeWalkClip("walk_down",   8f, "walk_down",   4));
        var walkU = AddState(sm, "Walk_Up",   MakeWalkClip("walk_up",     8f, "walk_up",     4));
        var walkS = AddState(sm, "Walk_Side", MakeWalkClip("walk_side",   8f, "walk_side",   4));
        var pickup= AddState(sm, "Pickup",    MakeWalkClip("pickup_down", 6f, "pickup_down", 3));

        sm.defaultState = idle;

        Trans(idle,  walkD, "IsMoving", true, "Direction", 0);
        Trans(idle,  walkU, "IsMoving", true, "Direction", 1);
        Trans(idle,  walkS, "IsMoving", true, "Direction", 2);
        TransF(walkD, idle);  TransF(walkU, idle);  TransF(walkS, idle);
        TransDir(walkD, walkU, 1); TransDir(walkD, walkS, 2);
        TransDir(walkU, walkD, 0); TransDir(walkU, walkS, 2);
        TransDir(walkS, walkD, 0); TransDir(walkS, walkU, 1);
        TransTrigger(idle, pickup, "Pickup"); TransTrigger(walkD, pickup, "Pickup");
        var back = pickup.AddTransition(idle); back.hasExitTime = true; back.exitTime = 1f;

        AssetDatabase.SaveAssets();

        var anim = player.AddComponent<Animator>();
        anim.runtimeAnimatorController = ctrl;

        var interactor = player.AddComponent<PlayerInteractor>();
        var bag = player.GetComponent<BagInventory>();
        var soI = new SerializedObject(interactor);
        soI.FindProperty("bagInventory").objectReferenceValue = bag;
        soI.FindProperty("interactLayer").intValue = LayerMask.GetMask("Default");
        soI.ApplyModifiedProperties();
    }

    // ──────────────────────────────────────────────────
    // HUD  (가방 게이지바 + 코인 패널 + 분리수거 팝업 포함)
    // ──────────────────────────────────────────────────
    static void CreateHUD(GameObject player)
    {
        var cvGo = new GameObject("HUD_Canvas");
        var cv   = cvGo.AddComponent<Canvas>();
        cv.renderMode  = RenderMode.ScreenSpaceOverlay;
        cv.sortingOrder = 10;
        var scaler = cvGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        cvGo.AddComponent<GraphicRaycaster>();

        // ── 점수 텍스트 (좌상단) ──
        var scoreTxt = MakeTMP(cvGo, "ScoreText", "Score: 0",
            new Vector2(110, -30), new Vector2(200, 45), new Vector2(0,1), new Vector2(0,1), 26);

        // ── 코인 패널 (좌상단, 점수 아래) ──
        var coinPanel = MakePanel(cvGo, "CoinPanel",
            new Vector2(110, -85), new Vector2(200, 45), new Vector2(0,1), new Vector2(0,1));
        coinPanel.GetComponent<Image>().color = new Color(0.1f, 0.08f, 0f, 0.85f);
        coinPanel.GetComponent<Image>().raycastTarget = false;
        // 코인 아이콘 (노란 원)
        var coinIconGo = new GameObject("CoinIcon");
        coinIconGo.transform.SetParent(coinPanel.transform, false);
        var coinIconRect = coinIconGo.AddComponent<RectTransform>();
        coinIconRect.anchorMin = new Vector2(0,0.5f); coinIconRect.anchorMax = new Vector2(0,0.5f);
        coinIconRect.pivot = new Vector2(0,0.5f); coinIconRect.anchoredPosition = new Vector2(8,0);
        coinIconRect.sizeDelta = new Vector2(28, 28);
        var coinIconImg = coinIconGo.AddComponent<Image>();
        coinIconImg.sprite = CreateCircleSprite(new Color(1f, 0.82f, 0f));
        coinIconImg.raycastTarget = false;
        var coinTxt = MakeTMP(coinPanel, "CoinText", "0",
            new Vector2(22, 0), new Vector2(155, 45), new Vector2(0,0.5f), new Vector2(0,0.5f), 26);
        coinTxt.alignment = TextAlignmentOptions.Left;

        // ── 정화율 텍스트 ──
        var purTxt = MakeTMP(cvGo, "PurificationText", "Clean: 0%",
            new Vector2(110, -140), new Vector2(220, 45), new Vector2(0,1), new Vector2(0,1), 24);

        // ── 타이머 (상단 중앙) ──
        var timerTxt = MakeTMP(cvGo, "TimerText", "10:00",
            new Vector2(0, -30), new Vector2(200, 60), new Vector2(0.5f,1f), new Vector2(0.5f,1f), 42);
        timerTxt.fontStyle = FontStyles.Bold;

        // ── 구역별 정화율 (우상단) ──
        var cityPurTxt = MakeTMP(cvGo, "CityPurText", "City: 0%",
            new Vector2(-35, -30), new Vector2(160, 36), new Vector2(1f,1f), new Vector2(1f,1f), 20);
        var riverPurTxt = MakeTMP(cvGo, "RiverPurText", "River: 0%",
            new Vector2(-35, -68), new Vector2(160, 36), new Vector2(1f,1f), new Vector2(1f,1f), 20);
        var beachPurTxt = MakeTMP(cvGo, "BeachPurText", "Beach: 0%",
            new Vector2(-35, -106), new Vector2(160, 36), new Vector2(1f,1f), new Vector2(1f,1f), 20);

        // ── 피드백 텍스트 (상단 중앙) ──
        var feedbackTmp = MakeTMP(cvGo, "FeedbackText", "",
            new Vector2(0, -50), new Vector2(680, 65), new Vector2(0.5f,1), new Vector2(0.5f,1), 32);
        feedbackTmp.color = Color.yellow;
        feedbackTmp.gameObject.SetActive(false);

        // ── 가방 게이지 패널 (하단 중앙) ──
        var bagPanel = MakePanel(cvGo, "BagPanel",
            new Vector2(0, 10), new Vector2(500, 80), new Vector2(0.5f,0), new Vector2(0.5f,0));
        bagPanel.GetComponent<Image>().color = new Color(0.08f, 0.08f, 0.1f, 0.88f);

        // 가방 라벨
        var bagLabel = MakeTMP(bagPanel, "BagLabel", "Bag",
            new Vector2(-185, 12), new Vector2(60, 30), new Vector2(0.5f,0.5f), new Vector2(0.5f,0.5f), 20);
        bagLabel.alignment = TextAlignmentOptions.Center;

        // 게이지 배경
        var gaugeBack = new GameObject("GaugeBg");
        gaugeBack.transform.SetParent(bagPanel.transform, false);
        var gbRect = gaugeBack.AddComponent<RectTransform>();
        gbRect.anchorMin = new Vector2(0.5f,0.5f); gbRect.anchorMax = new Vector2(0.5f,0.5f);
        gbRect.pivot = new Vector2(0.5f,0.5f); gbRect.anchoredPosition = new Vector2(10, 12);
        gbRect.sizeDelta = new Vector2(280, 22);
        var gbImg = gaugeBack.AddComponent<Image>();
        gbImg.color = new Color(0.1f, 0.1f, 0.1f, 1f);

        // 게이지 Fill
        var gaugeFill = new GameObject("GaugeFill");
        gaugeFill.transform.SetParent(gaugeBack.transform, false);
        var gfRect = gaugeFill.AddComponent<RectTransform>();
        gfRect.anchorMin = Vector2.zero; gfRect.anchorMax = new Vector2(1,1);
        gfRect.offsetMin = gfRect.offsetMax = Vector2.zero;
        var gfImg = gaugeFill.AddComponent<Image>();
        gfImg.type       = Image.Type.Filled;
        gfImg.fillMethod = Image.FillMethod.Horizontal;
        gfImg.fillAmount = 0f;
        gfImg.color      = new Color(0.3f, 0.85f, 0.3f);

        // 가방 카운트
        var bagCount = MakeTMP(bagPanel, "BagCount", "0 / 5",
            new Vector2(175, 12), new Vector2(100, 30), new Vector2(0.5f,0.5f), new Vector2(0.5f,0.5f), 20);
        bagCount.alignment = TextAlignmentOptions.Left;

        // ── 가방 슬롯 (하단) ──
        var slotRow = new GameObject("BagSlots");
        slotRow.transform.SetParent(cvGo.transform, false);
        var slotRowRect = slotRow.AddComponent<RectTransform>();
        slotRowRect.anchorMin = new Vector2(0.5f, 0); slotRowRect.anchorMax = new Vector2(0.5f, 0);
        slotRowRect.pivot = new Vector2(0.5f, 0); slotRowRect.anchoredPosition = new Vector2(0, 95);
        slotRowRect.sizeDelta = new Vector2(720, 58);
        var hLayout = slotRow.AddComponent<HorizontalLayoutGroup>();
        hLayout.spacing = 5; hLayout.childAlignment = TextAnchor.MiddleCenter;
        hLayout.childForceExpandWidth = false; hLayout.childForceExpandHeight = false;

        var bagSlotUIList = new BagSlotUI[12];
        for (int i = 0; i < 12; i++)
        {
            var slot = new GameObject($"Slot_{i}");
            slot.transform.SetParent(slotRow.transform, false);
            var slotRect = slot.AddComponent<RectTransform>(); slotRect.sizeDelta = new Vector2(52, 52);
            var slotBg = slot.AddComponent<Image>(); slotBg.color = new Color(0.12f,0.12f,0.14f,0.9f);

            var icon = new GameObject("Icon");
            icon.transform.SetParent(slot.transform, false);
            var iconRect = icon.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.1f,0.1f); iconRect.anchorMax = new Vector2(0.9f,0.9f);
            iconRect.offsetMin = iconRect.offsetMax = Vector2.zero;
            var iconImg = icon.AddComponent<Image>(); iconImg.preserveAspect = true; icon.SetActive(false);

            var hl = new GameObject("Highlight");
            hl.transform.SetParent(slot.transform, false);
            var hlRect = hl.AddComponent<RectTransform>();
            hlRect.anchorMin = Vector2.zero; hlRect.anchorMax = Vector2.one;
            hlRect.offsetMin = hlRect.offsetMax = Vector2.zero;
            var hlImg = hl.AddComponent<Image>(); hlImg.color = new Color(1f,0.9f,0.1f,0.45f); hlImg.enabled = false;

            var btn = slot.AddComponent<Button>();
            var slotUI = slot.AddComponent<BagSlotUI>();
            bagSlotUIList[i] = slotUI;
            var soS = new SerializedObject(slotUI);
            soS.FindProperty("iconImage").objectReferenceValue = iconImg;
            soS.FindProperty("selectionHighlight").objectReferenceValue = hlImg;
            soS.FindProperty("slotButton").objectReferenceValue = btn;
            soS.ApplyModifiedProperties();
            if (i >= 5) slot.SetActive(false);
        }

        // ── FeedbackUI 컴포넌트 ──
        var fui = cvGo.AddComponent<FeedbackUI>();
        var soF = new SerializedObject(fui);
        soF.FindProperty("feedbackText").objectReferenceValue = feedbackTmp;
        soF.ApplyModifiedProperties();

        // ── BagGaugeUI 컴포넌트 ──
        var gaugeUI = bagPanel.AddComponent<BagGaugeUI>();
        var soG = new SerializedObject(gaugeUI);
        soG.FindProperty("fillBar").objectReferenceValue = gfImg;
        soG.FindProperty("countText").objectReferenceValue = bagCount;
        soG.FindProperty("coinText").objectReferenceValue = coinTxt;
        soG.FindProperty("bagInventory").objectReferenceValue = player.GetComponent<BagInventory>();
        soG.ApplyModifiedProperties();

        // ── HudController ──
        var hud = cvGo.AddComponent<HudController>();
        var soH = new SerializedObject(hud);
        soH.FindProperty("scoreText").objectReferenceValue = scoreTxt;
        soH.FindProperty("coinText").objectReferenceValue = coinTxt;
        soH.FindProperty("purificationText").objectReferenceValue = purTxt;
        soH.FindProperty("timerText").objectReferenceValue    = timerTxt;
        soH.FindProperty("cityPurText").objectReferenceValue  = cityPurTxt;
        soH.FindProperty("riverPurText").objectReferenceValue = riverPurTxt;
        soH.FindProperty("beachPurText").objectReferenceValue = beachPurTxt;
        soH.FindProperty("bagInventory").objectReferenceValue = player.GetComponent<BagInventory>();
        var zmArr = soH.FindProperty("zoneManagers");
        zmArr.arraySize = 3;
        zmArr.GetArrayElementAtIndex(0).objectReferenceValue = cityZone;
        zmArr.GetArrayElementAtIndex(1).objectReferenceValue = riverZone;
        zmArr.GetArrayElementAtIndex(2).objectReferenceValue = beachZone;
        var slotsArr = soH.FindProperty("bagSlots");
        slotsArr.arraySize = 12;
        for (int i = 0; i < 12; i++)
            slotsArr.GetArrayElementAtIndex(i).objectReferenceValue = bagSlotUIList[i];
        soH.ApplyModifiedProperties();

        // ── BinSelectionUI 팝업 ──
        var popup = CreateBinSelectionPopup(cvGo);

        // ── PlayerInteractor 에 연결 ──
        var interactor = player.GetComponent<PlayerInteractor>();
        if (interactor)
        {
            var soI = new SerializedObject(interactor);
            soI.FindProperty("feedbackUI").objectReferenceValue = fui;
            soI.FindProperty("binSelectionUI").objectReferenceValue = popup;
            soI.ApplyModifiedProperties();
        }

        // ── BagGaugeUI Setup 호출은 런타임에 Start()에서 처리됨 ──
        // UpgradeShop 연결
        var shops = Object.FindObjectsByType<UpgradeShop>(FindObjectsSortMode.None);
        foreach (var shop in shops)
        {
            var soS = new SerializedObject(shop);
            soS.FindProperty("playerController").objectReferenceValue = player.GetComponent<PlayerController>();
            soS.FindProperty("playerInteractor").objectReferenceValue = interactor;
            soS.FindProperty("bagInventory").objectReferenceValue = player.GetComponent<BagInventory>();
            soS.FindProperty("feedbackUI").objectReferenceValue = fui;
            soS.ApplyModifiedProperties();
        }

        // ── GameManager에 ResultScreen 연결 (Result Canvas 생성) ──
        CreateResultCanvas(cvGo);

        Debug.Log("[Setup] HUD 생성 완료");
    }

    static BinSelectionUI CreateBinSelectionPopup(GameObject canvasParent)
    {
        // 반투명 어두운 오버레이 (전체 화면 덮기)
        var overlay = MakePanel(canvasParent, "BinSelectOverlay",
            Vector2.zero, Vector2.zero, Vector2.zero, Vector2.one);
        overlay.GetComponent<Image>().color = new Color(0, 0, 0, 0.55f);

        // 독립 Canvas + GraphicRaycaster → 버튼 클릭 확실히 처리
        var overlayCv = overlay.AddComponent<Canvas>();
        overlayCv.overrideSorting = true;
        overlayCv.sortingOrder = 50;
        overlay.AddComponent<GraphicRaycaster>();

        // 팝업 박스
        var popup = MakePanel(overlay, "BinSelectPanel",
            Vector2.zero, new Vector2(760, 420), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
        popup.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.15f, 0.97f);

        // 제목
        var titleTxt = MakeTMP(popup, "TitleText", "Recycling Station",
            new Vector2(0, 170), new Vector2(720, 52), new Vector2(0.5f,0.5f), new Vector2(0.5f,0.5f), 36);
        titleTxt.color = new Color(1f, 0.9f, 0.3f);

        // 현재 쓰레기 아이콘 (좌측)
        var iconGo = new GameObject("TrashIcon");
        iconGo.transform.SetParent(popup.transform, false);
        var iconRect = iconGo.AddComponent<RectTransform>();
        iconRect.anchorMin = iconRect.anchorMax = new Vector2(0.5f, 0.5f);
        iconRect.pivot = new Vector2(0.5f, 0.5f);
        iconRect.anchoredPosition = new Vector2(-290, 100);
        iconRect.sizeDelta = new Vector2(52, 52);
        var iconImg = iconGo.AddComponent<Image>();
        iconImg.preserveAspect = true;
        iconImg.color = Color.white;

        // 현재 쓰레기 이름
        var trashTxt = MakeTMP(popup, "TrashNameText", "",
            new Vector2(30, 100), new Vector2(620, 44), new Vector2(0.5f,0.5f), new Vector2(0.5f,0.5f), 28);
        trashTxt.color = Color.white;
        trashTxt.alignment = TextAlignmentOptions.Left;

        // 안내 문구
        var promptTxt = MakeTMP(popup, "PromptText", "Select the correct bin:",
            new Vector2(0, 45), new Vector2(720, 34), new Vector2(0.5f,0.5f), new Vector2(0.5f,0.5f), 22);
        promptTxt.color = new Color(0.8f, 0.8f, 0.8f);

        // 5개 수거함 버튼 컨테이너
        var binContainer = new GameObject("BinContainer");
        binContainer.transform.SetParent(popup.transform, false);
        var bcRect = binContainer.AddComponent<RectTransform>();
        bcRect.anchorMin = bcRect.anchorMax = new Vector2(0.5f, 0.5f);
        bcRect.pivot = new Vector2(0.5f, 0.5f);
        bcRect.anchoredPosition = new Vector2(0, -70);
        bcRect.sizeDelta = new Vector2(720, 165);
        var hLayout = binContainer.AddComponent<HorizontalLayoutGroup>();
        hLayout.spacing = 10;
        hLayout.childAlignment = TextAnchor.MiddleCenter;
        hLayout.childForceExpandWidth  = false;
        hLayout.childForceExpandHeight = false;

        // 닫기 버튼
        var closeBtn = MakeButton(popup, "CloseButton", "Close",
            new Vector2(0, -175), new Vector2(160, 50), new Color(0.55f, 0.15f, 0.15f));

        overlay.SetActive(false);

        // BinSelectionUI는 항상-활성 오브젝트에 부착
        var bsuiGo = new GameObject("BinSelectionUIController");
        bsuiGo.transform.SetParent(canvasParent.transform, false);
        bsuiGo.AddComponent<RectTransform>().sizeDelta = Vector2.zero;

        var bsui = bsuiGo.AddComponent<BinSelectionUI>();
        var so   = new SerializedObject(bsui);
        so.FindProperty("panel").objectReferenceValue            = overlay;
        so.FindProperty("trashNameText").objectReferenceValue    = trashTxt;
        so.FindProperty("trashIconImage").objectReferenceValue   = iconImg;
        so.FindProperty("binButtonContainer").objectReferenceValue = binContainer.transform;
        so.FindProperty("closeButton").objectReferenceValue      = closeBtn.GetComponent<Button>();
        so.ApplyModifiedProperties();

        return bsui;
    }

    static GameObject MakeButton(GameObject parent, string name, string label, Vector2 pos, Vector2 size, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f,0.5f); rect.anchorMax = new Vector2(0.5f,0.5f);
        rect.pivot = new Vector2(0.5f,0.5f); rect.anchoredPosition = pos; rect.sizeDelta = size;
        var img = go.AddComponent<Image>(); img.color = color;
        var btn = go.AddComponent<Button>(); btn.targetGraphic = img;

        // label: stretched inside button
        var labelGo = new GameObject("Label");
        labelGo.transform.SetParent(go.transform, false);
        var lr = labelGo.AddComponent<RectTransform>();
        lr.anchorMin = Vector2.zero; lr.anchorMax = Vector2.one;
        lr.offsetMin = lr.offsetMax = Vector2.zero;
        var tmp = labelGo.AddComponent<TextMeshProUGUI>();
        tmp.text = label; tmp.fontSize = 24; tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        tmp.enableWordWrapping = false;

        return go;
    }

    static void CreateResultCanvas(GameObject hudCanvas)
    {
        var cvGo = new GameObject("Result_Canvas");
        var cv   = cvGo.AddComponent<Canvas>();
        cv.renderMode = RenderMode.ScreenSpaceOverlay; cv.sortingOrder = 20;
        cvGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cvGo.AddComponent<GraphicRaycaster>();

        var panel = MakePanel(cvGo, "ResultPanel", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.one);
        panel.GetComponent<Image>().color = new Color(0,0,0,0.88f);

        // Dynamic title — wired to ResultScreen.titleText (changes based on timeUp)
        var titleTmp = MakeTMP(panel, "TitleText", "All Clear!",
            new Vector2(0, 260), new Vector2(600, 72), new Vector2(0.5f,0.5f), new Vector2(0.5f,0.5f), 54);
        titleTmp.color = Color.yellow;

        MakeTMP(panel, "Title",   "Game Clear!",      new Vector2(0, 210), new Vector2(600, 80),  new Vector2(0.5f,0.5f), new Vector2(0.5f,0.5f), 52).color = Color.yellow;
        var scoreTmp = MakeTMP(panel, "FinalScore",   "Final Score: 0",   new Vector2(0, 110),  new Vector2(500, 60), new Vector2(0.5f,0.5f), new Vector2(0.5f,0.5f), 38);
        var purTmp   = MakeTMP(panel, "Purification", "Purification: 0%", new Vector2(0, 40),   new Vector2(500, 60), new Vector2(0.5f,0.5f), new Vector2(0.5f,0.5f), 38);

        var starSprites = new Sprite[] {
            AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/ui_star_filled_01.png"),
            AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/ui_star_empty_01.png")
        };
        var starImgs = new Image[3];
        for (int i = 0; i < 3; i++)
        {
            var sg = new GameObject($"Star_{i}");
            sg.transform.SetParent(panel.transform, false);
            var sr = sg.AddComponent<RectTransform>();
            sr.anchorMin = sr.anchorMax = new Vector2(0.5f,0.5f);
            sr.pivot = new Vector2(0.5f,0.5f);
            sr.anchoredPosition = new Vector2((i-1)*90f, -50f);
            sr.sizeDelta = new Vector2(72,72);
            var si = sg.AddComponent<Image>(); si.sprite = starSprites[1]; si.preserveAspect = true;
            starImgs[i] = si;
        }

        var retryGo = MakeButton(panel, "RetryButton", "Retry", new Vector2(0,-170), new Vector2(240,65), new Color(0.15f,0.65f,0.2f));
        panel.SetActive(false);

        var rs = cvGo.AddComponent<ResultScreen>();
        var so = new SerializedObject(rs);
        so.FindProperty("resultPanel").objectReferenceValue = panel;
        so.FindProperty("titleText").objectReferenceValue   = titleTmp;
        so.FindProperty("finalScoreText").objectReferenceValue = scoreTmp;
        so.FindProperty("purificationRateText").objectReferenceValue = purTmp;
        so.FindProperty("starFilled").objectReferenceValue = starSprites[0];
        so.FindProperty("starEmpty").objectReferenceValue  = starSprites[1];
        so.FindProperty("retryButton").objectReferenceValue = retryGo.GetComponent<Button>();
        var sa = so.FindProperty("starImages"); sa.arraySize = 3;
        for (int i = 0; i < 3; i++) sa.GetArrayElementAtIndex(i).objectReferenceValue = starImgs[i];
        so.ApplyModifiedProperties();

        var gm = Object.FindFirstObjectByType<GameManager>();
        if (gm) { var soGm = new SerializedObject(gm); soGm.FindProperty("resultScreen").objectReferenceValue = rs; soGm.ApplyModifiedProperties(); }
    }

    // ──────────────────────────────────────────────────
    // 씬 저장
    // ──────────────────────────────────────────────────
    static void SaveScene()
    {
        Directory.CreateDirectory("Assets/Scenes");
        EditorSceneManager.SaveScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene(),
            "Assets/Scenes/MainScene.unity");
        AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
    }

    // ──────────────────────────────────────────────────
    // Animator 헬퍼
    // ──────────────────────────────────────────────────
    static AnimatorState AddState(AnimatorStateMachine sm, string n, AnimationClip c) { var s = sm.AddState(n); s.motion = c; return s; }
    static void Trans(AnimatorState f, AnimatorState t, string b, bool bv, string i, int iv) { var tr = f.AddTransition(t); tr.hasExitTime = false; tr.duration = 0; tr.AddCondition(bv ? AnimatorConditionMode.If : AnimatorConditionMode.IfNot, 0, b); tr.AddCondition(AnimatorConditionMode.Equals, iv, i); }
    static void TransF(AnimatorState f, AnimatorState t) { var tr = f.AddTransition(t); tr.hasExitTime = false; tr.duration = 0; tr.AddCondition(AnimatorConditionMode.IfNot, 0, "IsMoving"); }
    static void TransDir(AnimatorState f, AnimatorState t, int d) { var tr = f.AddTransition(t); tr.hasExitTime = false; tr.duration = 0; tr.AddCondition(AnimatorConditionMode.Equals, d, "Direction"); }
    static void TransTrigger(AnimatorState f, AnimatorState t, string trig) { var tr = f.AddTransition(t); tr.hasExitTime = false; tr.duration = 0; tr.AddCondition(AnimatorConditionMode.If, 0, trig); }

    static void Link(Component c, string field, Object val) { var so = new SerializedObject(c); so.FindProperty(field).objectReferenceValue = val; so.ApplyModifiedProperties(); }

    static AnimationClip MakeClip(string name, float fps, string spritePath)
    {
        var clip = new AnimationClip { name = name, frameRate = fps };
        var spr  = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        if (spr == null) return clip;
        var binding = EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite");
        AnimationUtility.SetObjectReferenceCurve(clip, binding, new[] {
            new ObjectReferenceKeyframe { time = 0, value = spr },
            new ObjectReferenceKeyframe { time = 1f/fps, value = spr }
        });
        string p = $"Assets/Animations/Player/{name}.anim";
        Directory.CreateDirectory("Assets/Animations/Player");
        AssetDatabase.CreateAsset(clip, p);
        return clip;
    }

    static AnimationClip MakeWalkClip(string name, float fps, string dirSuffix, int frameCount)
    {
        var clip = new AnimationClip { name = name, frameRate = fps };
        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        var kfs = new ObjectReferenceKeyframe[frameCount];
        for (int i = 0; i < frameCount; i++)
        {
            string spPath = $"Assets/Sprites/Player/chr_player_{dirSuffix}_{i+1:D2}.png";
            kfs[i] = new ObjectReferenceKeyframe { time = i / fps, value = AssetDatabase.LoadAssetAtPath<Sprite>(spPath) };
        }
        var binding = EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite");
        AnimationUtility.SetObjectReferenceCurve(clip, binding, kfs);

        string p = $"Assets/Animations/Player/{name}.anim";
        Directory.CreateDirectory("Assets/Animations/Player");
        AssetDatabase.CreateAsset(clip, p);
        return clip;
    }

    // ──────────────────────────────────────────────────
    // UI 헬퍼
    // ──────────────────────────────────────────────────
    static TextMeshProUGUI MakeTMP(GameObject parent, string name, string text,
        Vector2 pos, Vector2 size, Vector2 anchorMin, Vector2 anchorMax, int fontSize)
    {
        var go = new GameObject(name); go.transform.SetParent(parent.transform, false);
        var r  = go.AddComponent<RectTransform>();
        r.anchorMin = anchorMin; r.anchorMax = anchorMax; r.pivot = new Vector2(0.5f,0.5f);
        r.anchoredPosition = pos; r.sizeDelta = size;
        var t = go.AddComponent<TextMeshProUGUI>();
        t.text = text; t.fontSize = fontSize; t.color = Color.white;
        t.alignment = TextAlignmentOptions.Center; t.enableWordWrapping = false;
        return t;
    }

    static GameObject MakePanel(GameObject parent, string name,
        Vector2 pos, Vector2 size, Vector2 anchorMin, Vector2 anchorMax)
    {
        var go = new GameObject(name); go.transform.SetParent(parent.transform, false);
        var r  = go.AddComponent<RectTransform>();
        r.anchorMin = anchorMin; r.anchorMax = anchorMax; r.pivot = new Vector2(0.5f,0.5f);
        r.anchoredPosition = pos; r.sizeDelta = size;
        go.AddComponent<Image>().color = new Color(0.1f,0.1f,0.12f,0.85f);
        return go;
    }

    // ──────────────────────────────────────────────────
    // 스프라이트 헬퍼
    // ──────────────────────────────────────────────────
    static Sprite CreateSolidSprite(Color color)
    {
        var tex = new Texture2D(4,4);
        var px  = new Color[16];
        for (int i = 0; i < 16; i++) px[i] = color;
        tex.SetPixels(px); tex.Apply();
        return Sprite.Create(tex, new Rect(0,0,4,4), new Vector2(0.5f,0.5f), 4f,
            0, SpriteMeshType.FullRect, Vector4.one);
    }

    static Sprite CreateCircleSprite(Color color)
    {
        int s = 32;
        var tex = new Texture2D(s, s, TextureFormat.RGBA32, false);
        float r = s / 2f;
        for (int y = 0; y < s; y++)
            for (int x = 0; x < s; x++)
            {
                float dx = x - r, dy = y - r;
                float dist = Mathf.Sqrt(dx*dx + dy*dy);
                tex.SetPixel(x, y, dist <= r ? color : Color.clear);
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0,0,s,s), new Vector2(0.5f,0.5f), s);
    }

    static void CreateWorldLabel(GameObject parent, string text, Vector3 localPos)
    {
        // 월드 스페이스 TextMesh (Unity World Space)
        var go = new GameObject("Label");
        go.transform.SetParent(parent.transform);
        go.transform.localPosition = localPos;
        go.transform.localScale = Vector3.one * 0.05f;
        var tm = go.AddComponent<TextMesh>();
        tm.text = text; tm.fontSize = 36; tm.color = Color.white;
        tm.anchor = TextAnchor.MiddleCenter; tm.alignment = TextAlignment.Center;
    }

    static string BinName(BinType t) => t switch
    {
        BinType.Paper   => "Paper",
        BinType.Can     => "Can",
        BinType.Plastic => "Plastic",
        BinType.General => "General",
        BinType.Special => "Special",
        _               => t.ToString()
    };
}
#endif
