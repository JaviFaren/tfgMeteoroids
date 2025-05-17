using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class PHPManager : MonoBehaviour
{
    public static PHPManager Instance { get; private set; }

    private const string URL = "http://meteoroids.mygamesonline.org/";

    private void Awake()
    {
        //  Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }
    private async void Start()
    {
        // Si hay un token guardado, intenta auto-login
        if (!string.IsNullOrEmpty(UserSession.SessionToken))
        {
            UILoginManager.Instance.SetInputFieldText(UILoginManager.Instance.userNameEmailIF, UserSession.Name);
            UILoginManager.Instance.SetButtonsInteractable(false);
            UILoginManager.Instance.SetInputFieldsInteractable(false);

            await LoginWithToken();

            if (UserSession.Id != -1)
            {
                SceneManager.LoadScene(1);
            }
            else
            {
                UILoginManager.Instance.SetState(LoginMenuState.START);
                UILoginManager.Instance.SetState(LoginMenuState.LOGIN);
            }
        }
    }

    #region Request generica
    private IEnumerator SendRequestCoroutine(string uri, WWWForm form, TaskCompletionSource<UnityWebRequest> tcs)
    {
        UnityWebRequest request = form == null ? UnityWebRequest.Get(uri) : UnityWebRequest.Post(uri, form);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            tcs.SetException(new Exception(request.downloadHandler.text));
        }
        else
        {
            tcs.SetResult(request);
        }
    }

    public async Task<UnityWebRequest> SendRequestAsync(string uri, WWWForm form = null)
    {
        var tcs = new TaskCompletionSource<UnityWebRequest>();

        StartCoroutine(SendRequestCoroutine(uri, form, tcs));

        try
        {
            UnityWebRequest request = await tcs.Task;
            return request;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return null;
        }
    }

    public async Task<string> SendRequestAndGetString(string uri, WWWForm form = null)
    {
        UnityWebRequest request = await SendRequestAsync(uri, form);
        return request?.downloadHandler.text;
    }
    #endregion

    #region Registro
    public async Task<BaseResponse> RegisterUserAsync(string name, string email, string password)
    {
        WWWForm form = new();
        form.AddField("name", name);
        form.AddField("email", email);
        form.AddField("password", password);

        string json = await SendRequestAndGetString(URL + "register.php", form);

        if (!string.IsNullOrEmpty(json))
        {
            //Debug.Log("Respuesta del servidor: " + json);
            return JsonUtility.FromJson<BaseResponse>(json);
        }
        else
        {
            return new BaseResponse { success = false, message = "Error de red o del servidor" };
        }
    }

    public async Task Register(string name, string email, string password)
    {
        BaseResponse response = await RegisterUserAsync(name, email, password);

        if (response.success)
        {
            UILoginManager.Instance.SetStatusText(response.message);

            // Login automatico tras registro
            await Login(email, password);
        }
        else
        {
            UILoginManager.Instance.SetErrorText(response.code, response.message);
        }
    }
    #endregion

    #region Login
    public async Task<LoginResponse> LoginUserAsync(string identifier, string password)
    {
        WWWForm form = new();
        form.AddField("identifier", identifier); // Puede ser nombre o email
        form.AddField("password", password);

        string json = await SendRequestAndGetString(URL + "login.php", form);

        if (!string.IsNullOrEmpty(json))
        {
            //Debug.Log("Respuesta del servidor: " + json);
            return JsonUtility.FromJson<LoginResponse>(json);
        }
        else
        {
            return new LoginResponse { success = false, message = "Error de red o del servidor" };
        }
    }

    public async Task<LoginResponse> LoginWithTokenAsync()
    {
        WWWForm form = new();
        form.AddField("session_token", UserSession.SessionToken);

        string json = await SendRequestAndGetString(URL + "validate_token.php", form);

        if (!string.IsNullOrEmpty(json))
        {
            //Debug.Log("Respuesta del servidor: " + json);
            return JsonUtility.FromJson<LoginResponse>(json);
        }
        else
        {
            return new LoginResponse { success = false, message = "Error de red o token inválido" };
        }
    }

    public async Task LoginWithToken()
    {
        var autoLoginResponse = await LoginWithTokenAsync();

        if (autoLoginResponse.success)
        {
            UserSession.SetUserData(autoLoginResponse.user);
            Debug.Log("Bienvenido " + UserSession.Name + " (auto-login)");

            CustomizationData customData = await GetCustomizationDataAsync();
            UserSession.SetUserCustomizationData(customData);
        }
        else
        {
            UserSession.Clear(); // Borra token invalido
            Debug.Log(UserSession.Id);
            Debug.Log("Auto-login fallido: " + autoLoginResponse.message);
        }
    }

    public async Task Login(string identifier, string password)
    {
        LoginResponse response = await LoginUserAsync(identifier, password);

        if (response.success)
        {
            // Gestionar la sesion del usuario
            UserSession.SetUserData(response.user);

            CustomizationData customData = await GetCustomizationDataAsync();
            UserSession.SetUserCustomizationData(customData);

            UILoginManager.Instance.SetStatusText(response.message);
        }
        else
        {
            UILoginManager.Instance.SetErrorText(response.code, response.message);
        }
    }
    #endregion

    #region Logout
    public void Logout()
    {
        UserSession.Clear();
        SceneManager.LoadScene(0);
        Debug.Log("Sesión cerrada");
    }
    #endregion

    #region Personalizacion
    public async Task<CustomizationData> GetCustomizationDataAsync()
    {
        WWWForm form = new();
        form.AddField("session_token", UserSession.SessionToken);

        string json = await SendRequestAndGetString(URL + "get_customization.php", form);

        if (!string.IsNullOrEmpty(json))
        {
            CustomizationResponse response = JsonUtility.FromJson<CustomizationResponse>(json);

            if (response.success)
            {
                Debug.Log("Personalizacion obtenida correctamente");
                return response.customization;
            }
            else
            {
                Debug.LogWarning("Error al obtener personalizacion: " + response.message);
                return null;
            }
        }
        else
        {
            Debug.LogWarning("Respuesta vacia del servidor");
            return null;
        }
    }

    public async Task<bool> UpdateCustomizationFieldAsync(string field, string value)
    {
        WWWForm form = new();
        form.AddField("session_token", UserSession.SessionToken);
        form.AddField("field", field);
        form.AddField("value", value);

        string json = await SendRequestAndGetString(URL + "update_customization.php", form);

        if (!string.IsNullOrEmpty(json))
        {
            BaseResponse response = JsonUtility.FromJson<BaseResponse>(json);

            if (response.success)
            {
                Debug.Log(response.message);
                return true;
            }
            else
            {
                Debug.LogWarning($"Error al actualizar {field}: {response.message}");
                return false;
            }
        }
        else
        {
            Debug.LogWarning("Respuesta vacia del servidor.");
            return false;
        }
    }
    #endregion

    #region Marcadores
    public async Task<bool> AddMatch(MatchData data)
    {
        WWWForm form = new();
        form.AddField("id_player1", data.id_player1);
        form.AddField("score_player1", data.score_player1);
        form.AddField("id_player2", data.id_player2);
        form.AddField("score_player2", data.score_player2);
        form.AddField("id_player3", data.id_player3);
        form.AddField("score_player3", data.score_player3);
        form.AddField("id_player4", data.id_player4);
        form.AddField("score_player4", data.score_player4);
        form.AddField("score_total", data.score_total);
        form.AddField("date", data.date);
        form.AddField("waves", data.waves);
        form.AddField("shots_fired", data.shots_fired);
        form.AddField("obtained_upgrades", data.obtained_upgrades);
        form.AddField("obstacles_destroyed", data.obstacles_destroyed);

        string json = await SendRequestAndGetString(URL + "add_match.php", form);

        if (!string.IsNullOrEmpty(json))
        {
            BaseResponse response = JsonUtility.FromJson<BaseResponse>(json);

            if (response.success)
            {
                Debug.Log(response.message);
                return true;
            }
            else
            {
                Debug.LogWarning($"Error: {response.message}");
                return false;
            }
        }
        else
        {
            Debug.LogWarning("Respuesta vacia del servidor.");
            return false;
        }
    }

    public async Task<List<MatchData>> GetMatches()
    {
        string json = await SendRequestAndGetString(URL + "get_matches.php");

        if (!string.IsNullOrEmpty(json))
        {
            MatchesResponse response = JsonUtility.FromJson<MatchesResponse>(json);

            if (response.success)
            {
                Debug.Log("Partidas obtenidas correctamente");
                return response.games;
            }
            else
            {
                Debug.LogWarning("Error al obtener partidas: " + response.message);
                return null;
            }
        }
        else
        {
            Debug.LogWarning("Respuesta vacia del servidor");
            return null;
        }
    }
    #endregion
}