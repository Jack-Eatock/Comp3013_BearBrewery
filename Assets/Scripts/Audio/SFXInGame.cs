using System.Collections.Generic;
using UnityEngine;

namespace DistilledGames
{
	[RequireComponent(typeof(AudioSource))]
	public class SFXInGame : MonoBehaviour
	{
		protected AudioSource loopingAudioSource;
		private GameObject oneOfSources;
		[SerializeField] private int numOfSources = 3;
		private readonly List<AudioSource> sfxSources = new();
		[SerializeField] private float minDist = 1f, maxDist = 500f;

		private void Awake()
		{
			loopingAudioSource = GetComponent<AudioSource>();
			loopingAudioSource.outputAudioMixerGroup = AudioManager.Instance.AudioMixerGroup_SFX;
			loopingAudioSource.spatialBlend = 1f;
			loopingAudioSource.maxDistance = maxDist;
			loopingAudioSource.minDistance = minDist;

			oneOfSources = new GameObject("SFXSources");
			oneOfSources.transform.parent = transform;
			oneOfSources.transform.position = Vector3.zero;
			CreateSources();
		}

		private void CreateSources()
		{
			for (int i = 0; i < numOfSources; i++)
			{
				AudioSource source = oneOfSources.gameObject.AddComponent<AudioSource>();
				source.outputAudioMixerGroup = AudioManager.Instance.AudioMixerGroup_SFX;
				source.spatialBlend = 1f;
				source.maxDistance = maxDist;
				source.minDistance = minDist;
				sfxSources.Add(source);
			}
		}

		public void PlayOneClip(AudioClip clip, float volume)
		{
			volume = Mathf.Clamp(volume, 0f, 1f);

			// Grab an available source
			bool foundAvailableSource = false;
			for (int i = 0; i < sfxSources.Count; i++)
			{
				if (!sfxSources[i].isPlaying)
				{
					foundAvailableSource = true;
					sfxSources[i].PlayOneShot(clip, volume);
					break;
				}
			}

			if (!foundAvailableSource)
			{
				Debug.LogError("Failed to find available audio source. Add more sources!");
			}
		}

		public void LoopingClipPlay(AudioClip clip, float volume)
		{
			volume = Mathf.Clamp(volume, 0f, 1f);
			loopingAudioSource.clip = clip;
			loopingAudioSource.volume = volume;
			loopingAudioSource.Play();
		}

		public void LoopingClipStop()
		{
			loopingAudioSource.Stop();
		}
	}
}

