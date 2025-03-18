using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ---> Clase hija de Enemy para los meteoritos comunes.
// Las estadísticas como la vida o la velocidad se modifican en el prefab con el inspector de Unity.
// Las funciones Protected Override sobreescrben las funciones Protected Virtual. Si se incluye base.NombreFunción() ejecutará el código de la función del script Padre más el que se añada.
// Si no se incluye el override de una función del Padre y se llama desde otro script, se ejecutará el código del script Padre.
public class CommonMeteoroid : Enemy
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
        if (other.CompareTag("disparo"))
        {
            Debug.Log("heh, man dao");
            OnHitBehavior();
        }
    }

    protected override void MoveTowardsTarget()
    {
        base.MoveTowardsTarget();
    }

    protected override void OnHitBehavior()
    {
        Debug.Log("me desintegro");
        rb.velocity = Vector3.zero;
        OnDeath();
    }

    protected override void OnDeath()
    {
        gameObject.SetActive(false);
        this.transform.position = enemyContainer.transform.position;
    }
}
