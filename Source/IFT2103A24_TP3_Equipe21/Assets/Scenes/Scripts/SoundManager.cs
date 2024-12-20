using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
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
    private static readonly string[] melodies = { "Lena_Raine-Creator", "C418-Aria_Math" };
    private static readonly string[] foleys = { "Sound_Effect_TV-Wind_Sound_SOUND_EFFECT", "freesound_community-quiet_nature_sounds" };
    private static readonly string[] soundEffects = { "freesound_community-quiet_nature_sounds", "JDG_Le_bruit_d'un_scorpion_qui_meurt_(mp3cut.net)" };
    private static readonly string[][] audios = { foleys, soundEffects, melodies };
    private int currentTrackID = -1;

    enum Layer
    {
        Folley,
        SoundEffect,
        Melody
        // vocal
    }

    private string ChooseRandomTrackMelody()
    {
        return melodies[UnityEngine.Random.Range(0, melodies.Length)];
    }

    private string ChooseRandomTrackFoley()
    {
        return foleys[UnityEngine.Random.Range(0, foleys.Length)];
    }

    private string ChooseRandomTrackSoundEffect()
    {
        return soundEffects[UnityEngine.Random.Range(0, soundEffects.Length)];
    }

    private int GetTrackId()
    {
        if (deadTracks.Count > 0)
            return deadTracks.Dequeue();
        return ++currentTrackID;
    }

    public void Start()
    {
        beatDuration = 60f / bpm;

        PlayMusic(ChooseRandomTrackMelody(), 1.0f);
    }

    public void FixedUpdate()
    {
        if (Time.time - lastTime > timeBeforeSwitch)
        {
            lastTime = Time.time;
            PlayMusic(ChooseRandomTrackMelody(), 1.0f);
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlaySpatialSoundEffect(gameObject, ChooseRandomTrackSoundEffect(), 1.0f, 1.0f, 10.0f);
        }
    }

    public uint PlaySpatialSoundEffect(GameObject obj, string name, float volume, float minDistance, float maxDistance)
    {
        AudioSource source = obj.AddComponent<AudioSource>();
        source.clip = Resources.Load<AudioClip>($"Audio/{name}");
        source.spatialBlend = 1.0f;
        source.volume = volume;
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

    public uint PlaySpatializedFoley(GameObject obj, string name, float volume, float minDistance, float maxDistance, bool loop = true)
    {
        AudioSource source = obj.AddComponent<AudioSource>();
        source.clip = Resources.Load<AudioClip>($"Audio/{name}");
        source.spatialBlend = 1.0f;
        source.volume = volume;
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

    private System.Collections.IEnumerator DestroyAfterPlay(AudioSource source, int id)
    {
        yield return new WaitForSeconds(source.clip.length);
        deadTracks.Enqueue(id);
        Destroy(source);
    }

    public uint PlayMusic(string name, float volume)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = Resources.Load<AudioClip>($"Audio/{name}");
        source.volume = volume;
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

            audioLayers[currentTrack].Value.volume = Mathf.Lerp(1f, 0f, t);
            audioLayers[nextTrack].Value.volume = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        // Finaliser la transition
        audioLayers[currentTrack].Value.Stop();

        deadTracks.Enqueue(currentTrack);
        Destroy(audioLayers[currentTrack].Value);

        (nextTrack, currentTrack) = (currentTrack, nextTrack);

        timeBeforeSwitch = audioLayers[currentTrack].Value.clip.length - fadeDuration;
    }

    public void ActivateLayer(uint id)
    {
        if (audioLayers[(int)id].Value != null)
            audioLayers[(int)id].Value.volume = 1f;
    }

    public void DeactivateLayer(uint id)
    {
        if (audioLayers[(int)id].Value != null)
            audioLayers[(int)id].Value.volume = 0f;
    }

    public void SetLayerVolume(uint id, float volume)
    {
        if (audioLayers[(int)id].Value != null)
            audioLayers[(int)id].Value.volume = Mathf.Clamp01(volume);
    }
}
