using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    [Header("Menus")]
    public GameObject customizationMenu;
    public GameObject playMenu;

    [Header("Botones")]
    public Button customizationMenuBTN;
    public Button playMenuBTN;
    public Button socialMenuBTN;
    public Button settingsMenuBTN;

    [Header("Colores")]
    public Color unselectedColor = new(159, 159, 159, 255);
    public Color customizationMenuBTNSelectedColor = new(159, 159, 159, 255);
    public Color customizationMenuBTNUnselectedColor = new(255, 0, 9, 255);
    public Color playMenuBTNSelectedColor = new(131, 255, 90, 255);
    public Color playMenuBTNUnselectedColor = new(10, 255, 0, 255);
    public Color socialMenuBTNSelectedColor = new(255, 93, 242, 255);
    public Color socialMenuBTNUnselectedColor = new(255, 59, 207, 255);
    public Color settingsMenuBTNSelectedColor = new(0, 83, 255, 255);
    public Color settingsMenuBTNUnselectedColor = new(76, 137, 255, 255);

    [Header("Estado")]
    private MainMenuState main_Menu_State;
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
        ManageNavigationButtons();
        SetState(MainMenuState.START);
    }

    public void SetState(MainMenuState newState)
    {
        mainMenuState = newState;
    }
    private void ManageMainMenuState(MainMenuState newState)
    {
        switch (newState)
        {
            case MainMenuState.START:
                ChangeButtonColor(customizationMenuBTN, customizationMenuBTNUnselectedColor);
                ChangeButtonColor(playMenuBTN, playMenuBTNUnselectedColor);
                ChangeButtonColor(socialMenuBTN, socialMenuBTNUnselectedColor);
                ChangeButtonColor(settingsMenuBTN, settingsMenuBTNUnselectedColor);
                customizationMenu.SetActive(false);
                playMenu.SetActive(false);
                break;
            case MainMenuState.CUSTOMIZATION:
                customizationMenu.SetActive(true);
                playMenu.SetActive(false);
                break;
            case MainMenuState.GAME:
                customizationMenu.SetActive(false);
                playMenu.SetActive(true);
                break;
            case MainMenuState.SOCIAL:
                break;
            case MainMenuState.SETTINGS:
                break;

        }
    } 

    public void ChangeButtonColor(Button button, Color color)
    {
        button.gameObject.GetComponent<Image>().color = color;
    }
    private void ManageNavigationButtons(bool bActivate = true)
    {
        switch (bActivate)
        {
            case true:
                customizationMenuBTN.onClick.AddListener(() => { CustomizationButton(); });
                playMenuBTN.onClick.AddListener(() => { PlayButton(); });
                break;
            case false:
                customizationMenuBTN.onClick.RemoveAllListeners();
                playMenuBTN.onClick.RemoveAllListeners();
                break;
        }
    }

    private void CustomizationButton()
    {
        if (mainMenuState == MainMenuState.CUSTOMIZATION)
        {
            SetState(MainMenuState.START);
        } 
        else
        {
            ChangeButtonColor(customizationMenuBTN, customizationMenuBTNSelectedColor);
            ChangeButtonColor(playMenuBTN, unselectedColor);
            ChangeButtonColor(socialMenuBTN, unselectedColor);
            ChangeButtonColor(settingsMenuBTN, unselectedColor);
            SetState(MainMenuState.CUSTOMIZATION);
        }
    }
    private void PlayButton()
    {
        if (mainMenuState == MainMenuState.GAME)
        {
            SetState(MainMenuState.START);
        }
        else
        {
            ChangeButtonColor(customizationMenuBTN, unselectedColor);
            ChangeButtonColor(playMenuBTN, playMenuBTNSelectedColor);
            ChangeButtonColor(socialMenuBTN, unselectedColor);
            ChangeButtonColor(settingsMenuBTN, unselectedColor);
            SetState(MainMenuState.GAME);
        }
    }
}
