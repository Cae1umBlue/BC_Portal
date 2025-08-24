using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/AudioData")]
public class AudioData : ScriptableObject
{
    [System.Serializable]
    public class AudioItem
    {
        public string name;
        public string key;
    }

    [Header("BGM")]
    public List<AudioItem> bgmList = new List<AudioItem>();

    [Header("SFX")]
    public List<AudioItem> sfxList = new List<AudioItem>();
}
