using UnityEngine;

public class InterfaceManager : MonoBehaviour
{
    public static InterfaceManager Instance; // Instance globale

    public int numberOfPlayers; // Nombre de joueurs sélectionnés

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persiste entre les scènes
        }
        else
        {
            Destroy(gameObject); // Évite les doublons
        }
    }
}
