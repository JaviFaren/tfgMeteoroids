using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class ConnectionManager : MonoBehaviourPunCallbacks
{
    public static ConnectionManager instance;

    public TextMeshProUGUI maxPlayersText;
    public int maxPlayerid;
    public bool openLobby;
    public Image candado;

    public Image botonCrear, botonBuscar, botonCrearSala, botonCerrarSala, botonJugar;
    public GameObject panelCrear, panelBuscar, panelSalaCreada;

    public TextMeshProUGUI nombreSala;

    private void Awake()
    {
        //  Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        //DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        Connect();

        maxPlayerid = 1;
        openLobby = true;

        botonCrear.color = MenuManager.instance.colors["buttonCustomizationSelected"];
        botonBuscar.color = MenuManager.instance.colors["buttonUnselected"];
    }

    //  Gestionar conexión
    public void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Conectado al master");
        //PhotonNetwork.LocalPlayer.NickName = UIManager.instance.mainMenuManager.playerName; --> Coger el nombre de usuario de shared Prefernces
    }

    //  Gestionar sala
    public RoomOptions SetUpRoom()
    {
        return new()
        {
            MaxPlayers = 4,
            IsVisible = true,
            IsOpen = true
        };
    }
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom("Sala de " + PhotonNetwork.LocalPlayer.NickName, SetUpRoom(), TypedLobby.Default); // ---> Cambiar nombre de la sala

        Debug.Log("Sala creada");
    }
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        MenuManager.instance.playMenuManager.SetState(GameMenuState.IN_ROOM);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log("No ha sido posible crear la sala - " + message + " _ " + returnCode);
    }
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Conectado a la sala");

        if (PhotonNetwork.IsMasterClient)
        {
            MenuManager.instance.playMenuManager.ManageStartMatchButton();
        }
        else
        {
            MenuManager.instance.playMenuManager.ManageStartMatchButton(false);
        }
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log("No ha sido posible conectarse a la sala - " + message + " _ " + returnCode);
    }


    //  Gestionar desconexión
    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.Log("Desconectado del master - " + cause);
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        Debug.Log("Desconectado de la sala");
    }

    //  Jugar
    public void Play()
    {
        PhotonNetwork.LoadLevel(2);
    }
    public void DisconnectFromGame()
    {
        PhotonNetwork.LoadLevel(1);
        //Disconnect();
    }
}

//public void CrearSala()
//{
//    botonCrearSala.gameObject.SetActive(false);

//    RoomOptions r = new RoomOptions();
//    r.MaxPlayers = maxPlayerid;
//    r.IsOpen = openLobby;
//    r.IsVisible = true;

//    if(nombreSala.text == "")
//    {
//        nombreSala.text = "SalaDefault";
//    }

//    PhotonNetwork.CreateRoom(nombreSala.text, r, TypedLobby.Default);
//}

//public void maxPlayersState()
//{
//    if(maxPlayerid < 4)
//    {
//        maxPlayerid++;
//    }
//    else
//    {
//        maxPlayerid = 1;
//    }
//    maxPlayersText.text = maxPlayerid + " Jugadores";
//}

//public void IsOpen()
//{
//    if (openLobby)
//    {
//        openLobby = false;
//        candado.color = Color.red;
//    }
//    else
//    {
//        openLobby = true;
//        candado.color = Color.green;
//    }
//}

//public void SalaState(int state)
//{
//    if(state == 1)
//    {
//        botonCrear.color = menuManager.colors["buttonPlaySelected"];
//        botonBuscar.color = menuManager.colors["buttonUnselected"];
//        panelCrear.SetActive(true);
//        panelBuscar.SetActive(false);
//    }
//    else
//    {
//        botonBuscar.color = menuManager.colors["buttonPlaySelected"];
//        botonCrear.color = menuManager.colors["buttonUnselected"];
//        panelBuscar.SetActive(true);
//        panelCrear.SetActive(false);
//        //ejecutar la busqueda de salas visibles
//    }
//}

//public override void OnCreatedRoom()
//{
//    panelSalaCreada.SetActive(true);
//}

//public void borrarSala()
//{
//    PhotonNetwork.LeaveRoom();
//    panelSalaCreada.SetActive(false);
//}

//[PunRPC]
//public void iniciarPartida()
//{
//    //Cositas de empezar a cambiar de escena
//}
