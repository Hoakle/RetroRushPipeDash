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
        private static string PATH_CHUNK = "/Scripts/Editor/CSVs/ChunkConfigData.csv";
        private static string PATH_FACES = "/Scripts/Editor/CSVs/FacesConfigData";
        
        [MenuItem("ConfigImporter/LoadFromCSV")]
        public static void LoadFromCsv()
        {
            string[] allLines = File.ReadAllLines(Application.dataPath + PATH_CHUNK);
            foreach (var line in allLines)
            {
                string[] lineData = line.Split(',');

                ChunkConfigData chunk = ScriptableObject.CreateInstance<ChunkConfigData>();
                chunk.Id = int.Parse(lineData[0]);
                chunk.Weight = int.Parse(lineData[1]);
                chunk.ChunkDepth = int.Parse(lineData[2]);
                //chunk.IncompatibleNextChunk = lineData[3].Split('|').Select(int.Parse).ToList();

                string[] faceLines = File.ReadAllLines(Application.dataPath + PATH_FACES + "_" + chunk.Id + ".csv");
                foreach (var face in faceLines)
                {
                    string[] faceDataLine = face.Split(',');
                    PipeFaceConfigData faceData = new PipeFaceConfigData();
                    faceData.Depth = int.Parse(faceDataLine[0]);
                    faceData.FaceIndex = int.Parse(faceDataLine[1]);
                    faceData.PickableType = (PickableType) int.Parse(faceDataLine[2]);
                    faceData.Exist = int.Parse(faceDataLine[3]) == 1;
                    chunk.PipeFaceConfigs.Add(faceData);
                }
                
                AssetDatabase.CreateAsset(chunk, $"Assets/Data/ChunkConfig_{chunk.Id}.asset");
            }
            
            AssetDatabase.SaveAssets();
        }
    }
}
