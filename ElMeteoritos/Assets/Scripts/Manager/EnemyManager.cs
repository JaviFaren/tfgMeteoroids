using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//public class EnemyManager : MonoBehaviourPun
//{
//    public static EnemyManager Instance { get; private set; }

//    [Header("Configuracion de pools")]
//    [SerializeField] private List<EnemyPoolConfig> enemyPoolConfigs;
//    private readonly Dictionary<EnemyType, EnemyPool> enemyPools = new();

//    [Header("Enemigos")]
//    private readonly List<Enemy> ActiveEnemies = new();
//    private static readonly Vector3 HiddenSpawnPosition = new(-9999, -9999, 0);

//    [Header("Configuracion de enemigos")]
//    [SerializeField] private List<WeightedEnemyType> commonEnemyTypes;
//    [SerializeField] private List<WeightedEnemyType> specialEnemyTypes;

//    [Header("PowerUps")]
//    public float powerUpSpawnChance;

//    private void Awake()
//    {
//        // Singleton
//        if (Instance != null && Instance != this)
//        {
//            Destroy(gameObject);
//            return;
//        }
//        Instance = this;
//    }

//    #region Initialize
//    public void InitializeEnemyPools()
//    {
//        foreach (var config in enemyPoolConfigs)
//        {
//            if (!enemyPools.ContainsKey(config.type))
//                enemyPools[config.type] = new EnemyPool(config.type, config.container);

//            for (int i = 0; i < config.amount; i++)
//            {
//                GameObject enemy = PhotonNetwork.InstantiateRoomObject(config.prefabPath, HiddenSpawnPosition, Quaternion.identity);
//                enemy.SetActive(false);

//                photonView.RPC(nameof(RegisterEnemyInPool), RpcTarget.AllBuffered, (int)config.type, enemy.GetComponent<PhotonView>().ViewID);
//            }
//        }
//    }

//    [PunRPC]
//    private void RegisterEnemyInPool(int typeInt, int viewID)
//    {
//        EnemyType type = (EnemyType)typeInt;
//        PhotonView view = PhotonView.Find(viewID);
//        if (view == null)
//        {
//            Debug.LogError("PhotonView no encontrado al registrar enemigo en pool.");
//            return;
//        }

//        if (!enemyPools.ContainsKey(type))
//        {
//            var config = enemyPoolConfigs.Find(c => c.type == type);
//            if (config == null)
//            {
//                Debug.LogError($"No se encontro configuración para el tipo de enemigo {type}");
//                return;
//            }
//            enemyPools[type] = new EnemyPool(type, config.container);
//        }

//        GameObject enemy = view.gameObject;
//        enemy.transform.SetParent(enemyPools[type].Container, false);
//        enemyPools[type].ReturnToPool(enemy);
//    }
//    #endregion

//    #region Enemy List Management
//    private void AddEnemyToEnemiesList(Enemy enemy)
//    {
//        if (!ActiveEnemies.Contains(enemy)) ActiveEnemies.Add(enemy);
//    }

//    private void RemoveEnemyFromEnemiesList(Enemy enemy)
//    {
//        if (ActiveEnemies.Contains(enemy)) ActiveEnemies.Remove(enemy);
//    }

//    public IReadOnlyList<Enemy> GetActiveEnemies() => ActiveEnemies.AsReadOnly();

//    public int GetEnemiesCount() { return ActiveEnemies.Count; }
//    #endregion

//    #region Spawn
//    public void SpawnEnemy(EnemyType type)
//    {
//        if (!PhotonNetwork.IsMasterClient) return;

//        GameObject enemy = GetInactiveEnemy(type);
//        if (enemy == null)
//        {
//            Debug.LogWarning($"No hay enemigos disponibles en el pool para el tipo {type}");
//            return;
//        }

//        Vector3 spawnPos = enemy.GetComponent<Enemy>().GetSpawnPosition();
//        int viewID = enemy.GetComponent<PhotonView>().ViewID;
//        photonView.RPC(nameof(RPC_ActivateEnemy), RpcTarget.All, viewID, spawnPos);
//    }

//    [PunRPC]
//    public void RPC_ActivateEnemy(int viewID, Vector3 position, PhotonMessageInfo info)
//    {
//        PhotonView view = PhotonView.Find(viewID);
//        if (view == null)
//        {
//            Debug.LogError("PhotonView no encontrado al activar enemigo.");
//            return;
//        }

//        GameObject enemy = view.gameObject;
//        //enemy.GetComponent<ScreenPositionCheck>().enabled = false;
//        enemy.transform.position = position;
//        enemy.SetActive(true);

//        if (enemy.TryGetComponent(out Enemy enemyComp))
//        {
//            float lag = (float)(PhotonNetwork.Time - info.SentServerTime);
//            enemyComp.Activate(lag);
//            AddEnemyToEnemiesList(enemyComp);
//        }
//    }

//    public GameObject GetInactiveEnemy(EnemyType type)
//    {
//        return enemyPools.TryGetValue(type, out EnemyPool pool) ? pool.GetInactiveEnemy() : null;
//    }

//    public Vector3 GetRandomSpawnPoint()
//    {
//        const float margin = 5f;
//        Vector3 bottomLeft = GameManager.Instance.cameraBounds.BottomLeft;
//        Vector3 topRight = GameManager.Instance.cameraBounds.TopRight;

//        return Random.Range(0, 4) switch
//        {
//            0 => new Vector3(bottomLeft.x - margin, Random.Range(bottomLeft.y, topRight.y), 0),
//            1 => new Vector3(topRight.x + margin, Random.Range(bottomLeft.y, topRight.y), 0),
//            2 => new Vector3(Random.Range(bottomLeft.x, topRight.x), bottomLeft.y - margin, 0),
//            3 => new Vector3(Random.Range(bottomLeft.x, topRight.x), topRight.y + margin, 0),
//            _ => Vector3.zero
//        };
//    }
//    #endregion

//    #region Despawn
//    public void DespawnEnemy(int viewID)
//    {
//        photonView.RPC(nameof(RPC_DespawnEnemy), RpcTarget.All, viewID);
//    }

//    [PunRPC]
//    private void RPC_DespawnEnemy(int viewID)
//    {
//        PhotonView view = PhotonView.Find(viewID);
//        if (view != null)
//        {
//            if (view.TryGetComponent(out Enemy enemy))
//            {
//                RemoveEnemyFromEnemiesList(enemy);

//                if (enemyPools.TryGetValue(enemy.enemyType, out EnemyPool pool))
//                    pool.ReturnToPool(enemy.gameObject);
//            }
//        }
//    }
//    #endregion

//    #region Enemy Management
//    public EnemyType GetRandomCommonEnemyType() => GetRandomWeightedEnemy(commonEnemyTypes);

//    public EnemyType GetRandomSpecialEnemyType() => GetRandomWeightedEnemy(specialEnemyTypes);

//    private EnemyType GetRandomWeightedEnemy(List<WeightedEnemyType> list)
//    {
//        if (list == null || list.Count == 0)
//        {
//            Debug.LogWarning("Lista vacía de tipos de enemigos.");
//            return EnemyType.METEOROID_COMMON;
//        }

//        float totalWeight = 0f;
//        foreach (var entry in list)
//            totalWeight += entry.weight;

//        float randomValue = Random.Range(0f, totalWeight);
//        float cumulative = 0f;

//        foreach (var entry in list)
//        {
//            cumulative += entry.weight;
//            if (randomValue <= cumulative)
//                return entry.type;
//        }

//        return list[Random.Range(0, list.Count)].type; // Fallback por seguridad
//    }
//    #endregion

//    #region Power Ups
//    public void TrySpawnPowerUp(EnemyType enemyType, Vector3 position)
//    {
//        if (!PhotonNetwork.IsMasterClient) return;

//        var config = DatabaseManager.Instance.enemyPowerUpConfigDatabase.GetEnemyPowerUpConfig(enemyType);
//        var powerUps = config?.allowedPowerUps;

//        if (powerUps == null || powerUps.Count == 0) return;

//        if (Random.value > powerUpSpawnChance) return;

//        int effectIndex = Random.Range(0, powerUps.Count);
//        GameObject powerUpObject = PhotonNetwork.InstantiateRoomObject("PowerUp", position, Quaternion.identity);
//        powerUpObject.GetComponent<PhotonView>().RPC("RPC_Initialize", RpcTarget.AllBuffered, (int)enemyType, effectIndex);
//    }
//    #endregion
//}

//[System.Serializable]
//public class EnemyPoolConfig
//{
//    public EnemyType type;
//    public string prefabPath;
//    public int amount;
//    public Transform container;
//}

//public class EnemyPool
//{
//    public EnemyType Type { get; }
//    public Transform Container => container;

//    private readonly HashSet<GameObject> inactiveEnemies = new();
//    private readonly Transform container;

//    public EnemyPool(EnemyType type, Transform container)
//    {
//        Type = type;
//        this.container = container;
//    }

//    public GameObject GetInactiveEnemy()
//    {
//        if (inactiveEnemies.Count == 0) return null;

//        GameObject enemy = inactiveEnemies.First();
//        inactiveEnemies.Remove(enemy);
//        return enemy;
//    }

//    public void ReturnToPool(GameObject enemy)
//    {
//        enemy.transform.position = container.position;
//        enemy.SetActive(false);
//        inactiveEnemies.Add(enemy);
//    }
//}

//[System.Serializable]
//public class WeightedEnemyType
//{
//    public EnemyType type;
//    [Tooltip("Probabilidad relativa (no necesita sumar 100)")] public float weight;
//}

public class EnemyManager : MonoBehaviourPun
{
    public static EnemyManager Instance { get; private set; }

    [Header("Configuración de pools")]
    [SerializeField] private List<EnemyPoolConfig> enemyPoolConfigs;
    private readonly Dictionary<EnemyType, EnemyPool> enemyPools = new();

    [Header("Enemigos")]
    private readonly List<Enemy> ActiveEnemies = new();
    private static readonly Vector3 HiddenSpawnPosition = new(-9999, -9999, 0);

    [Header("Configuración de enemigos")]
    [SerializeField] private List<WeightedEnemyType> commonEnemyTypes;
    [SerializeField] private List<WeightedEnemyType> specialEnemyTypes;

    [Header("PowerUps")]
    public float powerUpSpawnChance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    #region Initialize
    public void InitializeEnemyPools()
    {
        foreach (var config in enemyPoolConfigs)
        {
            if (!enemyPools.ContainsKey(config.type))
                enemyPools[config.type] = new EnemyPool(config.type, config.container);

            Enemy[] sceneEnemies = config.container.GetComponentsInChildren<Enemy>(true);
            foreach (Enemy enemy in sceneEnemies)
            {
                if (enemy.enemyType == config.type)
                {
                    enemy.gameObject.SetActive(false);
                    int viewID = enemy.GetComponent<PhotonView>().ViewID;
                    photonView.RPC(nameof(RegisterEnemyInPool), RpcTarget.AllBuffered, (int)config.type, viewID);
                }
            }
        }
    }

    [PunRPC]
    private void RegisterEnemyInPool(int typeInt, int viewID)
    {
        EnemyType type = (EnemyType)typeInt;
        PhotonView view = PhotonView.Find(viewID);
        if (view == null)
        {
            Debug.LogError("PhotonView no encontrado al registrar enemigo en pool.");
            return;
        }

        if (!enemyPools.ContainsKey(type))
        {
            var config = enemyPoolConfigs.Find(c => c.type == type);
            if (config == null)
            {
                Debug.LogError($"No se encontró configuración para el tipo de enemigo {type}");
                return;
            }
            enemyPools[type] = new EnemyPool(type, config.container);
        }

        GameObject enemy = view.gameObject;
        enemy.transform.SetParent(enemyPools[type].Container, false);
        enemyPools[type].ReturnToPool(enemy);
    }
    #endregion

    #region Enemy List Management
    private void AddEnemyToEnemiesList(Enemy enemy)
    {
        if (!ActiveEnemies.Contains(enemy)) ActiveEnemies.Add(enemy);
    }

    private void RemoveEnemyFromEnemiesList(Enemy enemy)
    {
        if (ActiveEnemies.Contains(enemy)) ActiveEnemies.Remove(enemy);
    }

    public IReadOnlyList<Enemy> GetActiveEnemies() => ActiveEnemies.AsReadOnly();

    public int GetEnemiesCount() { return ActiveEnemies.Count; }
    #endregion

    #region Spawn
    public void SpawnEnemy(EnemyType type)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        GameObject enemy = GetInactiveEnemy(type);
        if (enemy == null)
        {
            Debug.LogWarning($"No hay enemigos disponibles en el pool para el tipo {type}");
            return;
        }

        Vector3 spawnPos = enemy.GetComponent<Enemy>().GetSpawnPosition();
        int viewID = enemy.GetComponent<PhotonView>().ViewID;
        photonView.RPC(nameof(RPC_ActivateEnemy), RpcTarget.All, viewID, spawnPos);
    }

    [PunRPC]
    public void RPC_ActivateEnemy(int viewID, Vector3 position, PhotonMessageInfo info)
    {
        PhotonView view = PhotonView.Find(viewID);
        if (view == null)
        {
            Debug.LogError("PhotonView no encontrado al activar enemigo.");
            return;
        }

        GameObject enemy = view.gameObject;
        enemy.transform.position = position;
        enemy.SetActive(true);

        if (enemy.TryGetComponent(out Enemy enemyComp))
        {
            float lag = (float)(PhotonNetwork.Time - info.SentServerTime);
            enemyComp.Activate(lag);
            AddEnemyToEnemiesList(enemyComp);
        }
    }

    public GameObject GetInactiveEnemy(EnemyType type)
    {
        return enemyPools.TryGetValue(type, out EnemyPool pool) ? pool.GetInactiveEnemy() : null;
    }

    public Vector3 GetRandomSpawnPoint()
    {
        const float margin = 5f;
        Vector3 bottomLeft = GameManager.Instance.cameraBounds.BottomLeft;
        Vector3 topRight = GameManager.Instance.cameraBounds.TopRight;

        return Random.Range(0, 4) switch
        {
            0 => new Vector3(bottomLeft.x - margin, Random.Range(bottomLeft.y, topRight.y), 0),
            1 => new Vector3(topRight.x + margin, Random.Range(bottomLeft.y, topRight.y), 0),
            2 => new Vector3(Random.Range(bottomLeft.x, topRight.x), bottomLeft.y - margin, 0),
            3 => new Vector3(Random.Range(bottomLeft.x, topRight.x), topRight.y + margin, 0),
            _ => Vector3.zero
        };
    }
    #endregion

    #region Despawn
    public void DespawnEnemy(int viewID)
    {
        photonView.RPC(nameof(RPC_DespawnEnemy), RpcTarget.All, viewID);
    }

    [PunRPC]
    private void RPC_DespawnEnemy(int viewID)
    {
        PhotonView view = PhotonView.Find(viewID);
        if (view != null)
        {
            if (view.TryGetComponent(out Enemy enemy))
            {
                RemoveEnemyFromEnemiesList(enemy);

                if (enemyPools.TryGetValue(enemy.enemyType, out EnemyPool pool))
                    pool.ReturnToPool(enemy.gameObject);
            }
        }
    }
    #endregion

    #region Enemy Management
    public EnemyType GetRandomCommonEnemyType() => GetRandomWeightedEnemy(commonEnemyTypes);

    public EnemyType GetRandomSpecialEnemyType() => GetRandomWeightedEnemy(specialEnemyTypes);

    private EnemyType GetRandomWeightedEnemy(List<WeightedEnemyType> list)
    {
        if (list == null || list.Count == 0)
        {
            Debug.LogWarning("Lista vacía de tipos de enemigos.");
            return EnemyType.METEOROID_COMMON;
        }

        float totalWeight = 0f;
        foreach (var entry in list)
            totalWeight += entry.weight;

        float randomValue = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var entry in list)
        {
            cumulative += entry.weight;
            if (randomValue <= cumulative)
                return entry.type;
        }

        return list[Random.Range(0, list.Count)].type;
    }
    #endregion

    #region Power Ups
    public void TrySpawnPowerUp(EnemyType enemyType, Vector3 position)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        var config = DatabaseManager.Instance.enemyPowerUpConfigDatabase.GetEnemyPowerUpConfig(enemyType);
        var powerUps = config?.allowedPowerUps;

        if (powerUps == null || powerUps.Count == 0) return;

        if (Random.value > powerUpSpawnChance) return;

        int effectIndex = Random.Range(0, powerUps.Count);
        GameObject powerUpObject = PhotonNetwork.InstantiateRoomObject("PowerUp", position, Quaternion.identity);
        powerUpObject.GetComponent<PhotonView>().RPC("RPC_Initialize", RpcTarget.AllBuffered, (int)enemyType, effectIndex);
    }
    #endregion
}

[System.Serializable]
public class EnemyPoolConfig
{
    public EnemyType type;
    [Tooltip("Transform contenedor con enemigos desactivados en escena")]
    public Transform container;
}

public class EnemyPool
{
    public EnemyType Type { get; }
    public Transform Container => container;

    private readonly HashSet<GameObject> inactiveEnemies = new();
    private readonly Transform container;

    public EnemyPool(EnemyType type, Transform container)
    {
        Type = type;
        this.container = container;
    }

    public GameObject GetInactiveEnemy()
    {
        if (inactiveEnemies.Count == 0) return null;

        GameObject enemy = inactiveEnemies.First();
        inactiveEnemies.Remove(enemy);
        return enemy;
    }

    public void ReturnToPool(GameObject enemy)
    {
        enemy.transform.position = container.position;
        enemy.SetActive(false);
        inactiveEnemies.Add(enemy);
    }
}

[System.Serializable]
public class WeightedEnemyType
{
    public EnemyType type;
    [Tooltip("Probabilidad relativa (no necesita sumar 100)")]
    public float weight;
}
