using Photon.Realtime;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayMenuManager : MonoBehaviour
{
    [Header("Menus")]
    public GameObject roomsMenu;
    public GameObject inRoomMenu;

    [Header("Botones")]
    public Button createRoomBTN;
    public Button joinRoomBTN;
    public Button startMatchBTN;
    public Button setRoomPrivacyBTN;
    public Button leaveRoomBTN;

    [Header("Textos")]
    public TextMeshProUGUI roomNameTMP;
    public TextMeshProUGUI maxPlayersInRoomTMP;
    public TextMeshProUGUI privacyRoomRMP;

    [Header("Slider")]
    public Slider maxPlayersInRoomSlider;

    [Header("Campos de texto")]
    public TMP_InputField roomNameIF;

    [Header("Jugadores")]
    public List<GameObject> playersPanels;
    public List<GameObject> playersSpaceships;
    public List<TextMeshProUGUI> playersNamesTMP;

    [Header("Salas")]
    public GameObject roomPanelPrefab;
    public GameObject roomsPanelContent;
    private string selectedRoomName = string.Empty;

    [Header("Flags")]
    private bool isRoomPublic = true;

    [Header("Estado")]
    private PlayMenuState _playMenuState = PlayMenuState.START;
    public PlayMenuState PlayMenuState
    {
        get => _playMenuState;
        set
        {
            if (_playMenuState == value) return;
            _playMenuState = value;
            OnStateChange?.Invoke(_playMenuState);
        }
    }
    public event Action<PlayMenuState> OnStateChange;
    private PlayMenuState prevPlayMenuState = PlayMenuState.ROOMS;

    private void OnEnable()
    {
        OnStateChange += HandleStateChange;

        SetState(prevPlayMenuState);

        UIMainMenuManager.Instance.EnableNavigationButtons(true);
    }
    private void OnDisable() => OnStateChange -= HandleStateChange;

    #region Gestionar estado
    public void SetState(PlayMenuState newState) => PlayMenuState = newState;

    private void HandleStateChange(PlayMenuState newState)
    {
        switch (newState)
        {
            case PlayMenuState.ROOMS:
                // ---> Menus
                UpdateMenuState(true, false);
                // ---> Textos
                ClearInputFields(roomNameIF);
                UpdateMaxPlayersText();
                // ---> Botones
                SetRoomPrivacyVisual();
                // ---> Comprobaciones
                CanJoinRoom();
                CanCreateRoom();
                break;

            case PlayMenuState.IN_ROOM:
                // ---> Menus
                UpdateMenuState(false, true);
                break;
        }

        prevPlayMenuState = newState;
    }

    private void UpdateMenuState(bool showRooms, bool showInRoom)
    {
        roomsMenu.SetActive(showRooms);
        inRoomMenu.SetActive(showInRoom);
    }
    #endregion

    #region Cliente maestro
    public void ManageStartMatchButton(bool active) => startMatchBTN.gameObject.SetActive(active);
    #endregion

    #region Gestionar elementos de la interfaz
    public void UpdateMaxPlayersText() => maxPlayersInRoomTMP.text = $"{(int)maxPlayersInRoomSlider.value} Jugadores";
    public void UpdatePlayersPanel(Photon.Realtime.Player[] playerList)
    {
        for (int i = 0; i < playersPanels.Count; i++)
        {
            bool active = i < playerList.Length;
            playersPanels[i].SetActive(active);
            if (active)
            {
                playersNamesTMP[i].text = playerList[i].NickName;
                //playersSpaceships[i].GetComponent<Image>().sprite = ---> Cambiar cuande se guarde la personalizacion de la nave en shared preferences
            }
        }
    }
    public void UpdateRoomName(string name) => roomNameTMP.text = name;
    #endregion

    #region Botones
    public void OnCreateRoomButtonClick() => ConnectionManager.Instance.CreateRoom(roomNameIF.text, (int)maxPlayersInRoomSlider.value, isRoomPublic);
    public void OnJoinRoomButtonClick() => ConnectionManager.Instance.JoinRoom(selectedRoomName);
    public void OnStartMatchButtonClick() => ConnectionManager.Instance.StartMatch();
    public void OnSetRoomPrivacyButtonClick() => ToggleRoomPrivacy();

    public void OnLeaveRoomButtonClick()
    {
        ConnectionManager.Instance.LeaveRoom();
        SetState(PlayMenuState.ROOMS);
    }
    #endregion

    #region Crear sala
    private void ToggleRoomPrivacy()
    {
        isRoomPublic = !isRoomPublic;
        SetRoomPrivacyVisual();
    }
    private void SetRoomPrivacyVisual()
    {
        if (setRoomPrivacyBTN.TryGetComponent<Animator>(out var animator))
        {
            animator.CrossFade(isRoomPublic ? "Open" : "Close", 0.2f);
        }
        privacyRoomRMP.text = isRoomPublic ? "Abierta" : "Cerrada";
    }
    #endregion

    #region Unirse a sala
    public void DisplayRooms(List<RoomInfo> roomList)
    {
        ClearRooms();

        foreach (var room in roomList)
        {
            if (room.IsOpen && room.PlayerCount < room.MaxPlayers)
            {
                var panel = Instantiate(roomPanelPrefab, Vector3.zero, Quaternion.identity, roomsPanelContent.transform);
                panel.GetComponent<RoomPanel>().UpdateRoomPanelInfo(
                    room.Name,
                    room.PlayerCount,
                    room.MaxPlayers,
                    room.IsOpen
                );
            }
        }
    }
    public void SelectRoomToJoin(string roomName)
    {
        selectedRoomName = roomName;
        CanJoinRoom();
    }

    private void ClearRooms()
    {
        foreach (Transform child in roomsPanelContent.transform) Destroy(child.gameObject);
    }
    #endregion

    #region Verificaciones
    public void CanCreateRoom() => createRoomBTN.interactable = !string.IsNullOrEmpty(roomNameIF.text);
    public void CanJoinRoom() => joinRoomBTN.interactable = !string.IsNullOrEmpty(selectedRoomName);
    #endregion

    #region Limpieza de textos
    private void ClearInputFields(params TMP_InputField[] inputs)
    {
        foreach (var input in inputs) input.text = string.Empty;
    }
    #endregion
}
