using Photon.Pun;
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

    [Header("Fondo")]
    public Image backgroundIMG;
    public List<Sprite> backgrounds = new();
    public float animDuration = 1f;
    private Sprite lastBackground;

    [Header("Paneles")]
    public GameObject matchInfoPanel;
    public GameObject playersInfoPanel;
    public GameObject playerControlsPanel;
    public GameObject leaveMatchPanel;
    public GameObject endMatchPanel;

    [Header("Botones")]
    public Button shootBTN;
    public Button menuBTN;
    public Button leaveMatchBTN;
    public Button continueBTN;

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
    private void OnEnable()
    {
        SoundManager.Instance.StartGameMusicLoop();
    }
    private void OnDisable()
    {
        SoundManager.Instance.StopGameMusicLoop();
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

    public void RemovePlayerPanel(int playerID)
    {
        var panelToUpdate = currentPlayersPanels.FirstOrDefault(p => p.TryGetComponent(out PlayerPanel pp) && pp.playerID == playerID);
        if (panelToUpdate != null && panelToUpdate.TryGetComponent(out PlayerPanel panel))
        {
            currentPlayersPanels.Remove(panelToUpdate);
            Destroy(panelToUpdate);
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

    #region Botones
    public void OnMenuButtonClick()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.Instance.ButtonClick);

        leaveMatchPanel.SetActive(true);

        menuBTN.gameObject.SetActive(false);
    }

    public void OnLeaveMatchButtonClick()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.Instance.ButtonClick);

        var player = PlayerManager.Instance.GetPlayerByID(int.Parse(PhotonNetwork.LocalPlayer.UserId));
        player.photonView.RPC(nameof(player.LeaveMatch), RpcTarget.All);

        PhotonNetwork.AutomaticallySyncScene = false;

        ConnectionManager.Instance.ReturnToMainMenu();
    }

    public void OnContinueButtonClick()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.Instance.ButtonClick);

        leaveMatchPanel.SetActive(false);

        menuBTN.gameObject.SetActive(true);
    }
    #endregion

    #region Fondo
    public IEnumerator ChangeToRandomBackground()
    {
        yield return FadeBackgroundSprite(GetRandomBackground());
    }

    private Sprite GetRandomBackground()
    {
        if (backgrounds.Count <= 1) return backgrounds[0];

        Sprite newSprite;
        do
        {
            newSprite = backgrounds[Random.Range(0, backgrounds.Count)];
        } while (newSprite == lastBackground);

        lastBackground = newSprite;
        return newSprite;
    }

    private IEnumerator FadeBackgroundSprite(Sprite newSprite)
    {
        float halfDuration = animDuration / 2f;
        Color color = backgroundIMG.color;

        // Fade out
        for (float t = 0; t < halfDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / halfDuration;
            backgroundIMG.color = new Color(color.r, color.g, color.b, 1f - normalizedTime);
            yield return null;
        }

        // Cambio de sprite en el punto más bajo de alpha
        backgroundIMG.sprite = newSprite;

        // Fade in
        for (float t = 0; t < halfDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / halfDuration;
            backgroundIMG.color = new Color(color.r, color.g, color.b, normalizedTime);
            yield return null;
        }

        // Asegurar opacidad al final
        backgroundIMG.color = new Color(color.r, color.g, color.b, 1f);
    }
    #endregion
}
