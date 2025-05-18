using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICustomizationMenuManager : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private GameObject customizationColorPanel;
    [SerializeField] private ColorPickerControl colorPicker;
    [SerializeField] private GameObject customizationSkinPanel;
    private readonly List<UISkinSelector> activeSkinSelectors = new();

    [Header("Botones")]
    [SerializeField] private Button nextCategoryBTN;
    [SerializeField] private Button prevCategoryBTN;
    [SerializeField] private Button confirmCustomizationBTN;
    [SerializeField] private Button customizeSkinBTN;
    [SerializeField] private Button customizeColorBTN;

    [Header("Textos")]
    [SerializeField] private TextMeshProUGUI customizationCategoryTMP;

    [Header("Imagenes")]
    [SerializeField] private Image playerVisualizer;

    [Header("Prefabs y Referencias")]
    [SerializeField] private GameObject skinSelectorPrefab;
    [SerializeField] private Transform skinsPanelContent;

    [Header("Estado")]
    private CustomizationMenuState _customizationMenuState = CustomizationMenuState.START;
    public CustomizationMenuState CustomizationMenuState
    {
        get => _customizationMenuState;
        set
        {
            if (_customizationMenuState == value) return;
            _customizationMenuState = value;
            OnStateChange?.Invoke(_customizationMenuState);
        }
    }
    public event Action<CustomizationMenuState> OnStateChange;

    private Dictionary<CustomizationMenuState, string> categoryNames;
    private Dictionary<CustomizationMenuState, CustomizationField> skinFieldMap;
    private Dictionary<CustomizationMenuState, CustomizationField> colorFieldMap;

    private void OnEnable()
    {
        OnStateChange += HandleStateChange;

        SetState(CustomizationMenuState.SPACESHIP);
    }
    private void OnDisable()
    {
        OnStateChange -= HandleStateChange;

        SetState(CustomizationMenuState.START);
    }

    private void Awake()
    {
        if (!colorPicker) colorPicker = customizationColorPanel.GetComponentInChildren<ColorPickerControl>();

        categoryNames = new()
        {
            { CustomizationMenuState.SPACESHIP, "Nave" },
            { CustomizationMenuState.SHOT, "Disparo" },
            { CustomizationMenuState.PROPULSION, "Propulsión" },
            { CustomizationMenuState.TRAIL, "Rastro" }
        };

        skinFieldMap = new()
        {
            { CustomizationMenuState.SPACESHIP, CustomizationField.SPACESHIP_SKIN },
            { CustomizationMenuState.SHOT, CustomizationField.SHOT_SKIN },
            { CustomizationMenuState.PROPULSION, CustomizationField.PROPULSION_SKIN },
            { CustomizationMenuState.TRAIL, CustomizationField.TRAIL_SKIN }
        };

        colorFieldMap = new()
        {
            { CustomizationMenuState.SPACESHIP, CustomizationField.SPACESHIP_COLOR },
            { CustomizationMenuState.SHOT, CustomizationField.SHOT_COLOR },
            { CustomizationMenuState.PROPULSION, CustomizationField.PROPULSION_COLOR },
            { CustomizationMenuState.TRAIL, CustomizationField.TRAIL_COLOR }
        };
    }

    #region STATE
    private void ChangeCategory(int direction)
    {
        int min = (int)CustomizationMenuState.SPACESHIP;
        int max = (int)CustomizationMenuState.SHOT;
        int range = max - min + 1;

        int currentIndex = (int)CustomizationMenuState - min;
        currentIndex = (currentIndex + direction + range) % range;

        SetState((CustomizationMenuState)(currentIndex + min));
    }

    public void SetState(CustomizationMenuState newState) => CustomizationMenuState = newState;

    private void HandleStateChange(CustomizationMenuState newState)
    {
        customizationCategoryTMP.text = categoryNames.TryGetValue(newState, out var name) ? name : newState.ToString();
        UpdatePlayerVisualizer();
        OpenSkinPicker();
    }
    #endregion

    #region BUTTONS
    public void OnNextCategoryButtonClick() => ChangeCategory(1);
    public void OnPreviousCategoryButtonClick() => ChangeCategory(-1);
    #endregion

    #region SKINS
    public void OpenSkinPicker()
    {
        customizeSkinBTN.interactable = false;
        customizeColorBTN.interactable = true;
        confirmCustomizationBTN.gameObject.SetActive(false);

        customizationColorPanel.SetActive(false);
        customizationSkinPanel.SetActive(true);

        ClearSkins();

        if (!skinFieldMap.TryGetValue(CustomizationMenuState, out var field)) return;

        var skins = DatabaseManager.Instance.customizationDatabase.GetSkinsByField(field);

        int currentSkinID = GetCurrentSkinIDFromSession(field);

        foreach (var skin in skins)
        {
            var selectorGO = Instantiate(skinSelectorPrefab, skinsPanelContent);
            if (selectorGO.TryGetComponent(out UISkinSelector selector))
            {
                selector.Setup(skin.id, skin.sprite, field, this);
                selector.SetSelected(skin.id == currentSkinID);

                activeSkinSelectors.Add(selector);
            }
        }
    }

    private void ClearSkins()
    {
        foreach (Transform child in skinsPanelContent) Destroy(child.gameObject);
        activeSkinSelectors.Clear();
    }

    public void OnSkinSelected(UISkinSelector selected)
    {
        foreach (var selector in activeSkinSelectors)
        {
            selector.SetSelected(selector == selected);
        }

        UserSession.SetUserCustomizationValue(selected.customizationField, selected.skinID);

        UpdatePlayerVisualizer();
    }

    private int GetCurrentSkinIDFromSession(CustomizationField field)
    {
        return field switch
        {
            CustomizationField.SPACESHIP_SKIN => UserSession.SpaceshipSkin,
            CustomizationField.PROPULSION_SKIN => UserSession.PropulsionSkin,
            CustomizationField.TRAIL_SKIN => UserSession.TrailSkin,
            CustomizationField.SHOT_SKIN => UserSession.ShotSkin,
            _ => -1
        };
    }
    #endregion

    #region COLOR
    public void OpenColorPicker()
    {
        customizeSkinBTN.interactable = true;
        customizeColorBTN.interactable = false;
        confirmCustomizationBTN.gameObject.SetActive(true);

        customizationSkinPanel.SetActive(false);
        customizationColorPanel.SetActive(true);

        confirmCustomizationBTN.onClick.RemoveAllListeners();

        if (colorFieldMap.TryGetValue(CustomizationMenuState, out var field))
        {
            string colorHex = GetCurrentColorFromSession(field);

            if (!string.IsNullOrEmpty(colorHex) && ColorUtility.TryParseHtmlString("#" + colorHex, out Color parsedColor))
            {
                colorPicker.SetColor(parsedColor);
            }

            confirmCustomizationBTN.onClick.AddListener(() => SaveColor(field));
        }
    }

    private void SaveColor(CustomizationField field)
    {
        var colorHex = ColorUtility.ToHtmlStringRGB(colorPicker.currentColor);
        UserSession.SetUserCustomizationValue(field, colorHex);
    }

    private string GetCurrentColorFromSession(CustomizationField field)
    {
        return field switch
        {
            CustomizationField.SPACESHIP_COLOR => UserSession.SpaceshipColor,
            CustomizationField.PROPULSION_COLOR => UserSession.PropulsionColor,
            CustomizationField.TRAIL_COLOR => UserSession.TrailColor,
            CustomizationField.SHOT_COLOR => UserSession.ShotColor,
            _ => ""
        };
    }
    #endregion

    #region PLAYER VISUALIZER
    private void UpdatePlayerVisualizer()
    {
        if (!colorFieldMap.TryGetValue(CustomizationMenuState, out var colorField) ||
        !skinFieldMap.TryGetValue(CustomizationMenuState, out var skinField))
            return;

        // Color
        if (ColorUtility.TryParseHtmlString("#" + GetCurrentColorFromSession(colorField), out Color color))
        {
            playerVisualizer.color = color;
        }

        // Sprite
        var skins = DatabaseManager.Instance.customizationDatabase.GetSkinsByField(skinField);
        var skin = skins.FirstOrDefault(s => s.id == GetCurrentSkinIDFromSession(skinField));

        if (skin != null)
        {
            playerVisualizer.sprite = skin.sprite;
        }
    }

    public void SetPlayerVisualizerSprite(Sprite sprite)
    {
        playerVisualizer.sprite = sprite;
    }
    #endregion
}
