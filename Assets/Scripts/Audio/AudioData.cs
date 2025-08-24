using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/AudioData")]
public class AudioData : MonoBehaviour
{
    [Header("BGM")]
    public List<string> bgmKeys;
    public List<string> sfxKeys;
}
