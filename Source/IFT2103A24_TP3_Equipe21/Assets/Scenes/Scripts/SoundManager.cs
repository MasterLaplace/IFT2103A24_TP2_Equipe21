using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour // PersistentSingleton<SoundManager>
{
    public Dictionary<string, KeyValuePair<AudioClip, AudioSource>> audioSources = new();
    public AudioSource currentTrack = null;   // Piste actuellement en cours
    public AudioSource nextTrack = null;     // Piste suivante à jouer
    public float bpm = 120f;          // Battements par minute
    public float fadeDuration = 5.0f;   // Durée du fondu croisé
    private float beatDuration = 0f;  // Durée d'un battement
    private float lastTime = 0f;      // Dernier temps de mise à jour
    private const float timeBeforeSwitch = 30.0f; // Temps avant de changer de piste
    private static readonly string[] strings = { "Lena_Raine-Creator", "C418-Aria_Math" };

    private string ChooseRandomTrack()
    {
        return strings[UnityEngine.Random.Range(0, strings.Length)];
    }

    public void Start()
    {
        beatDuration = 60f / bpm;

        PlayMusic(ChooseRandomTrack(), 1.0f);

        // GameObject gameObject = new("BackgroundMusic");
        // gameObject.transform.position = new Vector3(0, 0, 0);
        // AddSpatializedFoley(gameObject, "Lena_Raine-Creator", 0f, 5.0f, 20.0f);
        // AddSpatializedFoley(gameObject, "Sound_Effect_T-Wind_Sound_SOUND_EFFECT", 0f, 5.0f, 20.0f);

        // foreach (KeyValuePair<string, KeyValuePair<AudioClip, AudioSource>> audioSource in audioSources)
        // {
        //     if (audioSource.Value.Value == null)
        //         continue;

        //     Debug.Log($"Audio source: {audioSource.Key}");
        //     audioSource.Value.Value.Play();
        // }
    }

    // protected override void Awake()
    // {
    //     base.Awake();
    // }

    public void FixedUpdate()
    {
        if (currentTrack == null)
            return;

        if (Time.time - lastTime > timeBeforeSwitch)
        {
            lastTime = Time.time;
            PlayMusic(ChooseRandomTrack(), 1.0f);
        }
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

    public void PlayMusic(string name, float volume)
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

        if (currentTrack != null)
        {
            SwitchTrack(name);
            return;
        }

        currentTrack = audioSources[name].Value;
        currentTrack.volume = volume;

        currentTrack.Play();
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
