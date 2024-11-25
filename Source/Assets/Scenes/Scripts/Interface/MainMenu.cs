using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject BlackScreenPrefab;

    public void StartGame()
    {
        //met un ecran noir pour le chargement a mettre ici.
        Instantiate(BlackScreenPrefab);
        SceneManager.LoadScene("Simulation");
        // delete the blackboard when the scene is loaded
        while (GameObject.Find("BlackScreen(Clone)") != null)
            Destroy(BlackScreenPrefab);
    }
    public void SettingButton()
    {
        SceneManager.LoadScene("SettingScene");
    }
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
