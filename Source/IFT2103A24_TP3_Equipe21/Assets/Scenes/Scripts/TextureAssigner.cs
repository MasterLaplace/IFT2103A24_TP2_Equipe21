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
                // Material bodyMaterial = t.gameObject.GetComponent<Material>();
                Material bodyMaterial = new(Shader.Find("Standard"))
                {
                    mainTexture = Resources.Load<Texture>("Textures/danslaplace")
                };
                // bodyMaterial.mainTexture = Resources.Load<Texture>("Textures/C6");

                t.gameObject.GetComponent<Renderer>().material = bodyMaterial;
            }
            if (t.gameObject.name == "Head")
            {
                // Material headMaterial = t.gameObject.GetComponent<Material>();
                Material headMaterial = new(Shader.Find("Standard"))
                {
                    mainTexture = Resources.Load<Texture>("Textures/C6")
                };
                // headMaterial.mainTexture = Resources.Load<Texture>("Textures/C6");

                t.gameObject.GetComponent<Renderer>().material = headMaterial;
            }
        }
    }
}
