using Photon.Pun;
using UnityEngine;

public class PlayerShot : MonoBehaviour
{
    public int shotID;

    public Player Owner { get; private set; }

    public int ownerPlayerID;
    public int damage;
    public float lifetime;

    [SerializeField] private bool isPiercing;
    public bool IsPiercing => isPiercing;
    private bool hasHit;

    private readonly PlayerManager playerManager = PlayerManager.Instance;

    public void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnDestroy()
    {
        if (playerManager != null)
        {
            playerManager.UnregisterShot(shotID);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (hasHit && !IsPiercing) return;

        if (other.CompareTag("Enemy"))
        {
            if (Owner == null || !Owner.photonView.IsMine) return;

            if (other.TryGetComponent<PhotonView>(out var enemyView))
            {
                hasHit = true;

                Owner.photonView.RPC(nameof(Player.ReportHitToMaster), RpcTarget.MasterClient,
                    shotID, ownerPlayerID, enemyView.ViewID, -damage);

                //if (!isPiercing) Destroy(gameObject);
            }
        }
    }

    public void InitializeBullet(Player owner, float shotForce, bool isPiercing, float lag, int shotID)
    {
        Owner = owner;
        ownerPlayerID = Owner.playerID;
        damage = Owner.playerStats.ShootDamage;

        this.isPiercing = isPiercing;
        this.shotID = shotID;

        hasHit = false;

        playerManager.RegisterShot(shotID, this);

        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(transform.right * shotForce, ForceMode.Impulse);
        rigidbody.position += rigidbody.velocity * lag;
    }
    //public void InitializeBullet(Player owner, float shotForce, bool isPiercing, float lag)
    //{
    //    Owner = owner;
    //    ownerPlayerID = Owner.playerID;
    //    damage = Owner.playerStats.ShootDamage;

    //    this.isPiercing = isPiercing;

    //    Rigidbody rigidbody = GetComponent<Rigidbody>();
    //    rigidbody.AddForce(transform.right * shotForce, ForceMode.Impulse);
    //    rigidbody.position += rigidbody.velocity * lag;
    //}
}
