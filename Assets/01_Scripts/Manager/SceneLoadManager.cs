using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneThumbnail
{
    string sceneName;
    Sprite thumbnail;
}

public class SceneLoadManager : Singleton<SoundManager>
{
    Dictionary<int, SceneThumbnail> thumbnailDict = new Dictionary<int, SceneThumbnail>();
    List<SceneThumbnail> sceneThumbnails;

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
