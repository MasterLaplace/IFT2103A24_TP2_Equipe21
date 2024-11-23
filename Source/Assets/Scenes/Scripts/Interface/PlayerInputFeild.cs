using UnityEngine;
using UnityEngine.UI;

public class PlayerInputFields : MonoBehaviour
{
    public InputField inputForward;
    public InputField inputLeft;
    public InputField inputRight;
    public InputField inputShoot;
    private void Start()
    {
    // Trouver tous les InputFields dans les enfants
        InputField[] inputFields = GetComponentsInChildren<InputField>();

        foreach (InputField inputField in inputFields)
        {
            switch (inputField.name)
            {
                case "AvanceInputFeild":
                    inputForward = inputField;
                    break;
                case "GaucheInputFeild":
                    inputLeft = inputField;
                    break;
                case "DroiteInputFeild":
                    inputRight = inputField;
                    break;
                case "TirerInputFeild":
                    inputShoot = inputField;
                    break;
            }
        }

        // Debug pour vérifier les assignations
        Debug.Log($"Forward: {inputForward}, Left: {inputLeft}, Right: {inputRight}, Shoot: {inputShoot}");

        // Si l'un des champs est null, affiche une erreur
        if (inputForward == null || inputLeft == null || inputRight == null || inputShoot == null)
        {
            Debug.LogError("Certains InputFields ne sont pas assignés !");
        }
    }
    // Méthode pour récupérer les touches assignées
    public PlayerControls GetPlayerControls()
    {
        return new PlayerControls(
            inputForward.text,
            inputLeft.text,
            inputRight.text,
            inputShoot.text
        );
    }
}

// Classe pour représenter les contrôles d'un joueur
public class PlayerControls
{
    public string forward;
    public string left;
    public string right;
    public string shoot;

    public PlayerControls(string forward, string left, string right, string shoot)
    {
        this.forward = forward;
        this.left = left;
        this.right = right;
        this.shoot = shoot;
    }
}

