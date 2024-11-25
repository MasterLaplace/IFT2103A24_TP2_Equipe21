// using UnityEngine;
// using TMPro; // Nécessaire pour TMP_Dropdown
// using System;

// public class TMPBindingDropdownManager : MonoBehaviour
// {
//     public TMP_Dropdown avanceDropdown;
//     public TMP_Dropdown reculerDropdown;
//     public TMP_Dropdown tirerDropdown;

//     private KeyCode[] keyCodes;

//     void Start()
//     {
//         // Récupérer tous les KeyCodes possibles
//         keyCodes = (KeyCode[])Enum.GetValues(typeof(KeyCode));

//         // Peupler les trois TMP_Dropdowns
//         PopulateDropdown(avanceDropdown);
//         PopulateDropdown(reculerDropdown);
//         PopulateDropdown(tirerDropdown);

//         // Ajouter les listeners pour détecter les choix
//         avanceDropdown.onValueChanged.AddListener(delegate { OnDropdownValueChanged("Avancer", avanceDropdown); });
//         reculerDropdown.onValueChanged.AddListener(delegate { OnDropdownValueChanged("Reculer", reculerDropdown); });
//         tirerDropdown.onValueChanged.AddListener(delegate { OnDropdownValueChanged("Tirer", tirerDropdown); });
//     }

//     void PopulateDropdown(TMP_Dropdown dropdown)
//     {
//         dropdown.ClearOptions();

//         // Ajouter chaque KeyCode comme option
//         foreach (KeyCode keyCode in keyCodes)
//         {
//             dropdown.options.Add(new TMP_Dropdown.OptionData(keyCode.ToString()));
//         }

//         dropdown.RefreshShownValue();
//     }

//     void OnDropdownValueChanged(string actionName, TMP_Dropdown dropdown)
//     {
//         // Récupérer le KeyCode sélectionné
//         string selectedKeyCode = dropdown.options[dropdown.value].text;
//         Debug.Log($"Action '{actionName}' assignée à la touche : {selectedKeyCode}");
//     }
// }
