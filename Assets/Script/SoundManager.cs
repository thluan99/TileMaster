using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : GameSingleton<SoundManager>
{
    [SerializeField] private Sound[] _musicSound;
    [SerializeField] private Sound[] _sfxSound;
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSource;

    private void Start() 
    {
        PlayMusic("Background");
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(_musicSound, x => x.name == name);
        if (s == null)
        {
            Debug.Log("Not have this sound!");
        }
        else
        {
            _musicSource.clip = s.audioClip;
            _musicSource.Play();
        }
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(_sfxSound, x => x.name == name);
        if (s == null)
        {
            Debug.Log("Not have this sound!");
        }
        else
        {
            _sfxSource.PlayOneShot(s.audioClip);
        }
    }

    public void ToggleMusic()
    {
        _musicSource.mute = !_musicSource.mute;
    }
    public void ToggleSFX()
    {
        _sfxSource.mute = !_sfxSource.mute;
    }

    public void MusicVolume(float volume)
    {
        _musicSource.volume = volume;
    }

    public void SFXVolume(float volume)
    {
        _sfxSource.volume = volume;
    }
}