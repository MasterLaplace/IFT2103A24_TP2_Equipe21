using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
public class MainMenu : MonoBehaviour
{
    public GameObject playerPrefab; // Assigné dans l'inspecteur
    public Transform parentContainer; // Parent où placer les cases des joueurs
    public float horizontalSpacing = 170f; // Espace horizontal entre les préfabriqués
    public Vector2 prefabSize = new Vector2(150f, 150f); // Taille des préfabriqués (moins large et plus grand)
    public float initialOffset = 70f; // Décalage initial pour le premier préfabriqué
    public List<PlayerControls> playerControls = new();
    [SerializeField] private GameObject BlackScreenPrefab;

    public void StartGame()
    {
        //met un ecran noir pour le chargement a mettre ici.
        Instantiate(BlackScreenPrefab);
        SceneManager.LoadScene("SelectModeScene");
        // delete the blackboard when the scene is loaded
        while (GameObject.Find("BlackScreen(Clone)") != null)
            Destroy(BlackScreenPrefab);
    }
    public void SettingButton()
    {
        SceneManager.LoadScene("SettingScene");
    }

    public void SoloButton()
    {
        SceneManager.LoadScene("SoloScene");
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void StartSimulation(List<PlayerControls> playerControls)
    {
        //met un ecran noir pour le chargement a mettre ici.
        Instantiate(BlackScreenPrefab);
        Debug.Log("player cintrôle count chatte : " + playerControls.Count);
        // GameModeManager.Instance.IsNetworkMode = false;
        GameModeManager.Instance.PlayerControls = new List<PlayerControls>(playerControls);
        // Dictionary<int, PlayerControls> playerControlsCopy = new Dictionary<int, PlayerControls>(playerControls);
        SceneManager.LoadScene("Simulation");
        // delete the blackboard when the scene is loaded
        while (GameObject.Find("BlackScreen(Clone)") != null)
            Destroy(BlackScreenPrefab);
        // SceneManager.LoadScene("Simulation");
    }

    public void StartMultiplayer()
    {
        //met un ecran noir pour le chargement a mettre ici.
        Instantiate(BlackScreenPrefab);
        SceneManager.LoadScene("NetworkScene");
        // delete the blackboard when the scene is loaded
        while (GameObject.Find("BlackScreen(Clone)") != null)
            Destroy(BlackScreenPrefab);
    }

    public void StartNetwork()
    {
        //lance la simulation en mode réseau
        GameModeManager.Instance.IsNetworkMode = true;
        // recuperer les valeurs des champs IP et Port
        GameModeManager.Instance.ServerIP = GameObject.Find("IPInputField").GetComponent<TMPro.TMP_InputField>().text;
        string portText = GameObject.Find("PortInputField").GetComponent<TMPro.TMP_InputField>().text;
        if (int.TryParse(portText, out int port))
        {
            GameModeManager.Instance.ServerPort = port;
        }
        else
        {
            Debug.LogError("Invalid port number");
            return;
        }
        Instantiate(BlackScreenPrefab);
        SceneManager.LoadScene("Simulation");
        // delete the blackboard when the scene is loaded
        while (GameObject.Find("BlackScreen(Clone)") != null)
            Destroy(BlackScreenPrefab);
    }
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "BindingScene")
            return;
        if (playerPrefab == null || parentContainer == null)
            return;

        for (int i = 0; i < InterfaceManager.Instance.numberOfPlayers; i++)
        {
            var newPlayerCase = Instantiate(playerPrefab, parentContainer);
            newPlayerCase.name = "Player " + (i + 1);

            RectTransform rectTransform = newPlayerCase.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = prefabSize;
                rectTransform.anchorMin = new Vector2(0, 0.5f);
                rectTransform.anchorMax = new Vector2(0, 0.5f);
                rectTransform.pivot = new Vector2(0, 0.5f);
                rectTransform.anchoredPosition = new Vector2(i * horizontalSpacing + initialOffset, 0);
            }

            // Initialiser le script PlayerInputFields pour s'assurer qu'il détecte les TMP_InputFields
            PlayerInputFields playerInputFields = newPlayerCase.GetComponent<PlayerInputFields>();
            if (playerInputFields == null)
            {
                Debug.LogError($"Le prefab {newPlayerCase.name} ne contient pas le script PlayerInputFields !");
            }
        }
    }

      private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void SavePlayerControls()
    {
        playerControls.Clear();

        Debug.Log("parentContainer.childCount : " + parentContainer?.childCount);
        for (int i = 0; i < parentContainer.childCount; i++)
        {
            var playerCase = parentContainer.GetChild(i).GetComponent<PlayerInputFields>();
            if (playerCase != null)
            {
                playerControls.Add(playerCase.GetPlayerControls());
                // Debug.Log($"Joueur {i + 1}: Avancer={playerControls[i].forward}, Tirer={playerControls[i].shoot}, Reculer={playerControls[i].back}");
            }
        }
        Debug.Log("playerControls.Count bite : " + playerControls.Count);
        StartSimulation(playerControls);
    }
}
