using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingScene : MonoBehaviour
{
    public void ReturnButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
