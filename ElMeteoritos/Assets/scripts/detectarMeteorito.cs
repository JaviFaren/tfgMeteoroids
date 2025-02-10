using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class detectarMeteorito : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "disparo")
        {
            Debug.Log("heh, me mato");
        }
    }
}
