using System.Collections;
using UnityEngine;

public class Jumping : Animate
{
    public float jumpForce = 5f;
    public float squashFactor = 0.5f;
    public float stretchDuration = 0.1f;
    public float returnDuration = 0.2f;

    private Rigidbody rb;
    private Vector3 originalScale;
    private bool isGrounded = true;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalScale = transform.localScale;
    }

    public override void PerformAnimation(params object[] args)
    {
        if (args.Length > 0 && args[0] is float v)
        {
            jumpForce = v;
        }

        if (isGrounded)
        {
            StartCoroutine(AnimateJump());
        }
    }

    private IEnumerator AnimateJump()
    {
        isGrounded = false;

        // Squash phase (anticipation)
        Vector3 squashScale = new(originalScale.x + squashFactor, originalScale.y - squashFactor, originalScale.z + squashFactor);
        float elapsedTime = 0f;
        while (elapsedTime < stretchDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale, squashScale, elapsedTime / stretchDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = squashScale;

        // Apply jump force
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        // Stretch phase
        Vector3 stretchScale = new(originalScale.x - squashFactor, originalScale.y + squashFactor, originalScale.z - squashFactor);
        elapsedTime = 0f;
        while (elapsedTime < stretchDuration)
        {
            transform.localScale = Vector3.Lerp(squashScale, stretchScale, elapsedTime / stretchDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = stretchScale;

        // Return to original scale
        elapsedTime = 0f;
        while (elapsedTime < returnDuration)
        {
            transform.localScale = Vector3.Lerp(stretchScale, originalScale, elapsedTime / returnDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = originalScale;

        // Wait until grounded
        while (!isGrounded)
        {
            yield return null;
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
