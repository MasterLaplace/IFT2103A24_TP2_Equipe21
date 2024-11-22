using UnityEngine;

public class BindingManager : MonoBehaviour
{
    public GameObject playerSlotPrefab; // Préfab pour un emplacement de joueur
    public Transform slotsParent; // Parent où les emplacements seront créés

    void Start()
    {
        int players = InterfaceManager.Instance.numberOfPlayers; // Récupère le nombre de joueurs
        for (int i = 0; i < players; i++)
        {
            // Instancie un slot pour chaque joueur
            GameObject slot = Instantiate(playerSlotPrefab, slotsParent);
            slot.name = "PlayerSlot_" + (i + 1);
            // Tu peux ici configurer des textes ou autres paramètres pour chaque slot
        }
    }
}
