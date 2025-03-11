using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class controlPartida : MonoBehaviour
{
    public static controlPartida instance;

    [Header("Debug")]
    public bool bSpawnM = false;
    public GameObject player;

    [Header("Spawn Meteoritos")]
    public Camera cam;
    public Vector3 bottomLeft;
    public Vector3 topRight;
    public float distance;
    public GameObject meteoroidPrefab;
    public int oleada;
    private bool nuevaOleada;
    public TextMeshProUGUI oleadaText;
    public int playerCount;

    private void Awake()
    {
        if (instance == null) 
        { 
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        cam = Camera.main;
    }

    void Start()
    {
        distance = Mathf.Abs(cam.transform.position.z);

        bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, distance));
        topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, distance));

        oleada = 1;
        playerCount = 4;
        nuevaOleada = true;

        //StartCoroutine(prueba());
    }

    void Update()
    {
        if (bSpawnM == true)
        {
            SpawnMeteoroid();
            bSpawnM = false;
        }

        //oleadaController();
    }

    void SpawnMeteoroid()
    {

        Vector3 spawnPosition = ChooseEnemySpawnPoint();
        Debug.Log(spawnPosition);
        GameObject meteoroid = Instantiate(meteoroidPrefab, spawnPosition, Quaternion.identity);
        meteoroid.GetComponent<detectarMeteorito>().SetTarget(Vector3.zero);
    }

    Vector3 ChooseEnemySpawnPoint()
    {
        float margin = 10f;
        float xSpawn = 0, ySpawn = 0;
        int side = Random.Range(0, 4);
        Debug.Log("Side: " + side);

        switch (side)
        {
            case 0: //Izquierda
                xSpawn = bottomLeft.x - margin;
                ySpawn = Random.Range(bottomLeft.y, topRight.y);
                
                break;
            case 1: //Derecha
                xSpawn = topRight.x + margin;
                ySpawn = Random.Range(bottomLeft.y, topRight.y);
                break;
            case 2: //Abajo
                xSpawn = Random.Range(bottomLeft.x, topRight.x);
                ySpawn = bottomLeft.y - margin;
                break;
            case 3: //Arriba
                xSpawn = Random.Range(bottomLeft.x, topRight.x);
                ySpawn = topRight.y + margin;
                break;
            default:
                break;
        }
        return new Vector3(xSpawn, ySpawn, 0);
    }

    public void oleadaController()
    {
        if (nuevaOleada)
        {
            StartCoroutine(waveStarter());
        }
    }

    public IEnumerator waveStarter()
    {
        oleadaText.gameObject.SetActive(true);
        nuevaOleada = false;
        string oleadaNum = "OLEADA " + oleada;
        for(int i = 0; i < oleadaNum.Length; i++)
        {
            oleadaText.text = oleadaText.text + oleadaNum[i];
            yield return new WaitForSeconds(0.3f);
        }
        yield return new WaitForSeconds(1);
        oleadaText.text = "";
        oleadaText.gameObject.SetActive(false);

        startWave();
    }
    public int enemiesNum = 0;

    public void startWave()
    {
        
        int FactorJugadores = Mathf.RoundToInt(1 + (playerCount - 1) * 0.25f);
        enemiesNum = Mathf.RoundToInt((3 + Mathf.Pow(oleada, 1.5f)) * FactorJugadores);
        if (oleada == 1)
        {
            enemiesNum = 4 * playerCount;
        }
        else
        {
            enemiesNum = (Mathf.RoundToInt(oleada / 2) + 4) * playerCount;
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
