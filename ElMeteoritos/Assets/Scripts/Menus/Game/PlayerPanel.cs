using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

// ---> Clase que controla el panel de nombre, puntuación y vidas del jugador.
// Se gestiona completamente mediante el PlayerUIManager.
public class PlayerPanel : MonoBehaviour
{
    [Header("Nombre")]
    [SerializeField] TextMeshProUGUI playerName;

    [Header("Puntuación")]
    [SerializeField] TextMeshProUGUI playerScore;

    [Header("Vidas")]
    [SerializeField] GameObject playerLifePrefab;
    [SerializeField] GameObject playerLifesPanel;
    [SerializeField] List<GameObject> currentPlayerLifes = new();

    [Header("Jugador asigando")]
    public int playerID;
    PlayerManager currentPlayer;

    public void IniatializePanel()
    {
        currentPlayer = GetPlayerByID();
        SetPlayerName(currentPlayer.username);
        SetPlayerScore(currentPlayer.playerStats.score);
        InitializeLifesPanel(currentPlayer.playerStats.currentLifes);
    }

    // ---> Asignar jugador
    private PlayerManager GetPlayerByID()
    {
        return GameController.instance.playersList.FirstOrDefault(e => e.userID == playerID);
    }
    public void SetID(int ID)
    {
        playerID = ID;
    }

    // ---> Nombre del jugador
    public void SetPlayerName (string name)
    {
        playerName.text = name;
    }

    // ---> Puntuación del jugador
    public void SetPlayerScore (int score)
    {
        playerScore.text = score.ToString();
    }

    // ---> Vidas del jugador
    public void InitializeLifesPanel(int lifes)
    {
        foreach (var life in currentPlayerLifes)
        {
            Destroy(life);
        }
        currentPlayerLifes.Clear();

        for (int i = 0; i < lifes; i++)
        {
            GameObject currentLife = Instantiate(playerLifePrefab, playerLifesPanel.transform, true);
            currentPlayerLifes.Add(currentLife);
        }
    }
    public void UpdateLifesPanel(int lifes)
    {
        while (lifes > currentPlayerLifes.Count)
        {
            GameObject currentLife = Instantiate(playerLifePrefab, playerLifesPanel.transform, true);
            currentPlayerLifes.Add(currentLife);
        }

        while (lifes < currentPlayerLifes.Count)
        {
            GameObject currentLife = currentPlayerLifes[0];
            currentPlayerLifes.Remove(currentLife);
            Destroy(currentLife);
        }
    }
}
