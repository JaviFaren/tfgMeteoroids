using Photon.Pun;
using System.Collections;
using UnityEngine;

public class DivisibleMeteoroid : Enemy
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
        //else if (other.CompareTag("PlayerShot"))
        //{
        //    if (other.TryGetComponent<PlayerShot>(out var shot))
        //    {
        //        OnHitBehavior(-shot.damage, shot.ownerPlayerID);
        //    }
        //}
    }

    protected override void OnSpawnBehavior(float lag)
    {
        SetRandomTargetAndMoveTowards(lag);
    }

    public override Vector3 GetSpawnPosition() => EnemyManager.Instance.GetRandomSpawnPoint();

    public override void OnHitBehavior(int damage, int playerID)
    {
        ModifyLives(damage);

        if (currentLives <= 0)
        {
            rb.velocity = Vector3.zero;
            StartCoroutine(Divide(playerID));
        }
    }

    protected override void OnDeathBehavior()
    {
        photonView.RPC(nameof(RPC_ManageSpriteRenderer), RpcTarget.All, true);
    }

    private IEnumerator Divide(int playerID)
    {
        photonView.RPC(nameof(RPC_ManageSpriteRenderer), RpcTarget.All, false);

        SpawnChildren();

        yield return null;

        OnDeath(playerID);
    }

    private void SpawnChildren()
    {
        int childNum = Random.Range(2, 6); // 6 es exclusivo

        for (int i = 0; i < childNum; i++)
        {
            GameObject hijoGO = EnemyManager.Instance.GetInactiveEnemy(EnemyType.METEOROID_DIVISIBLE_CHILD);

            if (hijoGO == null)
            {
                Debug.LogWarning("No hay suficientes hijos en el pool");
                continue;
            }

            var hijo = hijoGO.GetComponent<DivisibleMeteoroidChild>();

            Vector3 posicionSpawn = transform.position + Random.insideUnitSphere * 1.5f;
            hijo.SetCustomSpawnPosition(posicionSpawn);

            if (hijo.TryGetComponent<PhotonView>(out var pv))
            {
                EnemyManager.Instance.photonView.RPC(nameof(EnemyManager.Instance.RPC_ActivateEnemy), RpcTarget.All, pv.ViewID, posicionSpawn);
            }
        }
    }

    [PunRPC]
    void RPC_ManageSpriteRenderer(bool active)
    {
        sr.enabled = active;
    }
}

//public class DivisibleMeteoroid : Enemy
//{
//    public GameObject[] hijos;
//    [Tooltip("Velocidad a la que saldrá propulsado el hijo")]
//    public float hijoSpeed;

//    public GameObject agresor;
//    public int hijosRestantes;

//    protected override void Update()
//    {
//        if (hijosRestantes == 0)
//        {
//            //sumar puntos al primer jugador que golpeo al enemigo
//            this.gameObject.SetActive(false);
//            this.gameObject.transform.position = Vector3.zero;
//        }
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
//                agresor = shot.Owner.gameObject;
//            }
//        }
//    }

//    //Mejorar
//    public void dividirHijos(int divisiones)
//    {
//        Vector3 perpendicularPos = Vector2.zero;
//        Vector3 perpendicularNeg = Vector2.zero;
//        for (int i = 0; i < divisiones; i++)
//        {
//            Vector2 perpendicularDirection;

//            hijos[i].SetActive(true);
//            if (i == 0)
//            {
//                Vector2 direction = (agresor.transform.position - transform.position).normalized;
//                perpendicularDirection = new Vector2(-direction.y, direction.x);
//                perpendicularPos = perpendicularDirection;
//                rb.velocity = perpendicularDirection * movementSpeed;
//            }
//            else if (i == 1)
//            {
//                Vector2 direction = (agresor.transform.position - transform.position).normalized;
//                perpendicularDirection = new Vector2(direction.y, -direction.x);
//                perpendicularNeg = perpendicularDirection;
//                rb.velocity = perpendicularDirection * movementSpeed;
//            }
//            else if (i == 2)
//            {
//                Vector2 direction = (agresor.transform.position - perpendicularNeg).normalized;
//                perpendicularDirection = new Vector2(direction.y, -direction.x);
//                rb.velocity = perpendicularDirection * movementSpeed;
//            }
//            else if (i == 3)
//            {
//                Vector2 direction = (agresor.transform.position - perpendicularNeg).normalized;
//                perpendicularDirection = new Vector2(-direction.y, direction.x);
//                rb.velocity = perpendicularDirection * movementSpeed;
//            }
//            else if (i == 4)
//            {
//                Vector2 direction = (agresor.transform.position - perpendicularPos).normalized;
//                perpendicularDirection = new Vector2(-direction.y, -direction.x);
//                rb.velocity = perpendicularDirection * movementSpeed;
//            }
//            else
//            {
//                Vector2 direction = (agresor.transform.position - perpendicularPos).normalized;
//                perpendicularDirection = new Vector2(direction.y, direction.x);
//                rb.velocity = perpendicularDirection * movementSpeed;
//            }
//            hijos[i].GetComponent<Rigidbody>().AddForce(perpendicularDirection * 8, ForceMode.Impulse);
//        }
//    }

//    public override Vector3 GetSpawnPosition() => EnemyManager.Instance.GetRandomSpawnPoint();

//    protected override void OnSpawnBehavior(float lag)
//    {
//        if (enemyType == EnemyType.METEOROID_DIVISIBLE)
//        {
//            maxLives = 2;
//            hijosRestantes = 2;
//        }
//        else
//        {
//            maxLives = 6;
//            hijosRestantes = 6;
//        }

//        SetRandomTargetAndMoveTowards(lag);
//    }

//    public override void OnHitBehavior(int damage, int playerID)
//    {
//        //Desactiva visualmente al padre para que salgan los hijos
//        this.GetComponent<SpriteRenderer>().enabled = false;
//        this.GetComponent<Collider>().enabled = false;
//        this.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;

//        if (enemyType == EnemyType.METEOROID_DIVISIBLE)
//        {
//            dividirHijos(2);
//        }
//        else if (enemyType == EnemyType.METEOROID_DIVISIBLE_CHILD)
//        {
//            dividirHijos(6);
//        }
//    }

//    protected override void OnDeathBehavior()
//    {
//        Debug.Log("me desintegro");
//        rb.velocity = Vector3.zero;
//    }
//}
