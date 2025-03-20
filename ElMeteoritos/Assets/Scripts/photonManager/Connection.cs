using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class Connection : MonoBehaviourPunCallbacks
{
    public MenuManager menuManager;

    public TextMeshProUGUI maxPlayersText;
    public int maxPlayerid;
    public bool openLobby;
    public Image candado;

    public Image botonCrear, botonBuscar, botonCrearSala, botonCerrarSala, botonJugar;
    public GameObject panelCrear, panelBuscar, panelSalaCreada;

    public TextMeshProUGUI nombreSala;

    // Start is called before the first frame update
    void Start()
    {
        maxPlayerid = 1;
        openLobby = true;

        botonCrear.color = menuManager.colors["buttonCustomizationSelected"];
        botonBuscar.color = menuManager.colors["buttonUnselected"];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CrearSala()
    {
        botonCrearSala.gameObject.SetActive(false);

        RoomOptions r = new RoomOptions();
        r.MaxPlayers = maxPlayerid;
        r.IsOpen = openLobby;
        r.IsVisible = true;

        if(nombreSala.text == "")
        {
            nombreSala.text = "SalaDefault";
        }

        PhotonNetwork.CreateRoom(nombreSala.text, r, TypedLobby.Default);
    }

    public void maxPlayersState()
    {
        if(maxPlayerid < 4)
        {
            maxPlayerid++;
        }
        else
        {
            maxPlayerid = 1;
        }
        maxPlayersText.text = maxPlayerid + " Jugadores";
    }

    public void IsOpen()
    {
        if (openLobby)
        {
            openLobby = false;
            candado.color = Color.red;
        }
        else
        {
            openLobby = true;
            candado.color = Color.green;
        }
    }

    public void SalaState(int state)
    {
        if(state == 1)
        {
            botonCrear.color = menuManager.colors["buttonPlaySelected"];
            botonBuscar.color = menuManager.colors["buttonUnselected"];
            panelCrear.SetActive(true);
            panelBuscar.SetActive(false);
        }
        else
        {
            botonBuscar.color = menuManager.colors["buttonPlaySelected"];
            botonCrear.color = menuManager.colors["buttonUnselected"];
            panelBuscar.SetActive(true);
            panelCrear.SetActive(false);
            //ejecutar la busqueda de salas visibles
        }
    }

    public override void OnCreatedRoom()
    {
        panelSalaCreada.SetActive(true);
    }

    public void borrarSala()
    {
        PhotonNetwork.LeaveRoom();
        panelSalaCreada.SetActive(false);
    }

    [PunRPC]
    public void iniciarPartida()
    {
        //Cositas de empezar a cambiar de escena
    }
}
