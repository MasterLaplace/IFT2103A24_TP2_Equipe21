// using UnityEngine;
// using UnityEngine.UI;
// using System; // Pour Enum.GetValues()

// public class KeyCodeDropdown : MonoBehaviour
// {
//     public Dropdown dropdown;

//     void Start()
//     {
//         PopulateDropdown();
//     }

//     void PopulateDropdown()
//     {
//         // Clear existing options
//         dropdown.ClearOptions();

//         // Récupère tous les KeyCode
//         KeyCode[] keyCodes = (KeyCode[])Enum.GetValues(typeof(KeyCode));

//         // Convertit les KeyCodes en une liste de strings
//         foreach (KeyCode keyCode in keyCodes)
//         {
//             dropdown.options.Add(new Dropdown.OptionData(keyCode.ToString()));
//         }

//         // Met à jour le Dropdown pour afficher les nouvelles options
//         dropdown.RefreshShownValue();

//         // Abonnez-vous à l'événement pour détecter les changements
//         dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
//     }

//     void OnDropdownValueChanged(int value)
//     {
//         string selectedKeyCode = dropdown.options[value].text;
//         Debug.Log("KeyCode sélectionné : " + selectedKeyCode);
//     }
// }
