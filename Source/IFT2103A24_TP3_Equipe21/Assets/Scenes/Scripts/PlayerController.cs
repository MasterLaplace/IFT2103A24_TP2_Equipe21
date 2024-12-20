using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private readonly Dictionary<KeyCode, Command> commands = new();
    public float speed = 10.0f;
    public float jumpForce = 10.0f;
    private bool isGrounded = false;

    public void Start()
    {
        isGrounded = true;

        // Assigner les commandes aux touches
        commands[KeyCode.Space] = new JumpCommand(transform);
        commands[KeyCode.W] = new MoveCommand(transform, Vector3.forward);
        commands[KeyCode.S] = new MoveCommand(transform, Vector3.back);
        commands[KeyCode.A] = new MoveCommand(transform, Vector3.left);
        commands[KeyCode.D] = new MoveCommand(transform, Vector3.right);
    }

    public void Update()
    {
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        foreach (var entry in commands)
        {
            if (Input.GetKeyDown(entry.Key))
            {
                Invoker.Instance.AddCommand(entry.Value);
            }
        }

        if (Input.GetKey(KeyCode.W))
        {
            transform.position += forward * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += -forward * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += -right * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += right * speed * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
            isGrounded = false;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
