using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public  Dictionary<string, KeyValuePair<AudioClip, AudioSource>> audioSources = new();

    public void Start()
    {
        GameObject gameObject = new("BackgroundMusic");
        gameObject.transform.position = new Vector3(0, 0, 0);
        AddSpatializedAudioSource(gameObject, Resources.Load<AudioClip>("Audio/Lena_Raine-Creator"), 1.0f, 5.0f, 20.0f);

        foreach (KeyValuePair<string, KeyValuePair<AudioClip, AudioSource>> audioSource in audioSources)
        {
            Debug.Log($"Audio source: {audioSource.Key}");
            audioSource.Value.Value.Play();
        }
    }

    void AddSpatializedAudioSource(GameObject obj, AudioClip clip, float volume, float minDistance, float maxDistance)
    {
        AudioSource source = obj.AddComponent<AudioSource>();
        source.clip = clip;
        source.spatialBlend = 1.0f; // Définit le son en 3D
        source.volume = volume;
        source.loop = true;
        source.playOnAwake = true;
        source.rolloffMode = AudioRolloffMode.Linear; // Contrôle l'atténuation du son avec la distance
        source.minDistance = minDistance;
        source.maxDistance = maxDistance;

        if (audioSources.ContainsKey(clip.name))
            audioSources[clip.name] = new KeyValuePair<AudioClip, AudioSource>(clip, source);
        else
            audioSources.Add(clip.name, new KeyValuePair<AudioClip, AudioSource>(clip, source));
    }
}
