
using System;
using System.Collections.Generic;


[Serializable]
public class BaseResponse
{
    public bool success;
    public string message;
    public string code;
}

#region Usuario
[Serializable]
public class UserData
{
    public int id;
    public string name;
    public string email;
    public string session_token;
}

[Serializable]
public class LoginResponse : BaseResponse
{
    public UserData user;
}
#endregion

#region Personalizacion
[Serializable]
public class CustomizationData
{
    public string spaceship_color;
    public string propulsion_color;
    public string trail_color;
    public string shot_color;
    public int spaceship_skin;
    public int propulsion_skin;
    public int trail_skin;
    public int shot_skin;
}

[Serializable]
public class CustomizationResponse : BaseResponse
{
    public CustomizationData customization;
}
#endregion

#region Marcadores
[System.Serializable]
public class MatchData
{
    public int id_player1, id_player2, id_player3, id_player4;
    public int score_player1, score_player2, score_player3, score_player4;
    public int score_total;
    public string date;
    public int waves;
    public int shots_fired;
    public int obtained_upgrades;
    public int obstacles_destroyed;
    public string name_player1, name_player2, name_player3, name_player4;
}

[System.Serializable]
public class MatchesResponse
{
    public bool success;
    public string message;
    public string code;
    public List<MatchData> games;
}
#endregion

#region Ajustes
[Serializable]
public class SettingsData
{
    public string sound_music;
    public string sound_fx;
    public string controls_size;
}

[Serializable]
public class SettingsResponse : BaseResponse
{
    public SettingsData settings;
}
#endregion