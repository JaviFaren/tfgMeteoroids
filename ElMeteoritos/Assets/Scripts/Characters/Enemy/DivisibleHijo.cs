using UnityEngine;

public class DivisibleHijo : MonoBehaviour
{
    public DivisibleMeteoroid scriptPadre;
    public Animator animator;
    public Rigidbody rb;

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "disparo")
        {
            Destroy(other.gameObject);
            rb.velocity = Vector3.zero;
            animator.SetInteger("canDie", 1);
        }
    }

    public void desactivar()
    {
        this.gameObject.SetActive(false);
        this.gameObject.transform.position = scriptPadre.gameObject.transform.position;
        scriptPadre.hijosRestantes--;
    }
}
