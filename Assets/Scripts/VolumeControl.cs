using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeControl : MonoBehaviour
{
    [SerializeField]
    private AudioMixer _mixer = null;

    [SerializeField]
    private bool _controlMusic = false, _controlSoundEffects = false;

    [SerializeField]
    private AudioSource _audioSource = null;

    public void SetLevel(float sliderValue)
    {
        if (_controlMusic == true)
        {
            _mixer.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20);
        }
        else if (_controlSoundEffects == true)
        {
            _mixer.SetFloat("SoundEffectsVolume", Mathf.Log10(sliderValue) * 20);
        }
    }

    public void PlaySoundEffect()
    {
        _audioSource.Play();
    }
}
