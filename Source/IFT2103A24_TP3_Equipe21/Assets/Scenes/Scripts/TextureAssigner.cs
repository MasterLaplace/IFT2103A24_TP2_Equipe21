using UnityEngine;

public class TextureAssigner : MonoBehaviour
{
    public void Start()
    {
        Transform[] transforms = GetComponentsInChildren<Transform>();
        foreach (Transform t in transforms)
        {
            if (t.gameObject.name == "Body")
            {
                Material bodyMaterial = t.gameObject.GetComponent<Renderer>().material;

                bodyMaterial.mainTexture = Resources.Load<Texture>("Textures/C6");
            }
            if (t.gameObject.name == "Head")
            {
                Material headMaterial = t.gameObject.GetComponent<Renderer>().material;

                headMaterial.mainTexture = Resources.Load<Texture>("Textures/danslaplace");
            }
        }
    }
}
