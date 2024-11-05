using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    public GameObject projectilePrefab;
    [SerializeField] private float velocity = 10;
    [SerializeField] private float projectileVelocity = 40;
    [SerializeField] private float fov = 90;

    void Start()
    {
        if (fov < 1 || fov > 179)
            throw new System.ArgumentException("Field of view must be between 1 and 179 degrees");

        Camera.main.fieldOfView = fov;
    }

    void Update()
    {
        if (Input.GetButton("Fire2"))
        {
            Vector3 direction = Camera.main.transform.forward * velocity;
            Camera.main.transform.position += direction * Time.deltaTime;
        }

        if (Input.GetButton("Fire1"))
        {
            Vector3 direction = Camera.main.transform.forward * velocity;
            Camera.main.transform.position += -direction * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.W))
            Camera.main.transform.position += Vector3.forward * Time.deltaTime;
        if (Input.GetKey(KeyCode.S))
            Camera.main.transform.position += Vector3.back * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
            Camera.main.transform.position += Vector3.left * Time.deltaTime;
        if (Input.GetKey(KeyCode.D))
            Camera.main.transform.position += Vector3.right * Time.deltaTime;

        if (Input.GetMouseButton(1))
        {
            Camera.main.transform.Rotate(Vector3.up, Input.GetAxis("Mouse X"));
            Camera.main.transform.Rotate(Vector3.right, -Input.GetAxis("Mouse Y"));
        }

        float horizontal2 = Input.GetAxis("Horizontal");
        float vertical2 = Input.GetAxis("Vertical");

        if (vertical2 != 0)
            Camera.main.transform.Rotate(Vector3.right, -vertical2 * 100 * Time.deltaTime);
        if (horizontal2 != 0)
            Camera.main.transform.Rotate(Vector3.forward, -horizontal2 * 100 * Time.deltaTime);

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Joystick1Button5))
        {
            GameObject projectile = Instantiate(projectilePrefab, Camera.main.transform.position, Camera.main.transform.rotation);

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.velocity = Camera.main.transform.forward * projectileVelocity;
            Debug.Log("Projectile launched at " + rb.velocity + " m/s");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(Camera.main.transform.position, Camera.main.transform.position + Camera.main.transform.forward * 20);
    }
}
