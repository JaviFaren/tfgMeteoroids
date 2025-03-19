using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public List<GameObject> playersListsDebug; // ---> Agregar en el inspector de Unity prefabs de jugadores con distinto ID para hacer pruebas.
    public GameObject meteoroidPrefab; // ---> Anadir en el inspector de Unity un prefab de cualquier enemigo para hacer pruebas.

    [Header("Jugadores")]
    public List<PlayerManager> playersList = new();

    [Header("Partida")]
    public MatchState matchState; // Estado de la partida
    public Match currentMatch; 
    public bool isMatchActive; // Booleana que controla si la partida esta activa

    [Header("Oleada")]
    public int wave; // Numero de oleada
    public bool isWaveActive; // Booleana que controla si la oleada esta en marcha
    public WaveType waveType; // Tipo de oleada actual
    public float enemySpawnDelay; // Tipo de espera entre spawn de enemigos
    public int waveEnemyNumber; // Numero maximo de enemigos en este ronda
    public int commonEnemyNumber; // Numero maximo de enemigos comunes en este ronda
    public int specialEnemyNumber; // Numero maximo de enemigos especiales en este ronda
    public List<Enemy> spawnedEnemies = new(); // Lista con los enemigos spwneados en la ronda. Se añaden en Spawn Meteoroid y se eliminan en el metodo OnDeath de la clase Enemy
    public int defeatedEnemyNumber; // Numero de enemigos eliminados en la ronda. Cuando salta el metodo OnDeath de un enemigo lo aumenta en 1
    public int maxEnemyNumberOnScreen; // Numero maximo de enemigos en pantalla
    public bool allEnemiesDefeated => defeatedEnemyNumber == waveEnemyNumber; // Booleana que controla si se ha eliminado a todos los enemigos necesarios para pasar de ronda

    [Header("Enemigos especiales")]
    public float specialEnemyPercentage; // Porcentaje de enemigos especiales
    public int specialEnemyWaveDelay; // Numero de rondas iniciales sin enemigos especiales

    //public int oleada;
    //public bool nuevaOleada;
    //public int enemiesNum = 0;
    //public int enemies_Spawned = 0;
    //public int normalesMax = 0;
    //public int especialesMax = 0;
    //public float baseEnemiesPercentage = 0;
    //public int derrotados = 0;
    //public int maxEnPantalla = 6;
    //public bool canSpawn;
    [Tooltip("Comunes:0, Divx2:1, Divx5:2, Blindados:3, Curativos:4, Expl:5")]
    public GameObject[] contenedores_Enemigos;

    [Header("Camara")]
    public Camera mainCamera;
    public Vector3 bottomLeftBorder;
    public Vector3 topRightBorder;
    public float cameraDistance;

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

        //oleada = 1;
        //baseEnemiesPercentage = 0.15f;
        //playerCount = 4;
        //nuevaOleada = true;
        //canSpawn = false;
        //enemiesNum = 0;

        //asignacionPlayers();
        //StartCoroutine(prueba());

        StartCoroutine(SpawnPlayers());
        StartCoroutine(StartMatch());
    }
    private void Update()
    {
        // ---> Debug
        if (bSpawnM == true)
        {
            SpawnMeteoroid(meteoroidType);
            bSpawnM = false;
        }

        //WaveController();

        //if (canSpawn)
        //{
        //    EnemiesSpawner();
        //}
        
    }

    // ---> Camara
    public void SetCameraBorders()
    {
        cameraDistance = Mathf.Abs(mainCamera.transform.position.z);

        bottomLeftBorder = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, cameraDistance));
        topRightBorder = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, cameraDistance));
    }

    // ---> Spawn de jugadores
    public void AddPlayerToPlayersList(PlayerManager player) // ---> Funcion para agregar jugadores a la lista playersList
    {
        if (!playersList.Contains(player))
        {
            playersList.Add(player);
        }
    }
    public void RemovePlayerFromPlayersList(PlayerManager player) // ---> Funcion para quitar jugadores de la lista playersList
    {
        if (playersList.Contains(player))
        {
            playersList.Remove(player);
        }
    }
    public IEnumerator SpawnPlayers() // ---> Funcion para spawnear a los jugadores, esperando a que se inicialicen todos para que no haya fallos. Habra que cambiar algunas cosas al meter el Photon
    {
        //PhotonNetwork.PlayerList;
        foreach (var pla in playersListsDebug)
        {
            Instantiate(pla);
        }

        yield return new WaitUntil(() => playersList.TrueForAll(e => e.initialized)); // --> Esperar a que todos los jugadores tengan los componentes inicializados

        PlayerUIManager.instance.InitializePlayersPanel(); // ---> Inicializar los paneles de informacion de los jugadores
    }

    // ---> Gestion de enemigos
    public void SetDefeatedEnemies(int amount)
    {
        defeatedEnemyNumber = Mathf.Clamp(defeatedEnemyNumber + amount, 0, waveEnemyNumber);
    }
    public void SpawnEnemy()
    {
        //Hay que equilibrar el tipo de especiales que salen en cada ronda de cara a la entrega final del TFG
        int eligeEnemigo = Random.Range(1, waveEnemyNumber + 1);
        if (eligeEnemigo > commonEnemyNumber)
        {
            //spawnea especial
            //specialEnemyNumber--;
            SpawnMeteoroid(5);
        }
        else
        {
            //spawnea normal
            //commonEnemyNumber--;
            SpawnMeteoroid(0);
        }
    }
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

                spawnedEnemies.Add(contenedores_Enemigos[IDType].transform.GetChild(i).gameObject.GetComponent<Enemy>());

                break;
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
        //Debug.Log("Side: " + side);

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

    public void AddEnemyToSpawnedEnemiesList(Enemy enemy) // ---> Funcion para agregar enemigos a la lista spawnedEnemies
    {
        if (!spawnedEnemies.Contains(enemy))
        {
            spawnedEnemies.Add(enemy);
        }
    }
    public void RemoveEnemyFromSpawnedEnemiesList(Enemy enemy) // ---> Funcion para quitar enemigos de la lista spawnedEnemies
    {
        if (spawnedEnemies.Contains(enemy))
        {
            spawnedEnemies.Remove(enemy);
        }
    }

    // ---> Gestion de oleadas
    public IEnumerator StartMatch()
    {
        matchState = MatchState.INICIAR_RONDA;
        isMatchActive = true;

        yield return null;

        StartCoroutine(MatchLoop());
    }
    public IEnumerator MatchLoop()
    {
        while (isMatchActive)
        {
            switch (matchState)
            {
                case MatchState.INICIAR_RONDA:

                    PlayerUIManager.instance.ManagePlayersPanel(false); // Se desactiva el panel con informacion de los jugadores

                    defeatedEnemyNumber = 0;
                    spawnedEnemies.Clear();
                    //spawnedEnemyNumber = 0;
                    wave++;

                    waveType = SetWaveType(); // Se determina el tipo de oleada
                    Debug.Log("TIPO DE OLEADA: " + waveType);

                    waveEnemyNumber = CalculateNumberOfEnemies(); // Se calcula el numero de enemigos
                    Debug.Log("NUMERO DE ENEMGOS: " + waveEnemyNumber);
                    
                    StartCoroutine(PlayerUIManager.instance.WaveStarterText(wave));
                    yield return new WaitUntil(() => !PlayerUIManager.instance.animating); // Se espera a que termine la animacion del texto de empezar oleada
                    Debug.Log(" --- OLEADA " + wave + " INICIADA ---");

                    PlayerUIManager.instance.ManagePlayersPanel(true); // Se activa el panel con informacion de los jugadores

                    yield return null;

                    matchState = MatchState.RONDA;

                    break;

                case MatchState.RONDA:

                    switch (waveType)
                    {
                        case WaveType.COMMON_WAVE:

                            StartCoroutine(CommonWave());
                            yield return new WaitUntil(() => !isWaveActive);

                            break;

                        case WaveType.SPECIAL_WAVE:
                            break;

                        case WaveType.EASTEREGG_WAVE:
                            break;
                    }

                    matchState = MatchState.TERMINAR_RONDA;

                    break;

                case MatchState.TERMINAR_RONDA: // Aqui se reviviran a lo jugadores que hayan muerto al terminar (si se quiere hacer) 

                    // ---> Falta hacer que los enemigos que estan en pantalla desaparezcan cuando se ha cumplido el objetivo de la oleada (eliminar cierta cantidad de enemigos, sobrevivir cierta cantidad de tiempo, etc)

                    Debug.Log(" --- OLEADA TERMINADA --- ");

                    yield return new WaitForSeconds(0.75f);
                    matchState = MatchState.INICIAR_RONDA;

                    break;
            }
        }
    }

    public WaveType SetWaveType()
    {
        return WaveType.COMMON_WAVE;

        // Adapatar este metodo para que la ronda sea de un tipo u otro en funcion de las condiciones que queramos

        // ---> Ejemplo para que las rondas especiales sean cada 5 rondas y las rondas con easter egg sean con una probabilidad del 10% a partir de la ronda 20

        //if (wave >= 20) 
        //{
        //    //float randomFactor = Random.Range(0f, 1f);
        //    if (Random.Range(0f, 1f) <= 0.1)
        //    {
        //        return WaveType.EASTEREGG_WAVE;
        //    }                        
        //}
        //if (wave % 5 == 0)                      
        //{
        //    return WaveType.SPECIAL_WAVE;
        //}
        //else
        //{
        //    return WaveType.COMMON_WAVE;
        //}
    }
    public int CalculateNumberOfEnemies()
    {
        //Calcula el numero de enemigos normales y especiales que va a haber en la ronda que toque
        int playersFactor = Mathf.RoundToInt(1 + (playersList.Count - 1) * 0.25f);
        commonEnemyNumber = Mathf.CeilToInt((3 + Mathf.Pow(wave, 1.5f)) * playersFactor);

        if (wave <= specialEnemyWaveDelay)
        {
            //No hay enemigos especiales en el numero de oleadas determinado en specialEnemyWaveDelay
            return commonEnemyNumber;
        }
        else
        {
            specialEnemyNumber = Mathf.CeilToInt(commonEnemyNumber * specialEnemyPercentage);
            Debug.Log(specialEnemyNumber);
            commonEnemyNumber -= specialEnemyNumber;

            //Aumento de porcentaje de enemigos especiales para la proxima oleada hasta un maximo del 80%
            specialEnemyPercentage = Mathf.Clamp(specialEnemyPercentage + 0.01f, 0f, 0.8f);

            return commonEnemyNumber + specialEnemyNumber;
        }
    }

    // ---> Tipos de oleadas
    public IEnumerator CommonWave()
    {
        isWaveActive = true;
        while (!allEnemiesDefeated)
        {
            if (spawnedEnemies.Count <= maxEnemyNumberOnScreen)
            {
                SpawnEnemy();
                Debug.Log("ENEMIGOS SPAWNEADOS: " + spawnedEnemies.Count);

                yield return new WaitForSeconds(enemySpawnDelay);
            }
            else
            {
                yield return null;
            }
        }

        Debug.Log("TODOS LOS ENEMIGOS VENCIDOS: " + allEnemiesDefeated);
        isWaveActive = false;
    }

    // Codigo de gestion de rondas antiguo
    //public void WaveController()
    //{
    //    if (nuevaOleada)
    //    {
    //        Debug.Log("Oleada: " + oleada);
    //        StartCoroutine(PlayerUIManager.instance.WaveStarterText(oleada));
    //    }
    //}

    //public void EnemiesCalculation()
    //{
    //    //Calcula el numero de enemigos normales y especiales va a haber en la ronda que toque
    //    int FactorJugadores = Mathf.RoundToInt(1 + (playersList.Count - 1) * 0.25f);
    //    normalesMax = Mathf.CeilToInt((3 + Mathf.Pow(oleada, 1.5f)) * FactorJugadores);
    //    especialesMax = Mathf.CeilToInt(enemiesNum * baseEnemiesPercentage);
    //    normalesMax = normalesMax - especialesMax;
    //    enemiesNum = normalesMax + especialesMax;
    //    if (oleada <= 3)
    //    {
    //        //No hay enemigos especiales en las 3 primeras oleadas
    //        normalesMax = normalesMax + especialesMax;
    //        especialesMax = 0;
    //    }
    //    else
    //    {
    //        //Aumento de porcentaje de enemigos especiales para la proxima oleada hasta un maximo del 80%
    //        baseEnemiesPercentage = baseEnemiesPercentage + 0.01f;
    //        if (baseEnemiesPercentage > 0.8f)
    //        {
    //            baseEnemiesPercentage = 0.8f;
    //        }
    //    }
    //    Debug.Log("Numero de enemigos: " + enemiesNum);
    //}

    //public void EnemiesSpawner()
    //{
    //    //Hay que equilibrar el tipo de especiales que salen en cada ronda de cara a la entrega final del TFG
    //    if (enemies_Spawned < enemiesNum)
    //    {
    //        int eligeEnemigo = Random.Range(1, enemiesNum + 1);
    //        if (eligeEnemigo > normalesMax)
    //        {
    //            //spawnea especial
    //            especialesMax--;
    //            SpawnMeteoroid(5);
    //        }
    //        else
    //        {
    //            //spawnea normal
    //            normalesMax--;
    //            SpawnMeteoroid(0);
    //        }
    //        StartCoroutine(meteoritoCD());
    //        //Necesita restarse al morir un meteorito
    //        enemies_Spawned++;
    //    }
    //    else
    //    {
    //        Debug.Log("oleada terminada");
    //        enemies_Spawned = 0;
    //        canSpawn = false;
    //        oleada++;
    //        nuevaOleada = true;
    //    }
    //    Debug.Log("Enemigos spawneados: " + enemies_Spawned);
    //}

    //public IEnumerator meteoritoCD()
    //{
    //    canSpawn = false;
    //    yield return new WaitForSeconds(8);
    //    canSpawn = true;
    //}
}
