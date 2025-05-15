using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "PowerUpDatabase", menuName = "Database/PowerUpDatabase")]
public class PowerUpDatabase : ScriptableObject
{
    public List<PowerUpEffect> powerUpsList;
    private readonly Dictionary<int, PowerUpEffect> powerUps = new();

    private void OnEnable()
    {
        InitializeDictionary(powerUps, powerUpsList);
    }

    private void InitializeDictionary<T>(Dictionary<int, T> dictionary, List<T> list) where T : class
    {
        int id = 1;
        foreach (var item in list)
        {
            if (item == null) continue;

            var field = item.GetType().GetField("id");
            if (field != null)
            {
                field.SetValue(item, id);
                dictionary[id] = item;
                id++;
            }
        }
    }

    public PowerUpEffect GetPowerUpById(int id) => powerUps.ContainsKey(id) ? powerUps[id] : null;
}
