using System.Collections;
using System.Collections.Generic;
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
    }

    void Update()
    {
        if (bSpawnM == true)
        {
            SpawnMeteoroid();
            bSpawnM = false;
        }
    }

    void SpawnMeteoroid()
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
        }

        Vector3 spawnPosition = new Vector3(xSpawn, ySpawn, 0);
        Debug.Log(spawnPosition);
        GameObject meteoroid = Instantiate(meteoroidPrefab, spawnPosition, Quaternion.identity);
        meteoroid.GetComponent<detectarMeteorito>().SetTarget(Vector3.zero);
    }
}
