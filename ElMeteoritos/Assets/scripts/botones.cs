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

    public TextMeshProUGUI LogintextFieldUser, LogintextFieldPassword, RegistertextFieldUser, RegistertextFieldPassword, RegistertextFieldPasswordConfirm, RegistertextFieldEmail;
    public string user, password, email;

    // Start is called before the first frame update
    void Start()
    {
        registro.SetActive(false);
        Login.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void actualizarVariables(int login0Registro1)
    {
        if (RegistertextFieldPassword.text.Equals(RegistertextFieldPasswordConfirm.text) && login0Registro1 == 0)
        {
            user = LogintextFieldUser.text;
            password = LogintextFieldPassword.text;

            //Llamar a corrutina de login
        }
        else if (login0Registro1 == 1)
        {
            user = RegistertextFieldUser.text;
            password = RegistertextFieldPassword.text;
            email = RegistertextFieldEmail.text;

            Registrando();
            //Redireccionar a login tras el registro
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
        Debug.Log("mensaje: " + C.downloadHandler.text);
    }
}
