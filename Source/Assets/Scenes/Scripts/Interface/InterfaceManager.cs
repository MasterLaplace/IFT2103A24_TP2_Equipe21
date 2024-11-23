using UnityEngine;
using System.Collections.Generic;
public class InterfaceManager : MonoBehaviour
{
    public static InterfaceManager Instance; // Instance globale

    public int numberOfPlayers; // Nombre de joueurs sélectionnés

    public List<PlayerControls> playerControls = new List<PlayerControls>();
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
