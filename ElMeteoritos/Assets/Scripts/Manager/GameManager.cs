using Photon.Pun;
using System.Collections;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviourPun
{
    public static GameManager Instance { get; private set; }

    [Header("Partida")]
    [SerializeField] private MatchState matchState;

    [Header("Informacion de partida")]
    [SerializeField] private int wave;
    [SerializeField] private WaveType waveType;
    [SerializeField] private WaveConfig currentWaveConfig;

    [Header("Spawn de enemigosg")]
    [SerializeField] private WaveSettings waveSettings;

    [Header("Camara")]
    public CameraBounds cameraBounds;

    [Header("Flags")]
    [SerializeField] private SyncedBool isMatchActive = new("IsMatchActive");
    [SerializeField] private SyncedBool isWaveActive = new("IsWaveActive");

    private bool AllEnemiesDefeated => currentWaveConfig.DefeatedEnemies >= currentWaveConfig.TotalEnemies;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        currentWaveConfig = new WaveConfig();
        cameraBounds = new CameraBounds();
        //waveSettings = new WaveSettings();
    }

    private void Start()
    {
        StartCoroutine(InitializeMatch());
    }

    #region Partida
    private IEnumerator InitializeMatch()
    {
        yield return PlayerManager.Instance.SpawnPlayers();
        yield return null;

        cameraBounds.UpdateLargestScreen();
        yield return null;
        PlayerManager.Instance.ActivatePlayersScreenPositionCheck();

        yield return new WaitForSeconds(1f);

        Debug.Log("Todos los jugadores preparados - Iniciando partida");
        yield return StartMatch();
        //StartCoroutine(StartMatch());
    }

    private IEnumerator StartMatch()
    {
        //if (PhotonNetwork.IsMasterClient) isMatchActive.Value = true;
        //Debug.Log($"[StartMatch] isMatchActive: {isMatchActive.Value}");
        if (PhotonNetwork.IsMasterClient)
        {
            isMatchActive.Value = true;
        }
        else
        {
            yield return new WaitUntil(() => isMatchActive.Value);
        }

        Debug.Log($"[StartMatch] isMatchActive: {isMatchActive.Value}");

        matchState = MatchState.START_WAVE;

        yield return MatchLoop();
    }

    private IEnumerator MatchLoop()
    {
        while (isMatchActive.Value)
        {
            switch (matchState)
            {
                case MatchState.START_WAVE:
                    yield return StartWavePhase();
                    break;

                case MatchState.WAVE:
                    yield return WavePhase();
                    break;

                case MatchState.END_WAVE:
                    yield return EndWavePhase();
                    break;
            }
        }

        yield return OnEndGame();
    }

    private IEnumerator StartWavePhase()
    {
        UIGameManager.Instance.ManagePlayersPanel(false);

        currentWaveConfig.Reset();
        wave++;
        waveType = SetWaveType();
        Debug.Log("Tipo de oleada -> " + waveType);
        currentWaveConfig = waveSettings.CalculateWaveConfig(wave, PlayerManager.Instance.GetPlayersCount());

        yield return UIGameManager.Instance.WaveStarterText(wave);
        UIGameManager.Instance.ManagePlayersPanel(true);

        PlayerManager.Instance.SetPlayersCombatState(true);
        matchState = MatchState.WAVE;
    }

    private IEnumerator WavePhase()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            isWaveActive.Value = true;
        }
        else
        {
            yield return new WaitUntil(() => isWaveActive.Value);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            switch (waveType)
            {
                case WaveType.COMMON_WAVE:
                    yield return CommonWave();
                    break;

                case WaveType.SPECIAL_WAVE:
                    yield return SpecialWave();
                    break;

                case WaveType.EASTEREGG_WAVE:
                    yield return EasterEggWave();
                    break;
            }
        }
        else
        {
            while (isWaveActive.Value) yield return null;
        }

        matchState = MatchState.END_WAVE;
    }

    private IEnumerator EndWavePhase()
    {
        Debug.Log(" --- OLEADA TERMINADA --- ");
        yield return new WaitForSeconds(0.75f);
        matchState = MatchState.START_WAVE;
    }

    public IEnumerator OnEndGame()
    {
        yield return UIGameManager.Instance.EndMatchCountdownText();

        if (PhotonNetwork.IsMasterClient)
        {
            SaveMatchData();
            ConnectionManager.Instance.ReturnToMainMenu();
        }
    }

    private async void SaveMatchData()
    {
        var playerList = PlayerManager.Instance.GetPlayersList();
        int[] ids = new int[4];
        int[] scores = new int[4];
        int shotsFired = 0;
        int obstaclesDestroyed = 0;
        int obtainedUpgrades = 0;

        for (int i = 0; i < playerList.Count && i < 4; i++)
        {
            var player = playerList[i];
            ids[i] = player.playerID;
            scores[i] = player.playerStats.Score;
            shotsFired += player.playerStats.ShotsFired;
            obstaclesDestroyed += player.playerStats.EnemiesDefeated;
            obtainedUpgrades += player.playerStats.ObtainedUpgrades;
        }

        MatchData currentMatchData = new()
        {
            id_player1 = ids[0],
            score_player1 = scores[0],
            id_player2 = ids[1],
            score_player2 = scores[1],
            id_player3 = ids[2],
            score_player3 = scores[2],
            id_player4 = ids[3],
            score_player4 = scores[3],
            score_total = scores.Sum(),
            date = System.DateTime.Now.ToString("dd-MM-yyyy"),
            waves = wave,
            shots_fired = shotsFired,
            obstacles_destroyed = obstaclesDestroyed,
            obtained_upgrades = obtainedUpgrades
        };

        await PHPManager.Instance.AddMatch(currentMatchData);
    }

    public void CheckForEndGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (PlayerManager.Instance.GetPlayersList().TrueForAll(obj => obj.IsDead))
            {
                Debug.Log("Todos los jugadores han muerto. Terminando la partida.");
                isWaveActive.Value = false;
                isMatchActive.Value = false;
            }
        }
    }
    #endregion

    #region Oleada
    private WaveType SetWaveType()
    {
        return WaveType.COMMON_WAVE;

        // ---> Ejemplo para que las rondas especiales sean cada 5 rondas y las rondas con easter egg sean con una probabilidad del 10% a partir de la ronda 20

        //if (wave >= 20) 
        //{
        //    //float randomFactor = Random.Range(0f, 1f);
        //    if (Random.Range(0f, 1f) <= 0.1)
        //    {
        //        return WaveType.EASTEREGG_WAVE;
        //    }                        
        //}
        //if (wave % 5 == 0)                      
        //{
        //    return WaveType.SPECIAL_WAVE;
        //}
        //else
        //{
        //    return WaveType.COMMON_WAVE;
        //}
    }

    private IEnumerator CommonWave()
    {
        while (!AllEnemiesDefeated && isWaveActive.Value)
        {
            int currentEnemyCount = EnemyManager.Instance.GetEnemiesCount();
            int remainingEnemies = currentWaveConfig.TotalEnemies - currentWaveConfig.DefeatedEnemies;

            if (currentEnemyCount <= waveSettings.MaxEnemiesOnScreen &&
                currentEnemyCount + currentWaveConfig.DefeatedEnemies < currentWaveConfig.TotalEnemies)
            {
                //EnemyManager.Instance.SpawnEnemy(currentWaveConfig.GetNextEnemyType());
                EnemyManager.Instance.SpawnEnemy(EnemyType.METEOROID_DIVISIBLE);
                yield return new WaitForSeconds(waveSettings.GetCurrentSpawnDelay());
            }
            else
            {
                yield return null;
            }
        }

        if (PhotonNetwork.IsMasterClient) isWaveActive.Value = false;
    }

    private IEnumerator SpecialWave()
    {
        yield break;
    }

    private IEnumerator EasterEggWave()
    {
        yield break;
    }

    public void RegisterEnemyDefeat()
    {
        currentWaveConfig.DefeatedEnemies++;
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(SyncDefeatedEnemies), RpcTarget.Others, currentWaveConfig.DefeatedEnemies);
        }
    }

    [PunRPC]
    private void SyncDefeatedEnemies(int count)
    {
        currentWaveConfig.DefeatedEnemies = count;
    }
    #endregion

    #region Clases
    [System.Serializable]
    public class WaveConfig
    {
        public int TotalEnemies;
        public int CommonEnemies;
        public int SpecialEnemies;
        public int DefeatedEnemies;

        public int remainingCommon;
        public int remainingSpecial;

        public void Reset()
        {
            TotalEnemies = 0;
            CommonEnemies = 0;
            SpecialEnemies = 0;
            DefeatedEnemies = 0;
            remainingCommon = 0;
            remainingSpecial = 0;
        }

        public void Initialize(int common, int special)
        {
            CommonEnemies = remainingCommon = common;
            SpecialEnemies = remainingSpecial = special;
            TotalEnemies = common + special;
            DefeatedEnemies = 0;
        }

        public EnemyType GetNextEnemyType()
        {
            if (remainingSpecial == 0 && remainingCommon == 0)
            {
                Debug.LogWarning("No quedan enemigos por spawnear en esta oleada.");
                return EnemyType.METEOROID_COMMON; // Valor por defecto
            }

            if (remainingSpecial == 0)
            {
                remainingCommon--;
                return EnemyManager.Instance.GetRandomCommonEnemyType();
            }

            if (remainingCommon == 0)
            {
                remainingSpecial--;
                return EnemyManager.Instance.GetRandomSpecialEnemyType();
            }

            float total = remainingCommon + remainingSpecial;
            float roll = Random.Range(0f, total);

            if (roll < remainingCommon)
            {
                remainingCommon--;
                return EnemyManager.Instance.GetRandomCommonEnemyType();
            }

            remainingSpecial--;
            return EnemyManager.Instance.GetRandomSpecialEnemyType();
        }
    }

    [System.Serializable]
    public class WaveSettings
    {
        [SerializeField] private float baseSpawnDelay;
        [SerializeField] private float spawnDelayReduction;
        [SerializeField] private float specialEnemyPercentage;
        [SerializeField] private int specialEnemyWaveDelay;
        [SerializeField] private int maxEnemyNumberOnScreen;

        private float currentSpawnDelay;

        public int MaxEnemiesOnScreen => maxEnemyNumberOnScreen;

        public WaveConfig CalculateWaveConfig(int waveNumber, int playerCount)
        {
            var config = new WaveConfig();

            int playerFactor = Mathf.RoundToInt(1 + (playerCount - 1) * 0.25f);
            int commonEnemies = Mathf.CeilToInt((3 + Mathf.Pow(waveNumber, 1.5f)) * playerFactor);

            int specialEnemies = 0;
            if (waveNumber > specialEnemyWaveDelay)
            {
                specialEnemies = Mathf.CeilToInt(commonEnemies * specialEnemyPercentage);
                commonEnemies -= specialEnemies;

                // Aumentar la dificultad gradualmente
                specialEnemyPercentage = Mathf.Clamp(specialEnemyPercentage + 0.01f, 0f, 0.8f);
            }

            AdjustSpawnDelay(waveNumber);
            config.Initialize(commonEnemies, specialEnemies);

            return config;
        }

        private void AdjustSpawnDelay(int waveNumber)
        {
            if (waveNumber > 2 && currentSpawnDelay > 2f)
            {
                currentSpawnDelay = baseSpawnDelay * Mathf.Pow(1f - spawnDelayReduction, waveNumber - 2);
                currentSpawnDelay = Mathf.Max(currentSpawnDelay, 0.5f);
            }
            else
            {
                currentSpawnDelay = baseSpawnDelay;
            }
        }

        public float GetCurrentSpawnDelay() => currentSpawnDelay;
    }

    [System.Serializable]
    public class CameraBounds
    {
        private float cameraDistance;
        private Vector3 bottomLeftBorder;
        private Vector3 topRightBorder;

        public Vector3 BottomLeft => bottomLeftBorder;
        public Vector3 TopRight => topRightBorder;

        public void UpdateLargestScreen()
        {
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;

            var players = PlayerManager.Instance.GetPlayersList();
            if (players.Count == 0)
            {
                SetLocalCameraBorders();
                return;
            }

            foreach (var player in players)
            {
                minX = Mathf.Min(minX, player.BottomLeftBorder.x);
                minY = Mathf.Min(minY, player.BottomLeftBorder.y);
                maxX = Mathf.Max(maxX, player.TopRightBorder.x);
                maxY = Mathf.Max(maxY, player.TopRightBorder.y);
            }

            cameraDistance = Mathf.Abs(Camera.main.transform.position.z);
            bottomLeftBorder = new Vector3(minX, minY, cameraDistance);
            topRightBorder = new Vector3(maxX, maxY, cameraDistance);
        }

        private void SetLocalCameraBorders()
        {
            Camera mainCamera = Camera.main;
            cameraDistance = Mathf.Abs(mainCamera.transform.position.z);
            bottomLeftBorder = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, cameraDistance));
            topRightBorder = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, cameraDistance));
        }
    }

    public class SyncedBool
    {
        private string rpcName;
        private bool value;

        public bool Value
        {
            get => value;
            set
            {
                if (this.value == value) return;

                this.value = value;

                GameManager.Instance.photonView.RPC(nameof(GameManager.Instance.RPC_SyncGameFlag), RpcTarget.OthersBuffered, rpcName, value);
            }
        }

        public SyncedBool(string rpcName)
        {
            this.rpcName = rpcName;
        }
    }
    #endregion

    #region RPCs
    [PunRPC]
    public void RPC_SyncGameFlag(string flagName, bool value)
    {
        switch (flagName)
        {
            case "IsMatchActive": isMatchActive.Value = value; break;
            case "IsWaveActive": isWaveActive.Value = value; break;
        }
    }
    #endregion
}
