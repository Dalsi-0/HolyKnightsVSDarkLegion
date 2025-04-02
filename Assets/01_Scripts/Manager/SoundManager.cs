using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] AudioSource audioBgm;
    [SerializeField] AudioSource audioSfx;
    [SerializeField] AudioClip[] bgmClip;
    [SerializeField] AudioClip[] sfxClip;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider[] audioSlider;
    [SerializeField] Image[] muteBox;

    private float[] volume = new float[3];
    private bool[] SliderVolume = new bool[3] { false, false, false };

    Dictionary<AudioClip, List<int>> soundOneShot = new Dictionary<AudioClip, List<int>>();
    private int MaxDuplicateOneShotClips = 5;

    /// <summary>
    /// 사운드 옵션 세팅
    /// </summary>
    /// <param name="typeIndex"> MASTER = 0 , BGM = 1, SFX = 2 </param>
    protected void AudioControl(int typeIndex)
    {
        volume[typeIndex] = audioSlider[typeIndex].value;
        if (volume[typeIndex] == -40f)
        {
            if(!muteBox[typeIndex].gameObject.activeSelf) SetMute(typeIndex);
            SetMute(typeIndex);
            SliderVolume[typeIndex] = true;

            switch (typeIndex)
            {
                case 0:
                    audioMixer.SetFloat("MASTER", -80f);
                    break;
                case 1:
                    audioMixer.SetFloat("BGM", -80f);
                    break;
                case 2:
                    audioMixer.SetFloat("SFX", -80f);
                    break;
            }
        }
        else
        {
            if (SliderVolume[typeIndex])
            {
                SetMute(typeIndex);
                SliderVolume[typeIndex] = false;
            }

            switch (typeIndex)
            {
                case 0:
                    audioMixer.SetFloat("MASTER", volume[typeIndex]);
                    break;
                case 1:
                    audioMixer.SetFloat("BGM", volume[typeIndex]);
                    break;
                case 2:
                    audioMixer.SetFloat("SFX", volume[typeIndex]);
                    break;
            }
            
        }
        
    }

    /// <summary>
    /// BGM Index 번호로 원하는 BGM 세팅
    /// </summary>
    /// <param name="index">BGM 클립 번호 입력</param>
    public void SetBgm(int index)
    {
        if (audioBgm.isPlaying)
            audioBgm.Stop();

        audioBgm.clip = bgmClip[index];
        audioBgm.Play();
    }

    /// <summary>
    /// SFX Index 번호로 원하는 SFX 세팅
    /// </summary>
    /// <param name="index">SFX 클립 번호 입력</param>
    public void SetSfx(int index)
    {
        
        if (!soundOneShot.ContainsKey(sfxClip[index]))
        {
            soundOneShot[sfxClip[index]] = new List<int>() { 0 };
        }
        else
        {
            int count = soundOneShot[sfxClip[index]].Count;
            //한클립당 현재 재생수가 10개 넘으면 리턴한다
            if (count == MaxDuplicateOneShotClips) return;

            soundOneShot[sfxClip[index]].Add(0);
        }

        audioSfx.PlayOneShot(sfxClip[index]);
        StartCoroutine(RemoveSfx(index));

    }

    public IEnumerator RemoveSfx(int index)
    {
        yield return new WaitForSeconds(sfxClip[index].length);

        List<int> volumes;
        if (soundOneShot.TryGetValue(sfxClip[index], out volumes))
        {
            volumes.RemoveAt(0);
        }
    }

    /// <summary>
    /// 음소거 세팅
    /// </summary>
    /// <param name="typeIndex"> MASTER = 0 , BGM = 1, SFX = 2 </param>
    public void SetMute(int typeIndex)
    {
        if (muteBox[typeIndex].gameObject.activeSelf) // 뮤트 실행
        {
            muteBox[typeIndex].gameObject.SetActive(false);

            switch (typeIndex)
            {
                case 0:
                    audioBgm.mute = true;
                    audioSfx.mute = true;
                    break;
                case 1:
                    audioBgm.mute = true;
                    break;
                case 2:
                    audioSfx.mute = true;
                    break;
            }
        }
        else // 뮤트 해제
        {
            muteBox[typeIndex].gameObject.SetActive(true);

            switch (typeIndex)
            {
                case 0:
                    if (muteBox[1].gameObject.activeSelf) audioBgm.mute = false;
                    if (muteBox[2].gameObject.activeSelf) audioSfx.mute = false;
                    break;
                case 1:
                    if (muteBox[0].gameObject.activeSelf)
                    {
                        audioBgm.mute = false;
                    }
                    break;
                case 2:
                    if (muteBox[0].gameObject.activeSelf)
                    {
                        audioSfx.mute = false;
                    }
                    break;
            }
        }
    }
}
