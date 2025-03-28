using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DataDownloaderWindow : EditorWindow
{
#if UNITY_EDITOR

    private List<GameObject> myObj = new List<GameObject>();

    [MenuItem("Tools/Data Downloader Tool")]
    public static void ShowWindow()
    {
        GetWindow<DataDownloaderWindow>("Data Downloader Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("Clear TmpObjects", EditorStyles.boldLabel);

        if (GUILayout.Button("Clear TmpObjects Button"))
        {
            DestroyTmpObj();
        }

        GUILayout.Space(20);

        GUILayout.Label("Download Unit/Monster Data", EditorStyles.boldLabel);

        if (GUILayout.Button("Download Unit Data"))
        {
            GameObject obj = new GameObject("tmpUnitDataDownloader");
            CharacterDataDownloader downloader = obj.AddComponent<CharacterDataDownloader>();
            if (downloader != null)
            {
                downloader.StartDownload(true);
            }
            myObj.Add(obj);
        }

        if (GUILayout.Button("Download Monster Data"))
        {
            GameObject obj = new GameObject("tmpMonsterDataDownloader");
            CharacterDataDownloader downloader = obj.AddComponent<CharacterDataDownloader>();
            if (downloader != null)
            {
                downloader.StartDownload(false);
            }
            myObj.Add(obj);
        }

        GUILayout.Space(10);

        GUILayout.Label("Download Stage Data", EditorStyles.boldLabel);

        if (GUILayout.Button("Download Stage Data"))
        {
            GameObject obj = new GameObject("tmpStageDataDownloader");
            StageDataDownloader downloader = obj.AddComponent<StageDataDownloader>();
            if (downloader != null)
            {
                downloader.StartDownload();
            }
            myObj.Add(obj);
        }
    }

    private void DestroyTmpObj()
    {
        for (int i = 0; i < myObj.Count; i++)
        {
            if (myObj[i] != null)
            {
                DestroyImmediate(myObj[i]);
            }
        }
        myObj.Clear();
    }

#endif
}
