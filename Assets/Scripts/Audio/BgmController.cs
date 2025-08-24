using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class BgmController : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource[] bgmPlayers;
    private List<AudioSource> activePlayers = new List<AudioSource>();

    [Header("Audio clips")]
    private Dictionary<string, string> bgmKeyDict = new Dictionary<string, string>();
    private AudioLoader audioLoader;
    private AudioData audioData;
    private AudioMixerGroup bgmMixerGroup;
    private int bgmChannelIndex = 0;

    public void Init(AudioLoader loader, AudioMixer mixer, AudioData data)
    {
        audioLoader = loader;
        audioData = data;

        var groups = mixer.FindMatchingGroups("BGM");
        if (groups.Length == 0)
        {
            Debug.LogError("BGM 믹서 그룹을 찾을 수 없습니다.");
            return;
        }

        bgmMixerGroup = groups[0];

        foreach (var player in bgmPlayers)
        {
            player.playOnAwake = false;
            player.loop = true;
            player.outputAudioMixerGroup = mixer.FindMatchingGroups("BGM")[0];
            activePlayers.Add(player);
        }

        bgmKeyDict.Clear();
        foreach (var item in audioData.bgmList) // AudioItem 기준
        {
            if (!bgmKeyDict.ContainsKey(item.name))
                bgmKeyDict.Add(item.name, item.key);
        }
    }

    // 사용 가능한 AudioSource 반환 (없으면 새로 생성)
    private AudioSource GetAvailablePlayer()
    {
        foreach (var player in activePlayers)
        {
            if (!player.isPlaying)
                return player;
        }

        var newPlayer = gameObject.AddComponent<AudioSource>();
        newPlayer.playOnAwake = false;
        newPlayer.loop = true;
        newPlayer.outputAudioMixerGroup = bgmMixerGroup; // 기존 믹서 그룹 복사
        activePlayers.Add(newPlayer);
        return newPlayer;
    }

    public void Play(string name)
    {
        if (!bgmKeyDict.ContainsKey(name))
        {
            Debug.LogWarning($"BGM name not found: {name}");
            return;
        }

        string key = bgmKeyDict[name];
        audioLoader.LoadClip(key, clip =>
        {
            var player = GetAvailablePlayer();
            player.clip = clip;
            player.Play();
        });
    }

    public void Stop(string name)
    {
        if (!bgmKeyDict.ContainsKey(name)) return;

        string key = bgmKeyDict[name];
        foreach (var player in activePlayers)
        {
            if (player.isPlaying && player.clip != null && player.clip.name == key)
                player.Stop();
        }

        audioLoader.ReleaseClip(key);
    }

    public void StopAll()
    {
        foreach (var player in activePlayers)
            player.Stop();

        foreach (var key in bgmKeyDict.Values)
            audioLoader.ReleaseClip(key);
    }
}
