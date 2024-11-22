using UnityEngine;
using UnityEngine.SceneManagement;



public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel; // Référence au panel de la popup
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) // Touche Entrée pour activer/désactiver la pause
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true); // Affiche la popup
        Time.timeScale = 0f;       // Met le jeu en pause
        isPaused = true;
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false); // Cache la popup
        Time.timeScale = 1f;         // Reprend le jeu
        isPaused = false;
    }
    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Réinitialise le temps avant de quitter
        SceneManager.LoadScene("MainMenu");
    }
}
