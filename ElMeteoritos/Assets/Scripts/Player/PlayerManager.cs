using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ---> Clase general de los jugadores, tiene los datos que ssacar�n de la base de datos (Nombre y ID) y las funciones principales del jugador.
public class PlayerManager : MonoBehaviour
{
    [HideInInspector] public PlayerStats playerStats;
    [HideInInspector] public PlayerActions playerActions;
    [HideInInspector] public Spaceship spaceship;
    [HideInInspector] public Animator animator;

    [Header("Debug")]
    public int newScore;
    public int newLifes;
    public bool setNewLifes, setNewScore;

    [Header("Propiedades")]
    public string username; // ---> Se le asignar� el de la cuanta de usuario de la base de datos.
    public int userID; // ---> Se le asignar� el de la base de datos. Es importante que cada jugador tenga un ID �nico (si se hacen m�s prefabs de jugadores para testear hay que cambi�rselo a mano en el inspector de Unity) para que la interfaz de las vidas, nombre y puntuaci�n funcione bien.

    [Header("Flags")]
    public bool canMove = false; // ---> Booleana que sirve para controlar si el jugador puede moverse y rotar.
    public bool canShoot = false; // ---> Booleana que sirve para controlar si el jugador puede disparar.
    public bool initialized = false; // ---> Booleana que sirve para controlar si el jugador tiene todos los componentes y valores asignados.
    public bool isDead => playerStats.currentLifes == 0; // ---> Booleana que sirve para controlar si el jugador esta muerto.

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        playerActions = GetComponent<PlayerActions>();
        spaceship = GetComponentInChildren<Spaceship>();    
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        OnSpawn();
    }

    private void Update()
    {
        // ---> Debug
        if (setNewLifes)
        {
            setNewLifes = false;
            playerStats.ModifyLifes(newLifes);
        }
        if (setNewScore)
        {
            setNewScore = false;
            playerStats.StartCoroutine(playerStats.ModifyScore(newScore));
        }

        
    }

    private void OnSpawn()
    {
        // ---> Asignaci�n de estad�sticas del jugador
        //username = phView.Owner.NickName;
        playerStats.currentLifes = playerStats.maxLifes;

        // ---> Asignaci�n de botones y joystick (Seguramente habr� que cambiarlo al meter el Photon)
        playerActions.rotationJoystick = PlayerUIManager.instance.joystick;
        playerActions.velocitySlider = PlayerUIManager.instance.speedSlider;
        playerActions.heatBar = PlayerUIManager.instance.heatBar;
        playerActions.shootButton = PlayerUIManager.instance.shootButton;
        playerActions.shootButton.onClick.AddListener(delegate { playerActions.Fire(); });

        // ---> Se a�ade el jugador a la lista de jugadores del GameController
        playerActions.view.RPC("addPlayer", Photon.Pun.RpcTarget.All);

        // ---> El jugador se ha inicializado (Importante que vaya la �ltimo de esta funci�n)
        canMove = true;
        canShoot = true;
        initialized = true;
    }
    

    //public IEnumerator deathRelocate()
    //{
    //    this.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    //    playerActions.velocitySlider.value = 0;

    //    animator.SetBool("died", true);
    //    //yield return new WaitForSeconds(3.45f);
    //    //animator.SetBool("died", false);
    //}

    public void OnHitRelocate()
    {
        this.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        playerActions.velocitySlider.value = 0;

        animator.CrossFade("Respawn", 0.2f);
    }
}
