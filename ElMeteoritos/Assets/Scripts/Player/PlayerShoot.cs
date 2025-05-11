using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public Rigidbody rb;
    public int playerID;
    public GameObject owner;
    public int damage;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
}
