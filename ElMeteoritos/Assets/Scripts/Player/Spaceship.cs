using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ---> Clase que tiene toda la información de la nave.
// Aquí se harán los cambios en el menú de personalización.
public class Spaceship : MonoBehaviour
{
    [HideInInspector] public PlayerManager playerManager;

    [Header("Componentes")]
    public SpriteRenderer spriteRenderer;

    [Header("Propiedades Nave")]
    public Sprite spaceshipSprite;
    public Color spaceshipColor;
    public Sprite spaceshipTrail;

    [Header("Propiedades Propulsion")]
    public Color propulsorColor;
    public Sprite propulsorSprite;

    [Header("Propiedades Disparo")]
    public Color shootColor;

    private void Awake()
    {
        playerManager = GetComponentInParent<PlayerManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        spriteRenderer.sprite = spaceshipSprite;
        spriteRenderer.color = spaceshipColor;
    }
}
