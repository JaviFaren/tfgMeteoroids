using Photon.Pun;
using UnityEngine;

// ---> Clase padre/base de todos los enemigos, tiene todas las funciones y propiedades generales
public abstract class Enemy : MonoBehaviour
{
    [Header("Componentes")]
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Animator anim;
    [HideInInspector] public SpriteRenderer sr;
    [HideInInspector] public Collider enemyCollider;
    protected PhotonView photonView;

    [Header("Stats")]
    public int currentLives;
    public int maxLives;
    public float movementSpeed;
    public int damage;
    public int score;

    [Header("Propiedades")]
    public EnemyType enemyType;

    [Header("Flags")]
    public bool processingHit = false;

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody>();
        enemyCollider = GetComponent<Collider>();
        photonView = GetComponent<PhotonView>();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;

        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<Player>(out var player))
            {
                player.TakeDamage(-damage);
            }
        }
    }

    public abstract Vector3 GetSpawnPosition();

    public void Activate(float lag)
    {
        ModifyLives(maxLives);
        if (enemyCollider != null) enemyCollider.enabled = true;

        processingHit = false;

        OnSpawnBehavior(lag);
    }

    protected abstract void OnSpawnBehavior(float lag);
    public abstract void OnHitBehavior(int damage, int playerID);
    protected abstract void OnDeathBehavior();

    // ---> Stats
    public void ModifyLives(int amount)
    {
        currentLives = Mathf.Clamp(currentLives + amount, 0, maxLives);
    }

    public virtual void OnDeath(int playerID)
    {
        if (enemyCollider != null) enemyCollider.enabled = false;
        OnDeathBehavior();

        GameManager.Instance.RegisterEnemyDefeat();

        if (PhotonNetwork.IsMasterClient)
        {
            EnemyManager.Instance.TrySpawnPowerUp(enemyType, transform.position);
            EnemyManager.Instance.DespawnEnemy(photonView.ViewID);
            PlayerManager.Instance.GetPlayerByID(playerID)?.playerStats.ModifyScore(score);
        }
    }

    // ---> Movimiento
    protected virtual void SetRandomTargetAndMoveTowards(float lag)
    {
        Vector3 targetPosition = PlayerManager.Instance.GetRandomPlayerPosition();
        Vector2 direction = (targetPosition - transform.position).normalized;
        rb.velocity = direction * movementSpeed;

        rb.position += rb.velocity * lag;
    }
    protected virtual void MoveInStraightLine()
    {
        float direction = transform.position.x < 0 ? 1 : -1;
        rb.velocity = new Vector3(direction, 0, 0) * movementSpeed;
    }
}
