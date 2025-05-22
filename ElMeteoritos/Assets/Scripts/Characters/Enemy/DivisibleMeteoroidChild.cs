using Photon.Pun;
using System.Collections;
using UnityEngine;

public class DivisibleMeteoroidChild : Enemy
{
    private Vector3 spawnPos;

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
        else if (other.CompareTag("PlayerShot"))
        {
            if (other.TryGetComponent<PlayerShot>(out var shot))
            {
                OnHitBehavior(-shot.damage, shot.ownerPlayerID);
            }
        }
    }    

    public override Vector3 GetSpawnPosition()
    {
        return spawnPos;
    }
    public void SetCustomSpawnPosition(Vector3 pos)
    {
        spawnPos = pos;
    }

    protected override void OnSpawnBehavior(float lag)
    {
        Vector3 direction = Random.insideUnitCircle.normalized;
        rb.velocity = direction * movementSpeed;
        //float fuerza = Random.Range(2f, 5f);

        //rb.velocity = Vector3.zero;
        //rb.AddForce(direction * fuerza, ForceMode.Impulse);

        rb.position += rb.velocity * lag;
    }

    public override void OnHitBehavior(int damage, int playerID)
    {
        ModifyLives(damage);
        if (currentLives <= 0)
        {
            StartCoroutine(Death(playerID));
        }
    }

    protected override void OnDeathBehavior()
    {
        rb.velocity = Vector3.zero;
    }

    private IEnumerator Death(int playerID)
    {
        if (enemyCollider != null) enemyCollider.enabled = false;

        photonView.RPC(nameof(RPC_PlayAnimation), RpcTarget.All, "Death");

        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            return stateInfo.IsName("Death") && stateInfo.normalizedTime >= 1f;
        });

        OnDeath(playerID);
    }

    public override void OnDeath(int playerID)
    {
        OnDeathBehavior();

        if (PhotonNetwork.IsMasterClient)
        {
            EnemyManager.Instance.DespawnEnemy(photonView.ViewID);
            PlayerManager.Instance.GetPlayerByID(playerID)?.playerStats.ModifyScore(score);
        }
    }

    [PunRPC]
    void RPC_PlayAnimation(string animationName)
    {
        anim.Play(animationName);
    }
}

//public class DivisibleHijo : MonoBehaviour
//{
//    public DivisibleMeteoroid scriptPadre;
//    public Animator animator;
//    public Rigidbody rb;

//    // Start is called before the first frame update
//    void Start()
//    {

//    }

//    // Update is called once per frame
//    void Update()
//    {

//    }

//    public void OnTriggerEnter(Collider other)
//    {
//        if (other.tag == "disparo")
//        {
//            Destroy(other.gameObject);
//            rb.velocity = Vector3.zero;
//            animator.SetInteger("canDie", 1);
//        }
//    }

//    public void desactivar()
//    {
//        this.gameObject.SetActive(false);
//        this.gameObject.transform.position = scriptPadre.gameObject.transform.position;
//        scriptPadre.hijosRestantes--;
//    }
//}