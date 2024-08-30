using System.IO;
using System.Linq;
using HoakleEngine.Core.Localization;
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
        
        private static string PATH_LOCALIZATION_CONFIG = "Assets/Data/Localization/";
        private static string PATH_LOCALIZATION_CSV = "/Scripts/Editor/CSVs/Localization.tsv";
        
        [MenuItem("ConfigImporter/LoadFromCSV")]
        public static void LoadFromCsv()
        {
            LoadStageConfig();
            LoadLocalization();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void LoadLocalization()
        {
            LocalizationDataBase localizationDataBase = FindOrCreate<LocalizationDataBase>(PATH_LOCALIZATION_CONFIG, "LocalizationDataBase");
            localizationDataBase._Language.Clear();
            localizationDataBase.Keys.Clear();
            
            string[] allLines = File.ReadAllLines(Application.dataPath + PATH_LOCALIZATION_CSV);
            foreach (var language in allLines[0].Split("\t".ToCharArray()))
            {
                if(language == "Key")
                    continue;

                LanguageData languageData = FindOrCreate<LanguageData>(PATH_LOCALIZATION_CONFIG, language);
                languageData.Translations.Clear();
                localizationDataBase._Language.Add(languageData);
                EditorUtility.SetDirty(languageData);
            }
            
            for(var i = 1; i < allLines.Length; i++)
            {
                string[] lineData = allLines[i].Split("\t".ToCharArray());

                localizationDataBase.Keys.Add(lineData[0]);
                for (var j = 1; j < lineData.Length; j++)
                {
                    localizationDataBase._Language[j - 1].Translations.Add(lineData[j]);
                }
            }
            
            EditorUtility.SetDirty(localizationDataBase);
        }
        
        private static void LoadStageConfig()
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
        }
        
        private static TScriptableObject FindOrCreate<TScriptableObject>(string path, string name) where TScriptableObject : ScriptableObject
        {
            TScriptableObject scriptableObject;
            var UID = AssetDatabase.FindAssets(name, new string[] { path }).FirstOrDefault();
            if (UID != null)
            {
                scriptableObject = AssetDatabase.LoadAssetAtPath<TScriptableObject>(AssetDatabase.GUIDToAssetPath(UID));
            }
            else
            {
                scriptableObject = ScriptableObject.CreateInstance<TScriptableObject>();
                AssetDatabase.CreateAsset(scriptableObject, $"{path}{name}.asset");
            }
            
            return scriptableObject;
        }
    }
}
