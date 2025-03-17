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
    public Button exitBTN;
    public Button createRoomBTN;
    public Button joinRoomBTN;
    public Button startMatchBTN;

    [Header("Textos")]
    public TextMeshProUGUI roomNameTMP;
}
