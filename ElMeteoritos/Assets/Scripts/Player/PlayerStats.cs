using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.InputSystem.DefaultInputActions;

// ---> Clase con las estadísticas in-game del jugador, los valores se cambian en el prefab Player usando el editor de Unity (para que no haya que estar modificando el código todo el rato).
public class PlayerStats : MonoBehaviour
{
    [HideInInspector] public PlayerManager playerManager;
    private Animator animator;
    public PlayerActions playerActions;

    [Header("Stats")]
    public int currentLifes;
    public int maxLifes;
    public ShootType shootType;
    public int shootDamage;

    [Header("Estadísticas de partida")]
    public int score;
    public int enemiesDefeated;
    public int shoots;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        animator = GetComponent<Animator>();
        playerActions = GetComponent<PlayerActions>();
    }

    private void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("enemigo"))
        {
            ModifyLifes(-other.GetComponent<Enemy>().damage); // El menos es para que reste vida.

            StartCoroutine(deathRelocate());
        }
    }

    // ---> Gestionar vidas
    public void ModifyLifes(int amount) // ---> Función para cambiar las vidas del jugador que impide que bajen de 0, que suban más de las máximas y actualiza el panel del jugador. Se pasa un valor positivo para sumar vidas y uno negativo para restarlas.
    {
        Debug.Log("MAN DAO!! AYUAAA");

        currentLifes = Mathf.Clamp(currentLifes + amount, 0, maxLifes);
        PlayerUIManager.instance.UpdatePlayerPanel(playerManager);

        Debug.Log("ESTOY MUERTO? = " + playerManager.isDead);
    }

    public IEnumerator deathRelocate()
    {
        this.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        playerActions.velocitySlider.value = 0;

        animator.SetBool("died", true);
        yield return new WaitForSeconds(3.45f);
        animator.SetBool("died", false);
    }

    public void returnToCenter()
    {
        if (currentLifes <= 0)
        {
            playerManager.canMove = false;
            playerManager.canShoot = false;
            this.gameObject.SetActive(false);
            PlayerUIManager.instance.deathScreen.SetActive(true);
        }
        this.gameObject.transform.position = Vector3.zero;
    }

    // ---> Gestionar puntuacion
    public IEnumerator ModifyScore (int amount) // ---> Función para cambiar la puntuación del jugador que impide que baje de 0 y actualiza el panel del jugador usando una animación simple.
    {
        int finalScore = Mathf.Max(score + amount, 0);
        while (score != finalScore)
        {
            if (score < finalScore)
            {
                score = Mathf.Max(score + 1, 0);
            }
            else if (score > finalScore)
            {
                score = Mathf.Max(score - 1, 0);
            }
            PlayerUIManager.instance.UpdatePlayerPanel(playerManager);

            yield return new WaitForSeconds(0.05f);
        }
    }
}
