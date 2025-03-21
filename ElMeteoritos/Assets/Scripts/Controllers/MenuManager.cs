using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    [Header("Navegación")]
    public Button customizationMenuBTN;
    public Button playMenuBTN;
    public Button socialMenuBTN;
    public Button settingsMenuBTN;

    [Header("Menus")]
    public GameObject customizationMenu;
    public CustomizationMenuManager customizationMenuManager;
    public GameObject playMenu;
    public PlayMenuManager playMenuManager;

    [Header("Botones")]
    public Button exitBTN;

    [Header("Colores")]
    public Dictionary<string, Color> colors = new()
    {
        { "buttonUnselected", new Color32(159, 159, 159, 255) },
        { "buttonCustomizationSelected", new Color32(255, 0, 0, 255) },
        { "buttonCustomizationUnselected", new Color32(255, 90, 90, 255) },
        { "buttonPlaySelected", new Color32(64, 255, 0, 255) },
        { "buttonPlayUnselected", new Color32(131, 255, 90, 255) },
        { "buttonSocialSelected", new Color32(255, 59, 207, 255) },
        { "buttonSocialUnselected", new Color32(255, 93, 242, 255) },
        { "buttonSettingsSelected", new Color32(0, 43, 255, 255) },
        { "buttonSettingsUnselected", new Color32(64, 96, 255, 255) },
    };

    [Header("Estado")]
    public MainMenuState main_Menu_State;
    public MainMenuState mainMenuState
    {
        get { return main_Menu_State; }
        set
        {
            if (main_Menu_State == value) return;
            main_Menu_State = value;
            OnStateChange?.Invoke(main_Menu_State);
        }
    }
    public delegate void OnVariableChangeDelegate(MainMenuState newState);
    public event OnVariableChangeDelegate OnStateChange;

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

    private void Start()
    {
        OnStateChange += ManageMainMenuState;
        SetState(MainMenuState.START);
        ManageNavigationButtons();
    }

    // ---> Maquina de estados del menu principal
    public void SetState(MainMenuState newState)
    {
        mainMenuState = newState;
    }
    private void ManageMainMenuState(MainMenuState newState) // ---> Funcion que se llama cuando se cambia la variable mainMenuState y cambia entres los diferentes menus
    {
        switch (newState)
        {
            case MainMenuState.START:

                StartMenu();
                break;

            case MainMenuState.CUSTOMIZATION:

                OpenCustomizationMenu();
                break;

            case MainMenuState.GAME:

                if (!PhotonNetwork.IsConnected)
                {
                    ConnectionManager.instance.Connect();
                }

                OpenGameMenu();
                break;

            case MainMenuState.SOCIAL:

                OpenSocialMenu();
                break;

            case MainMenuState.SETTINGS:

                OpenSettingsMenu();
                break;

        }
    } 

    private void StartMenu()
    {
        customizationMenu.SetActive(false);
        playMenu.SetActive(false);

        ChangeButtonColor(customizationMenuBTN, colors["buttonCustomizationUnselected"]);
        ChangeButtonColor(playMenuBTN, colors["buttonPlayUnselected"]);
        ChangeButtonColor(socialMenuBTN, colors["buttonSocialUnselected"]);
        ChangeButtonColor(settingsMenuBTN, colors["buttonSettingsUnselected"]);
    }
    private void OpenCustomizationMenu()
    {
        customizationMenu.SetActive(true);
        playMenu.SetActive(false);

        ChangeButtonColor(customizationMenuBTN, colors["buttonCustomizationSelected"]);
        ChangeButtonColor(playMenuBTN, colors["buttonUnselected"]);
        ChangeButtonColor(socialMenuBTN, colors["buttonUnselected"]);
        ChangeButtonColor(settingsMenuBTN, colors["buttonUnselected"]);
    }
    private void OpenGameMenu()
    {
        customizationMenu.SetActive(false);
        playMenu.SetActive(true);

        ChangeButtonColor(customizationMenuBTN, colors["buttonUnselected"]);
        ChangeButtonColor(playMenuBTN, colors["buttonPlaySelected"]);
        ChangeButtonColor(socialMenuBTN, colors["buttonUnselected"]);
        ChangeButtonColor(settingsMenuBTN, colors["buttonUnselected"]);
    }
    private void OpenSocialMenu()
    {
        customizationMenu.SetActive(false);
        playMenu.SetActive(false);

        ChangeButtonColor(customizationMenuBTN, colors["buttonUnselected"]);
        ChangeButtonColor(playMenuBTN, colors["buttonUnselected"]);
        ChangeButtonColor(socialMenuBTN, colors["buttonSocialSelected"]);
        ChangeButtonColor(settingsMenuBTN, colors["buttonUnselected"]);
    }
    private void OpenSettingsMenu()
    {
        customizationMenu.SetActive(false);
        playMenu.SetActive(false);

        ChangeButtonColor(customizationMenuBTN, colors["buttonUnselected"]);
        ChangeButtonColor(playMenuBTN, colors["buttonUnselected"]);
        ChangeButtonColor(socialMenuBTN, colors["buttonUnselected"]);
        ChangeButtonColor(settingsMenuBTN, colors["buttonSettingsSelected"]);
    }

    // ---> Botones de navegación
    private void ManageNavigationButtons(bool bActivate = true)
    {
        switch (bActivate)
        {
            case true:
                customizationMenuBTN.onClick.AddListener(() => { SetNavigationButton(MainMenuState.CUSTOMIZATION); });
                playMenuBTN.onClick.AddListener(() => { SetNavigationButton(MainMenuState.GAME); });
                socialMenuBTN.onClick.AddListener(() => { SetNavigationButton(MainMenuState.SOCIAL); });
                settingsMenuBTN.onClick.AddListener(() => { SetNavigationButton(MainMenuState.SETTINGS); });
                break;
            case false:
                customizationMenuBTN.onClick.RemoveAllListeners();
                playMenuBTN.onClick.RemoveAllListeners();
                socialMenuBTN.onClick.RemoveAllListeners();
                settingsMenuBTN.onClick.RemoveAllListeners();
                break;
        }
    }
    private void SetNavigationButton (MainMenuState state)
    {
        if (mainMenuState == state)
        {
            SetState(MainMenuState.START);
        }
        else
        {
            SetState(state);
        }
    }

    // ---> Utilidades
    public void ChangeButtonColor(Button button, Color color)
    {
        button.gameObject.GetComponent<Image>().color = color;
    }
}
