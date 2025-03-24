using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;
using UnityEngine.UI;

// ---> Clase general de los jugadores, tiene los datos que ssacarán de la base de datos (Nombre y ID) y las funciones principales del jugador.
public class PlayerManager : MonoBehaviour
{
    [HideInInspector] public PlayerStats playerStats;
    [HideInInspector] public PlayerActions playerActions;
    [HideInInspector] public Spaceship spaceship;
    [HideInInspector] public Animator animator;
    [HideInInspector] public PhotonView phView;

    [Header("Debug")]
    public int newScore;
    public int newLifes;
    public bool setNewLifes, setNewScore;

    [Header("Propiedades")]
    public string username; // ---> Se le asignará el de la cuanta de usuario de la base de datos.
    public int userID; // ---> Se le asignará el de la base de datos. Es importante que cada jugador tenga un ID único (si se hacen más prefabs de jugadores para testear hay que cambiárselo a mano en el inspector de Unity) para que la interfaz de las vidas, nombre y puntuación funcione bien.

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
        phView = GetComponent<PhotonView>();
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
        // ---> Asignación de estadísticas del jugador
        //username = phView.Owner.NickName;
        playerStats.currentLifes = playerStats.maxLifes;

        // ---> Asignación de botones y joystick (Seguramente habrá que cambiarlo al meter el Photon)
        playerActions.rotationJoystick = PlayerUIManager.instance.joystick;
        playerActions.velocitySlider = PlayerUIManager.instance.speedSlider;
        playerActions.heatBar = PlayerUIManager.instance.heatBar;
        playerActions.shootButton = PlayerUIManager.instance.shootButton;
        if (phView.IsMine)
        {
            playerActions.shootButton.onClick.AddListener(delegate { playerActions.Fire(); });
        }

        // ---> Se añade el jugador a la lista de jugadores del GameController
        phView.RPC("addPlayer", RpcTarget.All);

        // ---> El jugador se ha inicializado (Importante que vaya la último de esta función)
        canMove = true;
        canShoot = true;
        initialized = true;
    }


    public IEnumerator deathRelocate()
    {
        //this.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        //playerActions.velocitySlider.value = 0;

        animator.SetBool("died", true);
        yield return new WaitForSeconds(3.45f);
        animator.SetBool("died", false);
    }

    [PunRPC]
    public void OnHitRelocate()
    {
        this.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        playerActions.velocitySlider.value = 0;
        canMove = false;
        canShoot = false;

        StartCoroutine(deathRelocate());

        //animator.CrossFade("Respawn", 0.2f);
    }

    public void SpawnMeteoroid(int IDType)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Vector3 spawnPosition = GameController.instance.ChooseEnemySpawnPoint();
        int randomPlayer = Random.Range(0, GameController.instance.playersList.Count);
        Vector3 targetPosition = GameController.instance.playersList[randomPlayer].transform.position;

        phView.RPC("SpawnEnemyRPC", RpcTarget.All, spawnPosition, targetPosition, IDType);
    }

    [PunRPC]
    public void SpawnEnemyRPC(Vector3 spawnPosition, Vector3 targetPosition, int IDType)
    {
        for (int i = 0; i < GameController.instance.contenedores_Enemigos[IDType].transform.childCount; i++)
        {
            GameObject enemy = GameController.instance.contenedores_Enemigos[IDType].transform.GetChild(i).gameObject;

            if (!enemy.activeInHierarchy)
            {
                enemy.transform.position = spawnPosition;
                enemy.SetActive(true);

                enemy.GetComponent<Enemy>().SetTarget(targetPosition);
                GameController.instance.spawnedEnemies.Add(enemy.GetComponent<Enemy>());

                break;
            }
        }
    }
}
