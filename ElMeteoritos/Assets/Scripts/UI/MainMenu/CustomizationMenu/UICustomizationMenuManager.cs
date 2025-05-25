using System;
using System.Collections.Generic;
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
    [SerializeField] private GameObject playerVisualizer;
    [SerializeField] private GameObject playerVisualizerIMG;

    [SerializeField] private GameObject spaceshipVisualizer;
    [SerializeField] private GameObject propulsionVisualizer;
    private Animator propulsionVisualizerAnim;
    [SerializeField] private GameObject trailVisualizer;
    [SerializeField] private GameObject shotVisualizer;


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

    public Camera skinCamera;

    private void OnEnable()
    {
        OnStateChange += HandleStateChange;

        playerVisualizer.SetActive(true);
        playerVisualizerIMG.SetActive(true);

        UpdatePlayerVisualizer();

        SetState(CustomizationMenuState.SPACESHIP);

        UIMainMenuManager.Instance.EnableNavigationButtons(true);
    }
    private void OnDisable()
    {
        OnStateChange -= HandleStateChange;

        playerVisualizer.SetActive(false);
        playerVisualizerIMG.SetActive(false);

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

        propulsionVisualizerAnim = propulsionVisualizer.GetComponent<Animator>();
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
        OpenSkinPicker();

        SetVisualizersActive(spaceship: false, propulsion: false, trail: false, shot: false);

        colorPicker.SetChangeThisColor(GetVisualizerByState(newState));

        switch (newState)
        {
            case CustomizationMenuState.SPACESHIP:
            case CustomizationMenuState.PROPULSION:
            case CustomizationMenuState.TRAIL:

                SetVisualizersActive(spaceship: true, propulsion: true, trail: true, shot: false);
                propulsionVisualizerAnim.SetFloat("Speed", 6f);

                break;

            case CustomizationMenuState.SHOT:

                SetVisualizersActive(spaceship: false, propulsion: false, trail: false, shot: true);

                break;
        }
    }

    private GameObject GetVisualizerByState(CustomizationMenuState state) => state switch
    {
        CustomizationMenuState.SPACESHIP => spaceshipVisualizer,
        CustomizationMenuState.PROPULSION => propulsionVisualizer,
        CustomizationMenuState.TRAIL => trailVisualizer,
        CustomizationMenuState.SHOT => shotVisualizer,
        _ => spaceshipVisualizer,
    };

    private void SetVisualizersActive(bool spaceship, bool propulsion, bool trail, bool shot)
    {
        spaceshipVisualizer.SetActive(spaceship);
        propulsionVisualizer.SetActive(propulsion);
        trailVisualizer.SetActive(trail);
        shotVisualizer.SetActive(shot);
    }
    #endregion

    #region BUTTONS
    public void OnNextCategoryButtonClick() 
    {
        SoundManager.Instance.PlayFXSound(SoundManager.Instance.ButtonClick);

        ChangeCategory(1); 
    }
    public void OnPreviousCategoryButtonClick() 
    {
        SoundManager.Instance.PlayFXSound(SoundManager.Instance.ButtonClick);

        ChangeCategory(-1); 
    }
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

            if (!string.IsNullOrEmpty(colorHex))
            {
                colorPicker.SetColor(ConvertColor(colorHex));
            }

            confirmCustomizationBTN.onClick.AddListener(() => SaveColor(field));
        }
    }

    private void SaveColor(CustomizationField field)
    {
        SoundManager.Instance.PlayFXSound(SoundManager.Instance.ButtonClick);

        var colorHex = ColorUtility.ToHtmlStringRGB(colorPicker.currentColor);
        UserSession.SetUserCustomizationValue(field, colorHex);

        OpenSkinPicker();
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

    public Color ConvertColor(string hexColor)
    {
        if (ColorUtility.TryParseHtmlString("#" + hexColor, out Color color))
            return color;

        Debug.LogError("Color hexadecimal no valido: " + hexColor);
        return Color.white;
    }
    #endregion

    #region PLAYER VISUALIZER
    private void UpdatePlayerVisualizer()
    {
        UpdateSpaceshipVisualizer();
        UpdatePropulsionVisualizer();
        UpdateTrailVisualizer();
        UpdateShotVisualizer();
    }

    private void UpdateSpaceshipVisualizer()
    {
        if (!spaceshipVisualizer.TryGetComponent<SpriteRenderer>(out var sr)) return;

        // Color
        sr.color = ConvertColor(UserSession.SpaceshipColor);

        // Skin
        var skin = DatabaseManager.Instance.customizationDatabase.GetShipSkinById(UserSession.SpaceshipSkin);
        if (skin != null) sr.sprite = skin.sprite;
    }

    private void UpdatePropulsionVisualizer()
    {
        if (!propulsionVisualizer.TryGetComponent<SpriteRenderer>(out var sr)) return;

        // Color
        sr.color = ConvertColor(UserSession.PropulsionColor);

        // Skin
        var skin = DatabaseManager.Instance.customizationDatabase.GetPropulsionSkinById(UserSession.PropulsionSkin);
        if (skin != null)
        {
            propulsionVisualizerAnim.runtimeAnimatorController = skin.animator;
            propulsionVisualizerAnim.SetFloat("Speed", 6);
        }
    }
    
    private void UpdateTrailVisualizer()
    {
        if (!trailVisualizer.TryGetComponent<ParticleSystem>(out var ps)) return;

        // Color
        var main = ps.main;
        main.startColor = ConvertColor(UserSession.TrailColor);

        // Skin
        var trailSkin = DatabaseManager.Instance.customizationDatabase.GetTrailSkinById(UserSession.TrailSkin);
        if (trailSkin == null)
        {
            Debug.LogWarning("TrailSkin not found");
            return;
        }

        var textureSheet = ps.textureSheetAnimation;
        textureSheet.RemoveSprite(0);
        while (textureSheet.spriteCount > 0)
        {
            textureSheet.RemoveSprite(0);
        }

        foreach (var sprite in trailSkin.sprites)
        {
            textureSheet.AddSprite(sprite);
        }
    }

    private void UpdateShotVisualizer()
    {
        if (!shotVisualizer.TryGetComponent<SpriteRenderer>(out var sr)) return;
        if (!shotVisualizer.TryGetComponent<Animator>(out var anim)) return;

        // Color
        sr.color = ConvertColor(UserSession.ShotColor);

        // Skin
        var skin = DatabaseManager.Instance.customizationDatabase.GetShotSkinById(UserSession.ShotSkin);
        if (skin != null)
        {
            sr.sprite = skin.sprite;
            anim.runtimeAnimatorController = skin.animator;
        }
    }
    #endregion
}
