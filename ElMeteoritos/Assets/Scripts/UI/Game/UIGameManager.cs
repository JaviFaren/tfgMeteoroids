using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terresquall;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGameManager : MonoBehaviour
{
    public static UIGameManager Instance { get; private set; }

    [Header("Paneles")]
    public GameObject matchInfoPanel;
    public GameObject playersInfoPanel;
    public GameObject playerControlsPanel;
    public GameObject popUpPanel;
    public GameObject endMatchPanel;

    [Header("Botones")]
    public Button shootBTN;
    public Button menuPopUpBTN;

    [Header("Sliders")]
    public Slider speedSlider;

    [Header("Joystick")]
    public VirtualJoystick virtualJoystick;

    [Header("Imagenes")]
    public Image shootButtonFill;

    [Header("Textos")]
    public TextMeshProUGUI waveTMP;
    public TextMeshProUGUI endMatchCountdownTMP;

    [Header("Paneles de jugador")]
    [SerializeField] private GameObject playerPanelPrefab;
    public List<GameObject> currentPlayersPanels = new();

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    #region Paneles de informacion de los jugadores
    public void InitilizePlayerPanel(int playerID, string playerName, int playerLives)
    {
        GameObject newPanel = Instantiate(playerPanelPrefab, playersInfoPanel.transform);
        if (newPanel.TryGetComponent(out PlayerPanel panel))
        {
            panel.SetID(playerID);
            panel.IniatializePanel(playerName, playerLives);
            currentPlayersPanels.Add(newPanel);
        }
    }

    public void ManagePlayersPanel(bool active = true)
    {
        playersInfoPanel.SetActive(active);
    }

    public void UpdatePlayerPanelLives(int playerID, int lives)
    {
        var panelToUpdate = currentPlayersPanels.FirstOrDefault(p => p.TryGetComponent(out PlayerPanel pp) && pp.playerID == playerID);
        if (panelToUpdate != null && panelToUpdate.TryGetComponent(out PlayerPanel panel))
        {
            panel.UpdateLivesPanel(lives);
        }
    }

    public void UpdatePlayerPanelScore(int playerID, int score)
    {
        var panelToUpdate = currentPlayersPanels.FirstOrDefault(p => p.TryGetComponent(out PlayerPanel pp) && pp.playerID == playerID);
        if (panelToUpdate != null && panelToUpdate.TryGetComponent(out PlayerPanel panel))
        {
            StartCoroutine(panel.SetPlayerScore(score));
        }
    }
    #endregion

    #region Textos
    public IEnumerator WaveStarterText(int waveNumber)
    {
        waveTMP.fontSharedMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 0.7f);
        waveTMP.gameObject.SetActive(true);

        string waveText = $"OLEADA {waveNumber}";
        StringBuilder builder = new();

        foreach (char c in waveText)
        {
            builder.Append(c);
            waveTMP.text = builder.ToString();
            yield return new WaitForSeconds(0.3f);
        }

        yield return new WaitForSeconds(1);

        for (int i = builder.Length - 1; i >= 0; i--)
        {
            builder.Length = i;
            waveTMP.text = builder.ToString();
            yield return new WaitForSeconds(0.1f);
        }

        waveTMP.gameObject.SetActive(false);
        waveTMP.fontSharedMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 0);
    }

    public IEnumerator EndMatchCountdownText()
    {
        endMatchPanel.SetActive(true);

        for (int i = 5; i >= 0; i--)
        {
            string text = $"Saliendo en {i}...";
            endMatchCountdownTMP.text = text;

            yield return new WaitForSeconds(1f);
        }

        endMatchPanel.SetActive(false);
    }
    #endregion
}
