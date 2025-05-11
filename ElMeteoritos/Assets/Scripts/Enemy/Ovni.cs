using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Ovni : MonoBehaviour
{
    public GameObject objetivo;
    public bool canMove;
    public float distancia;
    public Vector3 direction;
    public Rigidbody rb;
    public float Maxspeed = 20f;
    public float speed;
    public float targetSpeed;
    public float decelerationDistance = 2f;

    // Start is called before the first frame update
    //protected override void Start()
    //{
    //    base.Start();
    //}

    // Update is called once per frame
    void FixedUpdate()
    {
        //if (canMove)
        //{
        //    canMove = false;
        //    trackPlayer();
        //}
        if (objetivo != null)
        {
            distancia = Vector3.Distance(transform.position, objetivo.transform.position);
            targetSpeed = Maxspeed;
            if(distancia < decelerationDistance)
            {
                if(distancia < 1f)
                {
                    rb.velocity = Vector3.zero;
                    StartCoroutine(NewObjectiveCooldown());
                }
                else
                {
                    targetSpeed = Maxspeed * (distancia / decelerationDistance);
                    
                }
            }
            else
            {
                direction = (objetivo.transform.position - transform.position).normalized;
                rb.velocity = Vector3.Lerp(rb.velocity, direction * targetSpeed, decelerationDistance * Time.fixedDeltaTime);
            }
            
        }
        
    }

    public void trackPlayer()
    {
        direction = (objetivo.transform.position - transform.position).normalized;
    }

    public IEnumerator NewObjectiveCooldown()
    {
        yield return new WaitForSeconds(2.5f);
        canMove = true;
    }
}
