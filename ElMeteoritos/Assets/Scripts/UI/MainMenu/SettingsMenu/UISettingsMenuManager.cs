using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISettingsMenuManager : MonoBehaviour
{
    [Header("Sonido")]
    public GameObject musicTG;
    public GameObject fxTG;

    [Header("Controles")]
    public TMP_Dropdown controlsSizeDropdown;
    private readonly Dictionary<string, string> controlsSizeValuesMap = new()
    {
        { "Grande", "Big" },
        { "Pequeño", "Small" },
        { "Mediano", "Medium" }
    };

    private void OnEnable()
    {
        LoadSettings();

        UIMainMenuManager.Instance.EnableNavigationButtons(true);
    }

    public void LoadSettings()
    {
        musicTG.GetComponent<Toggle>().isOn = UserSession.SoundMusic == "Yes";
        fxTG.GetComponent<Toggle>().isOn = UserSession.SoundFX == "Yes";

        UpdateToggleIcon(musicTG);
        UpdateToggleIcon(fxTG);

        string displayText = GetDisplayText(UserSession.ControlsSize);
        int index = controlsSizeDropdown.options.FindIndex(opt => opt.text == displayText);
        controlsSizeDropdown.value = Mathf.Max(index, 0);

        UIMainMenuManager.Instance.EnableNavigationButtons(true);
    }

    public async void SaveSettings()
    {
        string controlsSizeValue = GetMappedSizeValue();

        var userSettings = new SettingsData
        {
            sound_music = IsToggleOn(musicTG) ? "Yes" : "No",
            sound_fx = IsToggleOn(fxTG) ? "Yes" : "No",
            controls_size = controlsSizeValue
        };

        UserSession.SetUserSettingsData(userSettings);
        await PHPManager.Instance.UpdateSettingsAsync(userSettings);
    }

    #region Controls
    private string GetMappedSizeValue()
    {
        string selectedText = controlsSizeDropdown.options[controlsSizeDropdown.value].text;
        return controlsSizeValuesMap.TryGetValue(selectedText, out var value) ? value : "Medium";
    }

    private string GetDisplayText(string storedValue)
    {
        foreach (var pair in controlsSizeValuesMap)
        {
            if (pair.Value == storedValue)
                return pair.Key;
        }
        return "Mediano";
    }
    #endregion

    #region Toggles
    private bool IsToggleOn(GameObject toggleGO)
    {
        return toggleGO.GetComponent<Toggle>().isOn;
    }

    private void UpdateToggleIcon(GameObject toggleGO)
    {
        var toggle = toggleGO.GetComponent<Toggle>();
        var image = toggleGO.GetComponent<Image>();
        var iconType = toggle.isOn
            ? UIIconType.SETTINGS_TOGGLE_ON
            : UIIconType.SETTINGS_TOGGLE_OFF;

        image.sprite = DatabaseManager.Instance.UIIconsDatabse.GetIcon(iconType).sprite;
    }

    public void OnMusicToggleValueChanged()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.Instance.ButtonClick);

        UpdateToggleIcon(musicTG);
    }

    public void OnFXToggleValueChanged()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.Instance.ButtonClick);

        UpdateToggleIcon(fxTG);
    }
    #endregion
}
