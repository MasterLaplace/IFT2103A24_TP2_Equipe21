using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnTime = 3f;
    [SerializeField] private int maxEnemies = 10;
    [SerializeField] public int playerCount = 5;
    [SerializeField] public int rows = 2;
    // [SerializeField] private bool networkMode = false;
    // [SerializeField] private string serverIP = "127.0.0.1";
    // [SerializeField] private int serverPort = 54000;
    [SerializeField] private string gameName = "SpaceWar";
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Simulation")
            return;

        if (GameModeManager.Instance.IsNetworkMode)
        {
            // Appeler GameStart avec le mode configuré
            NetworkGameStart(GameModeManager.Instance.ServerIP, GameModeManager.Instance.ServerPort);
        }
        else
        {
            LocalStartGame(GameModeManager.Instance.PlayerControls);
        }
    }
    public void NetworkGameStart(string serverIP, int serverPort)
    {
        NetworkClient networkClient = gameObject.AddComponent<NetworkClient>();
        networkClient.Create(serverIP, serverPort, gameName);
    }

    public void LocalStartGame(List<PlayerControls> playerControls)
    {
        SpawnPlayers(playerControls);
        StartSpawner();
    }

    private Rect CalculateViewportRect(int playerIndex, int totalPlayers, int rows)
    {
        int columns = Mathf.CeilToInt((float)totalPlayers / rows);
        float width = 1f / columns;
        float height = 1f / rows;
        int row = playerIndex / columns;
        int column = playerIndex % columns;
        return new Rect(column * width, 1 - (row + 1) * height, width, height);
    }

    private void SpawnPlayers(List<PlayerControls> playerControls)
    {
        for (int i = 0; i < playerCount; i++)
        foreach (var item in playerControls)
        {
            GameObject player = Instantiate(playerPrefab, new Vector3(i * 2 - (playerCount - 1), 1, 0), Quaternion.identity);
            Player playerScript = player.GetComponent<Player>();
            playerScript.SetupCameraViewport(CalculateViewportRect(i, playerCount, rows));
            //print all the dictionary values
            playerScript.SetupControls(playerControls[i]);
        }
    }

    private void StartSpawner()
    {
        gameObject.AddComponent<Spawner>().Create(enemyPrefab, spawnTime, maxEnemies);
    }
}
