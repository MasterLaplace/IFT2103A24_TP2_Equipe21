using UnityEngine;

public class PlayerBindingManager : MonoBehaviour
{
    public GameObject BindingPlayerCase; // Le prefab pour un joueur
    public Transform slotsParent;      // Parent où les slots seront placés

    void Start()
    {
        int players = InterfaceManager.Instance.numberOfPlayers; // Récupère le nombre de joueurs

        for (int i = 0; i < players; i++)
        {
            // Instancie un prefab pour chaque joueur
            GameObject slot = Instantiate(BindingPlayerCase, slotsParent);
            slot.name = "PlayerSlot_" + (i + 1);

            // Configure le texte pour indiquer "Joueur X"
            var playerText = slot.transform.Find("PlayerText").GetComponent<UnityEngine.UI.Text>();
            playerText.text = "Joueur " + (i + 1);
        }
    }
}
