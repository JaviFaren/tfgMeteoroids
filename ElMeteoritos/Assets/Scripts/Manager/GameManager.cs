//using Photon.Pun;
//using System.Collections;
//using UnityEngine;

//public class GameManager : MonoBehaviourPun
//{
//    public static GameManager Instance { get; private set; }

//    [Header("Partida")]
//    [SerializeField] private MatchState matchState;
//    //public Match currentMatch;

//    [Header("Informacion de oleada")]
//    [SerializeField] private int wave;
//    [SerializeField] private WaveType waveType;
//    [SerializeField] private int waveEnemyNumber;
//    [SerializeField] private int waveCommonEnemyNumber;
//    [SerializeField] private int remainingCommonEnemies;
//    [SerializeField] private int waveSpecialEnemyNumber;
//    [SerializeField] private int remainingSpecialEnemies;
//    [SerializeField] private int waveDefeatedEnemiesNumber;

//    [Header("Spawn de enemigos")]
//    [SerializeField] private float enemySpawnDelay;
//    [SerializeField] private float enemySpawnDelayReduction;
//    [SerializeField] private float specialEnemyPercentage;
//    [SerializeField] private int specialEnemyWaveDelay;
//    [SerializeField] private int maxEnemyNumberOnScreen;

//    [Header("Camara")]
//    float cameraDistance;
//    Vector3 bottomLeftBorder;
//    Vector3 topRightBorder;

//    [Header("Flags")]
//    [SerializeField] private bool _isMatchActive;
//    public bool IsMatchActive
//    {
//        get => _isMatchActive;
//        set => _isMatchActive = value;
//    }
//    [SerializeField] private bool _isWaveActive;
//    public bool IsWaveActive
//    {
//        get => _isWaveActive;
//        set => _isWaveActive = value;
//    }
//    private bool AllEnemiesDefeated => waveDefeatedEnemiesNumber == waveEnemyNumber;

//    private void Awake()
//    {
//        // Singleton
//        if (Instance != null && Instance != this)
//        {
//            Destroy(gameObject);
//            return;
//        }
//        Instance = this;
//    }
//    private void Start()
//    {
//        StartCoroutine(InitializeMatch());
//    }

//    #region Camara

//    public void UpdateLargestScreen()
//    {
//        float minX = float.MaxValue;
//        float minY = float.MaxValue;
//        float maxX = float.MinValue;
//        float maxY = float.MinValue;

//        foreach (var player in PlayerManager.Instance.GetPlayersList())
//        {
//            minX = Mathf.Min(minX, player.BottomLeftBorder.x);
//            minY = Mathf.Min(minY, player.BottomLeftBorder.y);
//            maxX = Mathf.Max(maxX, player.TopRightBorder.x);
//            maxY = Mathf.Max(maxY, player.TopRightBorder.y);
//        }

//        if (PlayerManager.Instance.GetPlayersCount() == 0)
//        {
//            SetCameraBordersLocal();
//            return;
//        }

//        bottomLeftBorder = new Vector3(minX, minY, cameraDistance);
//        topRightBorder = new Vector3(maxX, maxY, cameraDistance);

//        Debug.Log($"Borders updated - Min: {bottomLeftBorder}, Max: {topRightBorder}");
//    }

//    private void SetCameraBordersLocal()
//    {
//        Camera mainCamera = Camera.main;

//        cameraDistance = Mathf.Abs(mainCamera.transform.position.z);
//        bottomLeftBorder = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, cameraDistance));
//        topRightBorder = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, cameraDistance));
//    }

//    public Vector3 GetCameraBottomLeftBorder() => bottomLeftBorder;

//    public Vector3 GetCameraTopRightBorder() => topRightBorder;

//    #endregion

//    #region Partida

//    private IEnumerator InitializeMatch()
//    {
//        yield return StartCoroutine(PlayerManager.Instance.SpawnPlayers());

//        yield return null;

//        UpdateLargestScreen();

//        yield return null;

//        PlayerManager.Instance.ActivatePlayersScreenPositionCheck();

//        yield return null;

//        yield return new WaitForSeconds(1f);

//        Debug.Log("Todos los jugadores listos - Iniciando partida");
//        StartCoroutine(StartMatch());
//    }

//    public IEnumerator StartMatch()
//    {
//        IsMatchActive = true;
//        matchState = MatchState.START_WAVE;

//        yield return null;

//        StartCoroutine(MatchLoop());
//    }

//    public IEnumerator MatchLoop()
//    {
//        while (IsMatchActive)
//        {
//            switch (matchState)
//            {
//                case MatchState.START_WAVE:

//                    UIGameManager.Instance.ManagePlayersPanel(false); // Se desactiva el panel con informacion de los jugadores

//                    ResetWaveStats();

//                    wave++;
//                    waveType = SetWaveType();
//                    Debug.Log("Tipo de oleada -> " + waveType);
//                    waveEnemyNumber = CalculateNumberOfEnemies();
//                    Debug.Log("Nº maximo de enemigos -> " + waveEnemyNumber);

//                    yield return UIGameManager.Instance.WaveStarterText(wave);
//                    UIGameManager.Instance.ManagePlayersPanel(true);

//                    foreach (var player in PlayerManager.Instance.GetPlayersList())
//                    {
//                        player.CanShoot = player.CanGetDamaged = true;
//                    }

//                    matchState = MatchState.WAVE;

//                    break;

//                case MatchState.WAVE:

//                    IsWaveActive = true;

//                    if (PhotonNetwork.IsMasterClient)
//                    {
//                        switch (waveType)
//                        {
//                            case WaveType.COMMON_WAVE:

//                                yield return StartCoroutine(CommonWave());
//                                break;

//                            case WaveType.SPECIAL_WAVE:
//                                break;

//                            case WaveType.EASTEREGG_WAVE:
//                                break;
//                        }
//                    }
//                    else
//                    {
//                        while (IsWaveActive) yield return null;
//                    }

//                    matchState = MatchState.END_WAVE;

//                    break;

//                case MatchState.END_WAVE:

//                    Debug.Log(" --- OLEADA TERMINADA --- ");
//                    yield return new WaitForSeconds(0.75f);
//                    matchState = MatchState.START_WAVE;

//                    break;
//            }
//        }
//    }

//    #endregion

//    #region Configuracion de oleada

//    public WaveType SetWaveType()
//    {
//        return WaveType.COMMON_WAVE;

//        // Adapatar este metodo para que la ronda sea de un tipo u otro en funcion de las condiciones que queramos

//        // ---> Ejemplo para que las rondas especiales sean cada 5 rondas y las rondas con easter egg sean con una probabilidad del 10% a partir de la ronda 20

//        //if (wave >= 20) 
//        //{
//        //    //float randomFactor = Random.Range(0f, 1f);
//        //    if (Random.Range(0f, 1f) <= 0.1)
//        //    {
//        //        return WaveType.EASTEREGG_WAVE;
//        //    }                        
//        //}
//        //if (wave % 5 == 0)                      
//        //{
//        //    return WaveType.SPECIAL_WAVE;
//        //}
//        //else
//        //{
//        //    return WaveType.COMMON_WAVE;
//        //}
//    }
//    public int CalculateNumberOfEnemies()
//    {
//        int playersFactor = GetPlayersFactor();

//        waveCommonEnemyNumber = CalculateCommonEnemyCount(playersFactor);
//        waveSpecialEnemyNumber = CalculateSpecialEnemyCount();

//        waveCommonEnemyNumber -= waveSpecialEnemyNumber;

//        AdjustSpawnDelay();

//        remainingCommonEnemies = waveCommonEnemyNumber;
//        remainingSpecialEnemies = waveSpecialEnemyNumber;

//        return waveCommonEnemyNumber + waveSpecialEnemyNumber;
//    }

//    private int GetPlayersFactor()
//    {
//        return Mathf.RoundToInt(1 + (PlayerManager.Instance.GetPlayersCount() - 1) * 0.25f);
//    }
//    private int CalculateCommonEnemyCount(int playersFactor)
//    {
//        return Mathf.CeilToInt((3 + Mathf.Pow(wave, 1.5f)) * playersFactor);
//    }
//    private int CalculateSpecialEnemyCount()
//    {
//        if (wave <= specialEnemyWaveDelay)
//            return 0;

//        int specialEnemies = Mathf.CeilToInt(waveCommonEnemyNumber * specialEnemyPercentage);
//        waveCommonEnemyNumber -= specialEnemies;

//        // Aumentar la dificultad gradualmente
//        specialEnemyPercentage = Mathf.Clamp(specialEnemyPercentage + 0.01f, 0f, 0.8f);

//        return specialEnemies;
//    }
//    private void AdjustSpawnDelay()
//    {
//        if (wave > 2 && enemySpawnDelay > 2)
//        {
//            enemySpawnDelay -= enemySpawnDelay * enemySpawnDelayReduction;
//            //enemySpawnDelay = Mathf.Max(enemySpawnDelay, 0.5f); // límite mínimo opcional
//        }
//    }

//    public EnemyType GetRandomEnemyTypeForWave()
//    {
//        // Si no hay enemigos restantes
//        if (remainingSpecialEnemies == 0 && remainingCommonEnemies == 0)
//        {
//            Debug.LogWarning("No quedan enemigos por spawnear en esta oleada.");
//            return EnemyType.METEOROID_COMMON; // Valor por defecto
//        }

//        // Si no quedan enemigos especiales
//        if (remainingSpecialEnemies == 0)
//        {
//            remainingCommonEnemies--;
//            return EnemyManager.Instance.GetRandomCommonEnemyType();
//        }

//        // Si no quedan enemigos comunes
//        if (remainingCommonEnemies == 0)
//        {
//            remainingSpecialEnemies--;
//            return EnemyManager.Instance.GetRandomSpecialEnemyType();
//        }

//        // Si quedan enemigos comunes y especiales
//        float total = remainingCommonEnemies + remainingSpecialEnemies;
//        float roll = Random.Range(0f, total);

//        if (roll < remainingCommonEnemies)
//        {
//            remainingCommonEnemies--;
//            return EnemyManager.Instance.GetRandomCommonEnemyType();
//        }
//        else
//        {
//            remainingSpecialEnemies--;
//            return EnemyManager.Instance.GetRandomSpecialEnemyType();
//        }
//    }

//    public void SetWaveDefeatedEnemies(int amount)
//    {
//        waveDefeatedEnemiesNumber = Mathf.Clamp(waveDefeatedEnemiesNumber + amount, 0, waveEnemyNumber);
//        photonView.RPC(nameof(SyncWaveDefeatedEnemies), RpcTarget.Others, waveDefeatedEnemiesNumber);
//    }

//    [PunRPC]
//    public void SyncWaveDefeatedEnemies(int amount)
//    {
//        waveDefeatedEnemiesNumber = amount;
//    }

//    private void ResetWaveStats()
//    {
//        waveDefeatedEnemiesNumber = 0;
//        //EnemyManager.Instance.ClearEnemiesList();

//        foreach (var player in PlayerManager.Instance.GetPlayersList())
//        {
//            player.CanShoot = player.CanGetDamaged = false;
//        }
//    }

//    #endregion

//    #region Oleadas

//    public IEnumerator CommonWave()
//    {
//        while (!AllEnemiesDefeated)
//        {
//            int currentEnemyCount = EnemyManager.Instance.GetEnemiesCount();
//            int totalEnemiesToSpawn = waveEnemyNumber - waveDefeatedEnemiesNumber;

//            if (currentEnemyCount <= maxEnemyNumberOnScreen && currentEnemyCount + waveDefeatedEnemiesNumber < waveEnemyNumber)
//            {
//                EnemyManager.Instance.SpawnEnemy(GetRandomEnemyTypeForWave());
//                Debug.Log("Nº Enemigos activos -> " + currentEnemyCount);
//                yield return new WaitForSeconds(enemySpawnDelay);
//            }
//            else
//            {
//                yield return null;
//            }
//        }

//        // Sincronizacion del estado de la oleada
//        Player masterClient = PlayerManager.Instance.GetMasterPlayer();
//        masterClient.photonView.RPC(nameof(masterClient.RPC_SyncGameFlag), RpcTarget.All, nameof(IsWaveActive), false);
//    }

//    #endregion
//}

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
                EnemyManager.Instance.SpawnEnemy(currentWaveConfig.GetNextEnemyType());
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
