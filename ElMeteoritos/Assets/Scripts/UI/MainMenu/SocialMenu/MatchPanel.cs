using TMPro;
using UnityEngine;

public class MatchPanel : MonoBehaviour
{
    public MatchData MatchData;

    [Header("Textos")]
    public TextMeshProUGUI player1NameTMP;
    public TextMeshProUGUI player2NameTMP;
    public TextMeshProUGUI player3NameTMP;
    public TextMeshProUGUI player4NameTMP;
    public TextMeshProUGUI totalScoreTMP;
    public TextMeshProUGUI wavesTMP;

    public void Initialize(MatchData matchData)
    {
        this.MatchData = matchData;

        player1NameTMP.text = DisplayPlayerName(MatchData.name_player1);
        player2NameTMP.text = DisplayPlayerName(MatchData.name_player2);
        player3NameTMP.text = DisplayPlayerName(MatchData.name_player3);
        player4NameTMP.text = DisplayPlayerName(MatchData.name_player4);

        totalScoreTMP.text = MatchData.score_total.ToString();
        wavesTMP.text = MatchData.waves.ToString();
    }

    string DisplayPlayerName(string name)
{
    return string.IsNullOrEmpty(name) ? "Sin jugador" : name;
}

    public void OnMatchPanelClick()
    {

    }
}
