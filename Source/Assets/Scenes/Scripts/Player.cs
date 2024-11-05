using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject projectilePrefab;
    [SerializeField] private float velocity = 10;
    [SerializeField] private float projectileVelocity = 40;
    [SerializeField] private float fov = 90;
    [SerializeField] private int health = 100;
    [HideInInspector] public Camera playerCamera;
    [SerializeField] private GameObject SkyBox;
    private Vector3 mapLimit;

    public void SetupCameraViewport(Rect viewport)
    {
        if (fov < 1 || fov > 179)
            throw new System.ArgumentException("Field of view must be between 1 and 179 degrees");

        mapLimit = SkyBox.transform.localScale / 2;

        playerCamera = new GameObject("PlayerCamera").AddComponent<Camera>();
        playerCamera.rect = viewport;
        playerCamera.fieldOfView = fov;
        playerCamera.transform.position = RandomPosition();

        transform.position = playerCamera.transform.position;

        if (Camera.main != null)
            Camera.main.enabled = false;

        if (playerCamera.TryGetComponent<AudioListener>(out var audioListener))
            audioListener.enabled = true;
    }

    void Update()
    {
        HandleMovement();
        HandleShooting();
        SyncPositionWithCamera();
    }

    private void HandleMovement()
    {
        if (Input.GetButton("Fire2"))
        {
            Vector3 direction = playerCamera.transform.forward * velocity;
            playerCamera.transform.position += direction * Time.deltaTime;
        }

        if (Input.GetButton("Fire1"))
        {
            Vector3 direction = playerCamera.transform.forward * velocity;
            playerCamera.transform.position += -direction * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.W))
            playerCamera.transform.position += Vector3.forward * Time.deltaTime;
        if (Input.GetKey(KeyCode.S))
            playerCamera.transform.position += Vector3.back * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
            playerCamera.transform.position += Vector3.left * Time.deltaTime;
        if (Input.GetKey(KeyCode.D))
            playerCamera.transform.position += Vector3.right * Time.deltaTime;

        if (Input.GetMouseButton(1))
        {
            playerCamera.transform.Rotate(Vector3.up, Input.GetAxis("Mouse X"));
            playerCamera.transform.Rotate(Vector3.right, -Input.GetAxis("Mouse Y"));
        }

        float horizontal2 = Input.GetAxis("Horizontal");
        float vertical2 = Input.GetAxis("Vertical");

        if (vertical2 != 0)
            playerCamera.transform.Rotate(Vector3.right, -vertical2 * 100 * Time.deltaTime);
        if (horizontal2 != 0)
            playerCamera.transform.Rotate(Vector3.forward, -horizontal2 * 100 * Time.deltaTime);
    }

    private void HandleShooting()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Joystick1Button5))
        {
            GameObject projectile = Instantiate(projectilePrefab, playerCamera.transform.position + (2 * playerCamera.transform.forward), playerCamera.transform.rotation);

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.velocity = playerCamera.transform.forward * projectileVelocity;
        }
    }

    private void SyncPositionWithCamera()
    {
        transform.position = playerCamera.transform.position;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Debug.Log("Player died");
            Destroy(gameObject);
        }
    }

    private Vector3 RandomPosition()
    {
        float x = Random.Range(-mapLimit.x, mapLimit.x);
        float y = Random.Range(-mapLimit.y, mapLimit.y);
        float z = Random.Range(-mapLimit.z, mapLimit.z);

        return new Vector3(x, y, z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(Camera.main.transform.position, Camera.main.transform.position + Camera.main.transform.forward * 20);
    }
}
