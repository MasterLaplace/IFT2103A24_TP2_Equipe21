using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public int playerCount = 5;
    public int rows = 2;

    void Start()
    {
        SpawnPlayers();
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

    private void SpawnPlayers()
    {
        for (int i = 0; i < playerCount; i++)
        {
            GameObject player = Instantiate(playerPrefab, new Vector3(i * 2 - (playerCount - 1), 1, 0), Quaternion.identity);
            Player playerScript = player.GetComponent<Player>();
            playerScript.SetupCameraViewport(CalculateViewportRect(i, playerCount, rows));
        }
    }
}
