using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    public List<KeyValuePair<string, AudioSource>> audioLayers = new();
    public Queue<int> deadTracks = new(); // Piste supprimée
    public int currentTrack = 0;   // Piste actuellement en cours
    public int nextTrack = 0;     // Piste suivante à jouer
    public float bpm = 120f;          // Battements par minute
    public float fadeDuration = 5.0f;   // Durée du fondu croisé
    private float beatDuration = 0f;  // Durée d'un battement
    private float lastTime = 0f;      // Dernier temps de mise à jour
    private float timeBeforeSwitch = 30.0f; // Temps avant de changer de piste
    private static readonly List<string> melodies = new() { "Lena_Raine-Creator", "C418-Aria_Math" };
    private static readonly List<string> foleys =  new() { "Sound_Effect_TV-Wind_Sound_SOUND_EFFECT", "freesound_community-quiet_nature_sounds" };
    private static readonly List<string> soundEffects =  new() { "JDG_Le_bruit_d'un_scorpion_qui_meurt_(mp3cut.net)", "squalala" };
    private int currentTrackID = -1;
    private readonly float[] volumeLayers = { 1.0f, 1.0f, 1.0f };

    public enum Layer
    {
        Folley,
        SoundEffect,
        Melody
        // vocal
    }

    public static string ChooseRandomTrackMelody()
    {
        return melodies[UnityEngine.Random.Range(0, melodies.Count)];
    }

    public static string ChooseRandomTrackFoley()
    {
        return foleys[UnityEngine.Random.Range(0, foleys.Count)];
    }

    public static string ChooseRandomTrackSoundEffect()
    {
        return soundEffects[UnityEngine.Random.Range(0, soundEffects.Count)];
    }

    private int GetTrackId()
    {
        if (deadTracks.Count > 0)
            return deadTracks.Dequeue();
        return ++currentTrackID;
    }

    protected override void Awake()
    {
        base.Awake();
        beatDuration = 60f / bpm;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Simulation")
        {
            PlayMusic(ChooseRandomTrackMelody());
            PlaySpatializedFoley(ChooseRandomTrackFoley(), true);
        }
        else
            PlayMusic(ChooseRandomTrackMelody());
    }

    public void FixedUpdate()
    {
        if (Time.time - lastTime > timeBeforeSwitch)
        {
            lastTime = Time.time;
            PlayMusic(ChooseRandomTrackMelody());
        }
    }

    public uint PlaySpatialSoundEffect(GameObject obj, string name, float minDistance, float maxDistance)
    {
        AudioSource source = obj.AddComponent<AudioSource>();
        source.clip = Resources.Load<AudioClip>($"Audio/{name}");
        source.spatialBlend = 1.0f;
        source.volume = volumeLayers[(int)Layer.SoundEffect];
        source.loop = false;
        source.playOnAwake = false;
        source.rolloffMode = AudioRolloffMode.Linear;
        source.minDistance = minDistance;
        source.maxDistance = maxDistance;

        source.Play();

        int id = GetTrackId();
        if (id < audioLayers.Count)
        {
            audioLayers[id] = new KeyValuePair<string, AudioSource>(name, source);
        }
        else
        {
            audioLayers.Add(new KeyValuePair<string, AudioSource>(name, source));
        }

        StartCoroutine(DestroyAfterPlay(source, id));
        return (uint)id;
    }

    public uint PlaySpatialSoundEffect(string name)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = Resources.Load<AudioClip>($"Audio/{name}");
        source.volume = volumeLayers[(int)Layer.SoundEffect];
        source.loop = false;
        source.playOnAwake = false;

        source.Play();

        int id = GetTrackId();
        if (id < audioLayers.Count)
        {
            audioLayers[id] = new KeyValuePair<string, AudioSource>(name, source);
        }
        else
        {
            audioLayers.Add(new KeyValuePair<string, AudioSource>(name, source));
        }

        StartCoroutine(DestroyAfterPlay(source, id));
        return (uint)id;
    }

    public uint PlaySpatializedFoley(GameObject obj, string name, float minDistance, float maxDistance, bool loop = true)
    {
        AudioSource source = obj.AddComponent<AudioSource>();
        source.clip = Resources.Load<AudioClip>($"Audio/{name}");
        source.spatialBlend = 1.0f;
        source.volume = volumeLayers[(int)Layer.Folley];
        source.loop = loop;
        source.playOnAwake = false;
        source.rolloffMode = AudioRolloffMode.Linear;
        source.minDistance = minDistance;
        source.maxDistance = maxDistance;

        source.Play();

        int id = GetTrackId();
        if (id < audioLayers.Count)
        {
            audioLayers[id] = new KeyValuePair<string, AudioSource>(name, source);
        }
        else
        {
            audioLayers.Add(new KeyValuePair<string, AudioSource>(name, source));
        }

        if (!loop)
        {
            StartCoroutine(DestroyAfterPlay(source, id));
        }

        return (uint)id;
    }

    public uint PlaySpatializedFoley(string name, bool loop = true)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = Resources.Load<AudioClip>($"Audio/{name}");
        source.volume = volumeLayers[(int)Layer.Folley];
        source.loop = loop;
        source.playOnAwake = false;

        source.Play();

        int id = GetTrackId();
        if (id < audioLayers.Count)
        {
            audioLayers[id] = new KeyValuePair<string, AudioSource>(name, source);
        }
        else
        {
            audioLayers.Add(new KeyValuePair<string, AudioSource>(name, source));
        }

        if (!loop)
        {
            StartCoroutine(DestroyAfterPlay(source, id));
        }

        return (uint)id;
    }

    private System.Collections.IEnumerator DestroyAfterPlay(AudioSource source, int id)
    {
        yield return new WaitForSeconds(source.clip.length);
        deadTracks.Enqueue(id);
        Destroy(source);
    }

    public uint PlayMusic(string name)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = Resources.Load<AudioClip>($"Audio/{name}");
        source.volume = volumeLayers[(int)Layer.Melody];
        source.loop = true;
        source.playOnAwake = true;

        int id = GetTrackId();
        if (id < audioLayers.Count)
        {
            audioLayers[id] = new KeyValuePair<string, AudioSource>(name, source);
            Debug.Log($"Emplace {audioLayers[id].Value.clip.name} at {id}/{audioLayers.Count}");
        }
        else
        {
            audioLayers.Add(new KeyValuePair<string, AudioSource>(name, source));
            Debug.Log($"Add {audioLayers[id].Value.clip.name} at {id}/{audioLayers.Count}");
        }

        if (currentTrackID != 0)
        {
            Debug.Log($"Switching to {name} with id {id}");
            SwitchTrack(id);
            return (uint)id;
        }

        currentTrack = id;
        timeBeforeSwitch = source.clip.length - fadeDuration;

        Debug.Log($"Playing {name} with id {id}");

        source.Play();
        return (uint)id;
    }

    public void SwitchTrack(int id)
    {
        // Trouver le temps restant jusqu'au prochain battement
        float timeToNextBeat = beatDuration - (audioLayers[currentTrack].Value.time % beatDuration);
        nextTrack = id;
        StartCoroutine(SwitchAfterDelay(timeToNextBeat));
    }

    private System.Collections.IEnumerator SwitchAfterDelay(float delay)
    {
        // Attendre jusqu'au prochain battement
        yield return new WaitForSeconds(delay);

        // Configurer la nouvelle piste
        audioLayers[nextTrack].Value.volume = 0f;
        audioLayers[nextTrack].Value.time = audioLayers[currentTrack].Value.time % beatDuration; // Synchronisation des trames
        audioLayers[nextTrack].Value.Play();

        // Effectuer le fondu croisé
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;

            audioLayers[currentTrack].Value.volume = Mathf.Lerp(volumeLayers[(int)Layer.Melody], 0f, t);
            audioLayers[nextTrack].Value.volume = Mathf.Lerp(0f, volumeLayers[(int)Layer.Melody], t);

            yield return null;
        }

        // Finaliser la transition
        audioLayers[currentTrack].Value.Stop();

        deadTracks.Enqueue(currentTrack);
        Destroy(audioLayers[currentTrack].Value);

        (nextTrack, currentTrack) = (currentTrack, nextTrack);

        timeBeforeSwitch = audioLayers[currentTrack].Value.clip.length - fadeDuration;
    }

    public bool ActivateTrack(uint id)
    {
        if (audioLayers[(int)id].Value == null)
            return false;

        audioLayers[(int)id].Value.volume = 1f;
        return true;
    }

    public bool DeactivateTrack(uint id)
    {
        if (audioLayers[(int)id].Value == null)
            return false;

        audioLayers[(int)id].Value.volume = 0f;
        return true;
    }

    public bool SetTrackVolume(uint id, float volume)
    {
        if (audioLayers[(int)id].Value == null)
            return false;

        audioLayers[(int)id].Value.volume = Mathf.Clamp01(volume);
        return true;
    }

    public void SetLayerVolume(Layer layer, float volume)
    {
        Debug.Log($"Setting volume for {layer} to {volume}");
        foreach (var audio in audioLayers)
        {
            if (melodies.Contains(audio.Key) && layer == Layer.Melody)
            {
                audio.Value.volume = volume;
            }
            else if (foleys.Contains(audio.Key) && layer == Layer.Folley)
            {
                audio.Value.volume = volume;
            }
            else if (soundEffects.Contains(audio.Key) && layer == Layer.SoundEffect)
            {
                audio.Value.volume = volume;
            }
        }
        volumeLayers[(int)layer] = volume;
    }
}
