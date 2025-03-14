using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ---> Clase hija de Enemy para los meteoritos explosivos.
// Las estadísticas como la vida o la velocidad se modifican en el prefab con el inspector de Unity.
public class ExplosiveMeteoroid : Enemy
{
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    protected override void MoveTowardsTarget()
    {
        base.MoveTowardsTarget();
    }

    protected override void OnHitBehavior()
    {
        Debug.Log("exploto");
        animator.SetBool("enterExplosion", true);
        rb.velocity = Vector3.zero;
    }

    protected override void OnDeath()
    {
        base.OnDeath();
    }
}
