using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScaler : MonoBehaviour
{
    void Start()
    {
        AjustarTama�oFondoPantalla();
    }

    void AjustarTama�oFondoPantalla()
    {
        Camera cam = Camera.main;

        // Distancia de la c�mara al fondo
        float distanciaCamara = Mathf.Abs(cam.transform.position.z);

        // Obtener las dimensiones del sprite del fondo
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        float anchoSprite = spriteRenderer.bounds.size.x;
        float alturaSprite = spriteRenderer.bounds.size.y;

        // Campo de visi�n vertical de la c�mara (FOV) en radianes
        float fov = cam.fieldOfView * Mathf.Deg2Rad;

        // Calcular la altura visible de la c�mara en el espacio 3D (por la proyecci�n en perspectiva)
        float alturaCamara = 2f * Mathf.Tan(fov / 2f) * distanciaCamara;

        // Calcular el ancho visible de la c�mara en funci�n de su relaci�n de aspecto
        float anchoCamara = alturaCamara * cam.aspect;

        // Ajustar la escala del fondo seg�n la distancia de la c�mara y la relaci�n de aspecto
        float escalaX = anchoCamara / anchoSprite;
        float escalaY = alturaCamara / alturaSprite;

        // Usamos la mayor escala para que el fondo cubra toda la pantalla
        float escala = Mathf.Max(escalaX, escalaY);

        // Ajustamos la escala del fondo
        transform.localScale = new Vector3(escala, escala, 1);

        // Posicionamos el fondo correctamente en el eje Z para evitar distorsi�n
        transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, cam.transform.position.z + 60);
    }
}
