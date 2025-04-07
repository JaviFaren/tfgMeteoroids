using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScaler : MonoBehaviour
{
    void Start()
    {
        AjustarTamañoFondoPantalla();
    }

    void AjustarTamañoFondoPantalla()
    {
        Camera cam = Camera.main;

        // Distancia de la cámara al fondo
        float distanciaCamara = Mathf.Abs(cam.transform.position.z);

        // Obtener las dimensiones del sprite del fondo
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        float anchoSprite = spriteRenderer.bounds.size.x;
        float alturaSprite = spriteRenderer.bounds.size.y;

        // Campo de visión vertical de la cámara (FOV) en radianes
        float fov = cam.fieldOfView * Mathf.Deg2Rad;

        // Calcular la altura visible de la cámara en el espacio 3D (por la proyección en perspectiva)
        float alturaCamara = 2f * Mathf.Tan(fov / 2f) * distanciaCamara;

        // Calcular el ancho visible de la cámara en función de su relación de aspecto
        float anchoCamara = alturaCamara * cam.aspect;

        // Ajustar la escala del fondo según la distancia de la cámara y la relación de aspecto
        float escalaX = anchoCamara / anchoSprite;
        float escalaY = alturaCamara / alturaSprite;

        // Usamos la mayor escala para que el fondo cubra toda la pantalla
        float escala = Mathf.Max(escalaX, escalaY);

        // Ajustamos la escala del fondo
        transform.localScale = new Vector3(escala, escala, 1);

        // Posicionamos el fondo correctamente en el eje Z para evitar distorsión
        transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, cam.transform.position.z + 60);
    }
}
