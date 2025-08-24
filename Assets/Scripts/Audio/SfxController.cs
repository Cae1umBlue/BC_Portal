using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;

public class SfxController : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource[] sfxPlayers; // SFX용 AudioSource 배열
    private int currentIndex = 0; // 순환 재생용 인덱스

    [Header("Audio Keys")]
    [SerializeField] private string[] sfxKeys; // Addressables 키 배열

    private Dictionary<string, string> sfxKeyDict = new Dictionary<string, string>();
    private AudioLoader audioLoader;

    public void Init(AudioLoader loader, AudioMixer mixer)
    {
        audioLoader = loader;

        // AudioSource 배열 초기화
        foreach (var player in sfxPlayers)
        {
            player.playOnAwake = false;
            player.loop = false; // SFX는 기본적으로 루프 없음
            player.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
        }

        // 키값 Dictionary 초기화
        sfxKeyDict.Clear();
        foreach (var key in sfxKeys)
        {
            if (!sfxKeyDict.ContainsKey(key))
                sfxKeyDict.Add(key, key);
        }
    }

    // SFX 재생
    public void Play(string key)
    {
        if (!sfxKeyDict.ContainsKey(key))
        {
            Debug.LogWarning($"SFX key not found: {key}");
            return;
        }

        audioLoader.LoadClip(key, clip =>
        {
            var player = sfxPlayers[currentIndex];
            player.PlayOneShot(clip);

            currentIndex = (currentIndex + 1) % sfxPlayers.Length;
        });
    }

    public void Loop(string key)
    {
        if (!sfxKeyDict.ContainsKey(key))
        {
            Debug.LogWarning($"SFX key not found: {key}");
            return;
        }

        audioLoader.LoadClip(key, clip =>
        {
            var player = sfxPlayers[currentIndex];
            player.loop = true;
            player.Play();

            currentIndex = (currentIndex + 1) % sfxPlayers.Length;
        });
    }

    // 특정 SFX 정지 (루프용일 경우)
    public void Stop(string key)
    {
        if (!sfxKeyDict.ContainsKey(key)) return;

        var clipKey = sfxKeyDict[key];

        foreach (var player in sfxPlayers)
        {
            if (player.isPlaying && player.clip != null && player.clip.name == clipKey)
            {
                player.Stop();
            }
        }

        audioLoader.ReleaseClip(key);
    }

    // 모든 SFX 정지
    public void StopAll()
    {
        foreach (var player in sfxPlayers)
            player.Stop();

        foreach (var key in sfxKeys)
            audioLoader.ReleaseClip(key);
    }
}
