using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSelection : MonoBehaviour
{
    public void SelectPlayers(int players)
    {
        Debug.Log("SelectPlayers");
        InterfaceManager.Instance.numberOfPlayers = players;
        Debug.Log("players : " + players);
        Debug.Log("InterfaceManager.Instance.numberOfPlayers : " + InterfaceManager.Instance.numberOfPlayers);
        SceneManager.LoadScene("BindingScene");
    }
}
