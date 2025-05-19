using Photon.Pun;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [HideInInspector] public Player playerManager;

    [Header("Vida")]
    [SerializeField] private int _maxLives;
    [SerializeField] private int _currentLives;

    [Header("Disparo")]
    [SerializeField] private ShootType _shootType;
    [SerializeField] private int _defaultShootDamage;
    private int? shootDamageOverride = null;

    [Header("Estadisticas de partida")]
    [SerializeField] private int _score;
    [SerializeField] private int _enemiesDefeated;
    [SerializeField] private int _shotsFired;
    [SerializeField] private int _obtainedUpgrades;

    // Propiedades publicas con encapsulamiento. Son solo de lectura
    public int MaxLives => _maxLives;
    public int CurrentLives => _currentLives;
    public ShootType ShootType => _shootType;
    public int ShootDamage => shootDamageOverride ?? _defaultShootDamage;
    public int Score => _score;
    public int EnemiesDefeated => _enemiesDefeated;
    public int ShotsFired => _shotsFired;
    public int ObtainedUpgrades => _obtainedUpgrades;

    private void Awake()
    {
        playerManager = GetComponent<Player>();
    }
    private void Start()
    {
        ModifyLives(MaxLives);
    }

    #region Vidas
    public void ModifyLives(int amount)
    {
        int newLives = Mathf.Clamp(_currentLives + amount, 0, _maxLives);
        if (newLives != _currentLives)
        {
            playerManager.photonView.RPC(nameof(SyncLives), RpcTarget.All, newLives);
        }
    }

    [PunRPC]
    private void SyncLives(int lives)
    {
        _currentLives = lives;
        UIGameManager.Instance.UpdatePlayerPanelLives(playerManager.playerID, CurrentLives);
    }
    #endregion

    #region Puntuacion
    public void ModifyScore(int amount)
    {
        if (amount != 0)
        {
            int newScore = _score + amount;
            playerManager.photonView.RPC(nameof(SyncScore), RpcTarget.All, newScore);
        }
    }

    [PunRPC]
    private void SyncScore(int score)
    {
        _score = score;
        UIGameManager.Instance.UpdatePlayerPanelScore(playerManager.playerID, Score);
    }
    #endregion

    #region Disparo
    public void ModifyShootProperties(ShootType newType, float customHeatPerShot, int customDamage)
    {
        playerManager.photonView.RPC(nameof(SyncShootProperties), RpcTarget.All, newType, customHeatPerShot, customDamage);
    }

    [PunRPC]
    private void SyncShootProperties(ShootType newType, float customHeatPerShot, int customDamage)
    {
        _shootType = newType;

        if (customHeatPerShot >= 0)
            playerManager.playerActions.SetHeatPerShotOverride(customHeatPerShot);
        else
            playerManager.playerActions.ResetHeatPerShot();

        if (customDamage >= 0)
            SetShootDamageOverride(customDamage);
        else
            ResetShootDamage();
    }

    public void SetShootDamageOverride(int value) => shootDamageOverride = value;
    public void ResetShootDamage() => shootDamageOverride = null;
    #endregion

    #region Enemigos derrotados
    public void IncrementEnemiesDefeated()
    {
        playerManager.photonView.RPC(nameof(SyncIncrementEnemiesDefeated), RpcTarget.All);
    }

    [PunRPC]
    private void SyncIncrementEnemiesDefeated() => _enemiesDefeated++;
    #endregion

    #region Disparos realizados
    public void IncrementShotsFired()
    {
        playerManager.photonView.RPC(nameof(SyncIncrementShotsFired), RpcTarget.All);
    }

    [PunRPC]
    private void SyncIncrementShotsFired() => _shotsFired++;
    #endregion
}
