using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Controllers")]
    [SerializeField] private BgmController bgmController;
    [SerializeField] private SfxController sfxController;

    [Header("Audio Loader")]
    [SerializeField] private AudioLoader audioLoader;

    [Header("Volume Manager")]
    [SerializeField] private VolumeManager volumeManager;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    protected override void Initialize()
    {
        bgmController.Init(audioLoader, volumeManager.MIxer);
        sfxController.Init(audioLoader, volumeManager.MIxer);

        volumeManager.AudioSliders(bgmSlider, sfxSlider);
    }

    public void PlayBGM(string name) => bgmController.Play(name);
    public void StopBGM(string name) => bgmController.Stop(name);
    public void PlaySFX(string name) => sfxController.Play(name);
    public void StopSFX(string name) => sfxController.Stop(name);
    public void PlayLoopSFX(string name) => sfxController.Loop(name);
    public void StopAllBGM() => bgmController.StopAll();
    public void StopAllSFX() => sfxController.StopAll();
}





    //------------------ 기능 분리 전-------------------------
    
//    [Header("Volume Control")]
//    [SerializeField] private AudioMixer mixer;
//    private Slider bgmSlider;
//    private Slider sfxSlider;

//    [Header("Channel settings")]
//    [SerializeField] private int bgmChannels = 2;
//    [SerializeField] private int sfxChannels = 10;
//    private int bgmChannelIndex = 0;
//    private int sfxChannelIndex = 0;

//    [Header("Audio Sources")]
//    private AudioSource[] bgmPlayers;
//    private AudioSource[] sfxPlayers;

//    // Dictioanry 키값 관리 좋음 -> 문자열? enum 장단을 알고 진행하자
//    private Dictionary<string,AudioClip> bgmClips = new Dictionary<string,AudioClip>(); // _bgm 저장
//    private Dictionary<string,AudioClip> sfxClips = new Dictionary<string,AudioClip>(); // _sfx 저장

//    [System.Serializable]
//    public struct NamedAudioClip
//    {
//        public string name; // 사운드 이름
//        public AudioClip clip; // 오디오 클립
//    }

//    public NamedAudioClip[] bgmClipList; // _bgm 리스트
//    public NamedAudioClip[] sfxClipList; // _sfx 리스트

//    protected override void Initialize()
//    {
//        bgmPlayers = CreateAudioPlayers("BgmPlayer", "BGM", bgmChannels);
//        sfxPlayers = CreateAudioPlayers("SfxPlayer", "SFX", sfxChannels);
//        InitializeAudioClips();
//    }

//    // 필요할때 생성하고 사용하지 않을때 언로드하는 형식이 필요
//    // 초기에 로드해두는 것은 부담이 된다
//    private void InitializeAudioClips()
//    {
//        foreach(var _bgm in bgmClipList)
//        {
//            if(!bgmClips.ContainsKey(_bgm.name))
//            {
//                bgmClips.Add(_bgm.name, _bgm.clip); //_bgm 이름과 클립 저장
//            }
//        }

//        foreach(var _sfx in sfxClipList)
//        {
//            if(!sfxClips.ContainsKey(_sfx.name))
//            {
//                sfxClips.Add(_sfx.name, _sfx.clip); // _sfx 이름과 클립 저장
//            }
//        }
//    }

//    private AudioSource[] CreateAudioPlayers(string objectName, string mixerGroupName, int channelCount)
//    {
//        GameObject _audioObject = new GameObject(objectName);
//        _audioObject.transform.parent = transform;

//        AudioSource[] _audioSources = new AudioSource[channelCount];
//        AudioMixerGroup _mixerGroup = mixer.FindMatchingGroups(mixerGroupName)[0];

//        for (int i = 0; i < channelCount; i++)
//        {
//            _audioSources[i] = _audioObject.AddComponent<AudioSource>();
//            _audioSources[i].playOnAwake = false;
//            _audioSources[i].loop = false;
//            _audioSources[i].outputAudioMixerGroup = _mixerGroup;
//        }

//        return _audioSources;
//    }

//    //public void AudioSliders(Slider bgm, Slider sfx) // 볼륨 조절 슬라이더
//    //{
//    //    bgmSlider = bgm;
//    //    sfxSlider = sfx;

//    //    SetupSlider(bgmSlider, "BGMVolume", 0.75f);
//    //    SetupSlider(sfxSlider, "SFXVolume", 0.75f);
//    //}

//    private void SetupSlider(Slider slider, string volumeKey, float defaultVal) // 볼륨 조절 슬라이더 셋업
//    {
//        if (slider == null) return;

//        float _currentValue = PlayerPrefs.GetFloat(volumeKey, defaultVal);
//        slider.value = _currentValue;
//        slider.onValueChanged.AddListener(val => SetVolume(volumeKey, val));
//        SetVolume(volumeKey, _currentValue);
//    }

//    private void SetVolume(string key, float val) // 볼륨 값 저장
//    {
//        mixer.SetFloat(key, Mathf.Log10(val) * 20);
//        PlayerPrefs.SetFloat(key, val);
//    }

//    // 파라미터 스트링 -> 상수로
//    public void PlaySFX(string name)
//    {
//        if(sfxClips.ContainsKey(name))
//        {
//            AudioClip _clip = sfxClips[name];
//            AudioSource _player = sfxPlayers[sfxChannelIndex];

//            _player.PlayOneShot(_clip);

//            sfxChannelIndex = (sfxChannelIndex + 1) % sfxPlayers.Length;
//        }
//    }

//    public void PlayLoopSFX(string name)
//    {
//        if (sfxClips.ContainsKey(name))
//        {
//            AudioClip _clip = sfxClips[name];
//            AudioSource _player = sfxPlayers[sfxChannelIndex];

//            _player.clip = _clip;
//            _player.loop = true;
//            _player.Play();

//            sfxChannelIndex = (sfxChannelIndex + 1) % sfxPlayers.Length;
//        }
//    }

//    public void PlayBGM(string name)
//    {
//        if (bgmClips.ContainsKey(name))
//        {
//            AudioClip _clip = bgmClips[name];
//            AudioSource _player = bgmPlayers[bgmChannelIndex];

//            _player.clip = _clip;
//            _player.loop = true;
//            _player.Play();

//            bgmChannelIndex = (bgmChannelIndex + 1) % bgmPlayers.Length;
//        }
//    }

//    public void StopBGM(string name)
//    {
//        if(!bgmClips.ContainsKey(name)) return;

//        AudioClip _clip = bgmClips[name];

//        foreach (var _player in bgmPlayers)
//        {
//            if (_player.isPlaying && _player.clip == _clip)
//            {
//                _player.Stop();
//            }
//        }
//    }
//    public void StopSFX(string name)
//    {
//        if(!sfxClips.ContainsKey(name)) return;

//        AudioClip _clip = sfxClips[name];

//        foreach (var _player in sfxPlayers)
//        {
//            if (_player.isPlaying && _player.clip == _clip)
//            {
//                _player.Stop();
//            }
//        }
//    }

//    public void StopAllBGM()
//    {
//        foreach (var _player in bgmPlayers)
//        {
//            _player.Stop();
//        }
//    }

//    public void StopAllSFX()
//    {
//        foreach (var _player in sfxPlayers)
//        {
//            _player.Stop();
//        }
//    }

//}
