using UnityEngine;
using TMPro; // Nécessaire pour utiliser TMP_Dropdown

public class PlayerInputFields : MonoBehaviour
{
    public TMP_Dropdown dropdownForward;
    public TMP_Dropdown dropdownBack;
    public TMP_Dropdown dropdownShoot;

    private void Awake()
    {
        // Trouver tous les TMP_Dropdowns dans les enfants
        TMP_Dropdown[] dropdowns = GetComponentsInChildren<TMP_Dropdown>();

        Debug.Log($"Nombre de TMP_Dropdowns trouvés : {dropdowns.Length}");

        foreach (TMP_Dropdown dropdown in dropdowns)
        {
            Debug.Log($"Nom du TMP_Dropdown : {dropdown.name}");
            switch (dropdown.name)
            {
                case "AvanceDropdown": // Correspond au nom dans la hiérarchie
                    dropdownForward = dropdown;
                    break;
                case "TirerDropdown":
                    dropdownShoot = dropdown;
                    break;
                case "BackDropdown":
                    dropdownBack = dropdown;
                    break;
            }
        }

        // Debug pour vérifier les assignations
        Debug.Log($"Forward: {dropdownForward}, Shoot: {dropdownShoot}, Back: {dropdownBack}");

        // Vérification des assignations
        if (dropdownForward == null || dropdownShoot == null || dropdownBack == null)
        {
            Debug.LogError("Certains TMP_Dropdowns ne sont pas assignés !");
        }
    }

    // Méthode pour récupérer les touches assignées
    public PlayerControls GetPlayerControls()
    {
        return new PlayerControls(
            dropdownForward.options[dropdownForward.value].text,
            dropdownShoot.options[dropdownShoot.value].text,
            dropdownBack.options[dropdownBack.value].text
        );
    }
}

// Classe pour représenter les contrôles d'un joueur
public class PlayerControls
{
    public KeyCode forward = KeyCode.W;
    public KeyCode shoot = KeyCode.Space;
    public KeyCode back = KeyCode.S;

    public PlayerControls(string forward, string shoot, string back)
    {
        this.forward = StringToKeyCode(this.forward, forward);
        this.back = StringToKeyCode(this.back, back);
        this.shoot = StringToKeyCode(this.shoot, shoot);
    }

    public static KeyCode StringToKeyCode(KeyCode code, string key)
    {
        try
        {
            Debug.Log($"Key: {key}");
            return (KeyCode)System.Enum.Parse(typeof(KeyCode), key, true);
        }
        catch
        {
            Debug.LogError($"Invalid key string: {key}");
            return code;
        }
    }
}
