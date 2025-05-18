using System.Threading.Tasks;
using UnityEngine;

public static class UserSession
{
    #region USER
    public static int Id => PlayerPrefs.GetInt("user_id", -1);
    public static string Name => PlayerPrefs.GetString("user_name", "");
    public static string Email => PlayerPrefs.GetString("user_email", "");
    public static string SessionToken => PlayerPrefs.GetString("user_session_token", "");

    public static void SetUserData(UserData userData)
    {
        PlayerPrefs.SetInt("user_id", userData.id);
        PlayerPrefs.SetString("user_name", userData.name);
        PlayerPrefs.SetString("user_email", userData.email);
        PlayerPrefs.SetString("user_session_token", userData.session_token);
        PlayerPrefs.Save();
    }
    #endregion

    #region CUSTOMIZATION
    // Nave
    public static string SpaceshipColor => PlayerPrefs.GetString("user_customization_spaceship_color", "");
    public static int SpaceshipSkin => PlayerPrefs.GetInt("user_customization_spaceship_skin", -1);
    // Propulsion
    public static string PropulsionColor => PlayerPrefs.GetString("user_customization_propulsion_color", "");
    public static int PropulsionSkin => PlayerPrefs.GetInt("user_customization_propulsion_skin", -1);
    // Estela
    public static string TrailColor => PlayerPrefs.GetString("user_customization_trail_color", "");
    public static int TrailSkin => PlayerPrefs.GetInt("user_customization_trail_skin", -1);
    // Disparo
    public static string ShotColor => PlayerPrefs.GetString("user_customization_shot_color", "");
    public static int ShotSkin => PlayerPrefs.GetInt("user_customization_shot_skin", -1);

    public static void SetUserCustomizationData(CustomizationData customizationData)
    {
        PlayerPrefs.SetString("user_customization_spaceship_color", customizationData.spaceship_color);
        PlayerPrefs.SetInt("user_customization_spaceship_skin", customizationData.spaceship_skin);
        PlayerPrefs.SetString("user_customization_propulsion_color", customizationData.propulsion_color);
        PlayerPrefs.SetInt("user_customization_propulsion_skin", customizationData.propulsion_skin);
        PlayerPrefs.SetString("user_customization_trail_color", customizationData.trail_color);
        PlayerPrefs.SetInt("user_customization_trail_skin", customizationData.trail_skin);
        PlayerPrefs.SetString("user_customization_shot_color", customizationData.shot_color);
        PlayerPrefs.SetInt("user_customization_shot_skin", customizationData.shot_skin);
        PlayerPrefs.Save();
    }
    public static async void SetUserCustomizationValue(CustomizationField field, object value)
    {
        string key = field.ToString().ToLower();
        switch (value)
        {
            case string strValue:
                Debug.Log($"user_customization_{key} -> {strValue}");
                PlayerPrefs.SetString("user_customization_" + key, strValue);
                break;

            case int intValue:
                Debug.Log($"user_customization_{key} -> {intValue}");
                PlayerPrefs.SetInt("user_customization_" + key, intValue);
                break;

            default:
                Debug.LogError("Tipo de valor no valido para: " + key);
                return;
        }
        PlayerPrefs.Save();

        await PHPManager.Instance.UpdateCustomizationFieldAsync(key, value.ToString());
    }
    #endregion

    #region Settings
    public static string SoundMusic => PlayerPrefs.GetString("user_settings_sound_music", "Yes");
    public static string SoundFX => PlayerPrefs.GetString("user_settings_sound_fx", "Yes");
    public static string ControlsSize => PlayerPrefs.GetString("user_settings_controls_size", "Big");

    public static void SetUserSettingsData(SettingsData userSettings)
    {
        PlayerPrefs.SetString("user_settings_sound_music", userSettings.sound_music);
        PlayerPrefs.SetString("user_settings_sound_fx", userSettings.sound_fx);
        PlayerPrefs.SetString("user_settings_controls_size", userSettings.controls_size);
        PlayerPrefs.Save();
    }
    #endregion

    public static void Clear()
    {
        // Usuario
        PlayerPrefs.DeleteKey("user_id");
        PlayerPrefs.DeleteKey("user_name");
        PlayerPrefs.DeleteKey("user_email");
        PlayerPrefs.DeleteKey("user_session_token");
        // Personalizacion
        PlayerPrefs.DeleteKey("user_customization_spaceship_color");
        PlayerPrefs.DeleteKey("user_customization_spaceship_skin");
        PlayerPrefs.DeleteKey("user_customization_shot_color");
        PlayerPrefs.DeleteKey("user_customization_shot_skin");
        PlayerPrefs.DeleteKey("user_customization_propulsion_color");
        PlayerPrefs.DeleteKey("user_customization_propulsion_skin");
        PlayerPrefs.DeleteKey("user_customization_trail_color");
        PlayerPrefs.DeleteKey("user_customization_trail_skin");
        // Ajustes
        PlayerPrefs.DeleteKey("user_settings_sound_music");
        PlayerPrefs.DeleteKey("user_settings_sound_fx");
        PlayerPrefs.DeleteKey("user_settings_controls_size");

        PlayerPrefs.Save();
    }
}

