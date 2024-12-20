using UnityEngine;

public class Transformoule : MonoBehaviour
{
    public Vector3 scale = new(1, 2, 1);

    public void Start()
    {
        // Accéder au Transform attaché à ce GameObject
        Transform cubeTransform = transform;

        // Modifier l'échelle
        cubeTransform.localScale = scale;
    }
}
