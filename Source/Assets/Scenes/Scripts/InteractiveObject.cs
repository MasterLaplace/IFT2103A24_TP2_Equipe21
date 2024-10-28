using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    private Renderer objectRenderer;
    public GameObject objectPrefab;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform != transform)
                    return;

                objectRenderer.material.color = Random.ColorHSV();

                GameObject projectile = Instantiate(objectPrefab, Camera.main.transform.position, Quaternion.identity);

                Vector3 direction = hit.point - Camera.main.transform.position;
                float distance = direction.magnitude;
                float force = 10 * distance;

                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                rb.useGravity = true;
                rb.AddForce(direction.normalized * force + Vector3.up * 5, ForceMode.Impulse);

                objectRenderer.material.color = Color.red;

                Debug.Log("Object launched!");
            }
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
    }
}
