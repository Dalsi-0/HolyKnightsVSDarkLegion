using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Legacy;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
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
    private bool[] SliderVolume = new bool[3] {false, false, false};

    /// <summary>
    /// 사운드 옵션 세팅
    /// </summary>
    /// <param name="SoundIndex"> MASTER = 0 , BGM = 1, SFX = 2 </param>
    protected void AudioControl(int SoundIndex)
    {
        volume[SoundIndex] = audioSlider[SoundIndex].value;
        if (volume[SoundIndex] == -40f)
        {
            SetMute(SoundIndex);
            SliderVolume[SoundIndex] = true;

            switch (SoundIndex)
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
            if (SliderVolume[SoundIndex])
            {
                SetMute(SoundIndex);
                SliderVolume[SoundIndex] = false;
            }

            switch (SoundIndex)
            {
                case 0:
                    audioMixer.SetFloat("MASTER", volume[SoundIndex]);
                    break;
                case 1:
                    audioMixer.SetFloat("BGM", volume[SoundIndex]);
                    break;
                case 2:
                    audioMixer.SetFloat("SFX", volume[SoundIndex]);
                    break;
            }
            
        }
        
    }

    /// <summary>
    /// BGM Index 번호로 원하는 BGM 세팅
    /// </summary>
    /// <param name="Index">BGM 클립 번호 입력</param>
    public void SetBgm(int Index)
    {
        if (audioBgm.isPlaying)
            audioBgm.Stop();

        audioBgm.clip = bgmClip[Index];
        audioBgm.Play();
    }

    /// <summary>
    /// SFX Index 번호로 원하는 SFX 세팅
    /// </summary>
    /// <param name="Index">SFX 클립 번호 입력</param>
    public void SetSfx(int Index)
    {
        audioSfx.PlayOneShot(sfxClip[Index]);
    }

    /// <summary>
    /// 음소거 세팅
    /// </summary>
    /// <param name="SoundIndex"> MASTER = 0 , BGM = 1, SFX = 2 </param>
    public void SetMute(int SoundIndex)
    {
        if (muteBox[SoundIndex].gameObject.activeSelf) // 뮤트 실행
        {
            muteBox[SoundIndex].gameObject.SetActive(false);

            switch (SoundIndex)
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
            muteBox[SoundIndex].gameObject.SetActive(true);

            switch (SoundIndex)
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
