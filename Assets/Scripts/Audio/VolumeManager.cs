using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer mixer;
    public AudioMixer MIxer { get; private set; }
    private Slider bgmSlider;
    private Slider sfxSlider;


    public void AudioSliders(Slider bgm, Slider sfx) // 볼륨 조절 슬라이더
    {
        bgmSlider = bgm;
        sfxSlider = sfx;

        SetupSlider(bgmSlider, "BGMVolume", 0.75f);
        SetupSlider(sfxSlider, "SFXVolume", 0.75f);
    }

    public void SetupSlider(Slider slider, string mixerKey, float defaultVolume)
    {
        if (slider == null) return;

        float _currentValue = PlayerPrefs.GetFloat(mixerKey, defaultVolume);
        slider.value = _currentValue;
        slider.onValueChanged.AddListener(val => SetVolume(mixerKey, val));
        SetVolume(mixerKey, _currentValue);
    }

    private void SetVolume(string key, float val) // 볼륨 값 저장
    {
        mixer.SetFloat(key, Mathf.Log10(val) * 20);
        PlayerPrefs.SetFloat(key, val);
    }
}
