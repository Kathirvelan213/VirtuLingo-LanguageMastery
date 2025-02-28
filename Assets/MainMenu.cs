using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class LanguageMenu : MonoBehaviour
{
    public TMP_Dropdown languageDropdown;
    public TMP_Dropdown environmentDropdown;
    void Start()
    {
        List<string> languages = new List<string> {"English", "Telugu","Hindi", "Tamil" };
        languageDropdown.ClearOptions();
        languageDropdown.AddOptions(languages);
        languageDropdown.onValueChanged.AddListener(delegate { SetLanguage(); });

        List<string> environment = new List<string> { "Restaurant", "Supermarket" };
        environmentDropdown.ClearOptions();
        environmentDropdown.AddOptions(environment);
        environmentDropdown.onValueChanged.AddListener(delegate { set_environment(); });
    }

    public void SetLanguage()
    {
        int selectedLanguageIndex = languageDropdown.value;
        string selectedLanguage = languageDropdown.options[selectedLanguageIndex].text;

        PlayerPrefs.SetString("SelectedLanguage", selectedLanguage);
        PlayerPrefs.Save();

        Debug.Log("Selected Language: " + selectedLanguage);
    }
    public void set_environment()
    {
        int selectedenvironmentindex= environmentDropdown.value;
        string selectedenvironment= environmentDropdown.options[selectedenvironmentindex].text;
        PlayerPrefs.SetString("selectedenvironment",selectedenvironment);
        PlayerPrefs.Save();
        Debug.Log("selected environment" + selectedenvironment);
    }

}
