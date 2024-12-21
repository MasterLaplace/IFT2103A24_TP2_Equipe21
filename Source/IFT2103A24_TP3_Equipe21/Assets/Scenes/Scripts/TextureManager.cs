using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextureManager : MonoBehaviour
{
    public GameObject buttonPrefab; // Le prefab du bouton
    public Transform headTexturePanel; // Panel pour les textures de tête
    public Transform bodyTexturePanel; // Panel pour les textures de corps

    public void Start()
    {
        LoadTextures("Materials", headTexturePanel, "Head");
        LoadTextures("Materials", bodyTexturePanel, "Body");
    }

    private void LoadTextures(string folderPath, Transform panel, string type)
    {
        var materials = Resources.LoadAll<Material>(folderPath);

        foreach (var material in materials)
        {
            if (material.mainTexture == null)
            {
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
                buttonText.text = "Texture for "+ type + ": " + material.name; // Afficher le nom du matériau
            }

            button.GetComponent<Button>().onClick.AddListener(() => ApplyTexture(type, material));
        }
    }

    void ApplyTexture(string type, Material material)
    {
        PlayerDetailsManager.Instance.AddMaterial(type, material);
    }
}
