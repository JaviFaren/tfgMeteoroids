using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    [Header("Debug")]
    public bool bSpawnM = false;
    public List<GameObject> playersListsDebug; // ---> Añadir en el inspector de Unity prefabs de jugadores con distinto ID para hacer pruebas.
    public GameObject meteoroidPrefab; // ---> Añadir en el inspector de Unity un prefab de cualquier enemigo para hacer pruebas.

    [Header("Jugadores")]
    public List<PlayerManager> playersList = new();
    //public int playerCount;

    [Header("Oleadas")]
    public int oleada;
    public bool nuevaOleada;
    public int enemiesNum = 0;

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
        //playerCount = 4;
        nuevaOleada = true;

        //asignacionPlayers();
        //StartCoroutine(prueba());

        StartCoroutine(SpawnPlayers());
        StartCoroutine(PlayerUIManager.instance.WaveStarterText(oleada));
    }
    private void Update()
    {
        // ---> Debug
        if (bSpawnM == true)
        {
            SpawnMeteoroid();
            bSpawnM = false;
        }

        // ---> Interfaz
        //nombres[0] = playerInfo[0].username;
        //puntuaciones[0] = playerInfo[0].puntuacion;
        //vidas[0] = playerInfo[0].vidas;

        //oleadaController();
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
    public void SpawnMeteoroid()
    {
        Vector3 spawnPosition = ChooseEnemySpawnPoint();
        GameObject meteoroid = Instantiate(meteoroidPrefab, spawnPosition, Quaternion.identity);
        //meteoroid.GetComponent<detectarMeteorito>().SetTarget(Vector3.zero);
        meteoroid.GetComponent<Enemy>().SetTarget(Vector3.zero);
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
    public void StartWave()
    {
        int FactorJugadores = Mathf.RoundToInt(1 + (playersList.Count - 1) * 0.25f);
        enemiesNum = Mathf.RoundToInt((3 + Mathf.Pow(oleada, 1.5f)) * FactorJugadores);
        if (oleada == 1)
        {
            enemiesNum = 4 * playersList.Count;
        }
        else
        {
            enemiesNum = (Mathf.RoundToInt(oleada / 2) + 4) * playersList.Count;
        }

        //Debug.Log(enemiesNum);
    }
    //public IEnumerator prueba()
    //{
    //    startWave();
    //    yield return new WaitForSeconds(3);
    //    oleada++;
    //    StartCoroutine(prueba());
    //}
}
