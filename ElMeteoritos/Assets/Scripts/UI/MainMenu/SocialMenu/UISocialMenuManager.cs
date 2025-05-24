using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISocialMenuManager : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject matchesPanel;
    public Transform matchesPanelContent;
    public GameObject matchPanelPrefab;
    public GameObject detailedMatchPanel;
    private MatchData currentMatchData;

    [Header("Textos")]
    public TextMeshProUGUI player1NameTMP;
    public TextMeshProUGUI player1ScoreTMP;
    public TextMeshProUGUI player2NameTMP;
    public TextMeshProUGUI player2ScoreTMP;
    public TextMeshProUGUI player3NameTMP;
    public TextMeshProUGUI player3ScoreTMP;
    public TextMeshProUGUI player4NameTMP;
    public TextMeshProUGUI player4ScoreTMP;
    public TextMeshProUGUI totalScoreTMP;
    public TextMeshProUGUI wavesTMP;
    public TextMeshProUGUI obtainedUpgradesTextTMP;
    public TextMeshProUGUI obtainedUpgradesNumTMP;
    public TextMeshProUGUI shorsFiredTextTMP;
    public TextMeshProUGUI shorsFiredNumTMP;
    public TextMeshProUGUI obstaclesDestroyedTextTMP;
    public TextMeshProUGUI obstaclesDestroyedNumTMP;

    [Header("Botones")]
    public Button backBTN;

    [Header("Estado")]
    private SocialMenuState _socialMenuState = SocialMenuState.START;
    public SocialMenuState SocialMenuState
    {
        get => _socialMenuState;
        set
        {
            if (_socialMenuState == value) return;
            _socialMenuState = value;
            OnStateChange?.Invoke(_socialMenuState);
        }
    }
    public event Action<SocialMenuState> OnStateChange;

    private void OnEnable()
    {
        OnStateChange += HandleStateChange;
        SetState(SocialMenuState.MATCHES_LIST);

        UIMainMenuManager.Instance.EnableNavigationButtons(true);
    }
    private void OnDisable()
    {
        OnStateChange -= HandleStateChange;
        SetState(SocialMenuState.START);
    }

    #region Gestionar estado
    public void SetState(SocialMenuState newState) => SocialMenuState = newState;
    private void HandleStateChange(SocialMenuState newState)
    {
        switch (newState)
        {
            case SocialMenuState.MATCHES_LIST:
                LoadMatches();
                UpdateMenuState(true, false);
                break;

            case SocialMenuState.DETAILED_MATCH:
                InitializeDetailedMatchPanel();
                UpdateMenuState(false, true);
                break;
        }
    }
    private void UpdateMenuState(bool showMatches, bool showDetailedMatch)
    {
        matchesPanel.SetActive(showMatches);
        detailedMatchPanel.SetActive(showDetailedMatch);
    }
    #endregion

    #region Marcadores
    private void ClearMatches()
    {
        foreach (Transform child in matchesPanelContent) Destroy(child.gameObject);
    }

    private async void LoadMatches()
    {
        ClearMatches();

        var matchesList = await PHPManager.Instance.GetMatches();

        foreach (var match in matchesList)
        {
            var selectorGO = Instantiate(matchPanelPrefab, matchesPanelContent);
            if (selectorGO.TryGetComponent(out MatchPanel manager))
            {
                manager.Initialize(match);
            }
        }
    }
    #endregion

    #region Partida Detallada
    public void SetMatchData(MatchData matchData)
    {
        if (matchData == null) return;

        currentMatchData = matchData;
    }

    private void InitializeDetailedMatchPanel()
    {
        player1NameTMP.text = DisplayPlayerName(currentMatchData.name_player1);
        player1ScoreTMP.text = $"{currentMatchData.score_player1} pts";
        player2NameTMP.text = DisplayPlayerName(currentMatchData.name_player2);
        player2ScoreTMP.text = $"{currentMatchData.score_player2} pts";
        player3NameTMP.text = DisplayPlayerName(currentMatchData.name_player3);
        player3ScoreTMP.text = $"{currentMatchData.score_player3} pts";
        player4NameTMP.text = DisplayPlayerName(currentMatchData.name_player4);
        player4ScoreTMP.text = $"{currentMatchData.score_player4} pts";

        totalScoreTMP.text = $"{currentMatchData.score_total} pts";
        wavesTMP.text = $"Oleadas {currentMatchData.waves}";

        obtainedUpgradesNumTMP.text = currentMatchData.obtained_upgrades.ToString();
        shorsFiredNumTMP.text = currentMatchData.shots_fired.ToString();
        obstaclesDestroyedNumTMP.text = currentMatchData.obstacles_destroyed.ToString();
    }

    string DisplayPlayerName(string name) => string.IsNullOrEmpty(name) ? "Sin jugador" : name;
    #endregion

    #region Botones
    public void OnBackButtonClick()
    {
        currentMatchData = null;
        SetState(SocialMenuState.MATCHES_LIST);
    }
    #endregion
}
