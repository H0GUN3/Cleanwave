#if UNITY_EDITOR
using System.Threading.Tasks;
using MCPForUnity.Editor.Services;
using MCPForUnity.Editor.Services.Transport;
using UnityEditor;
using UnityEngine;

namespace CleanWave.Editor
{
    [InitializeOnLoad]
    public static class CleanWaveMcpAutoConnect
    {
        private const string UseHttpTransportKey = "MCPForUnity.UseHttpTransport";
        private const string AutoStartOnLoadKey = "MCPForUnity.AutoStartOnLoad";
        private const string HttpScopeKey = "MCPForUnity.HttpTransportScope";
        private const string HttpUrlKey = "MCPForUnity.HttpUrl";
        private const string SessionLockKey = "CleanWave.McpAutoConnect.SessionInit";

        static CleanWaveMcpAutoConnect()
        {
            if (EditorPrefs.GetBool(SessionLockKey, false))
            {
                return;
            }

            EditorPrefs.SetBool(SessionLockKey, true);
            EditorApplication.delayCall += () => _ = EnsureConnectedAsync();
        }

        private static async Task EnsureConnectedAsync()
        {
            try
            {
                EditorPrefs.SetBool(UseHttpTransportKey, true);
                EditorPrefs.SetBool(AutoStartOnLoadKey, true);
                EditorPrefs.SetString(HttpScopeKey, "local");
                EditorPrefs.SetString(HttpUrlKey, "http://127.0.0.1:8080");

                var started = await MCPServiceLocator.TransportManager.StartAsync(TransportMode.Http);
                Debug.Log(started
                    ? "[CleanWave] MCP HTTP 브리지 연결 시도 성공"
                    : "[CleanWave] MCP HTTP 브리지 연결 시도 실패");
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("[CleanWave] MCP 자동 연결 예외: " + ex.Message);
            }
        }
    }
}
#endif
