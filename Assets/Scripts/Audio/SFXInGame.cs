using UnityEngine;


namespace DistilledGames
{
    [RequireComponent(typeof(AudioSource))]
    public class SFXInGame : MonoBehaviour
    {
        protected AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = AudioManager.instance.AudioMixerGroup_SFX;
        }

        public void PlayOneClip(AudioClip clip, float volume)
        {
            volume = Mathf.Clamp(volume, 0f, 1f);
            audioSource.PlayOneShot(clip, volume);
        }
    }
}


