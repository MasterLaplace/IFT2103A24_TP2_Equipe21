using UnityEngine;
using System.Collections.Generic;

using Flk_API = Flakkari4Unity.API;
using CurrentProtocol = Flakkari4Unity.Protocol.V1;

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
    private NetworkClient networkClient;
    private readonly Dictionary<CurrentProtocol.EventId, CurrentProtocol.EventState> axisEvents = new(4);

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

        foreach (CurrentProtocol.EventId eventId in System.Enum.GetValues(typeof(CurrentProtocol.EventId)))
            axisEvents[eventId] = CurrentProtocol.EventState.NONE;

        networkClient = FindObjectOfType<NetworkClient>();

        if (networkClient == null || !networkClient.Enable)
            Debug.LogError("NetworkClient is not initialized or not enabled.");
    }

    void Update()
    {
        if (networkClient != null && networkClient.Enable)
        {
            Net_HandleMovement(networkClient);
            Net_HandleShooting(networkClient);
        }

        HandleMovement();
        HandleShooting();
        SyncPositionWithCamera();
    }

    private void Net_HandleMovement(NetworkClient networkClient)
    {
        List<CurrentProtocol.Event> events = new(8);

        HandleNetworkInput("Fire2", CurrentProtocol.EventId.MOVE_FRONT, ref events);
        HandleNetworkInput("Fire1", CurrentProtocol.EventId.MOVE_BACK, ref events);
        HandleNetworkInput(KeyCode.W, CurrentProtocol.EventId.MOVE_FRONT, ref events);
        HandleNetworkInput(KeyCode.S, CurrentProtocol.EventId.MOVE_BACK, ref events);

        if (Input.GetMouseButton(1))
        {
            HandleMouseMovement("Mouse X", CurrentProtocol.EventId.LOOK_LEFT, CurrentProtocol.EventId.LOOK_RIGHT, ref events);
            HandleMouseMovement("Mouse Y", CurrentProtocol.EventId.LOOK_DOWN, CurrentProtocol.EventId.LOOK_UP, ref events);
        }
        else
        {
            HandleMouseMovement("Horizontal", CurrentProtocol.EventId.LOOK_LEFT, CurrentProtocol.EventId.LOOK_RIGHT, ref events);
            HandleMouseMovement("Vertical", CurrentProtocol.EventId.LOOK_DOWN, CurrentProtocol.EventId.LOOK_UP, ref events);
        }

    }

    private void HandleNetworkInput(string inputName, CurrentProtocol.EventId eventId, ref List<CurrentProtocol.Event> events)
    {
        if (Input.GetButtonDown(inputName))
        {
            events.Add(new CurrentProtocol.Event { id = eventId, state = CurrentProtocol.EventState.PRESSED });
            Debug.Log("Input: " + inputName + " EventId: " + eventId + " EventState: " + CurrentProtocol.EventState.PRESSED);
        }

        else if (Input.GetButtonUp(inputName))
        {
            events.Add(new CurrentProtocol.Event { id = eventId, state = CurrentProtocol.EventState.RELEASED });
            Debug.Log("Input: " + inputName + " EventId: " + eventId + " EventState: " + CurrentProtocol.EventState.RELEASED);
        }
    }

    private void HandleNetworkInput(KeyCode keyCode, CurrentProtocol.EventId eventId, ref List<CurrentProtocol.Event> events)
    {
        if (Input.GetKeyDown(keyCode))
        {
            events.Add(new CurrentProtocol.Event { id = eventId, state = CurrentProtocol.EventState.PRESSED });
            Debug.Log("Input: " + keyCode + " EventId: " + eventId + " EventState: " + CurrentProtocol.EventState.PRESSED);
        }

        else if (Input.GetKeyUp(keyCode))
        {
            events.Add(new CurrentProtocol.Event { id = eventId, state = CurrentProtocol.EventState.RELEASED });
            Debug.Log("Input: " + keyCode + " EventId: " + eventId + " EventState: " + CurrentProtocol.EventState.RELEASED);
        }
    }

    private void HandleMouseMovement(string axisName, CurrentProtocol.EventId negativeEventId, CurrentProtocol.EventId positiveEventId, ref List<CurrentProtocol.Event> events)
    {
        float axisValue = Input.GetAxis(axisName);

        if (axisValue < 0 && axisEvents[negativeEventId] != CurrentProtocol.EventState.PRESSED)
        {
            axisEvents[negativeEventId] = CurrentProtocol.EventState.PRESSED;
            axisEvents[positiveEventId] = CurrentProtocol.EventState.RELEASED;
            events.Add(new CurrentProtocol.Event { id = negativeEventId, state = CurrentProtocol.EventState.PRESSED });
            Debug.Log("Input: " + axisName + " EventId: " + negativeEventId + " EventState: " + CurrentProtocol.EventState.PRESSED);
        }
        else if (axisValue > 0 && axisEvents[positiveEventId] != CurrentProtocol.EventState.PRESSED)
        {
            axisEvents[positiveEventId] = CurrentProtocol.EventState.PRESSED;
            axisEvents[negativeEventId] = CurrentProtocol.EventState.RELEASED;
            events.Add(new CurrentProtocol.Event { id = positiveEventId, state = CurrentProtocol.EventState.PRESSED });
            Debug.Log("Input: " + axisName + " EventId: " + positiveEventId + " EventState: " + CurrentProtocol.EventState.PRESSED);
        }

        if (axisValue == 0 && axisEvents[negativeEventId] == CurrentProtocol.EventState.PRESSED)
        {
            axisEvents[negativeEventId] = CurrentProtocol.EventState.RELEASED;
            events.Add(new CurrentProtocol.Event { id = negativeEventId, state = CurrentProtocol.EventState.RELEASED });
            Debug.Log("Input: " + axisName + " EventId: " + negativeEventId + " EventState: " + CurrentProtocol.EventState.RELEASED);
        }
        if (axisValue == 0 && axisEvents[positiveEventId] == CurrentProtocol.EventState.PRESSED)
        {
            axisEvents[positiveEventId] = CurrentProtocol.EventState.RELEASED;
            events.Add(new CurrentProtocol.Event { id = positiveEventId, state = CurrentProtocol.EventState.RELEASED });
            Debug.Log("Input: " + axisName + " EventId: " + positiveEventId + " EventState: " + CurrentProtocol.EventState.RELEASED);
        }
    }

    private void Net_HandleShooting(NetworkClient networkClient)
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Joystick1Button5))
        {
            byte[] packet = Flk_API.APIClient.ReqUserUpdate(CurrentProtocol.EventId.SHOOT, CurrentProtocol.EventState.RELEASED);
            networkClient.Send(packet);
        }
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

        if (Input.GetMouseButton(1))
        {
            playerCamera.transform.Rotate(Vector3.up, Input.GetAxis("Mouse X"));
            playerCamera.transform.Rotate(Vector3.right, -Input.GetAxis("Mouse Y"));
        }

        float vertical2 = Input.GetAxis("Vertical");

        if (vertical2 != 0)
            playerCamera.transform.Rotate(Vector3.right, -vertical2 * 100 * Time.deltaTime);

        float horizontal2 = Input.GetAxis("Horizontal");

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
