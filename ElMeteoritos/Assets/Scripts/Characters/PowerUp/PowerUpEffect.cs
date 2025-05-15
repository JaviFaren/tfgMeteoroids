using UnityEngine;

public abstract class PowerUpEffect : ScriptableObject
{
    public int id;
    [Tooltip("Duracion del efecto del PowerUp. Si es 0 o menor, se considera instantaneo")] public float duration;
    [Tooltip("Duracion del PowerUp")] public float lifetime;
    [Tooltip("Sprite del PowerUp")] public Sprite icon;

    public abstract void ApplyEffect(Player player);
    public abstract void RemoveEffect(Player player);
}
