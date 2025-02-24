using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEditor.ShaderData;

public class botones : MonoBehaviour
{
    public GameObject registro;
    public GameObject Login;

    public TextMeshProUGUI LogintextFieldUser, LogintextFieldPassword, RegistertextFieldUser, RegistertextFieldPassword, RegistertextFieldPasswordConfirm, RegistertextFieldEmail,
        errorUser, errorEmail, errorPassword, errorPasswordConf, errorUserLogin, errorPasswordLogin;
    public string user, password, email;
    string codigoDevuelto;

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
            else if (codigoDevuelto.Equals("errorCorreo"))
            {
                errorEmail.gameObject.SetActive(true);
                RegistertextFieldEmail.text = "";
            }
            else if (codigoDevuelto.Equals("errorUsuario"))
            {
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
        }
    }

    public void actualizarVariables(int login0Registro1)
    {
        if (RegistertextFieldPassword.text.Equals(RegistertextFieldPasswordConfirm.text) && comprobarpassword(RegistertextFieldPassword.text) && login0Registro1 == 1)
        {
            codigoDevuelto = "";
            user = RegistertextFieldUser.text;
            password = RegistertextFieldPassword.text;
            email = RegistertextFieldEmail.text;

            Registrando();
        }
        else if (!RegistertextFieldPassword.text.Equals(RegistertextFieldPasswordConfirm.text) || !comprobarpassword(RegistertextFieldPassword.text))
        {
            if (!RegistertextFieldPassword.text.Equals(RegistertextFieldPasswordConfirm.text))
            {
                codigoDevuelto = "errorPasswordConf";
            }
        }
        if (login0Registro1 == 0)
        {
            user = LogintextFieldUser.text;
            password = LogintextFieldPassword.text;

            //Llamar a corrutina de login

        }
    }

    public bool comprobarpassword(string password)
    {
        if(password.Contains("@") ||  password.Contains("*") || password.Contains("-") || password.Contains("_") || password.Contains("%") || password.Contains("#")){
            for (int i = 0; i < password.Length; i++)
            {
                if (!char.IsUpper(password[i]))
                {
                    for (int j = 0; j < password.Length; j++)
                    {
                        if (char.IsUpper(password[j])){
                            for (int u = 0; u < password.Length; u++)
                            {
                                if (char.IsDigit(password[i]))
                                {
                                    return true;
                                }
                            }
                            codigoDevuelto = "errorPassword";
                            return false;
                        }
                    }
                    codigoDevuelto = "errorPassword";
                    return false;
                }
            }
            codigoDevuelto = "errorPassword";
            return false;
        }
        return false;
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
}
