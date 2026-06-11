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
/// 메뉴: CleanWave → 전체 게임 자동 구축
/// 스프라이트 설정 → 프리팹 생성 → 씬 배치 → HUD → 애니메이터 까지 한 번에 처리
/// </summary>
public class CleanWaveFullSetup : EditorWindow
{
    [MenuItem("CleanWave/★ 전체 게임 자동 구축 ★")]
    public static void RunFullSetup()
    {
        if (!EditorUtility.DisplayDialog("CleanWave 전체 구축",
            "씬의 모든 오브젝트를 자동으로 생성합니다.\n기존 씬 내용이 초기화됩니다.\n계속하시겠습니까?", "예, 구축하기", "취소"))
            return;

        EditorUtility.DisplayProgressBar("CleanWave Setup", "스프라이트 임포트 설정...", 0.05f);
        FixSpriteImports();

        EditorUtility.DisplayProgressBar("CleanWave Setup", "씬 초기화...", 0.10f);
        ClearScene();

        EditorUtility.DisplayProgressBar("CleanWave Setup", "카메라 & 매니저 생성...", 0.15f);
        SetupManagers();

        EditorUtility.DisplayProgressBar("CleanWave Setup", "Grid & Tilemap 생성...", 0.20f);
        SetupTilemap();

        EditorUtility.DisplayProgressBar("CleanWave Setup", "맵 타일 배치...", 0.30f);
        PaintMap();

        EditorUtility.DisplayProgressBar("CleanWave Setup", "구역 게이트 생성...", 0.38f);
        CreateZoneGates();

        EditorUtility.DisplayProgressBar("CleanWave Setup", "수거함 배치...", 0.45f);
        PlaceTrashBins();

        EditorUtility.DisplayProgressBar("CleanWave Setup", "쓰레기 배치...", 0.55f);
        PlaceTrashItems();

        EditorUtility.DisplayProgressBar("CleanWave Setup", "업그레이드 상점 배치...", 0.62f);
        PlaceUpgradeShops();

        EditorUtility.DisplayProgressBar("CleanWave Setup", "플레이어 생성...", 0.70f);
        GameObject player = CreatePlayer();

        EditorUtility.DisplayProgressBar("CleanWave Setup", "애니메이터 컨트롤러 생성...", 0.78f);
        CreateAnimatorController(player);

        EditorUtility.DisplayProgressBar("CleanWave Setup", "HUD 생성...", 0.85f);
        CreateHUD(player);

        EditorUtility.DisplayProgressBar("CleanWave Setup", "결과화면 생성...", 0.92f);
        CreateResultScreen();

        EditorUtility.DisplayProgressBar("CleanWave Setup", "씬 저장...", 0.97f);
        SaveScene();

        EditorUtility.ClearProgressBar();
        EditorUtility.DisplayDialog("완료!", "CleanWave 씬 구축 완료!\nPlay 버튼을 눌러 테스트하세요.", "확인");
        Debug.Log("[CleanWave] ★ 전체 게임 자동 구축 완료!");
    }

    // ─────────────────────────────────────────────
    // 1. 스프라이트 임포트 설정 (픽셀아트, Point filter)
    // ─────────────────────────────────────────────
    static void FixSpriteImports()
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Sprites" });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) continue;

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.spritePixelsPerUnit = 32;
            importer.mipmapEnabled = false;

            // 파일명으로 pivot 결정
            string fileName = Path.GetFileNameWithoutExtension(path).ToLower();
            if (fileName.Contains("chr_") || fileName.Contains("bin_") || fileName.Contains("npc_") || fileName.Contains("bld_"))
                importer.spritePivot = new Vector2(0.5f, 0f); // 발 기준
            else
                importer.spritePivot = new Vector2(0.5f, 0.5f);

            importer.SaveAndReimport();
        }
        AssetDatabase.Refresh();
        Debug.Log("[Setup] 스프라이트 임포트 설정 완료");
    }

    // ─────────────────────────────────────────────
    // 2. 씬 초기화
    // ─────────────────────────────────────────────
    static void ClearScene()
    {
        var roots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (var go in roots)
        {
            if (go.name != "Main Camera") // 카메라 보존
                Object.DestroyImmediate(go);
        }
    }

    // ─────────────────────────────────────────────
    // 3. 카메라, GameManager, ScoreManager
    // ─────────────────────────────────────────────
    static void SetupManagers()
    {
        // Camera
        var cam = Camera.main ?? new GameObject("Main Camera").AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 6f;
        cam.backgroundColor = new Color(0.1f, 0.1f, 0.15f);
        cam.transform.position = new Vector3(10, 7.5f, -10);
        if (cam.GetComponent<CameraFollow>() == null) cam.gameObject.AddComponent<CameraFollow>();
        if (cam.gameObject.GetComponent<AudioListener>() == null) cam.gameObject.AddComponent<AudioListener>();

        // GameManager (with ScoreManager)
        var managerGo = new GameObject("GameManager");
        var gm = managerGo.AddComponent<GameManager>();
        managerGo.AddComponent<ScoreManager>();

        // Sorting layers (programmatically ensure)
        Debug.Log("[Setup] 카메라 & 매니저 생성 완료");
    }

    // ─────────────────────────────────────────────
    // 4. Grid & Tilemap 3레이어
    // ─────────────────────────────────────────────
    static Grid grid;
    static Tilemap groundMap, decorMap, wallMap;

    static void SetupTilemap()
    {
        var gridGo = new GameObject("Grid");
        grid = gridGo.AddComponent<Grid>();
        grid.cellSize = Vector3.one;

        groundMap = CreateTilemapLayer(gridGo, "Ground", -1);
        decorMap  = CreateTilemapLayer(gridGo, "Decoration", 0);
        wallMap   = CreateTilemapLayer(gridGo, "Wall", 1);

        // Wall 충돌
        var wallCol = wallMap.gameObject.AddComponent<TilemapCollider2D>();
        wallCol.usedByComposite = true;
        var comp = wallMap.gameObject.AddComponent<CompositeCollider2D>();
        comp.geometryType = CompositeCollider2D.GeometryType.Polygons;
        var rb = wallMap.gameObject.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;

        Debug.Log("[Setup] Tilemap 생성 완료");
    }

    static Tilemap CreateTilemapLayer(GameObject parent, string layerName, int sortOrder)
    {
        var go = new GameObject(layerName);
        go.transform.SetParent(parent.transform);
        var tm = go.AddComponent<Tilemap>();
        var tr = go.AddComponent<TilemapRenderer>();
        tr.sortingOrder = sortOrder;
        return tm;
    }

    // ─────────────────────────────────────────────
    // 5. 맵 타일 페인팅  (도심 0~19, 하천 22~41, 해안 44~63 — x축 연결)
    //    각 구역은 20x15 타일
    // ─────────────────────────────────────────────
    static void PaintMap()
    {
        // 스프라이트 로드
        var cityGround  = LoadSprite("Assets/Sprites/Tiles/tile_city_ground_01.png");
        var cityGrass   = LoadSprite("Assets/Sprites/Tiles/tile_city_grass_01.png");
        var cityRoad    = LoadSprite("Assets/Sprites/Tiles/tile_city_road_01.png");
        var cityWall    = LoadSprite("Assets/Sprites/Tiles/tile_city_wall_01.png");
        var riverWater  = LoadSprite("Assets/Sprites/Tiles/tile_river_water_01.png");
        var riverBank   = LoadSprite("Assets/Sprites/Tiles/tile_river_bank_01.png");
        var beachSand   = LoadSprite("Assets/Sprites/Tiles/tile_beach_sand_01.png");
        var beachWater  = LoadSprite("Assets/Sprites/Tiles/tile_beach_water_01.png");
        var gateTile    = LoadSprite("Assets/Sprites/Tiles/tile_gate_01.png");

        // ── 도심 구역 (x: 0-19, y: 0-14) ──
        PaintRect(groundMap, cityGround, 0, 0, 20, 15);
        PaintRect(groundMap, cityRoad,   0, 6, 20, 3);
        PaintRect(groundMap, cityRoad,   8, 0,  4, 15);
        PaintRect(decorMap,  cityGrass,  1, 10, 6, 4);
        PaintRect(decorMap,  cityGrass, 13, 10, 5, 4);
        PaintRect(decorMap,  cityGrass,  1,  1, 6, 4);
        PaintRect(decorMap,  cityGrass, 13,  1, 5, 4);
        PaintBorderCollider(wallMap, cityWall, 0, 0, 20, 15);

        // ── 하천 구역 (x: 22-41, y: 0-14) ──
        PaintRect(groundMap, riverBank,  22, 0, 20, 15);
        PaintRect(groundMap, riverWater, 22, 4, 20,  7);
        PaintRect(groundMap, riverBank,  22, 6, 20,  3);
        PaintBorderCollider(wallMap, cityWall, 22, 0, 20, 15);

        // ── 해안 구역 (x: 44-63, y: 0-14) ──
        PaintRect(groundMap, beachSand,  44, 0, 20, 15);
        PaintRect(groundMap, beachWater, 44, 0, 20,  5);
        PaintRect(groundMap, beachSand,  44, 4, 20,  1);
        PaintBorderCollider(wallMap, cityWall, 44, 0, 20, 15);

        // ── 연결 통로 ──
        PaintRect(groundMap, cityRoad,  20, 6, 2, 3);
        PaintRect(groundMap, riverBank, 42, 6, 2, 3);

        Debug.Log("[Setup] 맵 페인팅 완료");
    }

    static void PaintRect(Tilemap tm, Sprite spr, int x, int y, int w, int h)
    {
        if (spr == null) return;
        var tile = CreateTile(spr);
        for (int dy = 0; dy < h; dy++)
            for (int dx = 0; dx < w; dx++)
                tm.SetTile(new Vector3Int(x + dx, y + dy, 0), tile);
    }

    static void PaintBorder(Tilemap tm, Sprite spr, int x, int y, int w, int h)
    {
        if (spr == null) return;
        var tile = CreateTile(spr);
        for (int dx = 0; dx < w; dx++)
        {
            tm.SetTile(new Vector3Int(x + dx, y, 0), tile);
            tm.SetTile(new Vector3Int(x + dx, y + h - 1, 0), tile);
        }
        for (int dy = 1; dy < h - 1; dy++)
        {
            tm.SetTile(new Vector3Int(x, y + dy, 0), tile);
            tm.SetTile(new Vector3Int(x + w - 1, y + dy, 0), tile);
        }
    }

    static void PaintBorderCollider(Tilemap tm, Sprite spr, int x, int y, int w, int h)
    {
        if (spr == null) return;
        var tile = CreateColliderTile(spr);
        for (int dx = 0; dx < w; dx++)
        {
            tm.SetTile(new Vector3Int(x + dx, y, 0), tile);
            tm.SetTile(new Vector3Int(x + dx, y + h - 1, 0), tile);
        }
        for (int dy = 1; dy < h - 1; dy++)
        {
            tm.SetTile(new Vector3Int(x, y + dy, 0), tile);
            tm.SetTile(new Vector3Int(x + w - 1, y + dy, 0), tile);
        }
    }

    static System.Collections.Generic.Dictionary<string, Tile> tileCache = new System.Collections.Generic.Dictionary<string, Tile>();

    static Tile CreateTile(Sprite spr)
    {
        if (spr == null) return null;
        string key = spr.name;
        if (tileCache.TryGetValue(key, out Tile cached)) return cached;

        string tileDir = "Assets/Tiles";
        Directory.CreateDirectory(tileDir);
        string tilePath = $"{tileDir}/{key}.asset";

        var existing = AssetDatabase.LoadAssetAtPath<Tile>(tilePath);
        if (existing != null) { tileCache[key] = existing; return existing; }

        var tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = spr;
        tile.colliderType = Tile.ColliderType.None;
        AssetDatabase.CreateAsset(tile, tilePath);
        tileCache[key] = tile;
        return tile;
    }

    static Tile CreateColliderTile(Sprite spr)
    {
        if (spr == null) return null;
        string key = spr.name + "_col";
        if (tileCache.TryGetValue(key, out Tile cached)) return cached;

        string tileDir = "Assets/Tiles";
        Directory.CreateDirectory(tileDir);
        string tilePath = $"{tileDir}/{key}.asset";

        var existing = AssetDatabase.LoadAssetAtPath<Tile>(tilePath);
        if (existing != null) { tileCache[key] = existing; return existing; }

        var tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = spr;
        tile.colliderType = Tile.ColliderType.Grid;
        AssetDatabase.CreateAsset(tile, tilePath);
        tileCache[key] = tile;
        return tile;
    }

    // ─────────────────────────────────────────────
    // 6. 구역 게이트 (도심→하천, 하천→해안)
    // ─────────────────────────────────────────────
    static ZoneManager cityZone, riverZone, beachZone;
    static ZoneGate gate1, gate2;

    static void CreateZoneGates()
    {
        // ZoneManager 오브젝트들
        cityZone  = CreateZoneManagerGo("ZoneManager_City",  ZoneType.City);
        riverZone = CreateZoneManagerGo("ZoneManager_River", ZoneType.River);
        beachZone = CreateZoneManagerGo("ZoneManager_Beach", ZoneType.Beach);

        // GameManager 참조 연결은 런타임에 RegisterZone()으로 처리됨

        // Gate 1: 도심 → 하천 (x=21, y=6~8)
        gate1 = CreateGate("Gate_CityToRiver", new Vector3(21f, 7.5f, 0), new Vector2(2f, 3f));

        // Gate 2: 하천 → 해안 (x=43, y=6~8)
        gate2 = CreateGate("Gate_RiverToBeach", new Vector3(43f, 7.5f, 0), new Vector2(2f, 3f));

        // ZoneManager에 게이트 연결
        SerializedObject so1 = new SerializedObject(cityZone);
        so1.FindProperty("gateToNextZone").objectReferenceValue = gate1;
        so1.ApplyModifiedProperties();

        SerializedObject so2 = new SerializedObject(riverZone);
        so2.FindProperty("gateToNextZone").objectReferenceValue = gate2;
        so2.ApplyModifiedProperties();

        Debug.Log("[Setup] ZoneGate 생성 완료");
    }

    static ZoneManager CreateZoneManagerGo(string name, ZoneType zoneType)
    {
        var go = new GameObject(name);
        var zm = go.AddComponent<ZoneManager>();
        var so = new SerializedObject(zm);
        so.FindProperty("zoneType").enumValueIndex = (int)zoneType;
        so.ApplyModifiedProperties();
        return zm;
    }

    static ZoneGate CreateGate(string name, Vector3 pos, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.position = pos;

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = LoadSprite("Assets/Sprites/Tiles/tile_gate_01.png");
        sr.color = new Color(0.8f, 0.3f, 0.1f, 0.9f);
        sr.drawMode = SpriteDrawMode.Sliced;
        sr.size = size;
        sr.sortingOrder = 3;

        var col = go.AddComponent<BoxCollider2D>();
        col.size = size;

        var gate = go.AddComponent<ZoneGate>();

        // ZoneGate 직렬화 필드 연결
        var so = new SerializedObject(gate);
        so.FindProperty("gateVisual").objectReferenceValue = go;
        so.FindProperty("gateCollider").objectReferenceValue = col;
        so.ApplyModifiedProperties();

        return gate;
    }

    // ─────────────────────────────────────────────
    // 7. 수거함 배치 (구역마다 5종 세트)
    // ─────────────────────────────────────────────
    static void PlaceTrashBins()
    {
        // 도심 수거함: x=9~13, y=1 (하단 잔디 위)
        PlaceBinSet(ZoneType.City,  new Vector3(3f, 1.5f, 0));
        // 하천 수거함
        PlaceBinSet(ZoneType.River, new Vector3(25f, 1.5f, 0));
        // 해안 수거함
        PlaceBinSet(ZoneType.Beach, new Vector3(47f, 1.5f, 0));

        Debug.Log("[Setup] 수거함 배치 완료");
    }

    static readonly BinType[] binTypes = { BinType.Paper, BinType.Can, BinType.Plastic, BinType.General, BinType.Special };
    static readonly string[] binSprites = { "bin_paper_01", "bin_can_01", "bin_plastic_01", "bin_general_01", "bin_special_01" };

    static void PlaceBinSet(ZoneType zone, Vector3 origin)
    {
        for (int i = 0; i < 5; i++)
        {
            var go = new GameObject($"Bin_{binTypes[i]}_{zone}");
            go.transform.position = origin + new Vector3(i * 1.2f, 0, 0);
            go.layer = LayerMask.NameToLayer("Default");

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = LoadSprite($"Assets/Sprites/Bin/{binSprites[i]}.png");
            sr.sortingOrder = 2;

            var col = go.AddComponent<BoxCollider2D>();
            col.size = new Vector2(0.8f, 1.2f);
            col.offset = new Vector2(0, 0.6f);
            col.isTrigger = true;

            var bin = go.AddComponent<TrashBin>();
            var so = new SerializedObject(bin);
            so.FindProperty("binType").enumValueIndex = (int)binTypes[i];
            so.FindProperty("zoneType").enumValueIndex = (int)zone;
            so.ApplyModifiedProperties();
        }
    }

    // ─────────────────────────────────────────────
    // 8. 쓰레기 배치
    //    도심 20개, 하천 25개, 해안 30개
    // ─────────────────────────────────────────────
    static readonly TrashType[] cityTrash  = { TrashType.Paper, TrashType.Can, TrashType.Plastic, TrashType.Vinyl, TrashType.Food, TrashType.Paper, TrashType.Can, TrashType.Plastic, TrashType.Paper, TrashType.Can, TrashType.Plastic, TrashType.Vinyl, TrashType.Food, TrashType.Paper, TrashType.Can, TrashType.Plastic, TrashType.Vinyl, TrashType.Food, TrashType.Paper, TrashType.Can };
    static readonly TrashType[] riverTrash = { TrashType.Vinyl, TrashType.Plastic, TrashType.Can, TrashType.Food, TrashType.Vinyl, TrashType.Paper, TrashType.Plastic, TrashType.Vinyl, TrashType.Food, TrashType.Can, TrashType.Plastic, TrashType.Vinyl, TrashType.Paper, TrashType.Food, TrashType.Vinyl, TrashType.Can, TrashType.Plastic, TrashType.Paper, TrashType.Vinyl, TrashType.Food, TrashType.Plastic, TrashType.Vinyl, TrashType.Food, TrashType.Plastic, TrashType.Vinyl };
    static readonly TrashType[] beachTrash = { TrashType.Net, TrashType.Oil, TrashType.Plastic, TrashType.Net, TrashType.Oil, TrashType.Net, TrashType.Plastic, TrashType.Oil, TrashType.Net, TrashType.Oil, TrashType.Vinyl, TrashType.Net, TrashType.Oil, TrashType.Plastic, TrashType.Net, TrashType.Oil, TrashType.Net, TrashType.Plastic, TrashType.Oil, TrashType.Net, TrashType.Vinyl, TrashType.Net, TrashType.Oil, TrashType.Plastic, TrashType.Net, TrashType.Oil, TrashType.Vinyl, TrashType.Net, TrashType.Oil, TrashType.Plastic };

    static void PlaceTrashItems()
    {
        // 도심: 잔디/도로 위 분산 (통로 제외)
        Vector2[] cityPositions = GeneratePositions(20, 1f, 7f, 1f, 14f, new Vector2(7f, 5f), new Vector2(10f, 10f));
        PlaceTrashSet(cityPositions, cityTrash, ZoneType.City, 0);

        // 하천: 강가 위주
        Vector2[] riverPositions = GeneratePositions(25, 23f, 41f, 1f, 5f, new Vector2(26f, 3f), new Vector2(40f, 5f));
        PlaceTrashSet(riverPositions, riverTrash, ZoneType.River, 0);

        // 해안: 모래사장
        Vector2[] beachPositions = GeneratePositions(30, 45f, 63f, 5f, 14f, new Vector2(47f, 6f), new Vector2(62f, 13f));
        PlaceTrashSet(beachPositions, beachTrash, ZoneType.Beach, 0);

        Debug.Log("[Setup] 쓰레기 배치 완료");
    }

    static readonly string[] trashSprites = { "trash_paper_01", "trash_can_01", "trash_plastic_01", "trash_vinyl_01", "trash_food_01", "trash_net_01", "trash_oil_01" };

    static void PlaceTrashSet(Vector2[] positions, TrashType[] types, ZoneType zone, int startIdx)
    {
        var parent = new GameObject($"Trash_{zone}");
        for (int i = 0; i < positions.Length && i < types.Length; i++)
        {
            var go = new GameObject($"Trash_{types[i]}_{i}");
            go.transform.SetParent(parent.transform);
            go.transform.position = new Vector3(positions[i].x, positions[i].y, 0);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = LoadSprite($"Assets/Sprites/Trash/{trashSprites[(int)types[i]]}.png");
            sr.sortingOrder = 2;

            var col = go.AddComponent<CircleCollider2D>();
            col.radius = 0.25f;
            col.isTrigger = true;

            var item = go.AddComponent<TrashItem>();
            var so = new SerializedObject(item);
            so.FindProperty("trashType").enumValueIndex = (int)types[i];
            so.FindProperty("zoneType").enumValueIndex = (int)zone;
            so.ApplyModifiedProperties();
        }
    }

    static Vector2[] GeneratePositions(int count, float xMin, float xMax, float yMin, float yMax, Vector2 excludeMin, Vector2 excludeMax)
    {
        var result = new System.Collections.Generic.List<Vector2>();
        var rng = new System.Random(42);
        int attempts = 0;
        while (result.Count < count && attempts < 1000)
        {
            attempts++;
            float x = (float)(rng.NextDouble() * (xMax - xMin) + xMin);
            float y = (float)(rng.NextDouble() * (yMax - yMin) + yMin);
            // 제외 구역 스킵 (수거함 영역)
            if (x >= excludeMin.x && x <= excludeMax.x && y >= excludeMin.y && y <= excludeMax.y) continue;
            // 기존 위치와 너무 가까우면 스킵
            bool tooClose = false;
            foreach (var p in result) if (Vector2.Distance(p, new Vector2(x, y)) < 0.8f) { tooClose = true; break; }
            if (!tooClose) result.Add(new Vector2(Mathf.Round(x * 2) / 2f, Mathf.Round(y * 2) / 2f));
        }
        return result.ToArray();
    }

    // ─────────────────────────────────────────────
    // 9. 업그레이드 상점 (구역마다 3종)
    // ─────────────────────────────────────────────
    static void PlaceUpgradeShops()
    {
        PlaceShopSet(ZoneType.City,  new Vector3(14f, 1.5f, 0));
        PlaceShopSet(ZoneType.River, new Vector3(36f, 1.5f, 0));
        PlaceShopSet(ZoneType.Beach, new Vector3(58f, 1.5f, 0));
        Debug.Log("[Setup] 업그레이드 상점 배치 완료");
    }

    static readonly UpgradeType[] shopTypes = { UpgradeType.Tongs, UpgradeType.Bag, UpgradeType.Shoes };
    static readonly string[] shopColors = { "#FF8844", "#44BB44", "#4488FF" };

    static void PlaceShopSet(ZoneType zone, Vector3 origin)
    {
        var parent = new GameObject($"Shops_{zone}");
        for (int i = 0; i < 3; i++)
        {
            var go = new GameObject($"Shop_{shopTypes[i]}");
            go.transform.SetParent(parent.transform);
            go.transform.position = origin + new Vector3(i * 1.4f, 0, 0);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = LoadSprite("Assets/Sprites/Buildings/bld_shop_01.png");
            ColorUtility.TryParseHtmlString(shopColors[i], out Color c);
            sr.color = c;
            sr.sortingOrder = 2;

            var col = go.AddComponent<BoxCollider2D>();
            col.size = new Vector2(1f, 1.5f);
            col.offset = new Vector2(0, 0.75f);
            col.isTrigger = true;

            // Label
            var label = new GameObject("Label");
            label.transform.SetParent(go.transform);
            label.transform.localPosition = new Vector3(0, 1.8f, 0);
            // (TextMeshPro는 런타임에서만 가능하므로 태그만 추가)

            var shop = go.AddComponent<UpgradeShop>();
            var so = new SerializedObject(shop);
            so.FindProperty("upgradeType").enumValueIndex = (int)shopTypes[i];
            so.ApplyModifiedProperties();
        }
    }

    // ─────────────────────────────────────────────
    // 10. 플레이어 생성
    // ─────────────────────────────────────────────
    static GameObject CreatePlayer()
    {
        var go = new GameObject("Player");
        go.tag = "Player";
        go.transform.position = new Vector3(10f, 7.5f, 0);
        go.layer = LayerMask.NameToLayer("Default");

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = LoadSprite("Assets/Sprites/Player/chr_player_idle_down_01.png");
        sr.sortingOrder = 5;

        var rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        var col = go.AddComponent<CapsuleCollider2D>();
        col.size = new Vector2(0.5f, 0.9f);
        col.offset = new Vector2(0f, 0.45f);

        var pc = go.AddComponent<PlayerController>();
        var bag = go.AddComponent<BagInventory>();

        // CameraFollow 연결
        var camFollow = Object.FindFirstObjectByType<CameraFollow>();
        if (camFollow != null)
        {
            camFollow.SetTarget(go.transform);
        }

        Debug.Log("[Setup] 플레이어 생성 완료");
        return go;
    }

    // ─────────────────────────────────────────────
    // 11. Animator Controller 생성 및 연결
    // ─────────────────────────────────────────────
    static void CreateAnimatorController(GameObject player)
    {
        string savePath = "Assets/Animations/Controllers/PlayerAnimator.controller";
        Directory.CreateDirectory("Assets/Animations/Controllers");

        var controller = AnimatorController.CreateAnimatorControllerAtPath(savePath);

        // 파라미터 추가
        controller.AddParameter("IsMoving", AnimatorControllerParameterType.Bool);
        controller.AddParameter("Direction", AnimatorControllerParameterType.Int);
        controller.AddParameter("FaceX", AnimatorControllerParameterType.Float);
        controller.AddParameter("Pickup", AnimatorControllerParameterType.Trigger);

        var rootSM = controller.layers[0].stateMachine;

        // 스테이트: Idle_Down
        var idleDown = rootSM.AddState("Idle_Down");
        idleDown.motion = LoadClip("Assets/Sprites/Player/chr_player_idle_down_01.png", "idle_down");

        var idleUp = rootSM.AddState("Idle_Up");
        idleUp.motion = LoadClip("Assets/Sprites/Player/chr_player_idle_up_01.png", "idle_up");

        var idleSide = rootSM.AddState("Idle_Side");
        idleSide.motion = LoadClip("Assets/Sprites/Player/chr_player_idle_side_01.png", "idle_side");

        // Walk 클립 (4프레임 애니메이션)
        var walkDown = rootSM.AddState("Walk_Down");
        walkDown.motion = CreateWalkClip(new[] {
            "Assets/Sprites/Player/chr_player_walk_down_01.png",
            "Assets/Sprites/Player/chr_player_walk_down_02.png",
            "Assets/Sprites/Player/chr_player_walk_down_03.png",
            "Assets/Sprites/Player/chr_player_walk_down_04.png"
        }, "walk_down");

        var walkUp = rootSM.AddState("Walk_Up");
        walkUp.motion = CreateWalkClip(new[] {
            "Assets/Sprites/Player/chr_player_walk_up_01.png",
            "Assets/Sprites/Player/chr_player_walk_up_02.png",
            "Assets/Sprites/Player/chr_player_walk_up_03.png",
            "Assets/Sprites/Player/chr_player_walk_up_04.png"
        }, "walk_up");

        var walkSide = rootSM.AddState("Walk_Side");
        walkSide.motion = CreateWalkClip(new[] {
            "Assets/Sprites/Player/chr_player_walk_side_01.png",
            "Assets/Sprites/Player/chr_player_walk_side_02.png",
            "Assets/Sprites/Player/chr_player_walk_side_03.png",
            "Assets/Sprites/Player/chr_player_walk_side_04.png"
        }, "walk_side");

        // Pickup 클립
        var pickupDown = rootSM.AddState("Pickup_Down");
        pickupDown.motion = CreateWalkClip(new[] {
            "Assets/Sprites/Player/chr_player_pickup_down_01.png",
            "Assets/Sprites/Player/chr_player_pickup_down_02.png",
            "Assets/Sprites/Player/chr_player_pickup_down_03.png"
        }, "pickup_down", fps: 8);

        // 기본 스테이트 = Idle_Down
        rootSM.defaultState = idleDown;

        // 트랜지션: Idle_Down → Walk_Down (IsMoving && Direction==0)
        AddTransition(idleDown, walkDown, "IsMoving", true,  "Direction", 0);
        AddTransition(idleDown, walkUp,   "IsMoving", true,  "Direction", 1);
        AddTransition(idleDown, walkSide, "IsMoving", true,  "Direction", 2);
        // Walk → Idle
        AddTransitionFalse(walkDown, idleDown, "IsMoving");
        AddTransitionFalse(walkUp,   idleUp,   "IsMoving");
        AddTransitionFalse(walkSide, idleSide, "IsMoving");
        // 방향 전환
        AddTransition(walkDown, walkUp,   "IsMoving", true, "Direction", 1);
        AddTransition(walkDown, walkSide, "IsMoving", true, "Direction", 2);
        AddTransition(walkUp,   walkDown, "IsMoving", true, "Direction", 0);
        AddTransition(walkUp,   walkSide, "IsMoving", true, "Direction", 2);
        AddTransition(walkSide, walkDown, "IsMoving", true, "Direction", 0);
        AddTransition(walkSide, walkUp,   "IsMoving", true, "Direction", 1);
        // Idle 방향 전환
        AddTransitionInt(idleDown, idleUp,   "Direction", 1);
        AddTransitionInt(idleDown, idleSide, "Direction", 2);
        AddTransitionInt(idleUp,   idleDown, "Direction", 0);
        AddTransitionInt(idleUp,   idleSide, "Direction", 2);
        AddTransitionInt(idleSide, idleDown, "Direction", 0);
        AddTransitionInt(idleSide, idleUp,   "Direction", 1);
        // Pickup
        AddTriggerTransition(idleDown, pickupDown, "Pickup");
        AddTriggerTransition(walkDown, pickupDown, "Pickup");
        var backTrans = pickupDown.AddTransition(idleDown);
        backTrans.hasExitTime = true;
        backTrans.exitTime = 1f;

        AssetDatabase.SaveAssets();

        // 플레이어에 Animator 추가
        var anim = player.AddComponent<Animator>();
        anim.runtimeAnimatorController = controller;

        // PlayerInteractor 추가 (Animator 후에)
        var interactor = player.AddComponent<PlayerInteractor>();

        // PlayerInteractor에 BagInventory 연결
        var bag = player.GetComponent<BagInventory>();
        var soInteractor = new SerializedObject(interactor);
        soInteractor.FindProperty("bagInventory").objectReferenceValue = bag;

        // InteractLayer = Default (0)
        int defaultLayer = LayerMask.GetMask("Default");
        soInteractor.FindProperty("interactLayer").intValue = defaultLayer;
        soInteractor.ApplyModifiedProperties();

        Debug.Log("[Setup] Animator Controller 생성 완료: " + savePath);
    }

    static AnimationClip LoadClip(string spritePath, string clipName)
    {
        var clip = new AnimationClip();
        clip.name = clipName;
        clip.frameRate = 4;
        var spr = LoadSprite(spritePath);
        if (spr == null) return clip;

        var binding = new UnityEditor.EditorCurveBinding();
        binding.type = typeof(SpriteRenderer);
        binding.path = "";
        binding.propertyName = "m_Sprite";

        var kf = new ObjectReferenceKeyframe[2];
        kf[0] = new ObjectReferenceKeyframe { time = 0f, value = spr };
        kf[1] = new ObjectReferenceKeyframe { time = 0.25f, value = spr };
        AnimationUtility.SetObjectReferenceCurve(clip, binding, kf);

        string savePath = $"Assets/Animations/Player/{clipName}.anim";
        Directory.CreateDirectory("Assets/Animations/Player");
        AssetDatabase.CreateAsset(clip, savePath);
        return clip;
    }

    static AnimationClip CreateWalkClip(string[] spritePaths, string clipName, float fps = 8f)
    {
        var clip = new AnimationClip();
        clip.name = clipName;
        clip.frameRate = fps;
        clip.wrapMode = WrapMode.Loop;

        var sprites = new Sprite[spritePaths.Length];
        for (int i = 0; i < spritePaths.Length; i++)
            sprites[i] = LoadSprite(spritePaths[i]);

        var binding = new UnityEditor.EditorCurveBinding();
        binding.type = typeof(SpriteRenderer);
        binding.path = "";
        binding.propertyName = "m_Sprite";

        float frameTime = 1f / fps;
        var kf = new ObjectReferenceKeyframe[sprites.Length];
        for (int i = 0; i < sprites.Length; i++)
            kf[i] = new ObjectReferenceKeyframe { time = i * frameTime, value = sprites[i] };

        AnimationUtility.SetObjectReferenceCurve(clip, binding, kf);

        // 루프 설정
        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        string savePath = $"Assets/Animations/Player/{clipName}.anim";
        Directory.CreateDirectory("Assets/Animations/Player");
        AssetDatabase.CreateAsset(clip, savePath);
        return clip;
    }

    static void AddTransition(AnimatorState from, AnimatorState to, string boolName, bool boolVal, string intName, int intVal)
    {
        var t = from.AddTransition(to);
        t.hasExitTime = false;
        t.duration = 0f;
        t.AddCondition(boolVal ? AnimatorConditionMode.If : AnimatorConditionMode.IfNot, 0, boolName);
        t.AddCondition(AnimatorConditionMode.Equals, intVal, intName);
    }

    static void AddTransitionFalse(AnimatorState from, AnimatorState to, string boolName)
    {
        var t = from.AddTransition(to);
        t.hasExitTime = false;
        t.duration = 0f;
        t.AddCondition(AnimatorConditionMode.IfNot, 0, boolName);
    }

    static void AddTransitionInt(AnimatorState from, AnimatorState to, string intName, int intVal)
    {
        var t = from.AddTransition(to);
        t.hasExitTime = false;
        t.duration = 0f;
        t.AddCondition(AnimatorConditionMode.Equals, intVal, intName);
    }

    static void AddTriggerTransition(AnimatorState from, AnimatorState to, string trigger)
    {
        var t = from.AddTransition(to);
        t.hasExitTime = false;
        t.duration = 0f;
        t.AddCondition(AnimatorConditionMode.If, 0, trigger);
    }

    // ─────────────────────────────────────────────
    // 12. HUD Canvas
    // ─────────────────────────────────────────────
    static void CreateHUD(GameObject player)
    {
        var canvasGo = new GameObject("HUD_Canvas");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGo.AddComponent<GraphicRaycaster>();

        // Score text
        var scoreTxt = CreateTMP(canvasGo, "ScoreText", "점수: 0", new Vector2(120, -40), new Vector2(240, 50), new Vector2(0,1), new Vector2(0,1), 28);
        var coinTxt  = CreateTMP(canvasGo, "CoinText",  "코인: 0",  new Vector2(120, -100), new Vector2(240, 50), new Vector2(0,1), new Vector2(0,1), 28);
        var purTxt   = CreateTMP(canvasGo, "PurificationText", "정화율: 0%", new Vector2(120, -160), new Vector2(240, 50), new Vector2(0,1), new Vector2(0,1), 28);

        // Feedback text (중앙 상단)
        var feedbackGo = CreateTMP(canvasGo, "FeedbackText", "", new Vector2(0, -60), new Vector2(600, 70), new Vector2(0.5f,1), new Vector2(0.5f,1), 32);
        feedbackGo.color = Color.yellow;
        feedbackGo.gameObject.SetActive(false);

        // 가방 슬롯 패널 (하단 중앙)
        var bagPanel = CreatePanel(canvasGo, "BagPanel", new Vector2(0, 10), new Vector2(660, 80), new Vector2(0.5f,0), new Vector2(0.5f,0));
        var layout = bagPanel.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 4;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        var bagSlotUIList = new BagSlotUI[12];
        for (int i = 0; i < 12; i++)
        {
            var slotGo = new GameObject($"BagSlot_{i}");
            slotGo.transform.SetParent(bagPanel.transform, false);
            var slotRect = slotGo.AddComponent<RectTransform>();
            slotRect.sizeDelta = new Vector2(50, 50);

            var bg = slotGo.AddComponent<Image>();
            bg.color = new Color(0.15f, 0.15f, 0.15f, 0.85f);

            // 아이콘
            var iconGo = new GameObject("Icon");
            iconGo.transform.SetParent(slotGo.transform, false);
            var iconRect = iconGo.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.1f, 0.1f);
            iconRect.anchorMax = new Vector2(0.9f, 0.9f);
            iconRect.offsetMin = iconRect.offsetMax = Vector2.zero;
            var iconImg = iconGo.AddComponent<Image>();
            iconImg.preserveAspect = true;
            iconGo.SetActive(false);

            // 하이라이트
            var hlGo = new GameObject("Highlight");
            hlGo.transform.SetParent(slotGo.transform, false);
            var hlRect = hlGo.AddComponent<RectTransform>();
            hlRect.anchorMin = Vector2.zero;
            hlRect.anchorMax = Vector2.one;
            hlRect.offsetMin = hlRect.offsetMax = Vector2.zero;
            var hlImg = hlGo.AddComponent<Image>();
            hlImg.color = new Color(1f, 0.9f, 0.1f, 0.4f);
            hlImg.enabled = false;

            var btn = slotGo.AddComponent<Button>();
            var slotUI = slotGo.AddComponent<BagSlotUI>();
            bagSlotUIList[i] = slotUI;

            var soSlot = new SerializedObject(slotUI);
            soSlot.FindProperty("iconImage").objectReferenceValue = iconImg;
            soSlot.FindProperty("selectionHighlight").objectReferenceValue = hlImg;
            soSlot.FindProperty("slotButton").objectReferenceValue = btn;
            soSlot.ApplyModifiedProperties();

            if (i >= 5) slotGo.SetActive(false);
        }

        // FeedbackUI
        var feedbackUI = canvasGo.AddComponent<FeedbackUI>();
        var soFeedback = new SerializedObject(feedbackUI);
        soFeedback.FindProperty("feedbackText").objectReferenceValue = feedbackGo;
        soFeedback.ApplyModifiedProperties();

        // HudController
        var hud = canvasGo.AddComponent<HudController>();
        var soHud = new SerializedObject(hud);
        soHud.FindProperty("scoreText").objectReferenceValue = scoreTxt;
        soHud.FindProperty("coinText").objectReferenceValue = coinTxt;
        soHud.FindProperty("purificationText").objectReferenceValue = purTxt;

        var bagInv = player.GetComponent<BagInventory>();
        soHud.FindProperty("bagInventory").objectReferenceValue = bagInv;

        // ZoneManager 배열 연결
        var zmArray = soHud.FindProperty("zoneManagers");
        zmArray.arraySize = 3;
        zmArray.GetArrayElementAtIndex(0).objectReferenceValue = cityZone;
        zmArray.GetArrayElementAtIndex(1).objectReferenceValue = riverZone;
        zmArray.GetArrayElementAtIndex(2).objectReferenceValue = beachZone;

        // BagSlots 배열
        var slotsArray = soHud.FindProperty("bagSlots");
        slotsArray.arraySize = 12;
        for (int i = 0; i < 12; i++)
            slotsArray.GetArrayElementAtIndex(i).objectReferenceValue = bagSlotUIList[i];

        soHud.ApplyModifiedProperties();

        // PlayerInteractor에 FeedbackUI 연결
        var interactor = player.GetComponent<PlayerInteractor>();
        if (interactor != null)
        {
            var soInteractor = new SerializedObject(interactor);
            soInteractor.FindProperty("feedbackUI").objectReferenceValue = feedbackUI;
            soInteractor.ApplyModifiedProperties();
        }

        Debug.Log("[Setup] HUD 생성 완료");
    }

    // ─────────────────────────────────────────────
    // 13. 결과화면 Canvas
    // ─────────────────────────────────────────────
    static void CreateResultScreen()
    {
        var canvasGo = new GameObject("Result_Canvas");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 20;
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGo.AddComponent<GraphicRaycaster>();

        // 어두운 패널
        var panel = CreatePanel(canvasGo, "ResultPanel", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.one);
        panel.GetComponent<Image>().color = new Color(0, 0, 0, 0.85f);

        // 텍스트
        var titleTmp  = CreateTMP(panel, "TitleText",         "게임 클리어!", new Vector2(0, 200), new Vector2(600, 80), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), 48);
        titleTmp.color = Color.yellow;
        var scoreTmp  = CreateTMP(panel, "FinalScoreText",    "최종 점수: 0", new Vector2(0, 100), new Vector2(500, 60), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), 36);
        var purTmp    = CreateTMP(panel, "PurificationText",  "전체 정화율: 0%", new Vector2(0, 30), new Vector2(500, 60), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), 36);

        // 별 3개
        var starsFilled = LoadSprite("Assets/Sprites/UI/ui_star_filled_01.png");
        var starsEmpty  = LoadSprite("Assets/Sprites/UI/ui_star_empty_01.png");
        var starImgList = new Image[3];
        for (int i = 0; i < 3; i++)
        {
            var starGo = new GameObject($"Star_{i}");
            starGo.transform.SetParent(panel.transform, false);
            var starRect = starGo.AddComponent<RectTransform>();
            starRect.anchorMin = new Vector2(0.5f, 0.5f);
            starRect.anchorMax = new Vector2(0.5f, 0.5f);
            starRect.pivot = new Vector2(0.5f, 0.5f);
            starRect.anchoredPosition = new Vector2((i - 1) * 80f, -60f);
            starRect.sizeDelta = new Vector2(64, 64);
            var starImg = starGo.AddComponent<Image>();
            starImg.sprite = starsEmpty;
            starImg.preserveAspect = true;
            starImgList[i] = starImg;
        }

        // 다시하기 버튼
        var btnPanel = CreatePanel(panel, "RetryButton", new Vector2(0, -160), new Vector2(220, 70), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
        btnPanel.GetComponent<Image>().color = new Color(0.2f, 0.7f, 0.2f);
        btnPanel.AddComponent<Button>();
        CreateTMP(btnPanel, "RetryLabel", "다시 하기", Vector2.zero, new Vector2(220, 70), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), 30);

        panel.SetActive(false);

        // ResultScreen 컴포넌트
        var rs = canvasGo.AddComponent<ResultScreen>();
        var soRs = new SerializedObject(rs);
        soRs.FindProperty("resultPanel").objectReferenceValue = panel;
        soRs.FindProperty("finalScoreText").objectReferenceValue = scoreTmp;
        soRs.FindProperty("purificationRateText").objectReferenceValue = purTmp;
        soRs.FindProperty("starFilled").objectReferenceValue = starsFilled;
        soRs.FindProperty("starEmpty").objectReferenceValue  = starsEmpty;
        soRs.FindProperty("retryButton").objectReferenceValue = btnPanel.GetComponent<Button>();

        var starsArr = soRs.FindProperty("starImages");
        starsArr.arraySize = 3;
        for (int i = 0; i < 3; i++)
            starsArr.GetArrayElementAtIndex(i).objectReferenceValue = starImgList[i];
        soRs.ApplyModifiedProperties();

        // GameManager에 ResultScreen 연결
        var gm = Object.FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            var soGm = new SerializedObject(gm);
            soGm.FindProperty("resultScreen").objectReferenceValue = rs;
            soGm.ApplyModifiedProperties();
        }

        // UpgradeShop 들에 참조 연결
        var shops = Object.FindObjectsByType<UpgradeShop>(FindObjectsSortMode.None);
        var player = GameObject.FindWithTag("Player");
        var bagInv  = player != null ? player.GetComponent<BagInventory>() : null;
        var pc      = player != null ? player.GetComponent<PlayerController>() : null;
        var interactor = player != null ? player.GetComponent<PlayerInteractor>() : null;
        var feedbackUI = Object.FindFirstObjectByType<FeedbackUI>();

        foreach (var shop in shops)
        {
            var soShop = new SerializedObject(shop);
            soShop.FindProperty("playerController").objectReferenceValue = pc;
            soShop.FindProperty("playerInteractor").objectReferenceValue = interactor;
            soShop.FindProperty("bagInventory").objectReferenceValue = bagInv;
            soShop.FindProperty("feedbackUI").objectReferenceValue = feedbackUI;
            soShop.ApplyModifiedProperties();
        }

        Debug.Log("[Setup] 결과화면 생성 완료");
    }

    // ─────────────────────────────────────────────
    // 씬 저장
    // ─────────────────────────────────────────────
    static void SaveScene()
    {
        string scenePath = "Assets/Scenes/MainScene.unity";
        Directory.CreateDirectory("Assets/Scenes");
        EditorSceneManager.SaveScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene(), scenePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[Setup] 씬 저장: " + scenePath);
    }

    // ─────────────────────────────────────────────
    // 유틸리티
    // ─────────────────────────────────────────────
    static Sprite LoadSprite(string path)
    {
        var spr = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (spr == null) Debug.LogWarning("[Setup] 스프라이트 없음: " + path);
        return spr;
    }

    static TextMeshProUGUI CreateTMP(GameObject parent, string name, string text,
        Vector2 anchoredPos, Vector2 size, Vector2 anchorMin, Vector2 anchorMax, int fontSize)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = size;
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableWordWrapping = false;
        return tmp;
    }

    static GameObject CreatePanel(GameObject parent, string name,
        Vector2 anchoredPos, Vector2 size, Vector2 anchorMin, Vector2 anchorMax)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = size;
        go.AddComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f, 0.7f);
        return go;
    }
}
#endif
