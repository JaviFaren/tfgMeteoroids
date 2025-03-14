using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ---> Clase hija de Enemy para los meteoritos comunes.
// Las estad�sticas como la vida o la velocidad se modifican en el prefab con el inspector de Unity.
// Las funciones Protected Override sobreescrben las funciones Protected Virtual. Si se incluye base.NombreFunci�n() ejecutar� el c�digo de la funci�n del script Padre m�s el que se a�ada.
// Si no se incluye el override de una funci�n del Padre y se llama desde otro script, se ejecutar� el c�digo del script Padre.
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
        base.OnTriggerEnter(other);
    }

    protected override void MoveTowardsTarget()
    {
        base.MoveTowardsTarget();
    }

    protected override void OnHitBehavior()
    {
        Debug.Log("me desintegro");
    }

    protected override void OnDeath()
    {
        base.OnDeath();
    }
}
