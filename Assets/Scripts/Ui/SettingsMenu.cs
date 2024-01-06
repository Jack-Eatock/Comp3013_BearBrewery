using DistilledGames;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{

	[SerializeField] private Slider master, music, sfx;

	private void OnEnable()
	{
		master.value = AudioManager.Instance.Master_GetMixerVolume();
		sfx.value = AudioManager.Instance.SFX_GetMixerVolume();
		music.value = AudioManager.Instance.Music_GetMixerVolume();
	}

	public void SetMusicVolume(float volume)
	{
		AudioManager.Instance.Music_SetMixerVolume(volume);
	}

	public void SetMasterVolume(float volume)
	{
		AudioManager.Instance.Master_SetMixerVolume(volume);
	}

	public void SetSFXVolume(float volume)
	{
		AudioManager.Instance.SFX_SetMixerVolume(volume);
	}

	public void BackButtonPressed()
	{
		AudioManager.Instance.SFX_PlayClip("Click", 1f);
		gameObject.SetActive(false);
	}
}
