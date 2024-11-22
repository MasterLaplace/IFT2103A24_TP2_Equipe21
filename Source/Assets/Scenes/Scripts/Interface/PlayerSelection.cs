using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSelection : MonoBehaviour
{
    public void SelectPlayers(int players)
    {
        InterfaceManager.Instance.numberOfPlayers = players; // Enregistre le choix
        SceneManager.LoadScene("BindingScene"); // Charge la scène suivante
    }
}
