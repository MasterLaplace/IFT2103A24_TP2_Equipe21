using UnityEngine;

public class Movement : Animate
{
    public float moveSpeed = 5f;
    public float tiltAngle = 15f;
    public float jumpForce = 7f;
    public float squashFactor = 0.3f;
    public float stretchDuration = 0.1f;
    public float returnDuration = 0.2f;

    private Rigidbody rb;
    private Vector3 originalScale;
    private bool isGrounded = true;
    private float chargeTime = 0f;
    private readonly float maxChargeTime = 2f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalScale = transform.localScale;
    }

    public override void PerformAnimation(params object[] args)
    {
        if (isGrounded)
        {
            // StartCoroutine(AnimateJump());
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleJump();
        UpdateSquashAndStretch();
    }

    private void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        if (Mathf.Abs(horizontalInput) > 0.01f)
        {
            Vector3 movement = new Vector3(horizontalInput * moveSpeed, rb.velocity.y, 0f);
            rb.velocity = movement;

            // Tilt capsule in the direction of movement
            float tilt = -horizontalInput * tiltAngle;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, tilt), Time.deltaTime * 10f);
        }
        else
        {
            // Reset tilt when not moving
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * 5f);
        }
    }

    private void HandleJump()
    {
        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            chargeTime += Time.deltaTime;
            float squashScale = Mathf.Clamp(chargeTime / maxChargeTime, 0, 1) * squashFactor;
            transform.localScale = new Vector3(originalScale.x + squashScale, originalScale.y - squashScale, originalScale.z + squashScale);
        }

        if (Input.GetKeyUp(KeyCode.Space) && isGrounded)
        {
            float finalJumpForce = jumpForce + (jumpForce * Mathf.Clamp(chargeTime / maxChargeTime, 0, 1));
            rb.AddForce(Vector3.up * finalJumpForce, ForceMode.Impulse);
            chargeTime = 0f;
            isGrounded = false;
        }
    }

    private void UpdateSquashAndStretch()
    {
        if (!isGrounded)
        {
            float stretchScale = Mathf.Clamp(rb.velocity.y / jumpForce, -1, 1) * squashFactor;
            if (stretchScale > 0) // Stretching
            {
                transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(originalScale.x - stretchScale, originalScale.y + stretchScale, originalScale.z - stretchScale), Time.deltaTime * 10f);
            }
            else // Squashing
            {
                transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(originalScale.x + Mathf.Abs(stretchScale), originalScale.y - Mathf.Abs(stretchScale), originalScale.z + Mathf.Abs(stretchScale)), Time.deltaTime * 10f);
            }
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * 5f);
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
