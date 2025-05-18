using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomizationDatabase", menuName = "Database/CustomizationDatabase")]
public class CustomizationDatabase : ScriptableObject
{
    [Header("Skins")]
    public List<ShipSkin> shipSkinsList;
    public List<ShotSkin> shotSkinsList;
    public List<PropulsionSkin> propulsionSkinsList;
    public List<TrailSkin> trailSkinsList;

    private readonly Dictionary<int, ShipSkin> shipSkins = new();
    private readonly Dictionary<int, ShotSkin> shotSkins = new();
    private readonly Dictionary<int, PropulsionSkin> propulsionSkins = new();
    private readonly Dictionary<int, TrailSkin> trailSkins = new();

    private void OnEnable()
    {
        InitializeDictionary(shipSkins, shipSkinsList);
        InitializeDictionary(shotSkins, shotSkinsList);
        InitializeDictionary(propulsionSkins, propulsionSkinsList);
        InitializeDictionary(trailSkins, trailSkinsList);
    }

    private void InitializeDictionary<T>(Dictionary<int, T> dictionary, List<T> list) where T : BaseSkin
    {
        dictionary.Clear();
        int id = 1;
        foreach (var item in list.Where(item => item != null))
        {
            item.id = id;
            dictionary[id] = item;
            id++;
        }
    }

    public ShipSkin GetShipSkinById(int id) => shipSkins.TryGetValue(id, out var skin) ? skin : null;
    public List<ShipSkin> GetAllShipSkins() => shipSkins.Values.ToList();

    public ShotSkin GetShotSkinById(int id) => shotSkins.TryGetValue(id, out var skin) ? skin : null;
    public List<ShotSkin> GetAllShotSkins() => shotSkins.Values.ToList();

    public PropulsionSkin GetPropulsionSkinById(int id) => propulsionSkins.TryGetValue(id, out var skin) ? skin : null;
    public List<PropulsionSkin> GetAllPropulsionSkins() => propulsionSkins.Values.ToList();

    public TrailSkin GetTrailSkinById(int id) => trailSkins.TryGetValue(id, out var skin) ? skin : null;
    public List<TrailSkin> GetAllTrailSkins() => trailSkins.Values.ToList();

    public List<BaseSkin> GetSkinsByField(CustomizationField field)
    {
        return field switch
        {
            CustomizationField.SPACESHIP_SKIN => shipSkins.Values.Cast<BaseSkin>().ToList(),
            CustomizationField.PROPULSION_SKIN => propulsionSkins.Values.Cast<BaseSkin>().ToList(),
            CustomizationField.TRAIL_SKIN => trailSkins.Values.Cast<BaseSkin>().ToList(),
            CustomizationField.SHOT_SKIN => shotSkins.Values.Cast<BaseSkin>().ToList(),
            _ => new List<BaseSkin>()
        };
    }
}

[System.Serializable]
public abstract class BaseSkin
{
    [HideInInspector] public int id;
    public Sprite sprite;
    public RuntimeAnimatorController animator;
}

[System.Serializable]
public class ShipSkin : BaseSkin { }

[System.Serializable]
public class ShotSkin : BaseSkin { }

[System.Serializable]
public class PropulsionSkin : BaseSkin { }

[System.Serializable]
public class TrailSkin : BaseSkin 
{
    public List<Sprite> sprites = new();
    public float startSize;
}
