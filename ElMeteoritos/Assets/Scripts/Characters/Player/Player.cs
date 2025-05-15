using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Componentes")]
    [HideInInspector] public PlayerStats playerStats;
    [HideInInspector] public PlayerActions playerActions;
    [HideInInspector] public PlayerPowerUp playerPowerUp;
    [HideInInspector] public Spaceship spaceship;
    [HideInInspector] public PhotonView photonView;

    [Header("Propiedades")]
    public string username;
    public int playerID;

    [Header("Flags")]
    [SerializeField] private bool _canMove;
    [SerializeField] private bool _canShoot;
    [SerializeField] private bool _canGetDamaged;
    [SerializeField] private bool _isOverheat;
    [SerializeField] private bool _isDead;
    [SerializeField] private bool _isInitialized;
    [SerializeField] private bool _isUIReady;
    public bool CanMove
    {
        get => _canMove;
        set => photonView.RPC(nameof(RPC_SyncFlag), RpcTarget.All, nameof(CanMove), value);
    }
    public bool CanShoot
    {
        get => _canShoot;
        set => photonView.RPC(nameof(RPC_SyncFlag), RpcTarget.All, nameof(CanShoot), value);
    }
    public bool CanGetDamaged
    {
        get => _canGetDamaged;
        set => photonView.RPC(nameof(RPC_SyncFlag), RpcTarget.All, nameof(CanGetDamaged), value);
    }
    public bool IsOverheat
    {
        get => _isOverheat;
        set => photonView.RPC(nameof(RPC_SyncFlag), RpcTarget.All, nameof(IsOverheat), value);
    }
    public bool IsDead
    {
        get => _isDead;
        set => photonView.RPC(nameof(RPC_SyncFlag), RpcTarget.All, nameof(IsDead), value);
    }
    public bool IsInitialized
    {
        get => _isInitialized;
        set => photonView.RPC(nameof(RPC_SyncFlag), RpcTarget.All, nameof(IsInitialized), value);
    }

    public Vector3 BottomLeftBorder { get; private set; }
    public Vector3 TopRightBorder { get; private set; }

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();

        playerStats = GetComponent<PlayerStats>();
        playerActions = GetComponent<PlayerActions>();
        playerPowerUp = GetComponent<PlayerPowerUp>();
        spaceship = GetComponentInChildren<Spaceship>();
    }
    private void Start()
    {
        if (!photonView.IsMine) return;

        CalculateScreenSize();

        UIGameManager.Instance.shootBTN.onClick.AddListener(playerActions.Fire);
        if (PhotonNetwork.LocalPlayer.IsMasterClient) EnemyManager.Instance.InitializeEnemyPools();
        StartCoroutine(InitializeRoutine(int.Parse(PhotonNetwork.LocalPlayer.UserId), PhotonNetwork.LocalPlayer.NickName));
    }

    #region CAMERA
    private void CalculateScreenSize()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        float cameraDistance = Mathf.Abs(cam.transform.position.z);
        BottomLeftBorder = cam.ViewportToWorldPoint(new Vector3(0, 0, cameraDistance));
        TopRightBorder = cam.ViewportToWorldPoint(new Vector3(1, 1, cameraDistance));

        photonView.RPC(nameof(RPC_SyncScreenSize), RpcTarget.AllBuffered, BottomLeftBorder, TopRightBorder);
    }
    [PunRPC]
    private void RPC_SyncScreenSize(Vector3 bottomLeftBorder, Vector3 topRightBorder)
    {
        BottomLeftBorder = bottomLeftBorder;
        TopRightBorder = topRightBorder;
    }
    #endregion

    #region INITIALIZE PLAYER
    private IEnumerator InitializeRoutine(int id, string nickname)
    {
        yield return null;

        Initialize(id, nickname);
        yield return new WaitUntil(() => IsInitialized && _isUIReady);
        Debug.Log($"Jugador {playerID} completamente inicializado");
    }
    public void Initialize(int id, string nickname)
    {
        photonView.RPC(nameof(SyncInitialize), RpcTarget.All, id, nickname);
    }
    [PunRPC]
    private void SyncInitialize(int id, string username)
    {
        this.playerID = id;
        this.username = username;

        spaceship.Initialize();

        PlayerManager.Instance.AddPlayerToPlayersList(this);
        StartCoroutine(InitializeUI());
    }
    private IEnumerator InitializeUI()
    {
        yield return null;

        if (UIGameManager.Instance != null && playerStats != null)
        {
            UIGameManager.Instance.InitilizePlayerPanel(
                playerID,
                username,
                playerStats.CurrentLives
            );
            photonView.RPC(nameof(SyncUIReady), RpcTarget.All);
        }
    }
    [PunRPC]
    private void SyncUIReady()
    {
        _isUIReady = true;
        Debug.Log($"UI lista para jugador {playerID}");

        if (photonView.IsMine)
        {
            FinishInitialization();
        }
    }
    public void FinishInitialization()
    {
        CanMove = true;
        IsInitialized = true;
    }
    #endregion

    #region ON PLAYER HIT
    public void TakeDamage(int damage)
    {
        if (!CanGetDamaged) return;

        CanGetDamaged = false;
        playerStats.ModifyLives(damage);
        photonView.RPC(nameof(OnHitBehavior), RpcTarget.All);
    }
    [PunRPC]
    public void OnHitBehavior()
    {
        playerActions.Stop();
        StartCoroutine(HandleHitEffect());
    }
    private IEnumerator HandleHitEffect()
    {
        spaceship.PlayTargetAnimation("Death");

        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo stateInfo = spaceship.spaceshipAnim.GetCurrentAnimatorStateInfo(0);
            return stateInfo.IsName("Death") && stateInfo.normalizedTime >= 1f;
        });

        HandlePlayerDeath();
    }
    #endregion

    #region PLAYER DEATH
    private void HandlePlayerDeath()
    {
        if (playerStats.CurrentLives <= 0)
        {
            IsDead = true;
            gameObject.SetActive(false);
            GameManager.Instance.CheckForEndGame();
        }
        else
        {
            RespawnPlayer();
        }
    }
    #endregion

    #region PLAYER RESPAWN
    private void RespawnPlayer()
    {
        transform.position = Vector3.zero;
        spaceship.PlayTargetAnimation("Respawn");
    }
    #endregion

    #region FLAGS
    [PunRPC]
    public void RPC_SyncFlag(string flagName, bool value)
    {
        switch (flagName)
        {
            case nameof(CanMove): _canMove = value; break;
            case nameof(CanShoot): _canShoot = value; break;
            case nameof(CanGetDamaged): _canGetDamaged = value; break;
            case nameof(IsOverheat): _isOverheat = value; break;
            case nameof(IsDead): _isDead = value; break;
            case nameof(IsInitialized): _isInitialized = value; break;
        }
    }
    #endregion
}
