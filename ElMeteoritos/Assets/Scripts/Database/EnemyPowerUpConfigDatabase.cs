using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyPowerUpConfigDatabase", menuName = "Database/EnemyPowerUpConfigDatabase")]
public class EnemyPowerUpConfigDatabase : ScriptableObject
{
    public List<EnemyPowerUpConfig> enemyPowerUpConfigsList;
    private readonly Dictionary<EnemyType, EnemyPowerUpConfig> enemyPowerUpConfigs = new();

    private void OnEnable()
    {
        InitializeDictionary(enemyPowerUpConfigs, enemyPowerUpConfigsList, e => e.enemyType);
    }

    private void InitializeDictionary<T>(Dictionary<EnemyType, T> dictionary, List<T> list, Func<T, EnemyType> keySelector)
    {
        foreach (var item in list)
        {
            EnemyType key = keySelector(item);
            dictionary[key] = item;
        }
    }

    public EnemyPowerUpConfig GetEnemyPowerUpConfig(EnemyType type) =>
        enemyPowerUpConfigs.TryGetValue(type, out var config) ? config : null;
}

[System.Serializable]
public class EnemyPowerUpConfig
{
    public EnemyType enemyType;
    public List<PowerUpEffect> allowedPowerUps;
}
