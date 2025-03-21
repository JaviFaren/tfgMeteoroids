using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour
{
    public TextMeshProUGUI roomNameTMP;
    public TextMeshProUGUI roomPlayersNum;
    public Image roomPrivacy;

    public Sprite openRoom;
    public Sprite closeRoom;

    public void SetName(string name)
    {
        roomNameTMP.text = name;
    }

    public void SetPlayersNum(int playersCount, int maxPlayersCount)
    {
        roomPlayersNum.text = playersCount + "/" + maxPlayersCount;
    }

    public void SetRoomPrivacy(bool isRoomOpen)
    {
        if (isRoomOpen)
        {
            roomPrivacy.sprite = openRoom;
        }
        else
        {
            roomPrivacy.sprite = closeRoom;
        }
    }

    public void UpdateRoomPanelInfo(string name, int playersCount, int maxPlayersCount, bool isRoomOpen)
    {
        SetName(name);
        SetPlayersNum(playersCount, maxPlayersCount);
        SetRoomPrivacy(isRoomOpen);
    }

    public void OnRoomClick()
    {
        MenuManager.instance.playMenuManager.selectedRoomName = roomNameTMP.text;
        MenuManager.instance.playMenuManager.CanJoinRoom();      
    }
}
