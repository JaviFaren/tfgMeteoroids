using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class Connection : MonoBehaviour
{
    public TextMeshProUGUI maxPlayersText;
    public int maxPlayerid;
    public bool openLobby;
    public Image candado;

    public TextMeshProUGUI nombreSala;

    // Start is called before the first frame update
    void Start()
    {
        maxPlayerid = 1;
        openLobby = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CrearSala()
    {
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
}
