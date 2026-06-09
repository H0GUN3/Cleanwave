using UnityEditor;
using UnityEngine;

namespace CleanWave.EditorTools
{
    [InitializeOnLoad]
    internal static class CleanWaveEditorGuard
    {
        private const string TestRunnerPath = "Assets/Editor/PlayModeTestRunner.cs";
        private const string TestRunnerMetaPath = "Assets/Editor/PlayModeTestRunner.cs.meta";
        private const double CheckIntervalSeconds = 0.5d;
        private static double _lastCheckTime;

        static CleanWaveEditorGuard()
        {
            EditorApplication.update += OnEditorUpdate;
            EditorApplication.playModeStateChanged += _ => ForceSanitize();
            RemoveInjectedPlayModeTestRunner();
        }

        private static void OnEditorUpdate()
        {
            if (EditorApplication.timeSinceStartup - _lastCheckTime < CheckIntervalSeconds)
            {
                return;
            }

            _lastCheckTime = EditorApplication.timeSinceStartup;
            ForceSanitize();
        }

        private static void ForceSanitize()
        {
            // Clear stale SessionState used by injected runner logic.
            SessionState.EraseString("PlayModeTest.State");
            SessionState.EraseString("PlayModeTest.Result");
            SessionState.EraseString("PlayModeTest.ScriptPath");
            RemoveInjectedPlayModeTestRunner();
        }

        private static void RemoveInjectedPlayModeTestRunner()
        {
            bool removedAny = false;

            if (AssetDatabase.AssetPathExists(TestRunnerPath))
            {
                removedAny |= AssetDatabase.DeleteAsset(TestRunnerPath);
            }

            if (AssetDatabase.AssetPathExists(TestRunnerMetaPath))
            {
                removedAny |= AssetDatabase.DeleteAsset(TestRunnerMetaPath);
            }

            if (removedAny)
            {
                Debug.Log("[CleanWaveEditorGuard] Removed injected PlayModeTestRunner.");
            }
        }
    }
}
