using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class CharacterDataDownloader : MonoBehaviour
{
    [SerializeField] private List<UnitSO> unitSOData = new List<UnitSO>();
    [SerializeField] private List<MonsterSO> monsterSOData = new List<MonsterSO>();

    private const string URL_UnitDataSheet = "https://docs.google.com/spreadsheets/d/1tgEgtsQp0vTR3rbdCwYv_y4s_4LdsDHd-6RXWyvgk0Y/export?format=tsv&range=A1:I10";
    private const string URL_MonsterDataSheet = "https://docs.google.com/spreadsheets/d/1tgEgtsQp0vTR3rbdCwYv_y4s_4LdsDHd-6RXWyvgk0Y/export?format=tsv&range=A12:H22";

    private const string unitSODataFolderPath = "Assets/01_Scripts/ScriptableObjects/Character/Unit/Data";
    private const string monsterSODataFolderPath = "Assets/01_Scripts/ScriptableObjects/Character/Monster/Data";

    private DataManager dataManager;

#if UNITY_EDITOR
    public void StartDownload(bool forUnit)
    {
        dataManager = FindObjectOfType<DataManager>();
        if (dataManager == null)
        {
            Debug.LogError("there is no DataManager in the scene.");
            return;
        }

        if (forUnit)
        {
            StartCoroutine(DownloadUnitData());
            return;
        }
        StartCoroutine(DownloadMonsterData());
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


    #region Unit

    IEnumerator DownloadUnitData()
    {
        UnityWebRequest www = UnityWebRequest.Get(URL_UnitDataSheet);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string tsvText = www.downloadHandler.text;
            string json = ConvertTSVToJson(tsvText);

            JArray jsonData = JArray.Parse(json);
            ApplyUnitDataToSO(jsonData);
        }

        ApplyUnitSOData();
    }

    private void ApplyUnitDataToSO(JArray jsonData)
    {
        ClearAllUnitSOData();
        unitSOData.Clear();

        for (int i = 0; i < jsonData.Count; i++)
        {
            JObject row = (JObject)jsonData[i];

            string unitID = row["ID"]?.ToString() ?? "";
            string unitName = row["Name"]?.ToString() ?? "";
            float unitHP = float.TryParse(row["HP"]?.ToString(), out float hp) ? hp : 0;
            float unitAtk = float.TryParse(row["Atk"]?.ToString(), out float atk) ? atk : 0;
            float unitAtkRange = float.TryParse(row["AtkRange"]?.ToString(), out float atkRange) ? atkRange : 0;
            float unitAtkDelay = float.TryParse(row["AtkDelay"]?.ToString(), out float atkDelay) ? atkDelay : 0;
            float unitSummonCost = float.TryParse(row["SummonCost"]?.ToString(), out float summonCost) ? summonCost : 0;
            float unitCoolDown = float.TryParse(row["CoolDown"]?.ToString(), out float coolDown) ? coolDown : 0;
            ATK_TYPE unitAtkType = Enum.TryParse<ATK_TYPE>(row["AtkType"]?.ToString(), out ATK_TYPE result) ? result : ATK_TYPE.MELEE;

            UnitSO unitData = new UnitSO();

            if (i < unitSOData.Count)
            {
                unitData = unitSOData[i];
            }
            else
            {
                unitData = CreateNewUnitDataSO(unitID);
                unitSOData.Add(unitData);
            }

            unitData.SetData(unitID, unitName, unitHP, unitAtk, unitAtkRange, unitAtkDelay, unitSummonCost, unitCoolDown, unitAtkType);
            EditorUtility.SetDirty(unitData);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void ClearAllUnitSOData()
    {
        if (!Directory.Exists(unitSODataFolderPath))
        {
            Debug.LogWarning("SO not found");
            return;
        }

        string[] files = Directory.GetFiles(unitSODataFolderPath, "*.asset");

        foreach (string file in files)
        {
            AssetDatabase.DeleteAsset(file);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private UnitSO CreateNewUnitDataSO(string fileName)
    {
        UnitSO newSO = ScriptableObject.CreateInstance<UnitSO>();

        if (!Directory.Exists(unitSODataFolderPath))
        {
            Directory.CreateDirectory(unitSODataFolderPath);
        }

        string assetPath = $"{unitSODataFolderPath}/{fileName}.asset";
        AssetDatabase.CreateAsset(newSO, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return newSO;
    }

    private void ApplyUnitSOData()
    {
        dataManager.SetUnitDatas(unitSOData);
    }

    #endregion


    /////////////////////////////////////////
    /////////////////////////////////////////
    /////////////////////////////////////////


    #region Monster

    IEnumerator DownloadMonsterData()
    {
        UnityWebRequest www = UnityWebRequest.Get(URL_MonsterDataSheet);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string tsvText = www.downloadHandler.text;
            string json = ConvertTSVToJson(tsvText);

            JArray jsonData = JArray.Parse(json);
            ApplyMonsterDataToSO(jsonData);
        }

        ApplyMonsterSOData();
    }

    private void ApplyMonsterDataToSO(JArray jsonData)
    {
        ClearAllMonsterSOData();
        unitSOData.Clear();

        for (int i = 0; i < jsonData.Count; i++)
        {
            JObject row = (JObject)jsonData[i];

            string monsterID = row["ID"]?.ToString() ?? "";
            string monsterName = row["Name"]?.ToString() ?? "";
            float monsterHP = float.TryParse(row["HP"]?.ToString(), out float hp) ? hp : 0;
            float monsterAtk = float.TryParse(row["Atk"]?.ToString(), out float atk) ? atk : 0;
            float monsterAtkRange = float.TryParse(row["AtkRange"]?.ToString(), out float atkRange) ? atkRange : 0;
            float monsterAtkDelay = float.TryParse(row["AtkDelay"]?.ToString(), out float atkDelay) ? atkDelay : 0;
            float monsterMoveSpeed = float.TryParse(row["MoveSpeed"]?.ToString(), out float moveSpeed) ? moveSpeed : 0;
            ATK_TYPE monsterAtkType = Enum.TryParse<ATK_TYPE>(row["AtkType"]?.ToString(), out ATK_TYPE result) ? result : ATK_TYPE.MELEE;

            MonsterSO monsterData = new MonsterSO();

            if (i < monsterSOData.Count)
            {
                monsterData = monsterSOData[i];
            }
            else
            {
                monsterData = CreateNewMonsterDataSO(monsterID);
                monsterSOData.Add(monsterData);
            }

            monsterData.SetData(monsterID, monsterName, monsterHP, monsterAtk, monsterAtkRange, monsterAtkDelay, monsterMoveSpeed, monsterAtkType);     
            EditorUtility.SetDirty(monsterData);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void ClearAllMonsterSOData()
    {
        if (!Directory.Exists(monsterSODataFolderPath))
        {
            Debug.LogWarning("SO not found");
            return;
        }

        string[] files = Directory.GetFiles(monsterSODataFolderPath, "*.asset");

        foreach (string file in files)
        {
            AssetDatabase.DeleteAsset(file);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private MonsterSO CreateNewMonsterDataSO(string fileName)
    {
        MonsterSO newSO = ScriptableObject.CreateInstance<MonsterSO>();

        if (!Directory.Exists(monsterSODataFolderPath))
        {
            Directory.CreateDirectory(monsterSODataFolderPath);
        }

        string assetPath = $"{monsterSODataFolderPath}/{fileName}.asset";
        AssetDatabase.CreateAsset(newSO, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return newSO;
    }

    private void ApplyMonsterSOData()
    {
        dataManager.SetMonsterDatas(monsterSOData);
    }

    #endregion

#endif
}