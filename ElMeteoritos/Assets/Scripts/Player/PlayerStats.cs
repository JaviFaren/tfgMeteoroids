using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.InputSystem.DefaultInputActions;

// ---> Clase con las estad�sticas in-game del jugador, los valores se cambian en el prefab Player usando el editor de Unity (para que no haya que estar modificando el c�digo todo el rato).
public class PlayerStats : MonoBehaviour
{
    [HideInInspector] public PlayerManager playerManager;

    [Header("Stats")]
    public int currentLifes;
    public int maxLifes;
    public ShootType shootType;
    public int shootDamage;

    [Header("Estad�sticas de partida")]
    public int score;
    public int enemiesDefeated;
    public int shoots;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
    }

    private void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("enemigo"))
        {
            ModifyLifes(-other.GetComponent<Enemy>().damage); // El menos es para que reste vida.

            //playerManager.OnHitRelocate();
            playerManager.phView.RPC("OnHitRelocate", RpcTarget.All);
            //StartCoroutine(playerManager.deathRelocate());
        }
    }

    // ---> Gestionar vidas

    public void ModifyLifes(int amount) // ---> Funci�n para cambiar las vidas del jugador que impide que bajen de 0, que suban m�s de las m�ximas y actualiza el panel del jugador. Se pasa un valor positivo para sumar vidas y uno negativo para restarlas.
    {
        Debug.Log("MAN DAO!! AYUAAA");

        currentLifes = Mathf.Clamp(currentLifes + amount, 0, maxLifes);
        playerManager.phView.RPC("MofifyPlayerPanel", RpcTarget.All);
        //PlayerUIManager.instance.UpdatePlayerPanel(playerManager);

        Debug.Log("ESTOY MUERTO? = " + playerManager.isDead);
    }

    public void ReturnToCenter()
    {
        playerManager.phView.RPC("ResetPosition", RpcTarget.All);
    }

    [PunRPC] 
    public void InitializePanels()
    {
        PlayerUIManager.instance.InitializePlayersPanel();
    }

    [PunRPC]
    public void MofifyPlayerPanel()
    {
        PlayerUIManager.instance.UpdatePlayerPanel(playerManager);
    }

    [PunRPC]
    public void ResetPosition()
    {
        if (!playerManager.isDead)
        {
            playerManager.canMove = true;
            playerManager.canShoot = true;
            //gameObject.SetActive(true);
        }
        else
        {
            PlayerUIManager.instance.deathScreen.SetActive(true);
            playerManager.canMove = false;
            playerManager.canShoot = false;
            gameObject.SetActive(false);
        }
        transform.position = Vector3.zero;
        //gameObject.SetActive(true);
    }

    // ---> Gestionar puntuacion
    public IEnumerator ModifyScore (int amount) // ---> Funci�n para cambiar la puntuaci�n del jugador que impide que baje de 0 y actualiza el panel del jugador usando una animaci�n simple.
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
