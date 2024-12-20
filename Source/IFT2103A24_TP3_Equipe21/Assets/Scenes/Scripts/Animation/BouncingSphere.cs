using UnityEngine;

public class BouncingSphere : MonoBehaviour
{
    public float anticipationTime = 0.2f; // Durée d'anticipation
    public float jumpHeight = 2.0f;       // Hauteur du saut
    public float jumpSpeed = 1.0f;        // Vitesse de saut
    public float squashFactor = 0.5f;     // Facteur d'écrasement/étirement
    private Vector3 initialScale;         // Échelle initiale de la sphère
    private Vector3 initialPosition;      // Position initiale de la sphère
    private bool isJumping = false;       // Indique si l'objet saute

    public void Start()
    {
        initialScale = transform.localScale;
        initialPosition = transform.position;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();
    }

    public void Jump()
    {
        if (!isJumping)
            StartCoroutine(JumpSequence());
    }

    public bool IsJumping()
    {
        return isJumping;
    }

    System.Collections.IEnumerator JumpSequence()
    {
        isJumping = true;

        float timer = 0f;
        while (timer < anticipationTime)
        {
            timer += Time.deltaTime;
            float t = timer / anticipationTime;

            transform.localScale = Vector3.Lerp(initialScale,
                new Vector3(initialScale.x + squashFactor, initialScale.y - squashFactor, initialScale.z + squashFactor), t);

            transform.position = Vector3.Lerp(initialPosition, initialPosition - new Vector3(0, 0.5f, 0), t);
            yield return null;
        }

        timer = 0f;
        Vector3 jumpApex = initialPosition + new Vector3(0, jumpHeight, 0);

        while (timer < jumpSpeed)
        {
            timer += Time.deltaTime;
            float t = timer / jumpSpeed;

            if (t < 0.5f)
            {
                float stretchT = t / 0.5f;
                transform.localScale = Vector3.Lerp(
                    new Vector3(initialScale.x + squashFactor, initialScale.y - squashFactor, initialScale.z + squashFactor),
                    new Vector3(initialScale.x - squashFactor, initialScale.y + squashFactor, initialScale.z - squashFactor),
                    stretchT);

                transform.position = Vector3.Lerp(initialPosition, jumpApex, stretchT);
            }
            else
            {
                float descendT = (t - 0.5f) / 0.5f;
                transform.localScale = Vector3.Lerp(
                    new Vector3(initialScale.x - squashFactor, initialScale.y + squashFactor, initialScale.z - squashFactor),
                    initialScale,
                    descendT);

                transform.position = Vector3.Lerp(jumpApex, initialPosition, descendT);
            }
            yield return null;
        }

        transform.localScale = initialScale;
        transform.position = initialPosition;
        isJumping = false;
    }
}
