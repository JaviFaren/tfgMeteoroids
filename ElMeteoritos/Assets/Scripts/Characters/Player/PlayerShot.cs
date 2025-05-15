using UnityEngine;

public class PlayerShot : MonoBehaviour
{
    public Player Owner { get; private set; }

    public int ownerPlayerID;
    public int damage;
    public float lifetime;

    private bool isPiercing;

    public void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (!isPiercing)
            {
                Destroy(gameObject);
            }
        }
    }

    public void InitializeBullet(Player owner, float shotForce, bool isPiercing, float lag)
    {
        Owner = owner;
        ownerPlayerID = Owner.playerID;
        damage = Owner.playerStats.ShootDamage;

        this.isPiercing = isPiercing;

        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(transform.right * shotForce, ForceMode.Impulse);
        rigidbody.position += rigidbody.velocity * lag;
    }
}
