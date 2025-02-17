using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class botones : MonoBehaviour
{
    public GameObject registro;
    public GameObject Login;

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

    

    public void conexionDB()
    {
        
    }

    public void esPulsado()
    {
        print("pulsado");
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
        StartCoroutine(llamadaTest());
    }

    public IEnumerator llamadaTest()
    {
        UnityWebRequest C = new UnityWebRequest();
        WWWForm form = new WWWForm();
        form.AddField("usuario", user);
        form.AddField("password", password);
        form.AddField("correo", email);
        C = UnityWebRequest.Post("http://tfgmeteoroids.mygamesonline.org/test.php", form);
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
