#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;

namespace CleanWave.Editor
{
    public class CleanWaveSetup : EditorWindow
    {
        [MenuItem("CleanWave/씬 자동 설정")]
        public static void ShowWindow()
        {
            GetWindow<CleanWaveSetup>("CleanWave Setup");
        }

        private void OnGUI()
        {
            GUILayout.Label("CleanWave 씬 설정 도우미", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (GUILayout.Button("게임 오브젝트 기초 생성"))
                CreateBaseObjects();

            if (GUILayout.Button("HUD Canvas 생성"))
                CreateHUDCanvas();

            if (GUILayout.Button("결과화면 Canvas 생성"))
                CreateResultCanvas();

            if (GUILayout.Button("더미 플레이어 생성"))
                CreateDummyPlayer();

            EditorGUILayout.HelpBox(
                "버튼을 순서대로 실행하세요.\n각 오브젝트는 Hierarchy에 생성됩니다.\n Inspector에서 SerializeField 필드를 연결해야 합니다.",
                MessageType.Info);
        }

        private static void CreateBaseObjects()
        {
            // GameManager
            if (FindFirstObjectByType<GameManager>() == null)
            {
                var go = new GameObject("GameManager");
                go.AddComponent<GameManager>();
                go.AddComponent<ScoreManager>();
                Debug.Log("[Setup] GameManager + ScoreManager 생성됨");
            }

            // Camera
            var cam = Camera.main;
            if (cam != null && cam.GetComponent<CameraFollow>() == null)
            {
                cam.GetComponent<Camera>().orthographicSize = 5f;
                cam.gameObject.AddComponent<CameraFollow>();
                Debug.Log("[Setup] CameraFollow 카메라에 추가됨");
            }

            // Grid + Tilemap (3 layers)
            if (FindFirstObjectByType<Grid>() == null)
            {
                var gridGo = new GameObject("Grid");
                var grid = gridGo.AddComponent<Grid>();
                grid.cellSize = new Vector3(1f, 1f, 0f);

                CreateTilemapLayer(gridGo, "Ground", -1);
                CreateTilemapLayer(gridGo, "Decoration", 0);
                var wallLayer = CreateTilemapLayer(gridGo, "Wall", 1);
                wallLayer.gameObject.AddComponent<TilemapCollider2D>();
                var cr = wallLayer.gameObject.AddComponent<CompositeCollider2D>();
                cr.geometryType = CompositeCollider2D.GeometryType.Polygons;
                var wallRb = wallLayer.gameObject.GetComponent<Rigidbody2D>();
                wallRb.bodyType = RigidbodyType2D.Static;
                Debug.Log("[Setup] Grid + Tilemap 레이어 생성됨 (Ground, Decoration, Wall)");
            }

            // Zone Managers
            CreateZoneManagerObject(ZoneType.City);
            CreateZoneManagerObject(ZoneType.River);
            CreateZoneManagerObject(ZoneType.Beach);

            Debug.Log("[Setup] 기초 오브젝트 생성 완료!");
        }

        private static Tilemap CreateTilemapLayer(GameObject parent, string layerName, int sortOrder)
        {
            var go = new GameObject(layerName);
            go.transform.SetParent(parent.transform);
            var tm = go.AddComponent<Tilemap>();
            var tr = go.AddComponent<TilemapRenderer>();
            tr.sortingLayerName = "Default";
            tr.sortingOrder = sortOrder;
            return tm;
        }

        private static void CreateZoneManagerObject(ZoneType zoneType)
        {
            string name = $"ZoneManager_{zoneType}";
            if (GameObject.Find(name) != null) return;
            var go = new GameObject(name);
            var zm = go.AddComponent<ZoneManager>();
            Debug.Log($"[Setup] {name} 생성됨 — Inspector에서 ZoneType={zoneType} 설정 필요");
        }

        private static void CreateHUDCanvas()
        {
            if (GameObject.Find("HUD_Canvas") != null)
            {
                Debug.Log("[Setup] HUD_Canvas가 이미 존재합니다.");
                return;
            }

            var canvasGo = new GameObject("HUD_Canvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();

            // Score
            CreateTextElement(canvasGo, "ScoreText", "점수: 0", new Vector2(10, -10), new Vector2(200, 40), new Vector2(0, 1));
            // Coin
            CreateTextElement(canvasGo, "CoinText", "코인: 0", new Vector2(10, -55), new Vector2(200, 40), new Vector2(0, 1));
            // Purification
            CreateTextElement(canvasGo, "PurificationText", "정화율: 0%", new Vector2(10, -100), new Vector2(200, 40), new Vector2(0, 1));

            // Feedback
            var feedbackGo = CreateTextElement(canvasGo, "FeedbackText", "", new Vector2(0, -30), new Vector2(400, 60), new Vector2(0.5f, 1));
            feedbackGo.SetActive(false);
            var feedbackComp = canvasGo.AddComponent<FeedbackUI>();

            // Bag slots
            var bagPanel = new GameObject("BagPanel");
            bagPanel.transform.SetParent(canvasGo.transform, false);
            var bagRect = bagPanel.AddComponent<RectTransform>();
            bagRect.anchorMin = new Vector2(0.5f, 0);
            bagRect.anchorMax = new Vector2(0.5f, 0);
            bagRect.pivot = new Vector2(0.5f, 0);
            bagRect.anchoredPosition = new Vector2(0, 10);
            bagRect.sizeDelta = new Vector2(600, 70);
            var layout = bagPanel.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 5;
            layout.childAlignment = TextAnchor.MiddleCenter;

            for (int i = 0; i < 12; i++)
            {
                var slot = new GameObject($"BagSlot_{i}");
                slot.transform.SetParent(bagPanel.transform, false);
                var slotRect = slot.AddComponent<RectTransform>();
                slotRect.sizeDelta = new Vector2(48, 48);
                var img = slot.AddComponent<Image>();
                img.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
                slot.AddComponent<BagSlotUI>();
                slot.AddComponent<Button>();
                if (i >= 5) slot.SetActive(false);
            }

            // HudController
            canvasGo.AddComponent<HudController>();

            Debug.Log("[Setup] HUD Canvas 생성 완료 — Inspector에서 필드 연결 필요!");
        }

        private static void CreateResultCanvas()
        {
            if (GameObject.Find("Result_Canvas") != null)
            {
                Debug.Log("[Setup] Result_Canvas가 이미 존재합니다.");
                return;
            }

            var canvasGo = new GameObject("Result_Canvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 20;
            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();

            var panel = new GameObject("ResultPanel");
            panel.transform.SetParent(canvasGo.transform, false);
            var panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            var panelImg = panel.AddComponent<Image>();
            panelImg.color = new Color(0, 0, 0, 0.8f);

            CreateTextElement(panel, "FinalScoreText", "최종 점수: 0", new Vector2(0, 60), new Vector2(400, 60), new Vector2(0.5f, 0.5f));
            CreateTextElement(panel, "PurificationRateText", "전체 정화율: 0%", new Vector2(0, -10), new Vector2(400, 60), new Vector2(0.5f, 0.5f));

            // Retry Button
            var btnGo = new GameObject("RetryButton");
            btnGo.transform.SetParent(panel.transform, false);
            var btnRect = btnGo.AddComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(0.5f, 0.5f);
            btnRect.anchorMax = new Vector2(0.5f, 0.5f);
            btnRect.pivot = new Vector2(0.5f, 0.5f);
            btnRect.anchoredPosition = new Vector2(0, -80);
            btnRect.sizeDelta = new Vector2(200, 60);
            var btnImg = btnGo.AddComponent<Image>();
            btnImg.color = new Color(0.2f, 0.6f, 0.2f, 1f);
            btnGo.AddComponent<Button>();
            CreateTextElement(btnGo, "RetryLabel", "다시 하기", Vector2.zero, new Vector2(200, 60), new Vector2(0.5f, 0.5f));

            var resultScreen = canvasGo.AddComponent<ResultScreen>();
            panel.SetActive(false);
            Debug.Log("[Setup] Result Canvas 생성 완료 — Inspector에서 필드 연결 필요!");
        }

        private static void CreateDummyPlayer()
        {
            if (GameObject.Find("Player") != null)
            {
                Debug.Log("[Setup] Player가 이미 존재합니다.");
                return;
            }

            var playerGo = new GameObject("Player");
            playerGo.tag = "Player";

            var sr = playerGo.AddComponent<SpriteRenderer>();
            sr.sprite = CreateColoredSprite(Color.cyan, 32, 48);
            sr.sortingOrder = 5;

            var rb = playerGo.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            var col = playerGo.AddComponent<CapsuleCollider2D>();
            col.size = new Vector2(0.5f, 0.8f);
            col.offset = new Vector2(0f, -0.1f);

            playerGo.AddComponent<PlayerController>();
            playerGo.AddComponent<PlayerInteractor>();
            playerGo.AddComponent<BagInventory>();

            // Link camera
            var camFollow = FindFirstObjectByType<CameraFollow>();
            if (camFollow != null)
                camFollow.SetTarget(playerGo.transform);

            Debug.Log("[Setup] Player 생성 완료 — Inspector에서 BagInventory, FeedbackUI, InteractLayer 연결 필요!");
        }

        private static GameObject CreateTextElement(GameObject parent, string name, string text, Vector2 anchoredPos, Vector2 size, Vector2 anchor)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent.transform, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = anchor;
            rect.anchoredPosition = anchoredPos;
            rect.sizeDelta = size;
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 20;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
            return go;
        }

        private static Sprite CreateColoredSprite(Color color, int width, int height)
        {
            var tex = new Texture2D(width, height);
            var pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
            tex.SetPixels(pixels);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 32f);
        }
    }
}
#endif
