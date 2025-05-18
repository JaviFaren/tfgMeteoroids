
#region Conexion
public enum ConnectionStatus
{
    NO_CONNECTED,
    CONNECTING,
    CONNECTED
}
#endregion

#region Partida
public enum MatchState
{
    START_WAVE,
    WAVE,
    END_WAVE
}
public enum WaveType
{
    COMMON_WAVE,
    SPECIAL_WAVE,
    EASTEREGG_WAVE
}
#endregion

#region Jugador
public enum ShootType
{
    DEFAULT,
    PIERCING,
    MACHINE_GUN,
    SHOTGUN
}
#endregion

#region Enemigos
public enum EnemyType
{
    METEOROID_COMMON,
    METEOROID_DIVISIBLE_X2,
    METEOROID_DIVISIBLE_X5,
    METEOROID_ARMORED,
    METEOROID_EXPLOSIVE,
    METEOROID_HEALING,
    OVNI
}
#endregion

#region Menus
// Menu login-registro
public enum LoginMenuState
{
    START,
    LOGIN,
    REGISTER
}

// Menu principal
public enum MainMenuState
{
    START,
    NO_MENU,
    CUSTOMIZATION,
    PLAY,
    SOCIAL,
    SETTINGS
}

// Menu personalizacion
public enum CustomizationMenuState
{
    START,
    SPACESHIP,
    PROPULSION,
    TRAIL,
    SHOT
}
public enum CustomizationField
{
    SPACESHIP_COLOR,
    SPACESHIP_SKIN,
    SHOT_COLOR,
    SHOT_SKIN,
    PROPULSION_COLOR,
    PROPULSION_SKIN,
    TRAIL_COLOR,
    TRAIL_SKIN
}

// Menu jugar
public enum PlayMenuState
{
    START,
    ROOMS,
    IN_ROOM
}

// Menu social
public enum SocialMenuState
{
    START,
    MATCHES_LIST,
    DETAILED_MATCH
}

// Iconos
public enum UIIconType
{
    PLAYER_LIFE,
    PLAYER_DEAD,
    SETTINGS_TOGGLE_ON,
    SETTINGS_TOGGLE_OFF
}
#endregion

#region Utilidad

#endregion