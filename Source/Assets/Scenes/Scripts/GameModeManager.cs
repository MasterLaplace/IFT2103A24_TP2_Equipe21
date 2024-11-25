using UnityEngine;
using System.Collections.Generic;

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Instance { get; private set; }

    public bool IsNetworkMode { get; set; } = false;
    public string ServerIP { get; set; } = "127.0.0.1";
    public int ServerPort { get; set; } = 54000;
    public List<PlayerControls> PlayerControls { get; set; }

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
