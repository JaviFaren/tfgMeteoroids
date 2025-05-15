using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager : MonoBehaviourPunCallbacks
{
    public static ConnectionManager Instance { get; private set; }

    private ConnectionStatus ConnectionStatus = ConnectionStatus.NO_CONNECTED;

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
    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    #region Estado de la conexion con Photon
    private void SetConnectionStatus(ConnectionStatus connectionStatus) => this.ConnectionStatus = connectionStatus;
    public ConnectionStatus GetConnectionStatus() => ConnectionStatus;
    #endregion

    #region Gestionar conexion
    public void Connect()
    {
        if (!PhotonNetwork.IsConnected)
        {
            SetConnectionStatus(ConnectionStatus.CONNECTING);

            PhotonNetwork.AuthValues = new AuthenticationValues((UserSession.Id).ToString());
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.LocalPlayer.NickName = UserSession.Name;
        }
    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Conectado al master con UserId: " + PhotonNetwork.LocalPlayer.UserId);

        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        SetConnectionStatus(ConnectionStatus.CONNECTED);
    }
    #endregion

    #region Gestionar desconexion
    public void Disconnect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        SetConnectionStatus(ConnectionStatus.NO_CONNECTED);
        Debug.Log("Desconectado del master - " + cause);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    #endregion

    #region Crear sala
    public RoomOptions SetUpRoom(int maxPlayers = 4, bool isOpen = true)
    {
        return new()
        {
            MaxPlayers = maxPlayers,
            IsVisible = isOpen,
            IsOpen = isOpen
        };
    }
    public void CreateRoom(string roomName, int roomMaxPlayers, bool roomIsOpen)
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.CreateRoom(roomName, SetUpRoom(roomMaxPlayers, roomIsOpen), TypedLobby.Default); ;
        }
    }
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        //UIMainMenuManager.Instance.playMenuManager.SetState(PlayMenuState.IN_ROOM);
        Debug.Log($"Sala creada - {PhotonNetwork.CurrentRoom.Name} | Max Players: {PhotonNetwork.CurrentRoom.MaxPlayers} | Open: {PhotonNetwork.CurrentRoom.IsOpen}");
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log("No ha sido posible crear la sala - " + message + " _ " + returnCode);
    }
    #endregion

    #region Unirse a sala
    public void JoinRoom(string roomName)
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.JoinRoom(roomName);
        }
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Debug.Log("Conectado a la sala");

        UIMainMenuManager.Instance.playMenuManager.SetState(PlayMenuState.IN_ROOM);

        UIMainMenuManager.Instance.playMenuManager.ManageStartMatchButton(PhotonNetwork.IsMasterClient);

        UIMainMenuManager.Instance.playMenuManager.UpdatePlayersPanel(PhotonNetwork.PlayerList);
        UIMainMenuManager.Instance.playMenuManager.UpdateRoomName(PhotonNetwork.CurrentRoom.Name);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log("No ha sido posible conectarse a la sala - " + message + " _ " + returnCode);
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        UIMainMenuManager.Instance.playMenuManager.UpdatePlayersPanel(PhotonNetwork.PlayerList);
    }
    #endregion

    #region Unirse a sala aleatoria
    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.Log("No ha sido posible conectarse a la sala - " + message + " _ " + returnCode);

        JoinOrCreateRoom();
    }

    public void JoinOrCreateRoom()
    {
        RoomOptions roomOptions = SetUpRoom();

        PhotonNetwork.JoinOrCreateRoom("Sala de " + PhotonNetwork.LocalPlayer.NickName, roomOptions, TypedLobby.Default);
    }
    #endregion

    #region Abandonar sala
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        Debug.Log("Desconectado de la sala");
    }
    #endregion

    #region Salas
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        UIMainMenuManager.Instance.playMenuManager.DisplayRooms(roomList);
    }
    #endregion

    #region Partida
    public void StartMatch()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel(2);
    }

    public void ReturnToMainMenu()
    {
        LeaveMatch();
        LeaveRoom();
    }

    public void LeaveMatch()
    {
        PhotonNetwork.LoadLevel(1);
    }
    #endregion
}
