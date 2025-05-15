
using System;


[Serializable]
public class BaseResponse
{
    public bool success;
    public string message;
    public string code;
}

// ---> Usuario
[Serializable]
public class UserData
{
    public int id;
    public string name;
    public string email;
    public string session_token;
}

[Serializable]
public class LoginResponse
{
    public bool success;
    public string message;
    public string code;
    public UserData user;
}

// ---> Personalizacion
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
public class CustomizationResponse
{
    public bool success;
    public string message;
    public string code;
    public CustomizationData customization;
}