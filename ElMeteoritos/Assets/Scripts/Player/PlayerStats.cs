using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ---> Clase con las estadísticas in-game del jugador, los valores se cambian en el prefab Player usando el editor de Unity (para que no haya que estar modificando el código todo el rato).
public class PlayerStats : MonoBehaviour
{
    [HideInInspector] public PlayerManager playerManager;

    [Header("Stats")]
    public int currentLifes;
    public int maxLifes;
    public ShootType shootType;

    [Header("Estadísticas de partida")]
    public int score;
    public int enemiesDefeated;
    public int shoots;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
    }

    private void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("enemigo"))
        {
            Debug.Log("COSORRO, ME IDENTIFICO COMO CADAVER");
        }
    }

    // ---> Gestionar vidas
    public void ModifyLifes(int amount) // ---> Función para cambiar las vidas del jugador que impide que bajen de 0, que suban más de las máximas y actualiza el panel del jugador.
    {
        currentLifes = Mathf.Clamp(currentLifes + amount, 0, maxLifes);
        PlayerUIManager.instance.UpdatePlayerPanel(playerManager);
    }

    // ---> Gestionar puntuacion
    public IEnumerator ModifyScoreIE (int amount) // ---> Función para cambiar la puntuación del jugador que impide que baje de 0 y actualiza el panel del jugador usando una animación simple.
    {
        int finalScore = Mathf.Max(score + amount, 0);
        while (score != finalScore)
        {
            if (score < finalScore)
            {
                score = Mathf.Max(score + 1, 0);
            }
            else if (score > finalScore)
            {
                score = Mathf.Max(score - 1, 0);
            }
            PlayerUIManager.instance.UpdatePlayerPanel(playerManager);

            yield return new WaitForSeconds(0.05f);
        }
    }
    //public void ModifyScore(int amount)
    //{
    //    score = Mathf.Max(score + amount, 0);
    //    PlayerUIManager.instance.UpdatePlayerPanel(playerManager);
    //}
}
