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

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        playerActions = GetComponent<PlayerActions>();
        spaceship = GetComponentInChildren<Spaceship>();
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
            playerStats.StartCoroutine(playerStats.ModifyScoreIE(newScore));
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
        playerActions.shootButton.onClick.AddListener(delegate { playerActions.Disparar(); });

        // ---> Se a�ade el jugador a la lista de jugadores del GameController
        GameController.instance.AddPlayerToPlayersList(this);

        // ---> El jugador se ha inicializado (Importante que vaya la �ltimo de esta funci�n)
        canMove = true;
        canShoot = true;
        initialized = true;
    }
}
