using Photon.Pun;
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
    [HideInInspector] public PhotonView phView;
    public Transform enemyContainer;

    [Header("Stats")]
    public int lifes;
    public int maxLifes;
    public float movementSpeed;
    public int damage;
    public int puntos;

    [Header("Propiedades")]
    public EnemyType enemyType;
    public GameObject target;
    public Vector3 targetPosition;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        phView = GetComponent<PhotonView>();
    }
    protected virtual void Start()
    {
        SetLifes(maxLifes);
    }

    protected virtual void Update()
    {
        if (GameController.instance.CheckPosition(gameObject) != null) { Relocate(); }
    }

    protected virtual void OnTriggerEnter (Collider other)
    {
        if (other.CompareTag("disparo"))
        {
            OnHitBehavior(-other.GetComponent<PlayerShoot>().damage);
            Destroy(other.gameObject);
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
    [PunRPC]
    public virtual void Teleport(Vector3 newPosition)
    {
        var photonTransformView = GetComponent<PhotonTransformView>();
        if (photonTransformView != null)
            photonTransformView.enabled = false;

        transform.position = newPosition;

        if (photonTransformView != null)
            photonTransformView.enabled = true;
    }

    public virtual void Relocate()
    {
        if (!phView.IsMine) return;

        Vector3 newPosition = transform.position;

        switch (GameController.instance.CheckPosition(gameObject))
        {
            case "above":
                newPosition = new Vector3(transform.position.x, GameController.instance.bottomLeftBorder.y, transform.position.z);
                break;
            case "below":
                newPosition = new Vector3(transform.position.x, GameController.instance.topRightBorder.y, transform.position.z);
                break;
            case "right":
                newPosition = new Vector3(GameController.instance.bottomLeftBorder.x, transform.position.y, transform.position.z);
                break;
            case "left":
                newPosition = new Vector3(GameController.instance.topRightBorder.x, transform.position.y, transform.position.z);
                break;
            default:
                return; // No cambio, salimos
        }

        phView.RPC("Teleport", RpcTarget.All, newPosition);
    }
}
