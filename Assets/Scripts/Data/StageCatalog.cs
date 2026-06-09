using UnityEngine;

namespace CleanWave.Data
{
    [CreateAssetMenu(fileName = "StageCatalog", menuName = "CleanWave/Stage Catalog")]
    public class StageCatalog : ScriptableObject
    {
        [System.Serializable]
        public class StageEntry
        {
            public string StageId;
            public string SceneName;
            public string DisplayName;
            [TextArea] public string EducationTopic;
        }

        public StageEntry[] Stages;
    }
}
