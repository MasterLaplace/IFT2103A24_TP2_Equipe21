using UnityEngine;
using TMPro; // Nécessaire pour utiliser TMP_InputField

public class PlayerInputFields : MonoBehaviour
{
    public TMP_InputField inputForward;
    public TMP_InputField inputBack;
    public TMP_InputField inputShoot;

    private void Awake()
    {
        // Trouver tous les TMP_InputFields dans les enfants
        TMP_InputField[] inputFields = GetComponentsInChildren<TMP_InputField>();

        Debug.Log($"Nombre de TMP_InputFields trouvés : {inputFields.Length}");

        foreach (TMP_InputField inputField in inputFields)
        {
            Debug.Log($"Nom du TMP_InputField : {inputField.name}");
            switch (inputField.name)
            {
                case "AvanceInputField": // Correspond au nom dans la hiérarchie
                    inputForward = inputField;
                    break;
                case "TirerInputField":
                    inputShoot = inputField;
                    break;
                case "BackInputField":
                    inputBack = inputField;
                    break;
            }
        }

        // Debug pour vérifier les assignations
        Debug.Log($"Forward: {inputForward}, Shoot: {inputShoot}, Back: {inputBack}");

        // Vérification des assignations
        if (inputForward == null || inputShoot == null || inputBack == null)
        {
            Debug.LogError("Certains TMP_InputFields ne sont pas assignés !");
        }
    }

    // Méthode pour récupérer les touches assignées
    public PlayerControls GetPlayerControls()
    {
        return new PlayerControls(
            inputForward.text,
            inputShoot.text,
            inputBack.text
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
