using Photon.Pun;
using System.Collections;
using UnityEngine;

public class DivisibleMeteoroidChild : Enemy
{
    private Vector3 spawnPos;   

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

        rb.position += rb.velocity * lag;
    }

    public override void OnHitBehavior(int damage, int playerID)
    {
        if (processingHit) return;

        processingHit = true;

        soundFX.PlayFXSound(soundFX.Hit);

        ModifyLives(damage);
        if (currentLives <= 0)
        {
            StartCoroutine(Death(playerID));
        }
        else
        {
            processingHit = false;
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