using Photon.Pun;
using System.Collections;
using UnityEngine;

public class PowerUp : MonoBehaviourPun
{
    private PowerUpEffect _effect;
    private Coroutine _lifetimeCoroutine;

    public void Initialize(PowerUpEffect effect)
    {
        _effect = effect;
        GetComponent<SpriteRenderer>().sprite = _effect.icon;
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
