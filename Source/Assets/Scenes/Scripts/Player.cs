using UnityEngine;
using System.Collections.Generic;

using Flk_API = Flakkari4Unity.API;
using CurrentProtocol = Flakkari4Unity.Protocol.V1;

public class Player : Flakkari4Unity.ECS.Entity
{
    public GameObject projectilePrefab;
    [SerializeField] private float velocity = 10;
    [SerializeField] private float projectileVelocity = 40;
    [SerializeField] private float fov = 90;
    [SerializeField] private int health = 100;
    [SerializeField] private float cooldown = 0.5f;
    private float lastShotTime;
    [HideInInspector] public Camera playerCamera;
    [HideInInspector] private NetworkClient networkClient = null;
    private bool isNetworkClientEnabled = false;
    [SerializeField] private GameObject skyBoxPrefab;
    private Vector3 mapLimit;
    private readonly Dictionary<CurrentProtocol.EventId, CurrentProtocol.EventState> axisEvents = new(4);

    public void SetupCameraViewport(Rect viewport)
    {
        if (fov < 1 || fov > 179)
            throw new System.ArgumentException("Field of view must be between 1 and 179 degrees");

        mapLimit = skyBoxPrefab.transform.localScale / 2;

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
            axisEvents[eventId] = CurrentProtocol.EventState.MAX_STATE;
    }

    public void SetupNetworkClient(NetworkClient networkClient)
    {
        this.networkClient = networkClient;
        isNetworkClientEnabled = true;
    }

    public void FixedUpdate()
    {
        if (isNetworkClientEnabled == false)
            return;

        List<CurrentProtocol.Event> events = new(8);
        Dictionary<CurrentProtocol.EventId, float> axisValues = new(4);

        Net_HandleMovement(networkClient, ref events, ref axisValues);
        Net_HandleShooting(networkClient, ref events);

        if (events.Count > 0 || axisValues.Count > 0)
            networkClient.Send(Flk_API.APIClient.ReqUserUpdates(events, axisValues));
    }

    public void Update()
    {
        HandleMovement();
        HandleShooting();
        SyncPositionWithCamera();
    }

    private void Net_HandleMovement(NetworkClient networkClient, ref List<CurrentProtocol.Event> events, ref Dictionary<CurrentProtocol.EventId, float> axisValues)
    {
        HandleNetworkInput("Fire2", CurrentProtocol.EventId.MOVE_FRONT, ref events);
        HandleNetworkInput("Fire1", CurrentProtocol.EventId.MOVE_BACK, ref events);
        HandleNetworkInput(KeyCode.W, CurrentProtocol.EventId.MOVE_FRONT, ref events);
        HandleNetworkInput(KeyCode.S, CurrentProtocol.EventId.MOVE_BACK, ref events);

        if (Input.GetMouseButton(1))
        {
            HandleMouseMovement("Mouse X", CurrentProtocol.EventId.LOOK_LEFT, CurrentProtocol.EventId.LOOK_RIGHT, ref axisValues);
            HandleMouseMovement("Mouse Y", CurrentProtocol.EventId.LOOK_DOWN, CurrentProtocol.EventId.LOOK_UP, ref axisValues);
        }
        else
        {
            HandleMouseMovement("Horizontal", CurrentProtocol.EventId.LOOK_LEFT, CurrentProtocol.EventId.LOOK_RIGHT, ref axisValues);
            HandleMouseMovement("Vertical", CurrentProtocol.EventId.LOOK_DOWN, CurrentProtocol.EventId.LOOK_UP, ref axisValues);
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

    private void HandleNetworkMouseInput(int keyCode, CurrentProtocol.EventId eventId, ref List<CurrentProtocol.Event> events)
    {
        if (Input.GetMouseButtonDown(keyCode))
        {
            events.Add(new CurrentProtocol.Event { id = eventId, state = CurrentProtocol.EventState.PRESSED });
            Debug.Log("Input: " + keyCode + " EventId: " + eventId + " EventState: " + CurrentProtocol.EventState.PRESSED);
        }

        else if (Input.GetMouseButtonUp(keyCode))
        {
            events.Add(new CurrentProtocol.Event { id = eventId, state = CurrentProtocol.EventState.RELEASED });
            Debug.Log("Input: " + keyCode + " EventId: " + eventId + " EventState: " + CurrentProtocol.EventState.RELEASED);
        }
    }

    private void HandleMouseMovement(string axisName, CurrentProtocol.EventId negativeEventId, CurrentProtocol.EventId positiveEventId , ref Dictionary<CurrentProtocol.EventId, float> axisValues)
    {
        float axisValue = Input.GetAxis(axisName);

        if (axisValue < 0 && axisEvents[negativeEventId] != CurrentProtocol.EventState.PRESSED)
        {
            axisEvents[negativeEventId] = CurrentProtocol.EventState.PRESSED;
            axisEvents[positiveEventId] = CurrentProtocol.EventState.RELEASED;
            axisValues[negativeEventId] = axisValue;
            axisValues[positiveEventId] = 0;
            Debug.Log("Input: " + axisName + " EventId: " + negativeEventId + " EventState: " + CurrentProtocol.EventState.PRESSED + " Value: " + axisValue);
        }
        else if (axisValue > 0 && axisEvents[positiveEventId] != CurrentProtocol.EventState.PRESSED)
        {
            axisEvents[positiveEventId] = CurrentProtocol.EventState.PRESSED;
            axisEvents[negativeEventId] = CurrentProtocol.EventState.RELEASED;
            axisValues[positiveEventId] = axisValue;
            axisValues[negativeEventId] = 0;
            Debug.Log("Input: " + axisName + " EventId: " + positiveEventId + " EventState: " + CurrentProtocol.EventState.PRESSED + " Value: " + axisValue);
        }

        if (axisValue == 0 && axisEvents[negativeEventId] == CurrentProtocol.EventState.PRESSED)
        {
            axisEvents[negativeEventId] = CurrentProtocol.EventState.RELEASED;
            axisValues[negativeEventId] = 0;
            Debug.Log("Input: " + axisName + " EventId: " + negativeEventId + " EventState: " + CurrentProtocol.EventState.RELEASED);
        }
        if (axisValue == 0 && axisEvents[positiveEventId] == CurrentProtocol.EventState.PRESSED)
        {
            axisEvents[positiveEventId] = CurrentProtocol.EventState.RELEASED;
            axisValues[positiveEventId] = 0;
            Debug.Log("Input: " + axisName + " EventId: " + positiveEventId + " EventState: " + CurrentProtocol.EventState.RELEASED);
        }
    }

    private void Net_HandleShooting(NetworkClient networkClient, ref List<CurrentProtocol.Event> events)
    {
        if (Time.time - lastShotTime < cooldown)
            return;

        HandleNetworkMouseInput(0, CurrentProtocol.EventId.SHOOT, ref events);
        HandleNetworkInput(KeyCode.Joystick1Button5, CurrentProtocol.EventId.SHOOT, ref events);
    }

    private void HandleMovement()
    {
        if (Input.GetButton("Fire2")) // move forward
        {
            Vector3 direction = playerCamera.transform.forward * velocity;
            playerCamera.transform.position += direction * Time.deltaTime;
        }

        if (Input.GetButton("Fire1")) // move back
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
        if (Time.time - lastShotTime < cooldown)
            return;

        if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Joystick1Button5))
        {
            lastShotTime = Time.time;

            GameObject projectile = Instantiate(projectilePrefab, playerCamera.transform.position + (2 * playerCamera.transform.forward), playerCamera.transform.rotation);

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.velocity = playerCamera.transform.forward * projectileVelocity;

            DestroyProjectile ds_script = projectile.GetComponent<DestroyProjectile>();
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
}
