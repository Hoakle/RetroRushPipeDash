using System.IO;
using System.Linq;
using RetroRush.Config;
using UnityEditor;
using UnityEngine;

namespace RetroRush
{
    public class CSVImporter
    {
        private static string PATH_MAIN_CONFIG = "Assets/Data/Config/";
        private static string PATH_STAGE_CONFIG = "Assets/Data/Config/Level/";
        private static string PATH_STAGE_CSV = "/Scripts/Editor/CSVs/StageConfigData.csv";
        
        [MenuItem("ConfigImporter/LoadFromCSV")]
        public static void LoadFromCsv()
        {
            LevelConfigData levelConfigData = FindOrCreate<LevelConfigData>(PATH_MAIN_CONFIG, "LevelConfigData");
            levelConfigData.StageConfigs.Clear();
            
            string[] allLines = File.ReadAllLines(Application.dataPath + PATH_STAGE_CSV);
            for(var i = 1; i < allLines.Length; i++)
            {
                string[] lineData = allLines[i].Split(',');

                StageConfigData stage = FindOrCreate<StageConfigData>(PATH_STAGE_CONFIG,$"StageConfig_{lineData[0]}");
                stage.Id = int.Parse(lineData[0]);
                stage.StageDepth = int.Parse(lineData[1]);
                stage.PipeFaceConfigs = lineData[2].Split('|').Select((s) => (PipeFaceType)int.Parse(s.Trim())).ToList();
                stage.NbCoin = stage.PipeFaceConfigs.FindAll(f => f == PipeFaceType.COIN).Count;
                EditorUtility.SetDirty(stage);
                
                levelConfigData.StageConfigs.Add(stage);
            }
            
            EditorUtility.SetDirty(levelConfigData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        private static TScriptableObject FindOrCreate<TScriptableObject>(string path, string name) where TScriptableObject : ScriptableObject
        {
            TScriptableObject scriptableObject;
            var UID = AssetDatabase.FindAssets(name, new string[] { path }).FirstOrDefault();
            if (UID != null)
            {
                Debug.LogError("UID: " + UID);
                scriptableObject = AssetDatabase.LoadAssetAtPath<TScriptableObject>(AssetDatabase.GUIDToAssetPath(UID));
            }
            else
            {
                scriptableObject = ScriptableObject.CreateInstance<TScriptableObject>();
                AssetDatabase.CreateAsset(scriptableObject, $"{path}{name}.asset");
            }

            Debug.LogError(scriptableObject);
            return scriptableObject;
        }
    }
}
