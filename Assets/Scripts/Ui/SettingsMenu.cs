using DistilledGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{

    [SerializeField] private Slider master, music, sfx;

    private void OnEnable()
    {
        master.value = AudioManager.instance.Master_GetMixerVolume();
        sfx.value = AudioManager.instance.SFX_GetMixerVolume();
        music.value = AudioManager.instance.Music_GetMixerVolume();
    }

    public void SetMusicVolume(float volume)
    {
        AudioManager.instance.Music_SetMixerVolume(volume);
    }

    public void SetMasterVolume(float volume)
    {
        AudioManager.instance.Master_SetMixerVolume(volume);
    }

    public void SetSFXVolume(float volume)
    {
        AudioManager.instance.SFX_SetMixerVolume(volume);
    }

    public void BackButtonPressed()
    {
        AudioManager.instance.SFX_PlayClip("Click", 1f);
        gameObject.SetActive(false);
    }
}
