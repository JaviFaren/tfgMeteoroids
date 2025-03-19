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
        SetLifes(maxLifes);
    }

    protected virtual void Update()
    {
        if (GameController.instance.CheckPosition(gameObject) != null) { RelocateEnemy(); }
    }

    protected virtual void OnTriggerEnter (Collider other)
    {
        if (other.CompareTag("disparo"))
        {
            OnHitBehavior(-other.GetComponent<PlayerShoot>().damage);
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

    protected virtual void OnHitBehavior(int damage)
    {
        
    }

    public virtual void OnSpawn()
    {

    }

    public virtual void OnDeath()
    {
        GameController.instance.SetDefeatedEnemies(1); // Aumenta en 1 el numero de enmigos derrotados en el GameController
        GameController.instance.RemoveEnemyFromSpawnedEnemiesList(this); // Elimina el enemigo de la lista spawnedEnemies del Game Controller
        gameObject.SetActive(false);
        this.transform.position = enemyContainer.transform.position;
    }

    public void SetLifes(int amount)
    {
        lifes = Mathf.Clamp(lifes + amount, 0, maxLifes);
    }

    // Falta hacer que los enemigos "mueran" (o lo que se quiera) al pasar los limites de la camara
    protected virtual void RelocateEnemy()
    {
        switch (GameController.instance.CheckPosition(gameObject))
        {
            case "above":
                transform.position = new Vector3(transform.position.x, GameController.instance.bottomLeftBorder.y, transform.position.z);
                break;
            case "below":
                transform.position = new Vector3(transform.position.x, GameController.instance.topRightBorder.y, transform.position.z);
                break;
            case "right":
                transform.position = new Vector3(GameController.instance.bottomLeftBorder.x, transform.position.y, transform.position.z);
                break;
            case "left":
                transform.position = new Vector3(GameController.instance.topRightBorder.x, transform.position.y, transform.position.z);
                break;
        }
    }
    //string ComprobarPosicion()
    //{
    //    float margin = 15;

    //    if (transform.position.y > GameController.instance.topRightBorder.y + margin)
    //    {
    //        return ("arriba");
    //    }
    //    else if (transform.position.y < GameController.instance.bottomLeftBorder.y - margin)
    //    {
    //        return ("abajo");
    //    }
    //    else if (transform.position.x > GameController.instance.topRightBorder.x + margin)
    //    {
    //        return ("derecha");
    //    }
    //    else if (transform.position.x < GameController.instance.bottomLeftBorder.x - margin)
    //    {
    //        return ("izquierda");
    //    }
    //    else { return null; }
    //}
}
