using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public AudioClip musicClip; // Référence à votre fichier audio
    private AudioSource audioSource;

    public void Start()
    {
        // Configure l'AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = musicClip;
        audioSource.loop = true;  // Répète la musique
        audioSource.playOnAwake = false;  // Ne commence pas automatiquement
        audioSource.volume = 0.5f;  // Ajuste le volume si nécessaire

        // Démarre la musique
        audioSource.Play();
    }
}

