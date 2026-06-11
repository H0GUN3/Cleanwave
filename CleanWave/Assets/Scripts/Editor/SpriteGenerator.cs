#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

namespace CleanWave.Editor
{
    public class SpriteGenerator : EditorWindow
    {
        [MenuItem("CleanWave/더미 스프라이트 생성")]
        public static void GenerateAllSprites()
        {
            string basePath = "Assets/Sprites";
            Directory.CreateDirectory(Application.dataPath + "/Sprites/Player");
            Directory.CreateDirectory(Application.dataPath + "/Sprites/Trash");
            Directory.CreateDirectory(Application.dataPath + "/Sprites/Bin");
            Directory.CreateDirectory(Application.dataPath + "/Sprites/UI");
            Directory.CreateDirectory(Application.dataPath + "/Sprites/Tiles");

            // Player sprites (32x48)
            GenerateSprite($"{basePath}/Player/chr_player_idle_down_01.png", 32, 48, new Color(0.4f, 0.6f, 1f));
            GenerateSprite($"{basePath}/Player/chr_player_idle_up_01.png", 32, 48, new Color(0.3f, 0.5f, 0.9f));
            GenerateSprite($"{basePath}/Player/chr_player_idle_side_01.png", 32, 48, new Color(0.35f, 0.55f, 0.95f));
            GenerateSprite($"{basePath}/Player/chr_player_walk_down_01.png", 32, 48, new Color(0.4f, 0.65f, 1f));
            GenerateSprite($"{basePath}/Player/chr_player_walk_down_02.png", 32, 48, new Color(0.4f, 0.62f, 0.98f));
            GenerateSprite($"{basePath}/Player/chr_player_walk_down_03.png", 32, 48, new Color(0.4f, 0.63f, 0.99f));
            GenerateSprite($"{basePath}/Player/chr_player_walk_down_04.png", 32, 48, new Color(0.4f, 0.64f, 1f));
            GenerateSprite($"{basePath}/Player/chr_player_walk_up_01.png", 32, 48, new Color(0.35f, 0.55f, 0.95f));
            GenerateSprite($"{basePath}/Player/chr_player_walk_up_02.png", 32, 48, new Color(0.33f, 0.53f, 0.93f));
            GenerateSprite($"{basePath}/Player/chr_player_walk_up_03.png", 32, 48, new Color(0.34f, 0.54f, 0.94f));
            GenerateSprite($"{basePath}/Player/chr_player_walk_up_04.png", 32, 48, new Color(0.36f, 0.56f, 0.96f));
            GenerateSprite($"{basePath}/Player/chr_player_walk_side_01.png", 32, 48, new Color(0.38f, 0.58f, 0.97f));
            GenerateSprite($"{basePath}/Player/chr_player_walk_side_02.png", 32, 48, new Color(0.37f, 0.57f, 0.96f));
            GenerateSprite($"{basePath}/Player/chr_player_walk_side_03.png", 32, 48, new Color(0.39f, 0.59f, 0.98f));
            GenerateSprite($"{basePath}/Player/chr_player_walk_side_04.png", 32, 48, new Color(0.41f, 0.61f, 1f));
            GenerateSprite($"{basePath}/Player/chr_player_pickup_down_01.png", 32, 48, new Color(0.4f, 0.7f, 0.4f));
            GenerateSprite($"{basePath}/Player/chr_player_pickup_down_02.png", 32, 48, new Color(0.4f, 0.72f, 0.4f));
            GenerateSprite($"{basePath}/Player/chr_player_pickup_down_03.png", 32, 48, new Color(0.4f, 0.68f, 0.4f));
            GenerateSprite($"{basePath}/Player/chr_player_pickup_side_01.png", 32, 48, new Color(0.5f, 0.7f, 0.4f));
            GenerateSprite($"{basePath}/Player/chr_player_pickup_side_02.png", 32, 48, new Color(0.5f, 0.72f, 0.4f));
            GenerateSprite($"{basePath}/Player/chr_player_pickup_side_03.png", 32, 48, new Color(0.5f, 0.68f, 0.4f));

            // Trash sprites (16x16)
            GenerateSprite($"{basePath}/Trash/trash_paper_01.png", 16, 16, new Color(1f, 1f, 0.6f));
            GenerateSprite($"{basePath}/Trash/trash_can_01.png", 16, 16, new Color(0.7f, 0.7f, 0.8f));
            GenerateSprite($"{basePath}/Trash/trash_plastic_01.png", 16, 16, new Color(0.3f, 0.8f, 1f));
            GenerateSprite($"{basePath}/Trash/trash_vinyl_01.png", 16, 16, new Color(0.9f, 0.5f, 0.9f));
            GenerateSprite($"{basePath}/Trash/trash_food_01.png", 16, 16, new Color(0.7f, 0.4f, 0.2f));
            GenerateSprite($"{basePath}/Trash/trash_net_01.png", 16, 16, new Color(0.5f, 0.8f, 0.5f));
            GenerateSprite($"{basePath}/Trash/trash_oil_01.png", 16, 16, new Color(0.1f, 0.1f, 0.1f));

            // Bin sprites (32x48)
            GenerateSprite($"{basePath}/Bin/bin_paper_01.png", 32, 48, new Color(0.9f, 0.9f, 0.1f));
            GenerateSprite($"{basePath}/Bin/bin_can_01.png", 32, 48, new Color(0.5f, 0.5f, 1f));
            GenerateSprite($"{basePath}/Bin/bin_plastic_01.png", 32, 48, new Color(0.3f, 0.9f, 0.9f));
            GenerateSprite($"{basePath}/Bin/bin_general_01.png", 32, 48, new Color(0.5f, 0.5f, 0.5f));
            GenerateSprite($"{basePath}/Bin/bin_special_01.png", 32, 48, new Color(0.9f, 0.3f, 0.3f));

            // Tiles (32x32)
            GenerateSprite($"{basePath}/Tiles/tile_city_ground_01.png", 32, 32, new Color(0.6f, 0.6f, 0.6f));
            GenerateSprite($"{basePath}/Tiles/tile_city_grass_01.png", 32, 32, new Color(0.3f, 0.7f, 0.3f));
            GenerateSprite($"{basePath}/Tiles/tile_city_road_01.png", 32, 32, new Color(0.4f, 0.4f, 0.45f));
            GenerateSprite($"{basePath}/Tiles/tile_city_wall_01.png", 32, 32, new Color(0.55f, 0.45f, 0.35f));
            GenerateSprite($"{basePath}/Tiles/tile_river_water_01.png", 32, 32, new Color(0.2f, 0.5f, 0.9f));
            GenerateSprite($"{basePath}/Tiles/tile_river_bank_01.png", 32, 32, new Color(0.6f, 0.8f, 0.4f));
            GenerateSprite($"{basePath}/Tiles/tile_beach_sand_01.png", 32, 32, new Color(0.95f, 0.9f, 0.6f));
            GenerateSprite($"{basePath}/Tiles/tile_beach_water_01.png", 32, 32, new Color(0.1f, 0.6f, 0.9f));
            GenerateSprite($"{basePath}/Tiles/tile_gate_01.png", 32, 32, new Color(0.8f, 0.3f, 0.1f));

            // UI sprites
            GenerateSprite($"{basePath}/UI/ui_star_filled_01.png", 32, 32, new Color(1f, 0.8f, 0f));
            GenerateSprite($"{basePath}/UI/ui_star_empty_01.png", 32, 32, new Color(0.4f, 0.4f, 0.4f));
            GenerateSprite($"{basePath}/UI/ui_coin_01.png", 16, 16, new Color(1f, 0.75f, 0f));
            GenerateSprite($"{basePath}/UI/ui_bag_01.png", 16, 16, new Color(0.7f, 0.5f, 0.3f));

            AssetDatabase.Refresh();
            Debug.Log("[SpriteGenerator] 더미 스프라이트 생성 완료! Assets/Sprites/ 확인하세요.");
        }

        private static void GenerateSprite(string assetPath, int width, int height, Color color)
        {
            string fullPath = Path.Combine(Application.dataPath.Replace("/Assets", "/"), assetPath);
            if (File.Exists(fullPath)) return;

            var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool isBorder = (x == 0 || x == width - 1 || y == 0 || y == height - 1);
                    tex.SetPixel(x, y, isBorder ? Color.black : color);
                }
            }
            tex.Apply();

            File.WriteAllBytes(fullPath, tex.EncodeToPNG());
            DestroyImmediate(tex);
        }
    }
}
#endif
