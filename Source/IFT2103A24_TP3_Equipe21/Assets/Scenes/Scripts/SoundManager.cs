using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour // PersistentSingleton<SoundManager>
{
    // public Dictionary<string, KeyValuePair<AudioClip, AudioSource>> audioSources = new();
    public List<Tuple<string, AudioSource>> audioLayers = new();
    public Queue<uint> deadTracks = new(); // Piste supprimée
    public uint currentTrack = 0;   // Piste actuellement en cours
    public uint nextTrack = 0;     // Piste suivante à jouer
    public float bpm = 120f;          // Battements par minute
    public float fadeDuration = 5.0f;   // Durée du fondu croisé
    private float beatDuration = 0f;  // Durée d'un battement
    private float lastTime = 0f;      // Dernier temps de mise à jour
    private const float timeBeforeSwitch = 30.0f; // Temps avant de changer de piste
    private static readonly string[] strings = { "Lena_Raine-Creator", "C418-Aria_Math" };
    private static uint currentTrackID = 0;

    enum Layer
    {
        Folley,
        SoundEffect,
        Melody,
        Vocals
    }

    private string ChooseRandomTrack()
    {
        return strings[UnityEngine.Random.Range(0, strings.Length)];
    }

    private uint GetTrackId()
    {
        if (deadTracks.Count > 0)
            return deadTracks.Dequeue();
        return currentTrackID++;
    }

    public void Start()
    {
        beatDuration = 60f / bpm;

        PlayMusic(ChooseRandomTrack(), 1.0f);
    }

    // protected override void Awake()
    // {
    //     base.Awake();
    // }

    public void FixedUpdate()
    {
        for (int i = 0; i < audioLayers.Count; i++)
        {
            // (bool)!audioLayers[i].Item2?.isPlaying
            if (audioLayers[i].Item2 != null && !audioLayers[i].Item2.isPlaying)
            {
                deadTracks.Enqueue((uint)i);
                Destroy(audioLayers[i].Item2);
            }
        }

        if (Time.time - lastTime > timeBeforeSwitch)
        {
            lastTime = Time.time;
            PlayMusic(ChooseRandomTrack(), 1.0f);
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

        uint id = GetTrackId();
        if (id < audioLayers.Count)
        {
            audioLayers[(int)id] = new Tuple<string, AudioSource>(name, source);
        }
        else
        {
            audioLayers.Add(new Tuple<string, AudioSource>(name, source));
        }
        return id;
    }

    public uint AddSpatializedFoley(GameObject obj, string name, float volume, float minDistance, float maxDistance, bool loop = true)
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

        uint id = GetTrackId();
        if (id < audioLayers.Count)
        {
            audioLayers[(int)id] = new Tuple<string, AudioSource>(name, source);
        }
        else
        {
            audioLayers.Add(new Tuple<string, AudioSource>(name, source));
        }
        return id;
    }

    public uint PlayMusic(string name, float volume)
    {
        if (!audioSources.ContainsKey(name) || !audioSources[name].Value)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = Resources.Load<AudioClip>($"Audio/{name}");
            source.volume = volume;
            source.loop = true;
            source.playOnAwake = true;

            audioSources.Add(name, new KeyValuePair<AudioClip, AudioSource>(source.clip, source));
        }

        currentTrackID++;

        if (currentTrack != null)
        {
            SwitchTrack(name);
            return currentTrackID;
        }

        currentTrack = audioSources[name].Value;
        currentTrack.volume = volume;

        currentTrack.Play();
        return currentTrackID;
    }

    public void SwitchTrack(string newClip)
    {
        Debug.Log($"Switching to {newClip}");
        // Trouver le temps restant jusqu'au prochain battement
        float timeToNextBeat = beatDuration - (currentTrack.time % beatDuration);
        AudioClip newClipAudio = Resources.Load<AudioClip>($"Audio/{newClip}");
        StartCoroutine(SwitchAfterDelay(newClipAudio, timeToNextBeat));
    }

    private System.Collections.IEnumerator SwitchAfterDelay(AudioClip newClip, float delay)
    {
        // Attendre jusqu'au prochain battement
        yield return new WaitForSeconds(delay);

        if (nextTrack == null)
        {
            nextTrack = gameObject.AddComponent<AudioSource>();
            nextTrack.loop = true;
        }

        // Configurer la nouvelle piste
        nextTrack.clip = newClip;
        nextTrack.volume = 0f;
        nextTrack.time = currentTrack.time % beatDuration; // Synchronisation des trames
        nextTrack.Play();

        // Effectuer le fondu croisé
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;

            currentTrack.volume = Mathf.Lerp(1f, 0f, t);
            nextTrack.volume = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        // Finaliser la transition
        currentTrack.Stop();
        (nextTrack, currentTrack) = (currentTrack, nextTrack);
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
