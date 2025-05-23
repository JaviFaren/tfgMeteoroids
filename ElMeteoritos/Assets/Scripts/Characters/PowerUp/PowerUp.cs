using Photon.Pun;
using System.Collections;
using UnityEngine;

public class PowerUp : MonoBehaviourPun
{
    private PowerUpEffect _effect;
    private Coroutine _lifetimeCoroutine;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    [PunRPC]
    public void RPC_Initialize(int enemyTypeInt, int effectIndex)
    {
        var config = DatabaseManager.Instance.enemyPowerUpConfigDatabase.GetEnemyPowerUpConfig((EnemyType)enemyTypeInt);
        var powerUps = config?.allowedPowerUps;

        if (powerUps == null || effectIndex < 0 || effectIndex >= powerUps.Count)
        {
            Debug.LogWarning("[PowerUp] Invalid config or effect index.");
            return;
        }

        Initialize(powerUps[effectIndex]);
    }

    public void Initialize(PowerUpEffect effect)
    {
        if (effect == null || effect.icon == null)
        {
            Debug.LogError("[PowerUp] Invalid effect or icon.");
            return;
        }

        _effect = effect;
        _spriteRenderer.sprite = _effect.icon;
        _lifetimeCoroutine = StartCoroutine(DestroyAfterLifetime(_effect.lifetime));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;

        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent(out Player player))
            {
                if (player.playerPowerUp.TryApplyPowerUpEffect(_effect))
                {
                    if (_lifetimeCoroutine != null) StopCoroutine(_lifetimeCoroutine);
                    PhotonNetwork.Destroy(gameObject);
                }
            }
        }
    }

    private IEnumerator DestroyAfterLifetime(float time)
    {
        yield return new WaitForSeconds(time);

        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
