using UnityEditor;
using UnityEngine;

public class DataDownloaderWindow : EditorWindow
{
#if UNITY_EDITOR
    [MenuItem("Tools/Data Downloader Tool")]
    public static void ShowWindow()
    {
        GetWindow<DataDownloaderWindow>("Data Downloader Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("Download Character/Stage Data", EditorStyles.boldLabel);

        if (GUILayout.Button("Download Character Data"))
        {
            GameObject obj = new GameObject("tmpCharacterDataDownloader");
            CharacterDataDownloader downloader = obj.AddComponent<CharacterDataDownloader>();
            if (downloader != null)
            {
                downloader.StartDownload();
            }

            DestroyImmediate(obj);
        }

        if (GUILayout.Button("Download Stage Data"))
        {
            GameObject obj = new GameObject("tmpStageDataDownloader");
            StageDataDownloader downloader = obj.AddComponent<StageDataDownloader>();
            if (downloader != null)
            {
                downloader.StartDownload();
            }

            DestroyImmediate(obj);
        }
    }
#endif

}
