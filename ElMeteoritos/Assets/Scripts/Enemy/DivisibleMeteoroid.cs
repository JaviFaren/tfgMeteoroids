using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivisibleMeteoroid : Enemy
{

    public GameObject[] hijos;
    [Tooltip("Velocidad a la que saldrá propulsado el hijo")]
    public float hijoSpeed;

    public GameObject agresor;
    public int hijosRestantes;

    protected override void Awake()
    {
        if (enemyType == EnemyType.DIVISIBLEx2)
        {
            maxLifes = 2;
            hijosRestantes = 2;
        }
        else
        {
            maxLifes = 6;
            hijosRestantes = 6;
        }
        base.Awake();
    }
    protected override void Start()
    {
        
    }

    protected override void Update()
    {
        if(hijosRestantes == 0)
        {
            //sumar puntos al primer jugador que golpeo al enemigo
            this.gameObject.SetActive(false);
            this.gameObject.transform.position = Vector3.zero;
        }
        base.Update();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        agresor = other.GetComponent<PlayerShoot>().owner;
        base.OnTriggerEnter(other);
    }

    protected override void MoveTowardsTarget()
    {
        //base.MoveTowardsTarget();
    }

    protected override void OnHitBehavior(int damage) // Calcula el dano recibido en funcion de la variable damage del disparo
    {
        //Desactiva visualmente al padre para que salgan los hijos
        this.GetComponent<SpriteRenderer>().enabled = false;
        this.GetComponent<Collider>().enabled = false;
        this.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;

        if (enemyType == EnemyType.DIVISIBLEx2)
        {
            dividirHijos(2);
        }
        else if(enemyType == EnemyType.DIVISIBLEx5)
        {
            dividirHijos(6);
        }
    }

    public override void OnDeath()
    {
        Debug.Log("me desintegro");
        rb.velocity = Vector3.zero;
        base.OnDeath();
    }

    //Mejorar
    public void dividirHijos(int divisiones)
    {
        Vector3 perpendicularPos = Vector2.zero;
        Vector3 perpendicularNeg = Vector2.zero;
        for (int i = 0; i < divisiones; i++)
        {
            Vector2 perpendicularDirection;

            hijos[i].SetActive(true);
            if(i == 0)
            {
                Vector2 direction = (agresor.transform.position - transform.position).normalized;
                perpendicularDirection = new Vector2(-direction.y, direction.x);
                perpendicularPos = perpendicularDirection;
                rb.velocity = perpendicularDirection * movementSpeed;
            }
            else if(i == 1)
            {
                Vector2 direction = (agresor.transform.position - transform.position).normalized;
                perpendicularDirection = new Vector2(direction.y, -direction.x);
                perpendicularNeg = perpendicularDirection;
                rb.velocity = perpendicularDirection * movementSpeed;
            }
            else if(i == 2)
            {
                Vector2 direction = (agresor.transform.position - perpendicularNeg).normalized;
                perpendicularDirection = new Vector2(direction.y, -direction.x);
                rb.velocity = perpendicularDirection * movementSpeed;
            }
            else if (i == 3)
            {
                Vector2 direction = (agresor.transform.position - perpendicularNeg).normalized;
                perpendicularDirection = new Vector2(-direction.y, direction.x);
                rb.velocity = perpendicularDirection * movementSpeed;
            }
            else if (i == 4)
            {
                Vector2 direction = (agresor.transform.position - perpendicularPos).normalized;
                perpendicularDirection = new Vector2(-direction.y, -direction.x);
                rb.velocity = perpendicularDirection * movementSpeed;
            }
            else
            {
                Vector2 direction = (agresor.transform.position - perpendicularPos).normalized;
                perpendicularDirection = new Vector2(direction.y, direction.x);
                rb.velocity = perpendicularDirection * movementSpeed;
            }
            hijos[i].GetComponent<Rigidbody>().AddForce(perpendicularDirection * 8, ForceMode.Impulse);
        }
    }
}
