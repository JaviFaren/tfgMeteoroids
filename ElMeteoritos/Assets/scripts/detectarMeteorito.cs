using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class detectarMeteorito : MonoBehaviour
{
    public float speed = 15;
    public Vector3 targetPosition;

    public enum Enemytype { normal, divisiblex2, divisiblex5, explosivo, blindado, curativo};
    public Enemytype enemytype;
    [SerializeField] Rigidbody rb;
    private GameObject player;
    public GameObject contenedorPadre;
    private int Vida;
    private GameObject colision;
    private Animator animator;

    private void Start()
    {
        
        animator = this.GetComponent<Animator>();
        rb = this.GetComponent<Rigidbody>();
        if(enemytype == Enemytype.blindado) { Vida = 3; }
        else { Vida = 1; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "disparo")
        {
            Debug.Log("heh, me mato");
            funcionalidad();
        }
    }

    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
        MoveTowardsTarget();
    }

    private void MoveTowardsTarget()
    {
        Vector2 direction = (targetPosition - transform.position).normalized;
        rb.AddForce(direction*speed, ForceMode.VelocityChange);
        //rb.velocity = direction * speed;
    }

    public void funcionalidad()
    {
        if (enemytype == Enemytype.normal)
        {

        }
        else if (enemytype == Enemytype.divisiblex2)
        {

        }
        else if (enemytype == Enemytype.divisiblex5)
        {

        }
        else if (enemytype == Enemytype.explosivo)
        {
            Debug.Log("exploto");
            animator.SetBool("enterExplosion", true);
            rb.velocity = Vector3.zero;
        }
        else if (enemytype == Enemytype.blindado)
        {

        }
        else if (enemytype == Enemytype.curativo)
        {

        }
    }

    public void muerteOBJ()
    {
        this.gameObject.SetActive(false);
        transform.position = contenedorPadre.transform.position;
    }
}
