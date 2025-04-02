using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class DataDownloaderWindow : EditorWindow
{
    [MenuItem("Tools/Data Downloader Tool")]
    public static void ShowWindow()
    {
        GetWindow<DataDownloaderWindow>("Data Downloader Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("Download Unit/Monster Data", EditorStyles.boldLabel);

        if (GUILayout.Button("Download Unit Data"))
        {
            DownloadUnitData(true).Forget();
        }

        if (GUILayout.Button("Download Monster Data"))
        {
            DownloadUnitData(false).Forget();
        }

        GUILayout.Space(10);

        GUILayout.Label("Download Stage Data", EditorStyles.boldLabel);

        if (GUILayout.Button("Download Stage Data"))
        {
            DownloadStageData().Forget();
        }
    }


    /// <summary>
    /// 유닛 또는 몬스터 데이터를 다운로드하고 완료되면 자동 삭제
    /// </summary>
    private async UniTaskVoid DownloadUnitData(bool isUnit)
    {
        GameObject obj = new GameObject(isUnit ? "tmpUnitDataDownloader" : "tmpMonsterDataDownloader");
        CharacterDataDownloader downloader = obj.AddComponent<CharacterDataDownloader>();

        if (downloader != null)
        {
            await downloader.StartDownload(isUnit);
        }

        Object.DestroyImmediate(obj);
    }

    /// <summary>
    /// 스테이지 데이터를 다운로드하고 완료되면 자동 삭제
    /// </summary>
    private async UniTaskVoid DownloadStageData()
    {
        GameObject obj = new GameObject("tmpStageDataDownloader");
        StageDataDownloader downloader = obj.AddComponent<StageDataDownloader>();

        if (downloader != null)
        {
            await downloader.StartDownload();
        }

        Object.DestroyImmediate(obj);
    }
}
#endif
