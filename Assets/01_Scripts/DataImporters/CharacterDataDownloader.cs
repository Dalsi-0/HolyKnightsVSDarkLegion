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
    public void StartDownload()
    {
        dataManager = FindObjectOfType<DataManager>();
        if (dataManager == null)
        {
            Debug.LogError("there is no DataManager in the scene.");
            return;
        }

        StartCoroutine(DownloadUnitData());
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
            string unitName = row["name"]?.ToString() ?? "";
            float unitHP = float.TryParse(row["HP"]?.ToString(), out float hp) ? hp : 0;
            float unitAtk = float.TryParse(row["Atk"]?.ToString(), out float atk) ? atk : 0;
            float unitAtkRange = float.TryParse(row["AtkRange"]?.ToString(), out float atkRange) ? atkRange : 0;
            float unitAtkDelay = float.TryParse(row["AtkDelay"]?.ToString(), out float atkDelay) ? atkDelay : 0;
            float unitSummonCost = float.TryParse(row["SummonCost"]?.ToString(), out float summonCost) ? summonCost : 0;
            float unitCoolDown = float.TryParse(row["CoolDown"]?.ToString(), out float coolDown) ? coolDown : 0;

            UnitSO abilityData = new UnitSO();


            /*
            if (i < unitSOData.Count)
            {
                abilityData = unitSOData[i];
            }
            else
            {
                abilityData = CreateNewAbilityDataSO(abilityName);
                unitSOData.Add(abilityData);
            }
            
            if (renameFiles)
            {
                RenameScriptableObjectFile(abilityData, abilityName);
            }

            abilityData.SetData(abilityEnum, abilityName, description, rankEnum, isUpgraded, values);*/
            EditorUtility.SetDirty(abilityData);

        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void ClearAllUnitSOData()
    {
        string folderPath = "Assets/08_Data/ScriptableObjects/Abilities";

        if (!Directory.Exists(folderPath))
        {
            Debug.LogWarning("SO not found");
            return;
        }

        string[] files = Directory.GetFiles(folderPath, "*.asset");

        foreach (string file in files)
        {
            AssetDatabase.DeleteAsset(file);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    private void ApplyUnitSOData()
    {
     //   abilityRepositoy.SetabilityDataSOs(abilityDataSO.ToArray());
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
           // ApplyDataToSO(jsonData);
        }

      //  ApplyMonsterDataSO();
    }

    private void ClearAllMonsterSOData()
    {
        string folderPath = "Assets/08_Data/ScriptableObjects/Abilities";

        if (!Directory.Exists(folderPath))
        {
            Debug.LogWarning("SO not found");
            return;
        }

        string[] files = Directory.GetFiles(folderPath, "*.asset");

        foreach (string file in files)
        {
            AssetDatabase.DeleteAsset(file);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    #endregion

#endif







    /*
    private void RenameScriptableObjectFile(AbilityDataSO so, string newFileName)
    {
        string path = AssetDatabase.GetAssetPath(so);
        string newPath = Path.GetDirectoryName(path) + "/" + newFileName + ".asset";

        if (path != newPath)
        {
            AssetDatabase.RenameAsset(path, newFileName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }*/
    /*
    private AbilityDataSO CreateNewAbilityDataSO(string fileName)
    {
        AbilityDataSO newSO = ScriptableObject.CreateInstance<AbilityDataSO>();

        string folderPath = "Assets/08_Data/ScriptableObjects/Abilities";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string assetPath = $"{folderPath}/{fileName}.asset";
        AssetDatabase.CreateAsset(newSO, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return newSO;
    }*/
}