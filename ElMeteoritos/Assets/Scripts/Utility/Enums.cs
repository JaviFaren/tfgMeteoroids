using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShootType 
{ 
  NADA, 
  PENETRANTE, 
  METRALLETA, 
  ESCOPETA 
};

public enum EnemyType 
{ 
    NORMAL, 
    DIVISIBLEx2,
    DIVISIBLEx5,
    EXPLOSIVO, 
    BLINDADO, 
    CURATIVO 
};

// ---> Menús
public enum MainMenuState
{
    START,
    CUSTOMIZATION,
    GAME,
    SOCIAL,
    SETTINGS
}

public enum CustomizationMenuCategory
{
    SPACESHIP_COLOR,
    SPACESHIP_SKIN,
    SHOT_COLOR,
    SHOT_SKIN,
    TRAIL_SKIN,
    PROPULSION_SKIN
}

public enum GameMenuState
{
    ROOMS,
    IN_ROOM
}
