using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class botones : MonoBehaviour
{
    public GameObject registro;
    public GameObject Login;

    public TextMeshProUGUI LogintextFieldUser, LogintextFieldPassword, RegistertextFieldUser, RegistertextFieldPassword, RegistertextFieldPasswordConfirm, RegistertextFieldEmail,
        errorUser, errorEmail, errorPassword, errorPasswordConf, errorUserLogin, errorPasswordLogin;
    public string user, password, email;
    string errorUserMSG1 = "Este usuario ya existe, elige otro.", errorUserMSG2 = "Este nombre es demasiado largo (Max. 16 caracteres) o demasiado corto.",
        errorEmailMSG1 = "Este correo ya esta en uso.", errorEmailMSG2 = "Correo inválido.";
    string codigoDevuelto;
    private int passwordMinLength = 12, userMaxLength = 16;

    // Start is called before the first frame update
    void Start()
    {
        registro.SetActive(false);
        Login.SetActive(true);
        codigoDevuelto = "";

    }

    // Update is called once per frame
    void Update()
    {
        if (RegistertextFieldPassword.text.Equals(RegistertextFieldPasswordConfirm.text))
        {
            errorPasswordConf.gameObject.SetActive(false);
        }
        else
        {
            errorPasswordConf.gameObject.SetActive(true);
        }

        if (!codigoDevuelto.Equals(""))
        {
            if (codigoDevuelto.Equals("exitoRegistro"))
            {
                codigoDevuelto = "";
                errorUser.gameObject.SetActive(false);
                errorEmail.gameObject.SetActive(false);
                errorPassword.gameObject.SetActive(false);
                errorPasswordConf.gameObject.SetActive(false);
                VolverLogin();
            }
            else if (codigoDevuelto.Equals("exitoLogin"))
            {
                errorPasswordLogin.gameObject.SetActive(false);
                errorUserLogin.gameObject.SetActive(false);
                SceneManager.LoadScene(1);
            }
            else if (codigoDevuelto.Equals("errorCorreo"))
            {
                errorEmail.text = errorEmailMSG1;
                errorEmail.gameObject.SetActive(true);
                RegistertextFieldEmail.text = "";
            }
            else if (codigoDevuelto.Equals("errorEmail@"))
            {
                errorEmail.text = errorEmailMSG2;
                errorEmail.gameObject.SetActive(true);
                RegistertextFieldEmail.text = "";
            }
            else if (codigoDevuelto.Equals("errorUsuario"))
            {
                errorUser.text = errorUserMSG1;
                errorUser.gameObject.SetActive(true);
                RegistertextFieldUser.text = "";
            }
            else if (codigoDevuelto.Equals("errorUserLong"))
            {
                errorUser.text = errorUserMSG2;
                errorUser.gameObject.SetActive(true);
                RegistertextFieldUser.text = "";
            }
            else if (codigoDevuelto.Equals("errorPasswordConf"))
            {
                errorPasswordConf.gameObject.SetActive(true);
            }
            else if (codigoDevuelto.Equals("errorPassword"))
            {
                errorPassword.gameObject.SetActive(true);
            }
            else if (codigoDevuelto.Equals("errorPasswordLogin"))
            {
                errorPasswordLogin.gameObject.SetActive(true);
            }
            else if (codigoDevuelto.Equals("errorUserLogin"))
            {
                errorUserLogin.gameObject.SetActive(true);
            }
        }
    }

    public void actualizarVariables(int login0Registro1)
    {
        if (RegistertextFieldPassword.text.Equals(RegistertextFieldPasswordConfirm.text) && comprobarpassword(RegistertextFieldPassword.text) && comprobarUser() && comprobarCorreo() && login0Registro1 == 1)
        {
            codigoDevuelto = "";
            user = RegistertextFieldUser.text;
            password = RegistertextFieldPassword.text;
            email = RegistertextFieldEmail.text;

            Registrando();
        }
        else if (!RegistertextFieldPassword.text.Equals(RegistertextFieldPasswordConfirm.text) && login0Registro1 == 1)
        {
            codigoDevuelto = "errorPasswordConf";
        }
        else if (!comprobarpassword(RegistertextFieldPassword.text) && login0Registro1 == 1)
        {
            codigoDevuelto = "errorPassword";
        }
        else if (!comprobarUser())
        {
            codigoDevuelto = "errorUserLong";
        }
        else if (!comprobarCorreo())
        {
            codigoDevuelto = "errorEmail@";
        }
        if (login0Registro1 == 0)
        {
            user = LogintextFieldUser.text;
            password = LogintextFieldPassword.text;

            Logueando();

        }
    }

    public bool comprobarCorreo()
    {
        if (RegistertextFieldEmail.text.Contains("@") && RegistertextFieldEmail.text.Length > 3)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool comprobarUser()
    {
        if(RegistertextFieldUser.text.Length <= userMaxLength && RegistertextFieldUser.text.Length > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool comprobarpassword(string password)
    {
        bool tieneCaracter = false;
        bool tieneMayus = false;
        bool tieneMinus = false;
        bool tieneNum = false;
        for (int i = 0; i < password.Length; i++)
        {
            if (!char.IsUpper(password[i]) && !tieneMayus)
            {
                tieneMayus = true;
            }
            if (char.IsLower(password[i]) && !tieneMinus)
            {
                tieneMinus = true;
            }
            if (char.IsDigit(password[i]) && !tieneNum)
            {
                tieneNum = true;
            }
            if ((password[i] == '@' || password[i] == '*' || password[i] == '-' || password[i] == '_' || password[i] == '%' || password[i] == '#') && !tieneCaracter)
            {
                tieneCaracter = true;
            }
        }
        if (tieneCaracter && tieneMayus && tieneMinus && tieneNum && password.Length >= passwordMinLength){
            return true;
        }
        else
        {
            return false;
        }
    }

    public void IrARegistro()
    {
        Login.SetActive(false);
        registro.SetActive(true);
    }

    public void VolverLogin()
    {
        Login.SetActive(true);
        registro.SetActive(false);
    }

    public void Registrando()
    {
        StartCoroutine(addRegistro());
    }

    public IEnumerator addRegistro()
    {
        UnityWebRequest C = new UnityWebRequest();
        WWWForm form = new WWWForm();
        form.AddField("usuario", user);
        form.AddField("password", password);
        form.AddField("correo", email);
        C = UnityWebRequest.Post("http://tfgmeteoroids.mygamesonline.org/registroDB.php", form);
        Debug.Log("lanzadito");
        yield return C.SendWebRequest();
        Debug.Log("lanzadito2");
        while (!C.isDone)
        {
            Debug.Log("esperando...");
            yield return null;
        }
        codigoDevuelto = C.downloadHandler.text;
    }

    public void Logueando()
    {
        StartCoroutine(loginRequest());
    }

    public IEnumerator loginRequest()
    {
        UnityWebRequest C = new UnityWebRequest();
        WWWForm form = new WWWForm();
        form.AddField("usuario", user);
        form.AddField("password", password);
        form.AddField("correo", email);
        C = UnityWebRequest.Post("http://tfgmeteoroids.mygamesonline.org/loginDB.php", form);
        Debug.Log("lanzadito");
        yield return C.SendWebRequest();
        Debug.Log("lanzadito2");
        while (!C.isDone)
        {
            Debug.Log("esperando...");
            yield return null;
        }
        codigoDevuelto = C.downloadHandler.text;
        Debug.Log(codigoDevuelto);
    }
}
