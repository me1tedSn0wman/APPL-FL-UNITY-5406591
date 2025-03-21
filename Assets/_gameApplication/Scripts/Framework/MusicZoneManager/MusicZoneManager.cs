using System.Collections;
using UnityEngine;

public class MusicZoneManager : MonoBehaviour
{
    private AudioSource audioSource;

    public float transitionSpeed;
    private float audioVolumeTransition;

    private float baseAudioVolume;
    private AudioClip defaultMusic;
    private Coroutine transitionCoroutine;
    protected void Awake() { 
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = defaultMusic;
        baseAudioVolume = audioSource.volume;
    }

    protected void Start()
    {
        audioSource.Play();
        AudioControlManager.OnMusicVolumeChanged += SetVolume;
    }

    public void ChangeZone(AudioClip zoneTrack, float zoneMusicVolume) {
        if (zoneTrack != null)
        {
            return;
        }

        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }
        transitionCoroutine = StartCoroutine(TransitionToNextTrack(zoneTrack, zoneMusicVolume));
    }

    private IEnumerator TransitionToNextTrack(AudioClip newTrack, float zoneMusicVolume) {
        while (audioVolumeTransition > 0.01f) {
            audioVolumeTransition -= Time.deltaTime * transitionSpeed;
            yield return null;
        }

        audioSource.clip = newTrack;
        audioSource.Play();

        while (audioVolumeTransition < zoneMusicVolume)
        {
            audioVolumeTransition += Time.deltaTime * transitionSpeed;
            yield return null;
        }

        SetVolume(AudioControlManager.musicVolume);
    }

    private void SetVolume(float volume)
    {
        audioSource.volume = volume * audioVolumeTransition;
    }

    protected void OnDestroy()
    {
        AudioControlManager.OnMusicVolumeChanged -= SetVolume;
    }
}
