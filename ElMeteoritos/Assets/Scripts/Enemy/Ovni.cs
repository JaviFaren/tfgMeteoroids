using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Ovni : Enemy
{
    public GameObject objetivo;
    public bool canMove = true;
    public float distancia;
    public Vector3 direction;
    public Vector3 lastDirection;
    public Vector3 lastPos;
    public float Maxspeed = 40f;
    public float speed;
    public float targetSpeed;
    public float decelerationDistance = 40f;

    //Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        trackPlayer();
        lastPos = objetivo.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if (canMove)
        //{
        //    canMove = false;
        //    trackPlayer();
        //}
        if (objetivo != null && canMove)
        {
            distancia = Vector3.Distance(transform.position, lastPos);
            targetSpeed = Maxspeed;
            if(distancia < decelerationDistance)
            {
                if (distancia < 20f)
                {
                    canMove = false;
                    
                    if(distancia < 3f)
                    {
                        StartCoroutine(NewObjectiveCooldown());
                    }
                }
                else
                {
                    targetSpeed = Maxspeed * (distancia / decelerationDistance);
                    
                }
            }
            else
            {
                if (canMove)
                {
                    lastDirection = direction;
                }
                rb.velocity = Vector3.Lerp(rb.velocity, lastDirection * targetSpeed, decelerationDistance * Time.fixedDeltaTime);
            }
            
        }
        if (!canMove)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, decelerationDistance * Time.fixedDeltaTime);
        }
    }

    public void trackPlayer()
    {
        direction = (objetivo.transform.position - transform.position).normalized;
        lastPos = objetivo.transform.position;
    }

    public IEnumerator NewObjectiveCooldown()
    {
        yield return new WaitForSeconds(2.5f);
        trackPlayer();
        canMove = true;
        rb.velocity = Vector3.zero;
    }
}
