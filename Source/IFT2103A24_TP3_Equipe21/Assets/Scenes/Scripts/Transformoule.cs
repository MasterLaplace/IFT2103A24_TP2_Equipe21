using UnityEngine;

public class Transformoule : MonoBehaviour
{
    public void Start()
    {
        // Accéder au Transform attaché à ce GameObject
        Transform cubeTransform = transform;

        // Modifier la rotation (en degrés)
        cubeTransform.rotation = Quaternion.Euler(45, 30, 0);

        // Modifier l'échelle
        cubeTransform.localScale = new Vector3(1, 2, 1);
    }
}
