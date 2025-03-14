using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    [Header("Debug")]
    public bool bSpawnM = false;
    [Tooltip("Comunes:0, Divx2:1, Divx5:2, Blindados:3, Curativos:4, Expl:5")]
    public int meteoroidType = 0;
    public List<GameObject> playersListsDebug; // ---> Añadir en el inspector de Unity prefabs de jugadores con distinto ID para hacer pruebas.
    public GameObject meteoroidPrefab; // ---> Añadir en el inspector de Unity un prefab de cualquier enemigo para hacer pruebas.

    [Header("Jugadores")]
    public List<PlayerManager> playersList = new();
    //public int playerCount;

    [Header("Oleadas")]
    public int oleada;
    public bool nuevaOleada;
    public int enemiesNum = 0;
    public int enemies_Spawned = 0;
    public int normalesMax = 0;
    public int especialesMax = 0;
    public float baseEnemiesPercentage = 0;
    public int derrotados = 0;
    public int maxEnPantalla = 6;
    public bool canSpawn;
    [Tooltip("Comunes:0, Divx2:1, Divx5:2, Blindados:3, Curativos:4, Expl:5")]
    public GameObject[] contenedores_Enemigos;

    [Header("Cámara")]
    public Camera mainCamera;
    public Vector3 bottomLeftBorder;
    public Vector3 topRightBorder;
    public float cameraDistance;

    // ---> Toda la gestión de la interfaz se ha movido (Se puede volver a la que había antes) a PlayerUIManager
    //[Header("Interfaz")]
    //public TextMeshProUGUI oleadaText;
    //public GameObject[] PlayerBoxes = new GameObject[4];
    //public string[] nombres = new string[4];
    //public int[] puntuaciones = new int[4];
    //[Range(0, 5)] public int[] vidas;
    //public GameObject[] contenedoresVidas = new GameObject[4];

    private void Awake()
    {
        if (instance == null) 
        { 
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        mainCamera = Camera.main;
    }
    private void Start()
    {
        SetCameraBorders();

        oleada = 1;
        baseEnemiesPercentage = 0.15f;
        //playerCount = 4;
        nuevaOleada = true;
        canSpawn = false;

        //asignacionPlayers();
        //StartCoroutine(prueba());

        StartCoroutine(SpawnPlayers());
    }
    private void Update()
    {
        // ---> Debug
        if (bSpawnM == true)
        {
            SpawnMeteoroid(meteoroidType);
            bSpawnM = false;
        }

        // ---> Interfaz
        //nombres[0] = playerInfo[0].username;
        //puntuaciones[0] = playerInfo[0].puntuacion;
        //vidas[0] = playerInfo[0].vidas;

        WaveController();

        if (canSpawn)
        {
            EnemiesSpawner();
        }
        
    }

    // ---> Cámara
    public void SetCameraBorders()
    {
        cameraDistance = Mathf.Abs(mainCamera.transform.position.z);

        bottomLeftBorder = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, cameraDistance));
        topRightBorder = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, cameraDistance));
    }

    // ---> Spawn de jugadores
    public void AddPlayerToPlayersList(PlayerManager player) // ---> Función para añadir jugadores a la lista
    {
        if (!playersList.Contains(player))
        {
            playersList.Add(player);
        }
    }
    public void RemovePlayerFromPlayersList(PlayerManager player) // ---> Función para quitar jugadores a la lista
    {
        if (playersList.Contains(player))
        {
            playersList.Remove(player);
        }
    }
    //public void asignacionPlayers() --> Se ha cambiado para la interfaz de prueba, está en PlayerUIManager
    //public void asignacionPlayers() --> Se ha cambiado para la interfaz de prueba, está en PlayerUIManager
    //{
    //    for (int i = 0; i < playerCount; i++)
    //    {
    //        Transform Hijo1 = PlayerBoxes[i].transform.GetChild(0);
    //        Transform Hijo2 = PlayerBoxes[i].transform.GetChild(1);
    //        contenedoresVidas[i] = PlayerBoxes[i].transform.GetChild(2).gameObject;
    //        //asignar numero de hijos activos en funcion de las vidas restantes de cada jugador

    //        //Hijo1.gameObject.GetComponent<TextMeshProUGUI>().text = nombres[i];
    //        //Hijo2.gameObject.GetComponent<TextMeshProUGUI>().text = "" + puntuaciones[i];
    //        PlayerBoxes[i].SetActive(true);
    //    }
    //}
    public IEnumerator SpawnPlayers() // ---> Función para spawnear a los juagdores, esperando a que se inicialicen todos para que no fallos. Habrá que cambiar algunas cosas al meter el Photon.
    {
        //PhotonNetwork.PlayerList;
        foreach (var pla in playersListsDebug)
        {
            Instantiate(pla);
        }

        yield return new WaitUntil(() => playersList.TrueForAll(e => e.initialized)); // --> Esperar a que todos los jugadores tengan los componentes inicializados

        PlayerUIManager.instance.InitializePlayersPanel(); // ---> Inicializar los paneles de información de los jugadores
    }

    // ---> Gestión de enemigos
    public void SpawnMeteoroid(int IDType)
    {
        //Comprueba el contenedor correspondiente en base al enemigo que tiene que mover, lo activa y lo lanza hacia un jugador aleatorio (Sin testear)
        Vector3 spawnPosition = ChooseEnemySpawnPoint();
        for(int i = 0; i < contenedores_Enemigos[IDType].transform.childCount; i++)
        {
            if (!contenedores_Enemigos[IDType].transform.GetChild(i).gameObject.activeInHierarchy)
            {
                contenedores_Enemigos[IDType].transform.GetChild(i).gameObject.transform.position = spawnPosition;
                contenedores_Enemigos[IDType].transform.GetChild(i).gameObject.SetActive(true);

                int randomPlayer = Random.Range(0, playersList.Count);
                contenedores_Enemigos[IDType].transform.GetChild(i).gameObject.GetComponent<Enemy>().SetTarget(playersList[randomPlayer].transform.position);
            }
        }
        //GameObject meteoroid = Instantiate(meteoroidPrefab, spawnPosition, Quaternion.identity);
        //meteoroid.GetComponent<detectarMeteorito>().SetTarget(Vector3.zero);
        //meteoroid.GetComponent<Enemy>().SetTarget(Vector3.zero);
    }


    public Vector3 ChooseEnemySpawnPoint()
    {
        float margin = 10f;
        float xSpawn = 0, ySpawn = 0;
        int side = Random.Range(0, 4);
        Debug.Log("Side: " + side);

        switch (side)
        {
            case 0: //Izquierda
                xSpawn = bottomLeftBorder.x - margin;
                ySpawn = Random.Range(bottomLeftBorder.y, topRightBorder.y);
                
                break;
            case 1: //Derecha
                xSpawn = topRightBorder.x + margin;
                ySpawn = Random.Range(bottomLeftBorder.y, topRightBorder.y);
                break;
            case 2: //Abajo
                xSpawn = Random.Range(bottomLeftBorder.x, topRightBorder.x);
                ySpawn = bottomLeftBorder.y - margin;
                break;
            case 3: //Arriba
                xSpawn = Random.Range(bottomLeftBorder.x, topRightBorder.x);
                ySpawn = topRightBorder.y + margin;
                break;
            default:
                break;
        }
        return new Vector3(xSpawn, ySpawn, 0);
    }

    // ---> Gestión de oleadas
    public void WaveController()
    {
        if (nuevaOleada)
        {
            StartCoroutine(PlayerUIManager.instance.WaveStarterText(oleada));
        }
    }
    //public IEnumerator WaveStarterText() ---> Se ha movido a PlayerUIManager
    //{
    //    oleadaText.fontSharedMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 0.7f);
    //    oleadaText.gameObject.SetActive(true);
    //    nuevaOleada = false;
    //    string oleadaNum = "OLEADA " + oleada;
    //    for(int i = 0; i < oleadaNum.Length; i++)
    //    {
    //        oleadaText.text = oleadaText.text + oleadaNum[i];
    //        yield return new WaitForSeconds(0.3f);
    //    }
    //    yield return new WaitForSeconds(1);

        
    //    for (int i = oleadaNum.Length - 1; i >= 0; i--)
    //    {
    //        if(oleadaText.text.Length == 1)
    //        {
    //            oleadaText.text = "";
    //        }
    //        else
    //        {
    //            oleadaText.text = oleadaText.text.Substring(0, oleadaText.text.Length - 1);
    //        }
            
    //        yield return new WaitForSeconds(0.1f);
    //    }
    //    //oleadaText.text = "";
    //    //oleadaText.gameObject.SetActive(false);
    //    oleadaText.fontSharedMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 0);

    //    //startWave();
    //}
    public void EnemiesCalculation()
    {
        //Calcula el numero de enemigos normales y especiales va a haber en la ronda que toque
        int FactorJugadores = Mathf.RoundToInt(1 + (playersList.Count - 1) * 0.25f);
        normalesMax = Mathf.CeilToInt((3 + Mathf.Pow(oleada, 1.5f)) * FactorJugadores);
        especialesMax = Mathf.CeilToInt(enemiesNum * baseEnemiesPercentage);
        normalesMax = normalesMax - especialesMax;
        enemiesNum = normalesMax + especialesMax;
        if (oleada <= 3)
        {
            //No hay enemigos especiales en las 3 primeras oleadas
            normalesMax = normalesMax + especialesMax;
            especialesMax = 0;
        }
        else
        {
            //Aumento de porcentaje de enemigos especiales para la proxima oleada hasta un maximo del 80%
            baseEnemiesPercentage = baseEnemiesPercentage + 0.01f;
            if (baseEnemiesPercentage > 0.8f)
            {
                baseEnemiesPercentage = 0.8f;
            }
        }
    }

    public void EnemiesSpawner()
    {
        //Hay que equilibrar el tipo de especiales que salen en cada ronda de cara a la entrega final del TFG
        if (enemies_Spawned <= enemiesNum)
        {
            int eligeEnemigo = Random.Range(1, enemiesNum + 1);
            if (eligeEnemigo > normalesMax)
            {
                //spawnea especial
                especialesMax--;
                SpawnMeteoroid(5);
            }
            else
            {
                //spawnea normal
                normalesMax--;
                SpawnMeteoroid(0);
            }
            StartCoroutine(meteoritoCD());
            //Necesita restarse al morir un meteorito
            enemies_Spawned++;
        }
    }

    public IEnumerator meteoritoCD()
    {
        canSpawn = false;
        yield return new WaitForSeconds(3);
        canSpawn = true;
    }


    
}
