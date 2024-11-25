using UnityEngine;
using UnityEngine.UI;
using System;

public class DropdownManager : MonoBehaviour
{
    public Dropdown targetDropdown; // Assignez ici le Dropdown via l'Inspector

    void Start()
    {
        if (targetDropdown != null)
        {
            PopulateDropdown(targetDropdown);
        }
        else
        {
            Debug.LogError("Dropdown non assigné !");
        }
    }

    void PopulateDropdown(Dropdown dropdown)
    {
        // Clear existing options
        dropdown.ClearOptions();

        // Récupère tous les KeyCode
        KeyCode[] keyCodes = (KeyCode[])Enum.GetValues(typeof(KeyCode));

        // Convertit les KeyCodes en une liste de strings
        foreach (KeyCode keyCode in keyCodes)
        {
            dropdown.options.Add(new Dropdown.OptionData(keyCode.ToString()));
        }

        // Met à jour le Dropdown
        dropdown.RefreshShownValue();

        // Ajout listener pour la sélection
        dropdown.onValueChanged.AddListener(value =>
        {
            string selectedKey = dropdown.options[value].text;
            Debug.Log("KeyCode sélectionné : " + selectedKey);
        });
    }
}
