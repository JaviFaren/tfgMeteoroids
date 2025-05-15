using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : MonoBehaviour
{
    [Header("Componentes")]
    private RectTransform _rectTransform;

    [Header("Textos")]
    public TextMeshProUGUI roomNameTMP;
    public TextMeshProUGUI roomPlayersNum;

    [Header("Imagenes")]
    public Image roomPrivacy;

    [Header("Sprites")]
    public Sprite openRoom;
    public Sprite closeRoom;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }
    private void Start()
    {
        _rectTransform.SetAsFirstSibling();
    }

    // ---> Setters
    public void SetName(string name) => roomNameTMP.text = name;
    public void SetPlayersNum(int playersCount, int maxPlayersCount) => roomPlayersNum.text = $"{playersCount}/{maxPlayersCount}";
    public void SetRoomPrivacy(bool isRoomOpen) => roomPrivacy.sprite = isRoomOpen ? openRoom : closeRoom;

    // ---> Gestion de la informacion de la sala
    public void UpdateRoomPanelInfo(string name, int playersCount, int maxPlayersCount, bool isRoomOpen)
    {
        SetName(name);
        SetPlayersNum(playersCount, maxPlayersCount);
        SetRoomPrivacy(isRoomOpen);
    }

    // ---> Botones
    public void OnRoomClick()
    {
        UIMainMenuManager.Instance.playMenuManager.SelectRoomToJoin(roomNameTMP.text);
    }
}
