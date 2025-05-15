using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UIIconsDatabase", menuName = "Database/UIIconsDatabase")]
public class UIIconsDatabase : ScriptableObject
{
    public List<UIIcon> UIIconsList;
    private readonly Dictionary<UIIconType, UIIcon> UIIcons = new();

    private void OnEnable()
    {
        InitializeDictionary();
    }

    private void InitializeDictionary()
    {
        foreach (var icon in UIIconsList)
        {
            if (!UIIcons.ContainsKey(icon.id))
                UIIcons.Add(icon.id, icon);
            else
                Debug.LogWarning($"Icono duplicado con ID: {icon.id} en UIIconsDatabase");
        }
    }

    public UIIcon GetIcon(UIIconType id)
    {
        UIIcons.TryGetValue(id, out var icon);
        return icon;
    }
}

[System.Serializable]
public class UIIcon
{
    public UIIconType id;
    public Sprite sprite;
    public RuntimeAnimatorController animator;
}
