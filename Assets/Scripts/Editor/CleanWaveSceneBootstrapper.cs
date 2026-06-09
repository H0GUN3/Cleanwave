#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace CleanWave.Editor
{
    public static class CleanWaveSceneBootstrapper
    {
        private static readonly string[] SceneNames =
        {
            "MainMenu",
            "Tutorial",
            "Stage1_City",
            "Stage2_River",
            "Stage3_Beach",
            "Result"
        };

        [MenuItem("CleanWave/Generate Base Scenes")]
        public static void GenerateBaseScenes()
        {
            const string sceneFolder = "Assets/Scenes";
            if (!AssetDatabase.IsValidFolder(sceneFolder))
            {
                Directory.CreateDirectory(sceneFolder);
                AssetDatabase.Refresh();
            }

            foreach (var sceneName in SceneNames)
            {
                var path = $"{sceneFolder}/{sceneName}.unity";
                if (File.Exists(path))
                {
                    continue;
                }

                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                CreateCameraAndLight(sceneName);
                EditorSceneManager.SaveScene(scene, path);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("CleanWave base scenes generated.");
        }

        private static void CreateCameraAndLight(string sceneName)
        {
            var cameraObject = new GameObject("Main Camera");
            var camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            cameraObject.tag = "MainCamera";

            var lightObject = new GameObject("Directional Light");
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;

            _ = new GameObject(sceneName + "_Root");
        }
    }
}
#endif
