using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class StageDataDownloader : MonoBehaviour
{
    private StageSO stageSOData;

    private const string URL_StageDataSheet = "https://docs.google.com/spreadsheets/d/1tgEgtsQp0vTR3rbdCwYv_y4s_4LdsDHd-6RXWyvgk0Y/export?format=tsv&gid=960013426&range=A1:H11";

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

        return newSO;
    }

    private void ApplyStageDataToSO(JArray jsonData)
    {
        for (int i = 0; i < jsonData.Count; i++)
        {
            JObject row = (JObject)jsonData[i];

            int stageNumber = int.TryParse(row["Stage"]?.ToString(), out int number) ? number : 0;
            int stageWave = int.TryParse(row["Wave"]?.ToString(), out int wave) ? wave : 0;

            int length = 0;//row["MonsterIDs"]?.ToString() ?? "";??
            string[] monsterIDs = new string[length];
            for (int j = 0; j < monsterIDs.Length; j++)
            {
                monsterIDs[j] = "DL_001";
            }

            int monsterCount = int.TryParse(row["MonsterCount"]?.ToString(), out int count) ? count : 0;
            float spawnInterval = float.TryParse(row["SpawnInterval"]?.ToString(), out float interval) ? interval : 0;

            StageSO stageData = new StageSO();

          //  stageData = CreateNewSOData<UnitSO>(unitID);

         //   stageData.SetData(unitID, unitName, unitHP, unitAtk, unitAtkRange, unitAtkDelay, unitSummonCost, unitCoolDown, unitAtkType);
            EditorUtility.SetDirty(stageData);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

#endif
}