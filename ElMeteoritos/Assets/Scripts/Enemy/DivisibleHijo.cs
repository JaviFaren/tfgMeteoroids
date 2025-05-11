using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivisibleHijo : MonoBehaviour
{
    public DivisibleMeteoroid scriptPadre;
    public Animator animator;
    public Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "disparo")
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
