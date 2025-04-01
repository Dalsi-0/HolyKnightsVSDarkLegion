using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : Singleton<SceneLoadManager>
{
    /// <summary>
    /// 씬 번호로 씬 전환
    /// </summary>
    /// <param name="sceneNum"> 씬 인덱스 번호 </param>
    public void NumLoadScene(int sceneNum)
    {
        StartCoroutine(Loading(sceneNum));
        //StartCoroutine(LoadingScene(sceneNum));
    }

    /// <summary>
    /// 로딩 (현재 게임 스케일이 작아서 바로 로딩되어 사용 X)
    /// </summary>
    /// <param name="sceneNum"> 씬 인덱스 번호 </param>
    /// <returns></returns>
    IEnumerator LoadingScene(int sceneNum)
    {
        UIManager.Instance.loadingThumbnail.gameObject.SetActive(true);
        AsyncOperation asyncOper = SceneManager.LoadSceneAsync(sceneNum);
        asyncOper.allowSceneActivation = false;
        float progress = 0f;

        while (!asyncOper.isDone)
        {
            progress = Mathf.Lerp(progress, asyncOper.progress, 1f);
            UIManager.Instance.loadingThumbnail.scrollbar.value = progress;
            UIManager.Instance.loadingThumbnail.text.text = Mathf.RoundToInt(progress * 100) + "%";
                        
            if (asyncOper.progress >= 0.9f)
            {
                yield return new WaitForSeconds(3);
                UIManager.Instance.loadingThumbnail.gameObject.SetActive(false);
                asyncOper.allowSceneActivation = true;
            }
            yield return null;
        }

    }

    /// <summary>
    /// 로딩
    /// </summary>
    /// <param name="sceneNum"> 씬 인덱스 번호 </param>
    /// <returns></returns>
    IEnumerator Loading(int sceneNum)
    {
        UIManager.Instance.loadingThumbnail.gameObject.SetActive(true);
        float progress = 0f;
        WaitForSeconds waitForSeconds = new WaitForSeconds(1);
        while (progress <= 100)
        {
            progress += Random.Range(5, 10);
            progress = Mathf.Min(progress, 97);
            UIManager.Instance.loadingThumbnail.scrollbar.value = progress / 100;
            UIManager.Instance.loadingThumbnail.text.text = progress + "%";
            yield return waitForSeconds;

            if (progress == 97)
            {
                SoundManager.Instance.SetSfx(0);
                SceneManager.LoadScene(sceneNum);
                break;
            }
        }
    }

    /// <summary>
    /// 씬 전환시 실행되는 메소드,
    /// 현재 변경 씬의 BGM으로 변경하는 메소드로 활용
    /// </summary>
    /// <param name="level"> 실행된 씬 인덱스 번호 </param>
    private void OnLevelWasLoaded(int level) // 유니티 5.4 이후로는 SceneManager.sceneLoaded 사용 대체
    {
        UIManager.Instance.loadingThumbnail.gameObject.SetActive(false);

        SoundManager.Instance.SetBgm(level);
    }
}