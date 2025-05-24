using UnityEngine;

// Clase hija de Enemy para los meteoritos comunes
// Las estadisticas como la vida o la velocidad se modifican en el prefab con el inspector de Unity
public class CommonMeteoroid : Enemy
{
    public override Vector3 GetSpawnPosition() => EnemyManager.Instance.GetRandomSpawnPoint();

    protected override void OnSpawnBehavior(float lag)
    {
        SetRandomTargetAndMoveTowards(lag);
    }

    public override void OnHitBehavior(int damage, int playerID)
    {
        if (processingHit) return;

        Debug.Log($"[OnHitBehavior] {gameObject.name} / {photonView.ViewID} -> Impacto");

        processingHit = true;

        ModifyLives(damage);

        if (currentLives <= 0)
        {
            OnDeath(playerID);
        }
        else
        {
            processingHit = false;
        }
    }

    protected override void OnDeathBehavior()
    {
        rb.velocity = Vector3.zero;
    }
}
