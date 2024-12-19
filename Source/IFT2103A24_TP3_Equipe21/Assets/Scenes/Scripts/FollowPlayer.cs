using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public GameObject playerPrefab = null;
    public Vector3 offset = new(0, 1, -1);
    public float rotationSpeed = 5.0f;
    private Vector3 currentRotation;

    public void Update()
    {
        if (playerPrefab == null && (playerPrefab = GameObject.FindWithTag("Player")) == null)
            return;

        float horizontal = Input.GetAxis("Mouse X") * rotationSpeed;
        float vertical = -Input.GetAxis("Mouse Y") * rotationSpeed;

        currentRotation.y += horizontal;
        currentRotation.x += vertical;
        currentRotation.x = Mathf.Clamp(currentRotation.x, -65, 20); // Limit vertical rotation

        Quaternion rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0);
        transform.position = playerPrefab.transform.position + rotation * offset;
        transform.LookAt(playerPrefab.transform.position);
    }
}
