using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : PersistentSingleton<SoundManager>
{
    public Dictionary<string, KeyValuePair<AudioClip, AudioSource>> audioSources = new();
    public AudioSource currentTrack;   // Piste actuellement en cours
    public AudioSource nextTrack;     // Piste suivante à jouer
    public float bpm = 120f;          // Battements par minute
    public float fadeDuration = 1f;   // Durée du fondu croisé
    private float beatDuration = 0f;  // Durée d'un battement

    public void Start()
    {
        beatDuration = 60f / bpm;

        GameObject gameObject = new("BackgroundMusic");
        gameObject.transform.position = new Vector3(0, 0, 0);
        AddSpatializedFoley(gameObject, "Lena_Raine-Creator", 0f, 5.0f, 20.0f);
        AddSpatializedFoley(gameObject, "Sound_Effect_T-Wind_Sound_SOUND_EFFECT", 0f, 5.0f, 20.0f);

        foreach (KeyValuePair<string, KeyValuePair<AudioClip, AudioSource>> audioSource in audioSources)
        {
            if (audioSource.Value.Value == null)
                continue;

            Debug.Log($"Audio source: {audioSource.Key}");
            audioSource.Value.Value.Play();
        }
    }

    protected override void Awake()
    {
        base.Awake();
    }

    public void PlaySpatialSoundEffect(GameObject obj, string name, float volume, float minDistance, float maxDistance)
    {
        AudioSource source;

        if (audioSources.ContainsKey(name))
        {
            obj.TryGetComponent<AudioSource>(out source);
            source.clip = audioSources[name].Key;
            source.volume = volume;
            source.minDistance = minDistance;
            source.maxDistance = maxDistance;
        }
        else
        {
            source = obj.AddComponent<AudioSource>();
            source.clip = Resources.Load<AudioClip>($"Audio/{name}");
            source.spatialBlend = 1.0f;
            source.volume = volume;
            source.loop = false;
            source.playOnAwake = false;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.minDistance = minDistance;
            source.maxDistance = maxDistance;

            audioSources.Add(name, new KeyValuePair<AudioClip, AudioSource>(source.clip, null));
        }

        source.Play();
    }

    public void AddSpatializedFoley(GameObject obj, string name, float volume, float minDistance, float maxDistance, bool loop = true)
    {
        AudioSource source;

        if (audioSources.ContainsKey(name))
        {
            obj.TryGetComponent<AudioSource>(out source);
            source.clip = audioSources[name].Key;
            source.volume = volume;
            source.minDistance = minDistance;
            source.maxDistance = maxDistance;
        }
        else
        {
            source = obj.AddComponent<AudioSource>();
            source.clip = Resources.Load<AudioClip>($"Audio/{name}");
            source.spatialBlend = 1.0f;
            source.volume = volume;
            source.loop = loop;
            source.playOnAwake = false;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.minDistance = minDistance;
            source.maxDistance = maxDistance;

            audioSources.Add(name, new KeyValuePair<AudioClip, AudioSource>(source.clip, null));
        }

        source.Play();
    }

    public void PlayMusic(string name)
    {
        if (!audioSources.ContainsKey(name) && audioSources[name].Value == null)
            return;

        if (currentTrack != null)
        {
            nextTrack = audioSources[name].Value;
        }
        else
        {
            currentTrack = audioSources[name].Value;
            currentTrack.volume = 1f;
        }
    }

    public void ActivateLayer(string name)
    {
        if (audioSources.ContainsKey(name) && audioSources[name].Value != null)
            audioSources[name].Value.volume = 1f;
    }

    public void DeactivateLayer(string name)
    {
        if (audioSources.ContainsKey(name) && audioSources[name].Value != null)
            audioSources[name].Value.volume = 0f;
    }

    public void SetLayerVolume(string name, float volume)
    {
        if (audioSources.ContainsKey(name) && audioSources[name].Value != null)
            audioSources[name].Value.volume = Mathf.Clamp01(volume);
    }
}
