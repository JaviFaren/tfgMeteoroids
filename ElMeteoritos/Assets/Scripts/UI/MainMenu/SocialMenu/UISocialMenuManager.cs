using System;
using UnityEngine;

public class UISocialMenuManager : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject matchesPanel;
    public Transform matchesPanelContent;
    public GameObject matchPanelPrefab;

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

                UpdateMenuState(true);
                break;

            case SocialMenuState.DETAILED_MATCH:
                break;
        }
    }
    private void UpdateMenuState(bool showMatches)
    {
        matchesPanel.SetActive(showMatches);
    }
    #endregion

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
}
