using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public Rigidbody rb;
    public int playerID;
    public int damage;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Si el proyectil est� en movimiento, rotamos el sprite para que apunte en la direcci�n de su movimiento
        //if (rb.velocity.sqrMagnitude > 0.1f)
        //{
        //    float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;  // Calculamos el �ngulo de movimiento
        //    Quaternion targetRotation = Quaternion.Euler(0, 0, angle);  // Convertimos el �ngulo a rotaci�n
        //    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360 * Time.deltaTime);  // Rotamos suavemente hacia la direcci�n
        //}
    }
}
