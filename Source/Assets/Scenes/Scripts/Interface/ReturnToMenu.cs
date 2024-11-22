using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenu : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu"); // Remplace "MainMenu" par le nom exact de ta scène de menu
        }
        if (GameObject.FindGameObjectsWithTag("Player").Length == 0)
        {
            SceneManager.LoadScene("MainMenu"); // Remplace "MainMenu" par le nom exact de ta scène de menu
        }
    }
}

