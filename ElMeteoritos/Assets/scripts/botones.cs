using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class botones : MonoBehaviour
{
    public GameObject registro;
    public GameObject Login;

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
}
