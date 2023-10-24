using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundPanel : MonoBehaviour
{
    private const string SFX_VALUE_KEY = "SFX_Value";
    private const string MUSIC_VALUE_KEY = "Music_Value";
    public Slider _musicSlider, _sfxSlider;
    public GameObject _musicDisableLine, _sfxDisableLine;

    private void Start() 
    {
        float musicValue = PlayerPrefs.GetFloat(MUSIC_VALUE_KEY);
        float sfxValue = PlayerPrefs.GetFloat(SFX_VALUE_KEY);

        if (PlayerPrefs.HasKey(MUSIC_VALUE_KEY))
            musicValue = PlayerPrefs.GetFloat(MUSIC_VALUE_KEY);
        else
            musicValue = 0.5f;

        if (PlayerPrefs.HasKey(SFX_VALUE_KEY))
            sfxValue = PlayerPrefs.GetFloat(SFX_VALUE_KEY);
        else
            sfxValue = 0.5f;

        _musicSlider.value = musicValue;
        _sfxSlider.value = sfxValue;

        MusicVolume();
        SFXVolume();
    }

    public void ToggleMusic()
    {
        SoundManager.Instance.ToggleMusic();
        _musicDisableLine.SetActive(!_musicDisableLine.activeSelf);
    }

    public void ToggleSFX()
    {
        SoundManager.Instance.ToggleSFX();
        _sfxDisableLine.SetActive(!_sfxDisableLine.activeSelf);
    }

    public void MusicVolume()
    {
        SoundManager.Instance.MusicVolume(_musicSlider.value);
    }

    public void SFXVolume()
    {
        SoundManager.Instance.SFXVolume(_sfxSlider.value);
    }

    private void OnDestroy() 
    {
        PlayerPrefs.SetFloat(MUSIC_VALUE_KEY, _musicSlider.value);
        PlayerPrefs.SetFloat(SFX_VALUE_KEY, _sfxSlider.value);
    }
}
