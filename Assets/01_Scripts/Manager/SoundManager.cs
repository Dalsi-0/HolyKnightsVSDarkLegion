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
    [SerializeField] Image[] MuteChks;

    private float[] volume = new float[3];
    private bool[] isMute = new bool[3];

    /// <summary>
    /// 사운드 옵션 세팅
    /// </summary>
    /// <param name="SoundIndex"> 0 = MASTER , 1 = BGM, 2 = SFX </param>
    protected void AudioControl(int SoundIndex)
    {
        volume[SoundIndex] = audioSlider[SoundIndex].value;
        SetMute(SoundIndex);
        if (volume[SoundIndex] == -40f)
        {
            switch (SoundIndex)
            {
                case 0:
                    audioMixer.SetFloat("MASTER", -80f);
                    MuteChks[SoundIndex].gameObject.SetActive(false);
                    audioBgm.mute = true;
                    audioSfx.mute = true;
                    break;
                case 1:
                    audioMixer.SetFloat("BGM", -80f);
                    MuteChks[SoundIndex].gameObject.SetActive(false);
                    audioBgm.mute = true;
                    break;
                case 2:
                    audioMixer.SetFloat("SFX", -80f);
                    MuteChks[SoundIndex].gameObject.SetActive(false);
                    audioSfx.mute = true;
                    break;
            }
        }
        else
        {
            switch (SoundIndex)
            {
                case 0:
                    audioMixer.SetFloat("MASTER", volume[SoundIndex]);
                    MuteChks[SoundIndex].gameObject.SetActive(true);
                    audioBgm.mute = false;
                    audioSfx.mute = false;
                    break;
                case 1:
                    audioMixer.SetFloat("BGM", volume[SoundIndex]);
                    MuteChks[SoundIndex].gameObject.SetActive(true);
                    audioBgm.mute = false;
                    break;
                case 2:
                    audioMixer.SetFloat("SFX", volume[SoundIndex]);
                    MuteChks[SoundIndex].gameObject.SetActive(true);
                    audioSfx.mute = false;
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

    public void SetMute(int SoundIndex)
    {
        if (isMute[SoundIndex] == true)
        {
            switch (SoundIndex)
            {
                case 0:
                    MuteChks[SoundIndex].gameObject.SetActive(false);
                    audioBgm.mute = true;
                    audioSfx.mute = true;
                    break;
                case 1:
                    MuteChks[SoundIndex].gameObject.SetActive(false);
                    audioBgm.mute = true;
                    break;
                case 2:
                    MuteChks[SoundIndex].gameObject.SetActive(false);
                    audioSfx.mute = true;
                    break;
            }

            isMute[SoundIndex] = false;
        }
        else
        {
            switch (SoundIndex)
            {
                case 0:
                    audioBgm.mute = false;
                    audioSfx.mute = false;
                    break;
                case 1:
                    audioBgm.mute = false;
                    if (MuteChks[0].gameObject.activeSelf)
                    {
                        MuteChks[0].gameObject.SetActive(true);
                        isMute[SoundIndex] = false;
                    }
                    break;
                case 2:
                    audioSfx.mute = false;
                    if (MuteChks[0].gameObject.activeSelf)
                    {
                        MuteChks[0].gameObject.SetActive(true);
                        isMute[SoundIndex] = false;
                    }
                    break;
            }
            MuteChks[SoundIndex].gameObject.SetActive(true);
            isMute[SoundIndex] = true;
        }
    }
}
