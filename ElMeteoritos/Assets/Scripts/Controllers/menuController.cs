using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Composites;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class menuController : MonoBehaviour
{
    public GameObject[] BarraInferior;
    public GameObject[] PantallasAsociadas;
    private Color selectedColor = new Color32(102, 37, 236, 255);
    private Color unselectedColor = new Color32(183, 139, 253, 255);
    public int selectedButton = -1;

    public GameObject[] VentanasPersonaliz;
    public int TaquillaState = 0;
    public TextMeshProUGUI tituloTaquilla;
    private string titulo1 = "COLOR", titulo2 = "PROPULSOR", titulo3 = "ESTELA";
    private string[] titulos = new string[3];



    // Start is called before the first frame update
    void Start()
    {
        tituloTaquilla.fontSharedMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 0);
        titulos[0] = titulo1;
        titulos[1] = titulo2;
        titulos[2] = titulo3;
        UpdateTaquilla();
        selectSection(-1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void selectSection(int ButtonID)
    {
        for(int i = 0; i < BarraInferior.Length; i++)
        {
            BarraInferior[i].GetComponent<Image>().color = unselectedColor;
            PantallasAsociadas[i].SetActive(false);
        }

        if(ButtonID != -1)
        {
            BarraInferior[ButtonID].GetComponent<Image>().color = selectedColor;
            PantallasAsociadas[ButtonID].SetActive(true);
        }
    }

    public void UpdateTaquilla()
    {
        for(int i = 0; i < VentanasPersonaliz.Length; i++)
        {
            VentanasPersonaliz[i].SetActive(false);
        }
        VentanasPersonaliz[TaquillaState].SetActive(true);
        tituloTaquilla.text = titulos[TaquillaState];
    }

    public void toggleWindow(bool esSiguiente)
    {
        if (esSiguiente)
        {
            if(TaquillaState == 2)
            {
                TaquillaState = 0;
            }
            else
            {
                TaquillaState++;
            }
        }
        else
        {
            if(TaquillaState == 0)
            {
                TaquillaState = 2;
            }
            else
            {
                TaquillaState--;
            }
        }
        UpdateTaquilla();
    }

}
