using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using JetBrains.Annotations;

public class AudioLoader : MonoBehaviour
{
    private Dictionary<string, AudioClip> loadedClips = new Dictionary<string, AudioClip>();

    public void LoadClip(string key, Action<AudioClip> callback)
    {
        if (loadedClips.ContainsKey(key))
        {
            callback?.Invoke(loadedClips[key]);
            return;
        }

        Addressables.LoadAssetAsync<AudioClip>(key).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                loadedClips[key] = handle.Result;
                callback?.Invoke(handle.Result);
            }
            else
            {
                Debug.LogWarning("오디오 클립 로드에 실패했습니다. 클립이 있는지 확인해주세요.");
            }
        };
    }

    public void ReleaseClip(string key)
    {
        if( loadedClips.ContainsKey(key))
        {
            Addressables.Release(loadedClips[key]);
            loadedClips.Remove(key);
        }
    }
}
