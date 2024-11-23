using System.Collections.Generic;
using UnityEngine;

public class PlayerBindingManager : MonoBehaviour
{
    public GameObject playerPrefab; // Assigné dans l'inspecteur.
    public Transform parentContainer; // Parent où placer les cases des joueurs.
    public float horizontalSpacing = 170f; // Espace horizontal entre les préfabriqués
    public Vector2 prefabSize = new Vector2(150f, 150f); // Taille des préfabriqués (moins large et plus grand)
    public float initialOffset = 70f; // Décalage initial pour le premier préfabriqué

    // Dictionnaire pour stocker les contrôles de chaque joueur
    private Dictionary<int, PlayerControls> playerControls = new Dictionary<int, PlayerControls>();

    private void Start()
    {
        if (playerPrefab == null || parentContainer == null)
        {
            Debug.LogError("Les références sont manquantes !");
            return;
        }

        // Génération des cases pour le nombre de joueurs
        for (int i = 0; i < InterfaceManager.Instance.numberOfPlayers; i++)
        {
            var newPlayerCase = Instantiate(playerPrefab, parentContainer);
            newPlayerCase.name = "Player " + (i + 1);

            // Ajuster la taille et la position du prefab
            RectTransform rectTransform = newPlayerCase.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = prefabSize;
                rectTransform.anchorMin = new Vector2(0, 0.5f);
                rectTransform.anchorMax = new Vector2(0, 0.5f);
                rectTransform.pivot = new Vector2(0, 0.5f);
                rectTransform.anchoredPosition = new Vector2(i * horizontalSpacing + initialOffset, 0);
            }

            Debug.Log("Instancié: " + newPlayerCase.name);
        }
    }

    // Méthode pour sauvegarder les contrôles
    public void SavePlayerControls()
    {
        playerControls.Clear();

        for (int i = 0; i < parentContainer.childCount; i++)
        {
            var playerCase = parentContainer.GetChild(i).GetComponent<PlayerInputFields>();
            if (playerCase != null)
            {
                playerControls[i] = playerCase.GetPlayerControls();
                Debug.Log($"Joueur {i + 1}: Avancer={playerControls[i].forward}, Gauche={playerControls[i].left}, Droite={playerControls[i].right}, Tirer={playerControls[i].shoot}");
            }
        }
    }

    // Méthode pour récupérer les contrôles d'un joueur
    public PlayerControls GetPlayerControls(int playerIndex)
    {
        if (playerControls.TryGetValue(playerIndex, out var controls))
        {
            return controls;
        }

        Debug.LogError($"Aucun contrôle trouvé pour le joueur {playerIndex + 1}");
        return null;
    }
}
