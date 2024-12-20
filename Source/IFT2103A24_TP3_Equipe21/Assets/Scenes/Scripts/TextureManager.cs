using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
public class TextureManager : MonoBehaviour
{
    public GameObject buttonPrefab; // Le prefab du bouton
    public Transform headTexturePanel; // Panel pour les textures de tête
    public Transform bodyTexturePanel; // Panel pour les textures de corps
    public Renderer headRenderer; // Renderer pour la tête
    public Renderer bodyRenderer; // Renderer pour le corps

    private void Start()
    {
        LoadTextures("Materials", headTexturePanel, headRenderer);
        LoadTextures("Materials", bodyTexturePanel, bodyRenderer);
    }

    void LoadTextures(string folderPath, Transform panel, Renderer targetRenderer)
    {
        var materials = Resources.LoadAll<Material>(folderPath);

        Debug.Log("Materials found: " + materials.Length);

        foreach (var material in materials)
        {
            if (material.mainTexture == null)
            {
                Debug.LogWarning("Material " + material.name + " has no texture.");
                continue;
            }

            GameObject button = Instantiate(buttonPrefab, panel);
            if (button == null)
            {
                Debug.LogError("Failed to instantiate button prefab.");
                return;
            }

            var texture = (Texture2D)material.mainTexture;
            button.GetComponent<Image>().sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                Vector2.zero
            );

            // Trouver le composant Text dans le bouton pour afficher le nom
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>(); // Utilise TextMeshPro si tu utilises TMP
            if (buttonText != null)
            {
                buttonText.text = material.name; // Afficher le nom du matériau
            }

            button.GetComponent<Button>().onClick.AddListener(() => ApplyTexture(targetRenderer, material));
        }
    }



    void ApplyTexture(Renderer renderer, Material material)
    {
        renderer.material = material;
    }
}
