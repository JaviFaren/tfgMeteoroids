using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class detectarMeteorito : MonoBehaviour
{
    public float speed = 3;
    public Vector3 targetPosition;

    [SerializeField] Rigidbody rb;

    private void Start()
    {
        //rb2d =  GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "disparo")
        {
            Debug.Log("heh, me mato");
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
        rb.velocity = direction * speed;
    }
}
