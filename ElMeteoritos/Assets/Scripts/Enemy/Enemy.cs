using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// ---> Clase padre/base de todos los enemigos, tiene todas las funciones y propiedades generales. 
// Las funciones Protected Virtual se usan como base para las de los hijos. Se puede cambiar el Protected por Public, como en la función SetTarget, para poder llamar a la función desde otro script, pero se debe poner igual en los scripts hijos.
// Para lanzar una función que pueden tener varios enemigos diferentes desde otro script se llama al script Padre, por ejemplo, el GetComponent<Enemy>().
public class Enemy : MonoBehaviour 
{
    [Header("Componentes")]
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Animator animator;
    public Transform enemyContainer;

    [Header("Stats")]
    public int lifes;
    public int maxLifes;
    public float movementSpeed;
    public int damage;

    [Header("Propiedades")]
    public EnemyType enemyType;
    public GameObject target;
    public Vector3 targetPosition;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }
    protected virtual void Start()
    {

    }

    protected virtual void OnTriggerEnter (Collider other)
    {
        if (other.CompareTag("disparo"))
        {
            //Debug.Log("heh, man dao");
            OnHitBehavior();
        }
    }

    public virtual void SetTarget(Vector3 target)
    {
        targetPosition = target;
        MoveTowardsTarget();
    }
    protected virtual void MoveTowardsTarget() 
    {
        Vector2 direction = (targetPosition - transform.position).normalized;
        rb.velocity = direction * movementSpeed;
        //rb.AddForce(direction * movementSpeed, ForceMode.VelocityChange);
    }

    protected virtual void OnHitBehavior()
    {

    }

    protected virtual void OnDeath()
    {
        gameObject.SetActive(false);
        this.transform.position = enemyContainer.transform.position;
    }
}
