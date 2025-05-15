using UnityEngine;

// Clase hija de Enemy para los meteoritos comunes
// Las estadisticas como la vida o la velocidad se modifican en el prefab con el inspector de Unity
public class CommonMeteoroid : Enemy
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
            OnDeath(playerID);
        }
    }

    protected override void OnDeathBehavior()
    {
        rb.velocity = Vector3.zero;
    }
}
