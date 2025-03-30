using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Networking;

public class StageDataDownloader : MonoBehaviour
{
    private StageSO stageSOData;

    private const string URL_StageDataSheet = "https://docs.google.com/spreadsheets/d/1tgEgtsQp0vTR3rbdCwYv_y4s_4LdsDHd-6RXWyvgk0Y/export?format=tsv&gid=566182702&range=A1:E61";

    private const string stageSODataFolderPath = "Assets/01_Scripts/ScriptableObjects/Stage/Data";
    
    private DataManager dataManager;

#if UNITY_EDITOR
    
    public void StartDownload()
    {
        dataManager = FindObjectOfType<DataManager>();

        if (dataManager == null)
        {
            Debug.LogError("there is no DataManager in the scene.");
            return;
        }

        StartCoroutine(DownloadData());
    }
    private string ConvertTSVToJson(string tsv)
    {
        string[] lines = tsv.Split('\n');
        if (lines.Length < 2) return "[]";

        string[] headers = lines[0].Split('\t');
        JArray jsonArray = new JArray();

        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Split('\t');
            JObject jsonObject = new JObject();

            for (int j = 0; j < headers.Length && j < values.Length; j++)
            {
                string cleanValue = values[j].Trim();
                jsonObject[headers[j].Trim()] = cleanValue;
            }

            jsonArray.Add(jsonObject);
        }

        return jsonArray.ToString();
    }

    IEnumerator DownloadData()
    {
        UnityWebRequest www = UnityWebRequest.Get(URL_StageDataSheet);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string tsvText = www.downloadHandler.text;
            string json = ConvertTSVToJson(tsvText);
            JArray jsonData = JArray.Parse(json);

            ClearSOData();

            ApplyStageDataToSO(jsonData);
        }

        dataManager.SetStageDatas(stageSOData);
    }

    private void ClearSOData()
    {
        if (!Directory.Exists(stageSODataFolderPath))
        {
            Debug.LogWarning("SO not found");
            return;
        }

        string[] files = Directory.GetFiles(stageSODataFolderPath, "*.asset");

        foreach (string file in files)
        {
            AssetDatabase.DeleteAsset(file);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private T CreateNewSOData<T>(string fileName) where T : ScriptableObject
    {
        T newSO = ScriptableObject.CreateInstance<T>();

        if (!Directory.Exists(stageSODataFolderPath))
        {
            Directory.CreateDirectory(stageSODataFolderPath);
        }

        string assetPath = $"{stageSODataFolderPath}/{fileName}.asset";
        AssetDatabase.CreateAsset(newSO, assetPath);
        EditorUtility.SetDirty(newSO);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return newSO;
    }

    private void ApplyStageDataToSO(JArray jsonData)
    {
        Dictionary<int, StageSO> stageDictionary = new Dictionary<int, StageSO>();

        foreach (JObject row in jsonData)
        {
            int stageNumber = int.TryParse(row["Stage"]?.ToString(), out int number) ? number : 0;
            int waveNumber = int.TryParse(row["Wave"]?.ToString(), out int wave) ? wave : 0;

            string[] monsterIDs = row["MonsterIDs"]?.ToString().Split(',') ?? new string[0];
            for (int i = 0; i < monsterIDs.Length; i++)
            {
                monsterIDs[i] = monsterIDs[i].Trim();
            }
            
            int[] monsterCounts = row["MonsterCount"]?.ToString().Split(',').Select(int.Parse).ToArray() ?? new int[0];
            float spawnInterval = float.TryParse(row["SpawnInterval"]?.ToString(), out float interval) ? interval : 0;

            if (!stageDictionary.TryGetValue(stageNumber, out StageSO stageData))
            {
                stageData = CreateNewSOData<StageSO>($"Stage_{stageNumber}");
                stageDictionary[stageNumber] = stageData;

                stageData.SetData(stageNumber, new List<WaveData>());
            }

            stageData.Waves.Add(new WaveData
            {
                waveNumber = waveNumber,
                monsterIDs = monsterIDs,
                monsterCounts = monsterCounts,
                spawnInterval = spawnInterval
            });

            EditorUtility.SetDirty(stageData);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

#endif
}