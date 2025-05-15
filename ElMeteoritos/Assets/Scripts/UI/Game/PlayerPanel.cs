using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour
{
    [Header("Nombre")]
    [SerializeField] TextMeshProUGUI playerName;

    [Header("Puntuación")]
    [SerializeField] TextMeshProUGUI playerScore;
    private int prevScore;

    [Header("Vidas")]
    [SerializeField] GameObject playerLivesPanel;
    [SerializeField] List<GameObject> currentPlayerIconsInLivesPanel = new();

    [Header("Iconos")]
    [SerializeField] private GameObject iconPrefab;

    [Header("Jugador asigando")]
    public int playerID;

    public void IniatializePanel(string playerName, int playerLives)
    {
        SetPlayerName(playerName);
        playerScore.text = 0.ToString();
        prevScore = 0;
        UpdateLivesPanel(playerLives, true);
    }

    // ---> Asignar jugador
    public void SetID(int ID)
    {
        playerID = ID;
    }

    // ---> Nombre del jugador
    public void SetPlayerName(string name)
    {
        playerName.text = name;
    }

    // ---> Puntuación del jugador
    public IEnumerator SetPlayerScore(int score)
    {
        while (score != prevScore)
        {
            if (score < prevScore)
            {
                prevScore -= 1;
            }
            else if (score > prevScore)
            {
                prevScore += 1;
            }

            playerScore.text = prevScore.ToString();
            yield return new WaitForSeconds(0.03f);
        }
    }

    // ---> Vidas del jugador
    public void UpdateLivesPanel(int lives, bool reset = false)
    {
        if (reset)
        {
            foreach (var life in currentPlayerIconsInLivesPanel)
                life.SetActive(false);

            currentPlayerIconsInLivesPanel.Clear();
        }

        while (currentPlayerIconsInLivesPanel.Count < lives)
        {
            GameObject life = Instantiate(iconPrefab, playerLivesPanel.transform);
            life.SetActive(false);
            currentPlayerIconsInLivesPanel.Add(life);
        }

        for (int i = 0; i < currentPlayerIconsInLivesPanel.Count; i++)
        {
            GameObject icon = currentPlayerIconsInLivesPanel[i];

            if (i < lives)
            {
                if (!icon.activeSelf)
                {
                    SetupIcon(icon, UIIconType.PLAYER_LIFE);
                    icon.SetActive(true);

                    if (icon.TryGetComponent(out Animator animator))
                    {
                        animator.Play("Idle", 0, 0);
                    }
                }
            }
            else
            {
                icon.SetActive(false);
            }
        }

        if (lives == 0)
        {
            GameObject icon = currentPlayerIconsInLivesPanel[0];
            SetupIcon(icon, UIIconType.PLAYER_DEAD);
            icon.SetActive(true);
        }
    }

    // ---> Iconos
    private void SetupIcon(GameObject icon, UIIconType iconID)
    {
        UIIcon iconConfig = DatabaseManager.Instance.UIIconsDatabse.GetIcon(iconID);

        icon.GetComponent<Image>().sprite = iconConfig.sprite;
        icon.GetComponent<Animator>().runtimeAnimatorController = iconConfig.animator;
    }
}
