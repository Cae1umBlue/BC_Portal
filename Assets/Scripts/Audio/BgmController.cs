using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class BgmController : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource[] bgmPlayers;
    private int bgmChannelIndex = 0;

    [Header("Audio clips")]
    [SerializeField] private string[] bgmKeys; // Addressable key 배열


    private Dictionary<string, string> bgmKeyDict = new Dictionary<string, string>();
    private AudioLoader audioLoader;

    public void Init(AudioLoader loader, AudioMixer mixer)
    {
        audioLoader = loader;

        foreach (var player in bgmPlayers)
        {
            player.playOnAwake = false;
            player.loop = true;
            player.outputAudioMixerGroup = mixer.FindMatchingGroups("BGM")[0];
        }

        bgmKeyDict.Clear();
        foreach (var key in bgmKeys)
        {
            if(!bgmKeyDict.ContainsKey(key))
                bgmKeyDict.Add(key, key);
        }
    }

    public void Play(string key)
    {
        if (!bgmKeyDict.ContainsKey(key))
        {
            Debug.LogWarning($"BGM key not found: {key}");
            return;
        }

        audioLoader.LoadClip(key, clip =>
        {
            var player = bgmPlayers[bgmChannelIndex];
            player.clip = clip;
            player.Play();

            bgmChannelIndex = (bgmChannelIndex + 1) % bgmPlayers.Length;
        });
    }

    public void Stop(string key) 
    {
        if (!bgmKeyDict.ContainsKey(key)) return;

        var clipKey = bgmKeyDict[key];

        foreach (var player in bgmPlayers)
        {
            if (player.isPlaying && player.clip != null && player.clip.name == clipKey)
            {
                player.Stop();
            }
        }

        audioLoader.ReleaseClip(key);
    }

    public void StopAll()
    {
        foreach (var player in bgmPlayers)
            player.Stop();

        foreach (var key in bgmKeys)
            audioLoader.ReleaseClip(key);
    }
}
