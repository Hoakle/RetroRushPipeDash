using System.IO;
using System.Linq;
using RetroRush.Config;
using RetroRush.Game.Gameplay;
using UnityEditor;
using UnityEngine;

namespace RetroRush
{
    public class CSVImporter
    {
        private static string PATH_STAGE = "/Scripts/Editor/CSVs/StageConfigData.csv";
        private static string PATH_FACES = "/Scripts/Editor/CSVs/FacesConfigData";
        
        [MenuItem("ConfigImporter/LoadFromCSV")]
        public static void LoadFromCsv()
        {
            string[] allLines = File.ReadAllLines(Application.dataPath + PATH_STAGE);
            for(var i = 1; i < allLines.Length; i++)
            {
                string[] lineData = allLines[i].Split(',');

                StageConfigData stage = ScriptableObject.CreateInstance<StageConfigData>();
                stage.Id = int.Parse(lineData[0]);
                stage.StageDepth = int.Parse(lineData[1]);
                stage.PipeFaceConfigs = lineData[2].Split('|').Select((s) => (PipeFaceType)int.Parse(s.Trim())).ToList();

                AssetDatabase.CreateAsset(stage, $"Assets/Data/StageConfig_{stage.Id}.asset");
            }
            
            AssetDatabase.SaveAssets();
        }
    }
}
