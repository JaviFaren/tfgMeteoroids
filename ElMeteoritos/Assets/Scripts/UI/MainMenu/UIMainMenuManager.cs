using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenuManager : MonoBehaviour
{
    public static UIMainMenuManager Instance { get; private set; }

    [Header("Paneles")]
    public GameObject navigationPanel;
    public GameObject disconnectionPanel;

    [Header("Navegación")]
    public Button customizationMenuBTN;
    public Button playMenuBTN;
    public Button socialMenuBTN;
    public Button settingsMenuBTN;

    [Header("Menus")]
    public GameObject customizationMenu;
    [HideInInspector] public UICustomizationMenuManager customizationMenuManager;
    public GameObject playMenu;
    [HideInInspector] public UIPlayMenuManager playMenuManager;
    public GameObject socialMenu;
    [HideInInspector] public UISocialMenuManager socialMenuManager;
    public GameObject settingsMenu;
    [HideInInspector] public UISettingsMenuManager settingsMenuManager;

    [Header("Textos")]
    public TextMeshProUGUI connectionsStatusTMP;

    [Header("Botones")]
    public Button exitBTN;
    public Button disconnectBTN;

    [Header("Colores")]
    public List<MainMenuButtonColorSet> buttonColorSets;
    private Dictionary<MainMenuState, MainMenuButtonColorSet> buttonColors;

    [Header("Estado")]
    private MainMenuState _mainMenuState = MainMenuState.START;
    public MainMenuState MainMenuState
    {
        get => _mainMenuState;
        set
        {
            if (_mainMenuState == value) return;
            _mainMenuState = value;
            OnStateChange?.Invoke(_mainMenuState);
        }
    }
    public event Action<MainMenuState> OnStateChange;
    private MainMenuState _previousState = MainMenuState.NO_MENU;

    private void OnEnable()
    {
        OnStateChange += HandleStateChange;
        SetState(MainMenuState.NO_MENU);
        SetConnectionStatusText();

        SoundManager.Instance.StartMenuMusicLoop();
    }
    private void OnDisable()
    {
        SoundManager.Instance.StopMenuMusicLoop();

        SetState(MainMenuState.NO_MENU);
        OnStateChange -= HandleStateChange;
    }
    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Componentes
        customizationMenuManager = customizationMenu.GetComponent<UICustomizationMenuManager>();
        playMenuManager = playMenu.GetComponent<UIPlayMenuManager>();
        socialMenuManager = socialMenu.GetComponent<UISocialMenuManager>();
        settingsMenuManager = settingsMenu.GetComponent<UISettingsMenuManager>();

        // Inicializar colores
        buttonColors = buttonColorSets?.ToDictionary(b => b.state, b => b) ?? new();
    }

    #region State
    public void SetState(MainMenuState newState) => MainMenuState = newState;
    private void HandleStateChange(MainMenuState newState)
    {
        if (_previousState == MainMenuState.SETTINGS && newState != MainMenuState.SETTINGS)
        {
            settingsMenuManager.SaveSettings();
        }

        UpdateButtonColors(newState);

        EnableNavigationButtons(false);

        switch (newState)
        {
            case MainMenuState.NO_MENU:
                EnableNavigationButtons(true);
                UpdateMenuState(false, false, false, false);
                break;

            case MainMenuState.CUSTOMIZATION:
                UpdateMenuState(true, false, false, false);
                break;

            case MainMenuState.PLAY:
                StartCoroutine(ConnectAndOpenPlayMenu());
                break;

            case MainMenuState.SOCIAL:
                UpdateMenuState(false, false, true, false);
                break;

            case MainMenuState.SETTINGS:
                UpdateMenuState(false, false, false, true);
                break;
        }

        _previousState = newState;
    }
    private void UpdateMenuState(bool showCustomization, bool showPlay, bool showSocial, bool showSettings)
    {
        customizationMenu.SetActive(showCustomization);
        playMenu.SetActive(showPlay);
        socialMenu.SetActive(showSocial);
        settingsMenu.SetActive(showSettings);
    }

    private void ToggleMenuState(MainMenuState targetState)
    {
        SetState(MainMenuState == targetState ? MainMenuState.NO_MENU : targetState);
    }

    private IEnumerator ConnectAndOpenPlayMenu()
    {
        if (!PhotonNetwork.IsConnected)
        {
            ConnectionManager.Instance.Connect();
            SetConnectionStatusText();

            yield return new WaitUntil(() => PhotonNetwork.InLobby);

            SetConnectionStatusText();
        }

        UpdateMenuState(false, true, false, false);
    }
    #endregion

    #region Botones
    private void UpdateButtonColors(MainMenuState activeState)
    {
        var buttons = new Dictionary<Button, MainMenuState>
        {
            { customizationMenuBTN, MainMenuState.CUSTOMIZATION },
            { playMenuBTN, MainMenuState.PLAY },
            { socialMenuBTN, MainMenuState.SOCIAL },
            { settingsMenuBTN, MainMenuState.SETTINGS }
        };

        foreach (var button in buttons)
        {
            var state = button.Value;
            var colorSet = buttonColors[state];
            var color = activeState == MainMenuState.NO_MENU ? colorSet.unselectedColor :
                        state == activeState ? colorSet.selectedColor : new Color32(159, 159, 159, 255);

            ChangeButtonColor(button.Key, color);
        }
    }

    public void ChangeButtonColor(Button button, Color color)
    {
        button.GetComponent<Image>().color = color;
    }

    public void OnCustomizationMenuButtonClick()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.Instance.ButtonClick);

        ToggleMenuState(MainMenuState.CUSTOMIZATION);
    }

    public void OnPlayMenuButtonClick()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.Instance.ButtonClick);

        ToggleMenuState(MainMenuState.PLAY);
    }
    public void OnSocialMenuButtonClick()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.Instance.ButtonClick);

        ToggleMenuState(MainMenuState.SOCIAL);
    }
    public void OnSettingsMenuButtonClick() 
    {
        SoundManager.Instance.PlayFXSound(SoundManager.Instance.ButtonClick);

        ToggleMenuState(MainMenuState.SETTINGS);
    }
    public void OnDisconnectButtonClick()
    {
        SoundManager.Instance.PlayFXSound(SoundManager.Instance.ButtonClick);

        ConnectionManager.Instance.Disconnect();
        PHPManager.Instance.Logout();
    }
    public void OnExitButtonClick() 
    {
        SoundManager.Instance.PlayFXSound(SoundManager.Instance.ButtonClick);

        ConnectionManager.Instance.ExitGame();
    }

    public void EnableNavigationButtons(bool enabled)
    {
        customizationMenuBTN.interactable = enabled;
        playMenuBTN.interactable = enabled;
        socialMenuBTN.interactable = enabled;
        settingsMenuBTN.interactable = enabled;
    }
    #endregion

    #region Texts
    public void SetConnectionStatusText()
    {
        switch (ConnectionManager.Instance.GetConnectionStatus())
        {
            case ConnectionStatus.NO_CONNECTED:
                connectionsStatusTMP.SetText("No conectado");
                break;
            case ConnectionStatus.CONNECTING:
                connectionsStatusTMP.SetText("Conectando...");
                break;
            case ConnectionStatus.CONNECTED:
                connectionsStatusTMP.SetText("Conectado");
                break;
        }
    }
    #endregion
}

[System.Serializable]
public struct MainMenuButtonColorSet
{
    public MainMenuState state;
    public Color selectedColor;
    public Color unselectedColor;
}
