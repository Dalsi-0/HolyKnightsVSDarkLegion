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

    /// <summary>
    /// 씬 이름으로 씬 전환
    /// </summary>
    /// <param name="sceneName"> 씬 이름 </param>
    public void StringLoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// 씬 번호로 씬 전환
    /// </summary>
    /// <param name="sceneNum"> 씬 인덱스 번호 </param>
    public void NumLoadScene(int sceneNum)
    {
        SceneManager.LoadScene(sceneNum);
    }

    /// <summary>
    /// 씬 전환시 실행되는 메소드,
    /// BGM 변경 용도로 사용
    /// </summary>
    /// <param name="level"> 실행된 씬 인덱스 번호 </param>
    private void OnLevelWasLoaded(int level) //유니티 5.4 이후로는 SceneManager.sceneLoaded 사용 대체
    {
        SoundManager.Instance.SetBgm(level);
    }
}
