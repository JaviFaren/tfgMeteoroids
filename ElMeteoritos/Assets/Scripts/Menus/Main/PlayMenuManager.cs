using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayMenuManager : MonoBehaviour
{
    [Header("Menus")]
    public GameObject roomsMenu;
    public GameObject inRoomMenu;

    [Header("Botones")]
    public Button createRoomBTN;
    public Button joinRoomBTN;
    public Button startMatchBTN;
    public Button privateRoomBTN;

    [Header("Textos")]
    public TextMeshProUGUI roomNameTMP;
    public TextMeshProUGUI maxPlayersInRoomTMP;

    [Header("Slider")]
    public Slider maxPlayersInRoomSlider;

    [Header("Capos de texto")]
    public TMP_InputField roomNameIF;

    [Header("Estado")]
    public GameMenuState game_Menu_State;
    public GameMenuState gameMenuState
    {
        get { return game_Menu_State; }
        set
        {
            if (game_Menu_State == value) return;
            game_Menu_State = value;
            OnStateChange?.Invoke(game_Menu_State);
        }
    }
    public delegate void OnVariableChangeDelegate(GameMenuState newState);
    public event OnVariableChangeDelegate OnStateChange;

    [Header("Flags")]
    private bool canCreateRoom;
    private bool canJoinRoom;
    public bool isRoomPublic;

    private void Start()
    {
        OnStateChange += ManageGameMenuState;
        SetState(GameMenuState.ROOMS);
    }

    // ---> Maquina de estados del menu de juego
    public void SetState(GameMenuState newState)
    {
        gameMenuState = newState;
    }
    private void ManageGameMenuState(GameMenuState newState) // ---> Funcion que se llama cuando se cambia la variable gameMenuState y cambia entres los diferentes menus
    {
        switch (newState)
        {
            case GameMenuState.ROOMS:

                UpdateMaxPlayersText();

                joinRoomBTN.interactable = false; // ---> Cuando tengamos lo de mostar las salas habra que cambiarlo
                createRoomBTN.interactable = false;

                inRoomMenu.SetActive(false);
                roomsMenu.SetActive(true);

                break;

            case GameMenuState.IN_ROOM:

                roomsMenu.SetActive(false);
                inRoomMenu.SetActive(true);

                break;
        }
    }

    public void ManageStartMatchButton(bool active = true)
    {
        startMatchBTN.gameObject.SetActive(active);
    }

    public void UpdateMaxPlayersText()
    {
        maxPlayersInRoomTMP.text = (maxPlayersInRoomSlider.value).ToString();
    }

    public void SetPublicRoom()
    {
        if (isRoomPublic)
        {
            privateRoomBTN.gameObject.GetComponent<Animator>().CrossFade("Close", 0.2f);
            isRoomPublic = false;
        }
        else
        {
            privateRoomBTN.gameObject.GetComponent<Animator>().CrossFade("Open", 0.2f);
            isRoomPublic = true;
        }
    }
    public void CanCreateRoom()
    {
        if (roomNameIF.text.Equals(""))
        {
            canCreateRoom = false;
        }
        else
        {
            canCreateRoom = true;
        }

        createRoomBTN.interactable = canCreateRoom;
    }
    public void CanJoinRoom()
    {

    }

    public int getRoomMaxPlayers()
    {
        return (int)maxPlayersInRoomSlider.value;
    }
    public string getRoomName()
    {
        return roomNameIF.text;
    }
}
