using System.Collections;
using UnityEngine;

public class Ovni : Enemy
{
    public float accelerationForce = 20f;
    public float decelerationDistance = 10f;
    public float closeEnoughDistance = 1.5f;
    public float delayBetweenTargets = 0.5f;

    private GameObject targetPlayer;
    private Coroutine movementCoroutine;

    public override Vector3 GetSpawnPosition() => EnemyManager.Instance.GetRandomSpawnPoint();

    protected override void OnSpawnBehavior(float lag)
    {
        GetRandomTarget();

        if (movementCoroutine != null)
            StopCoroutine(movementCoroutine);

        movementCoroutine = StartCoroutine(MoveToTargetLoop());

        rb.position += rb.velocity * lag;
    }

    private void GetRandomTarget()
    {
        var randomPlayer = PlayerManager.Instance.GetRandomPlayer();
        if (randomPlayer == null)
        {
            Debug.LogWarning("No se encontró un jugador aleatorio.");
            return;
        }

        targetPlayer = randomPlayer.gameObject;
    }

    public override void OnHitBehavior(int damage, int playerID)
    {
        if (processingHit) return;

        processingHit = true;

        soundFX.PlayFXSound(soundFX.Hit);

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
        if (movementCoroutine != null)
            StopCoroutine(movementCoroutine);
    }

    private IEnumerator MoveToTargetLoop()
    {
        if (!photonView.IsMine) yield break;

        while (gameObject.activeInHierarchy)
        {
            if (targetPlayer == null || targetPlayer.GetComponent<Player>().IsDead)
            {
                GetRandomTarget();
                yield return null;
                continue;
            }

            // Fija la posición actual del jugador
            Vector3 destination = targetPlayer.transform.position;
            Vector3 dir = (destination - transform.position).normalized;

            while (Vector3.Distance(transform.position, destination) > closeEnoughDistance)
            {
                float distance = Vector3.Distance(transform.position, destination);

                // Calcula la fuerza con desaceleración
                float speedFactor = Mathf.Clamp01(distance / decelerationDistance);
                Vector3 desiredVelocity = movementSpeed * speedFactor * dir;
                Vector3 velocityChange = desiredVelocity - rb.velocity;

                // Aplica la fuerza suavemente
                rb.AddForce(velocityChange * accelerationForce, ForceMode.Force);

                yield return new WaitForFixedUpdate();
            }

            rb.velocity = Vector3.zero;

            // Espera antes de volver a calcular el nuevo punto
            yield return new WaitForSeconds(delayBetweenTargets);
        }
    }

    private void CheckTargetIsAlive()
    {
        if (targetPlayer.GetComponent<Player>().IsDead)
        {
            GetRandomTarget();
        }
    }
}


//public class Ovni : Enemy
//{
//    public GameObject objetivo;
//    public bool canMove = true;
//    public float distancia;
//    public Vector3 direction;
//    public Vector3 lastDirection;
//    public Vector3 lastPos;
//    public float Maxspeed;
//    public float speed;
//    public float targetSpeed;
//    public float decelerationDistance = 40f;


//    void FixedUpdate()
//    {
//        if (objetivo != null && canMove && gameObject.activeInHierarchy)
//        {
//            distancia = Vector3.Distance(transform.position, lastPos);
//            targetSpeed = Maxspeed;
//            if (distancia < decelerationDistance)
//            {
//                if (distancia < 20f)
//                {
//                    canMove = false;

//                    if (distancia < 3f)
//                    {
//                        StartCoroutine(NewObjectiveCooldown());
//                    }
//                }
//                else
//                {
//                    targetSpeed = Maxspeed * (distancia / decelerationDistance);

//                }
//            }
//            else
//            {
//                if (canMove)
//                {
//                    lastDirection = direction;
//                }
//                rb.velocity = Vector3.Lerp(rb.velocity, lastDirection * targetSpeed, decelerationDistance * Time.fixedDeltaTime);
//            }

//        }
//        if (!canMove)
//        {
//            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, decelerationDistance * Time.fixedDeltaTime);
//        }
//    }

//    public void trackPlayer()
//    {
//        direction = (objetivo.transform.position - transform.position).normalized;
//        lastPos = objetivo.transform.position;
//    }

//    public IEnumerator NewObjectiveCooldown()
//    {
//        yield return new WaitForSeconds(2.5f);
//        trackPlayer();
//        canMove = true;
//        rb.velocity = Vector3.zero;
//    }

//    protected override void OnTriggerEnter(Collider other)
//    {
//        if (!photonView.IsMine) return;

//        if (other.CompareTag("Player"))
//        {
//            if (other.TryGetComponent<Player>(out var player))
//            {
//                player.TakeDamage(-damage);
//            }
//        }
//        else if (other.CompareTag("PlayerShot"))
//        {
//            if (other.TryGetComponent<PlayerShot>(out var shot))
//            {
//                OnHitBehavior(-shot.damage, shot.ownerPlayerID);
//            }
//        }
//    }

//    public override Vector3 GetSpawnPosition() => EnemyManager.Instance.GetRandomSpawnPoint();

//    protected override void OnSpawnBehavior(float lag)
//    {
//        objetivo = PlayerManager.Instance.GetRandomPlayer().gameObject;

//        trackPlayer();
//        lastPos = objetivo.transform.position;
//    }

//    public override void OnHitBehavior(int damage, int playerID)
//    {
//        ModifyLives(damage);

//        if (currentLives <= 0)
//        {
//            OnDeath(playerID);
//        }
//    }

//    protected override void OnDeathBehavior()
//    {

//    }
//}
