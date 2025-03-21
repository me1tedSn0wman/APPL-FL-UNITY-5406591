using System;
using UnityEngine;
using Utils;

public class AudioControlManager : Soliton<AudioControlManager>
{
    [SerializeField] private static float _soundVolume = 1f;
    public static float soundVolume
    {
        get { return _soundVolume; }
        set {
            _soundVolume = Mathf.Clamp01(value);
            OnSoundVolumeChanged?.Invoke(_soundVolume);
        }
    }
    public static event Action<float> OnSoundVolumeChanged;


    [SerializeField] private static float _musicVolume =1f;
    public static float musicVolume
    {
        get { return _musicVolume; }
        set { 
            _musicVolume = Mathf.Clamp01(value);
            OnMusicVolumeChanged?.Invoke(_musicVolume);
        }
    }
    public static event Action<float> OnMusicVolumeChanged;

    public void SetSoundVolumeWithoutNotification(float value) {
        _soundVolume = value;
    }

    public void SetMusicVolumeWithoutNotification(float value)
    {
        _musicVolume = value;
    }
}
