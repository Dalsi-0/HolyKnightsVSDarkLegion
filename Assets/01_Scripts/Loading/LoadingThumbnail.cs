using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class LoadingThumbnail : MonoBehaviour
{
    public Animator animator;
    public TextMeshProUGUI text;
    public Scrollbar scrollbar;

    private void OnEnable()
    {
        int num = Random.Range(0, 5);
        animator.SetInteger("CharacterNum", num);
    }

}
