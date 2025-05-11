using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class powerUPM : MonoBehaviour
{
    private Enemy enemy;
    public GameObject enemigo;
    private int lifetime;
    public int heal;
    // Start is called before the first frame update
    void Start()
    {
        lifetime = 10;
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()  
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Destroy(gameObject);
        }
    }
}
