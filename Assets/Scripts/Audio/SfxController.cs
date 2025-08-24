using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;

public class SfxController : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource[] sfxPlayers; // SFX용 AudioSource 배열
    private List<AudioSource> activePlayers = new List<AudioSource>();

    [Header("Audio Keys")]
    private Dictionary<string, string> sfxKeyDict = new Dictionary<string, string>();

    private AudioLoader audioLoader;
    private AudioData audioData;
    private AudioMixerGroup sfxMixerGroup; // 믹서 그룹 캐싱

    public void Init(AudioLoader loader, AudioMixer mixer,AudioData data)
    {
        audioLoader = loader;
        audioData = data;

        // "SFX" 믹서 그룹 캐싱
        var groups = mixer.FindMatchingGroups("SFX");
        if (groups.Length == 0)
        {
            Debug.LogError("SFX 믹서 그룹을 찾을 수 없습니다.");
            return;
        }
        sfxMixerGroup = groups[0];

        // AudioSource 배열 초기화
        foreach (var player in sfxPlayers)
        {
            player.playOnAwake = false;
            player.loop = false; // SFX는 기본적으로 루프 없음
            player.outputAudioMixerGroup = sfxMixerGroup;
            activePlayers.Add(player);
        }

        // 키값 Dictionary 초기화
        sfxKeyDict.Clear();
        foreach (var _item in audioData.sfxList) // AudioItem 기준
        {
            if (!sfxKeyDict.ContainsKey(_item.name))
                sfxKeyDict.Add(_item.name, _item.key);
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

        // 모두 사용 중이면 새 AudioSource 생성
        var newPlayer = gameObject.AddComponent<AudioSource>();
        newPlayer.playOnAwake = false;
        newPlayer.loop = false;
        newPlayer.outputAudioMixerGroup = sfxMixerGroup; // 캐싱된 그룹 사용
        activePlayers.Add(newPlayer);
        return newPlayer;
    }

    // SFX 재생
    public void Play(string name)
    {
        if (!sfxKeyDict.ContainsKey(name))
        {
            Debug.LogWarning($"SFX _key not found: {name}");
            return;
        }

        string key = sfxKeyDict[name];
        audioLoader.LoadClip(key, clip =>
        {
            var player = GetAvailablePlayer();
            player.PlayOneShot(clip);
        });
    }

    public void Loop(string name)
    {
        if (!sfxKeyDict.ContainsKey(name))
        {
            Debug.LogWarning($"SFX _key not found: {name}");
            return;
        }

        string key = sfxKeyDict[name];

        audioLoader.LoadClip(key, clip =>
        {
            var player = GetAvailablePlayer();
            player.loop = true;
            player.Play();
        });
    }

    // 특정 SFX 정지 (루프용일 경우)
    public void Stop(string name)
    {
        if (!sfxKeyDict.ContainsKey(name)) return;

        string key = sfxKeyDict[name];

        foreach (var player in sfxPlayers)
        {
            if (player.isPlaying && player.clip != null && player.clip.name == key)
            {
                player.Stop();
            }
        }

        audioLoader.ReleaseClip(key);
    }

    // 모든 SFX 정지
    public void StopAll()
    {
        foreach (var player in activePlayers)
            player.Stop();

        if (audioData != null)
        {
            foreach (var item in audioData.sfxList)
                audioLoader.ReleaseClip(item.key);
        }
    }
}
