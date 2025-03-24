using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Terresquall;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// ---> Clase que controla la interfaz del usuario in-game.
public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager instance;

    [Header("Flags")]
    public bool animating;

    [Header("Botones")]
    public Button shootButton;
    public GameObject deathScreen;

    [Header("Sliders")]
    public Slider speedSlider;

    [Header("Joystick")]
    public VirtualJoystick joystick;

    [Header("Imágenes")]
    public Image heatBar;

    [Header("Paneles")]
    public GameObject playersInfoPanel;

    [Header("Textos")]   
    public TextMeshProUGUI waveText;

    [Header("Gestión de los paneles de información")]
    [SerializeField] GameObject playerPanelPrefab;
    public List<GameObject> currentPlayersPanels = new();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // ---> Gestión de los paneles de información de los jugadores
    public void InitializePlayersPanel() 
    {
        // ---> Se limpia la interfaz de los paneles
        foreach (var playerPanel in currentPlayersPanels)
        {
            Destroy(playerPanel);
        }
        currentPlayersPanels.Clear();

        // ---> Se añaden paneles en función de los jugadores de la partida
        for (int i = 0; i < GameController.instance.playersList.Count; i++)
        {
            GameObject newPlayerPanel = Instantiate(playerPanelPrefab, playersInfoPanel.transform, true);
            newPlayerPanel.GetComponent<PlayerPanel>().SetID(GameController.instance.playersList[i].userID);
            newPlayerPanel.GetComponent<PlayerPanel>().IniatializePanel();
            currentPlayersPanels.Add(newPlayerPanel);
        }
    }
    public void ManagePlayersPanel(bool active = true)
    {
        switch (active)
        {
            case true:
                playersInfoPanel.SetActive(true);
                break;
            case false:
                playersInfoPanel.SetActive(false);
                break;
        }
    }
    public void UpdatePlayerPanel(PlayerManager player) // ---> Función para actualizar la información de los jugadores, se llama desde PlayerStats para que solo actualice la de ese jugador. Se puede dividir en dos funciones para actualizar la vida y la puntuación individualmente.
    {
        GameObject panelToUpdate = currentPlayersPanels.FirstOrDefault(e => e.GetComponent<PlayerPanel>().playerID == player.userID);
        panelToUpdate.GetComponent<PlayerPanel>().SetPlayerScore(player.playerStats.score);
        panelToUpdate.GetComponent<PlayerPanel>().UpdateLifesPanel(player.playerStats.currentLifes);
    }

    // ---> Gestión del texto de las oleadas
    public IEnumerator WaveStarterText(int waveNumber)
    {
        animating = true;

        waveText.fontSharedMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 0.7f);
        waveText.gameObject.SetActive(true);
        string oleadaNum = "OLEADA " + waveNumber;
        for (int i = 0; i < oleadaNum.Length; i++)
        {
            waveText.text = waveText.text + oleadaNum[i];
            yield return new WaitForSeconds(0.3f);
        }
        yield return new WaitForSeconds(1);


        for (int i = oleadaNum.Length - 1; i >= 0; i--)
        {
            if (waveText.text.Length == 1)
            {
                waveText.text = "";
            }
            else
            {
                waveText.text = waveText.text.Substring(0, waveText.text.Length - 1);
            }

            yield return new WaitForSeconds(0.1f);
        }
        waveText.gameObject.SetActive(false);
        waveText.fontSharedMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 0);

        animating = false;
    }
}
