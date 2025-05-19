using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class UILoginManager : MonoBehaviour
{
    public static UILoginManager Instance { get; private set; }

    [Header("Fondo")]
    public VideoPlayer backgroundVP;
    [Tooltip("Nombre del archivo de video en StreamingAssets")]
    public string videoFileName = "LoginVideoBackground.mp4";

    [Header("Menus")]
    public GameObject loginMenu;
    public GameObject registerMenu;
    public GameObject navigation;

    [Header("Botones")]
    public Button loginBTN;
    public Button registerBTN;
    public Button forgotPasswordBTN;
    public Button proceedLoginBTN;
    public Button proceedRegisterBTN;

    [Header("Textos")]
    public TextMeshProUGUI userErrorTMP;
    public TextMeshProUGUI passwordErrorTMP;
    public TextMeshProUGUI newUserNameErrorTMP;
    public TextMeshProUGUI newUserEmailErrorTMP;
    public TextMeshProUGUI newUserPasswordErrorTMP;
    public TextMeshProUGUI newUserConfirmPasswordErrorTMP;
    public TextMeshProUGUI loginStatusTMP;
    public TextMeshProUGUI registerStatusTMP;

    [Header("Campos de texto")]
    public TMP_InputField userNameEmailIF;
    public TMP_InputField userPasswordIF;
    public TMP_InputField newUserNameIF;
    public TMP_InputField newUserEmailIF;
    public TMP_InputField newUserPasswordlIF;
    public TMP_InputField newUserConfirmPasswordIF;

    [Header("Colores")]
    public List<LoginMenuButtonColorSet> buttonColorSets;
    private Dictionary<LoginMenuState, LoginMenuButtonColorSet> buttonColors;

    [Header("Estado")]
    private LoginMenuState _loginMenuState = LoginMenuState.START;
    public LoginMenuState LoginMenuState
    {
        get => _loginMenuState;
        set
        {
            if (_loginMenuState == value) return;
            _loginMenuState = value;
            OnStateChange?.Invoke(_loginMenuState);
        }
    }
    public event Action<LoginMenuState> OnStateChange;

    private void OnEnable()
    {
        OnStateChange += HandleStateChange;
        SetState(LoginMenuState.LOGIN);
    }
    private void OnDisable()
    {
        SetState(LoginMenuState.START);
        OnStateChange -= HandleStateChange;
    }
    private void Awake()
    {
        //  Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Inicializar colores
        buttonColors = buttonColorSets?.ToDictionary(b => b.state, b => b) ?? new();
    }
    private void Start() => SetupVideo();

    #region Background
    private void SetupVideo()
    {
        string path = Path.Combine(Application.streamingAssetsPath, videoFileName);
        backgroundVP.url = path;

        backgroundVP.prepareCompleted += (vp) =>
        {
            vp.Play();
        };

        backgroundVP.Prepare();
    }
    #endregion

    #region State
    public void SetState(LoginMenuState newState) => LoginMenuState = newState;

    private void HandleStateChange(LoginMenuState newState)
    {
        UpdateButtonColors(newState);

        SetButtonsInteractable(true);
        SetInputFieldsInteractable(true);

        // ---> Navegacion
        navigation.SetActive(true);

        switch (newState)
        {
            case LoginMenuState.LOGIN:
                // ---> Textos
                ClearLoginMenuTexts();
                // ---> Comprobaciones
                CanLogin();
                // ---> Menus
                UpdateMenuState(true, false);
                break;

            case LoginMenuState.REGISTER:
                // ---> Textos
                ClearRegisterMenuTexts();
                // ---> Comprobaciones
                CanRegister();
                // ---> Menus
                UpdateMenuState(false, true);
                break;
        }
    }

    private void UpdateMenuState(bool showLogin, bool showRegister)
    {
        loginMenu.SetActive(showLogin);
        registerMenu.SetActive(showRegister);
    }
    #endregion

    #region Buttons
    public void OnLoginButtonClick() => SetState(LoginMenuState.LOGIN);

    public void OnRegisterButtonClick() => SetState(LoginMenuState.REGISTER);

    public void OnForgotPasswordButtonClick() { }

    public async void OnProceedLoginButtonClick()
    {
        SetButtonsInteractable(false);
        SetInputFieldsInteractable(false);

        await PHPManager.Instance.Login(userNameEmailIF.text, userPasswordIF.text);

        if (UserSession.Id != -1) SceneManager.LoadScene(1);

        SetButtonsInteractable(true);
        SetInputFieldsInteractable(true);
    }

    public async void OnProceedRegisterButtonClick()
    {
        SetButtonsInteractable(false);
        SetInputFieldsInteractable(false);

        await PHPManager.Instance.Register(newUserNameIF.text, newUserEmailIF.text, newUserConfirmPasswordIF.text);

        if (UserSession.Id != -1) SceneManager.LoadScene(1);

        SetButtonsInteractable(true);
        SetInputFieldsInteractable(true);
    }

    private void UpdateButtonColors(LoginMenuState activeState)
    {
        var buttons = new Dictionary<Button, LoginMenuState>
        {
            { loginBTN, LoginMenuState.LOGIN },
            { registerBTN, LoginMenuState.REGISTER },
        };

        foreach (var button in buttons)
        {
            var state = button.Value;
            var colorSet = buttonColors[state];
            var color = state == activeState ? colorSet.selectedColor : colorSet.unselectedColor;

            ChangeButtonColor(button.Key, color);
        }
    }

    public void ChangeButtonColor(Button button, Color color) => button.GetComponent<Image>().color = color;

    public void SetButtonsInteractable(bool interactable)
    {
        loginBTN.interactable = interactable;
        registerBTN.interactable = interactable;

        switch (LoginMenuState)
        {
            case LoginMenuState.LOGIN:
                forgotPasswordBTN.interactable = interactable;
                proceedLoginBTN.interactable = interactable;
                break;
            case LoginMenuState.REGISTER:
                proceedRegisterBTN.interactable = interactable;
                break;
        }
    }
    #endregion

    #region Texts
    private void ClearTexts(params TextMeshProUGUI[] tmps)
    {
        foreach (var tmp in tmps) tmp.text = string.Empty;
    }

    private void ClearInputFields(params TMP_InputField[] inputs)
    {
        foreach (var input in inputs) input.text = string.Empty;
    }

    private void ClearLoginMenuTexts()
    {
        ClearInputFields(userNameEmailIF, userPasswordIF);
        ClearTexts(loginStatusTMP, userErrorTMP, passwordErrorTMP);
    }

    private void ClearRegisterMenuTexts()
    {
        ClearInputFields(newUserNameIF, newUserEmailIF, newUserPasswordlIF, newUserConfirmPasswordIF);
        ClearTexts(registerStatusTMP, newUserNameErrorTMP, newUserEmailErrorTMP, newUserPasswordErrorTMP, newUserConfirmPasswordErrorTMP);
    }

    public void SetText(TextMeshProUGUI tmp, string text) => tmp.text = text;

    public void SetStatusText(string text)
    {
        switch (LoginMenuState)
        {
            case LoginMenuState.LOGIN: SetText(loginStatusTMP, text); break;
            case LoginMenuState.REGISTER: SetText(registerStatusTMP, text); break;
        }
    }
    public void SetErrorText(string code, string text)
    {
        switch (code)
        {
            case "user_not_found": SetText(userErrorTMP, text); break;
            case "wrong_password": SetText(passwordErrorTMP, text); break;
            case "existing_name": SetText(newUserNameErrorTMP, text); break;
            case "existing_email": SetText(newUserEmailErrorTMP, text); break;
        }
    }
    public void SetInputFieldText(TMP_InputField inputField, string text) => inputField.text = text;

    public void SetInputFieldsInteractable(bool interactable)
    {
        switch (LoginMenuState)
        {
            case LoginMenuState.LOGIN:
                userNameEmailIF.interactable = interactable;
                userPasswordIF.interactable = interactable;
                break;
            case LoginMenuState.REGISTER:
                newUserNameIF.interactable = interactable;
                newUserEmailIF.interactable = interactable;
                newUserPasswordlIF.interactable = interactable;
                newUserConfirmPasswordIF.interactable = interactable;
                break;
        }
    }
    #endregion

    #region Checks
    public void CanLogin()
    {
        ClearTexts(userErrorTMP, passwordErrorTMP);

        proceedLoginBTN.interactable =
            !string.IsNullOrEmpty(userNameEmailIF.text) &&
            !string.IsNullOrEmpty(userPasswordIF.text);
    }
    public void CanRegister()
    {
        ClearTexts(newUserNameErrorTMP, newUserEmailErrorTMP, newUserPasswordErrorTMP, newUserConfirmPasswordErrorTMP);

        bool allFieldsFilled =
            !string.IsNullOrEmpty(newUserNameIF.text) &&
            !string.IsNullOrEmpty(newUserEmailIF.text) &&
            !string.IsNullOrEmpty(newUserPasswordlIF.text) &&
            !string.IsNullOrEmpty(newUserConfirmPasswordIF.text);

        bool passwordsMatch = false;

        if (allFieldsFilled)
        {
            passwordsMatch = newUserPasswordlIF.text == newUserConfirmPasswordIF.text;

            if (!passwordsMatch) newUserConfirmPasswordErrorTMP.text = "Las contraseñas no coinciden";
        }

        proceedRegisterBTN.interactable = passwordsMatch && allFieldsFilled;
    }
    #endregion
}

[System.Serializable]
public struct LoginMenuButtonColorSet
{
    public LoginMenuState state;
    public Color selectedColor;
    public Color unselectedColor;
}
