using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

// ---> Clase para almacenar y guardar en la base de datos la informaci�n de la partida.
// Al terminar la partida hay que pasar las puntuaciones de los jugadores (PlayerStats).
public class Match : MonoBehaviour
{
    [Header("Jugadores")]
    public List<PlayerManager> players = new();

    [Header("Propiedades")]
    public string matchDate;
    public float matchTime;

    [Header("Estadisticas generales")]
    public int globalScore;
    public int globalShoots;
    public int globalEnemies;  
}
