using Photon.Pun;
using System.Collections;
using UnityEngine;

// Clase hija de Enemy para los meteoritos explosivos
// Las estadisticas como la vida o la velocidad se modifican en el prefab con el inspector de Unity
public class ExplosiveMeteoroid : Enemy
{
    private Animator anim;

    protected override void Awake()
    {
        base.Awake();
        anim = GetComponentInChildren<Animator>();
    }

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
        else if (other.CompareTag("PlayerShot"))
        {
            if (other.TryGetComponent<PlayerShot>(out var shot))
            {
                OnHitBehavior(-shot.damage, shot.ownerPlayerID);
            }
        }
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

    }

    private IEnumerator Explosion(int playerID)
    {
        //anim.Play("Explosion");
        photonView.RPC(nameof(RPC_PlayAnimation), RpcTarget.All, "Explosion");

        int layerIndex = anim.GetLayerIndex("Actions Layer");
        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(layerIndex);
            return stateInfo.IsName("Explosion") && stateInfo.normalizedTime >= 1f;
        });

        OnDeath(playerID);
    }

    [PunRPC]
    void RPC_PlayAnimation(string animationName)
    {
        anim.Play(animationName);
    }
}
