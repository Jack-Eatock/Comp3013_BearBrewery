using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace DistilledGames
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;

        [SerializeField]
        private AudioMixer audioMixer;
        [SerializeField]
        private AudioMixerGroup audioMixerGroup_SFX;
        [SerializeField]
        private AudioMixerGroup audioMixerGroup_Music;
        [SerializeField]
        private AudioSource musicSource;

        [SerializeField]
        private int numOfSfxSources = 10;
        [SerializeField]
        private GameObject audioSourcesHolder;
        private List<AudioSource> sfxSources = new List<AudioSource>();

        private const int MAXVOLUME = -5, MINVOLUME = -50; // Decibals 
        private const string MUSICVOLUME = "MusicVolume", SFXVOLUME = "SFXVolume";

        [SerializeField]
        private AudioDefinitionsSO sfxClipDefinitions;

        [SerializeField]
        private AudioDefinitionsSO musicClipDefinitions;

        #region Getters

        public AudioMixer AudioMixer => audioMixer;
        public AudioMixerGroup AudioMixerGroup_SFX => audioMixerGroup_SFX;
        public AudioMixerGroup AudioMixerGroup_Music => audioMixerGroup_Music;

        #endregion

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);

            CreateSources();
        }

        #region SFX

        private void CreateSources()
        {
            for (int i = 0; i < numOfSfxSources; i++)
            {
                AudioSource source = audioSourcesHolder.AddComponent<AudioSource>();
                source.outputAudioMixerGroup = audioMixerGroup_SFX;
                sfxSources.Add(source);
            }
        }

        /// <summary>
        /// Plays one shot of an audio clip, this will be heard globally.
        /// </summary>
        /// <param name="clip">The audio clip to play</param>
        /// <param name="volume">The volume to play the clip (Sets source not mixer vol)</param>
        public void SFX_PlayClip(AudioClip clip, float volume)
        {
            SFX_Play(clip, volume);
        }

        /// <summary>
        /// Plays one shot of an audio clip from id, this will be heard globally.
        /// </summary>
        /// <param name="clip">The audio clip to plays id</param>
        /// <param name="volume">The volume to play the clip (Sets source not mixer vol)</param>
        public void SFX_PlayClip(string clipId, float volume)
        {
            AudioClip clip;
            if (TryGetClipUsingId(sfxClipDefinitions, clipId, out clip))
                SFX_Play(clip, volume);
        }

        private void SFX_Play(AudioClip clip, float volume)
        {
            // Grab an available source
            bool foundAvailableSource = false;
            for (int i = 0; i < sfxSources.Count; i++)
            {
                if (!sfxSources[i].isPlaying)
                {
                    foundAvailableSource = true;
                    Debug.Log("1");
                    sfxSources[i].PlayOneShot(clip, volume);
                    break;
                }
            }

            if (!foundAvailableSource)
                Debug.LogError("Failed to find available audio source. Add more sources!");
        }

        public void SFX_SetMixerVolume(float volume)
        {
            volume = ConvertFractionToDecibals(volume);
            audioMixer.SetFloat(SFXVOLUME, volume);
        }

        #endregion


        #region Music

        public void Music_PlayTrack(AudioClip clip)
        {
            musicSource.clip = clip;
            musicSource.Play();
        }

        public void Music_PlayTrack(string clipId)
        {
            AudioClip clip;
            if (TryGetClipUsingId(musicClipDefinitions, clipId, out clip))
            {
                musicSource.clip = clip;
                musicSource.Play();
            }
        }

        public void Music_SetMixerVolume(float volume)
        {
            volume = ConvertFractionToDecibals(volume);
            audioMixer.SetFloat(MUSICVOLUME, volume);
        }


        #endregion


        #region Helpers

        /// <summary>
        /// Takes in a value between 0 and 1. Returns the appropriate decibal volume. 
        /// </summary>
        public float ConvertFractionToDecibals(float fraction)
        {
            return Mathf.Lerp(MINVOLUME, MAXVOLUME, fraction);
        }

        private bool TryGetClipUsingId(AudioDefinitionsSO defToCheck, string id, out AudioClip outClip)
        {
            outClip = null;
            List<AudioClipDefinition> listToCheck = defToCheck.definitions;

            for (int i = 0; i < listToCheck.Count; i++)
            {
                if (listToCheck[i].ID == id)
                {
                    outClip = listToCheck[i].Clip;
                    return true;
                }
            }

            Debug.LogError("Failed to find clip with id " + id);
            return false;
        }


        #endregion

    }

    [Serializable]
    public struct AudioClipDefinition
    {
        public string ID;
        public AudioClip Clip;
    }
}