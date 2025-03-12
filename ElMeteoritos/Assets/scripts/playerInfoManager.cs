using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerInfoManager : MonoBehaviour
{
    public int vidas;
    public int puntuacion;
    public string username;

    private enum powerUP {nada, penetrante, metralleta, escopeta};
    private powerUP mejora;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
