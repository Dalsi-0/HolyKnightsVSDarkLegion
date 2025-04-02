using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 게임 일시정지, 실행용 버튼
/// </summary>
public class StopButton : MonoBehaviour
{
    [SerializeField] GameObject onPlayImage;
    [SerializeField] GameObject onStopImage;
    private bool nowStop = false;
    private Button _button;
    void Awake()
    {
        _button = GetComponent<Button>();
        if (_button != null)
            _button.onClick.AddListener(StopOrPlayGame);
    }
    public void StopOrPlayGame()
    {
        nowStop = !nowStop;
        if (nowStop)
        {
            onPlayImage.gameObject.SetActive(false);
            onStopImage.gameObject.SetActive(true);
        }
        else
        {
            onPlayImage.gameObject.SetActive(true);
            onStopImage.gameObject.SetActive(false);
        }
        GameManager.Instance.SetTimeScale(nowStop ? 0f : 1f);
    }
}
