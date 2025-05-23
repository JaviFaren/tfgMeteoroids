using Photon.Pun;
using System.Collections;
using UnityEngine;

// Clase hija de Enemy para los meteoritos explosivos
// Las estadisticas como la vida o la velocidad se modifican en el prefab con el inspector de Unity
public class ExplosiveMeteoroid : Enemy
{
    protected override void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;

        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<Player>(out var player))
            {
                player.TakeDamage(-damage);
            }
        }
        //else if (other.CompareTag("PlayerShot"))
        //{
        //    if (other.TryGetComponent<PlayerShot>(out var shot))
        //    {
        //        OnHitBehavior(-shot.damage, shot.ownerPlayerID);
        //    }
        //}
    }

    public override Vector3 GetSpawnPosition() => EnemyManager.Instance.GetRandomSpawnPoint();

    protected override void OnSpawnBehavior(float lag)
    {
        SetRandomTargetAndMoveTowards(lag);
    }

    public override void OnHitBehavior(int damage, int playerID)
    {
        ModifyLives(damage);

        if (currentLives <= 0)
        {
            rb.velocity = Vector3.zero;
            StartCoroutine(Explosion(playerID));
        }
    }

    protected override void OnDeathBehavior()
    {
        photonView.RPC(nameof(RPC_ManageSpriteRenderer), RpcTarget.All, true);
    }

    private IEnumerator Explosion(int playerID)
    {
        photonView.RPC(nameof(RPC_PlayAnimation), RpcTarget.All, "Explosion");

        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            return stateInfo.IsName("Explosion") && stateInfo.normalizedTime >= 1f;
        });

        OnDeath(playerID);
    }

    [PunRPC]
    void RPC_PlayAnimation(string animationName)
    {
        anim.Play(animationName);
    }

    [PunRPC]
    void RPC_ManageSpriteRenderer(bool active)
    {
        sr.enabled = active;
    }
}
