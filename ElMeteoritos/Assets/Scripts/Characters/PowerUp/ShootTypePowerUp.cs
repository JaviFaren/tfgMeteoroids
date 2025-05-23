using UnityEngine;

[CreateAssetMenu(menuName = "PowerUp/ShootType")]
public class ShootTypePowerUp : PowerUpEffect
{
    public ShootType shootType;

    [Tooltip("Valor negativo indica usar el default")] public float customHeatPerShot = -1f;
    [Tooltip("Valor negativo indica usar el default")] public int customDamage = -1;
    [Tooltip("Valor negativo indica usar el default")] public float customCooldown = -1;

    public override void ApplyEffect(Player player)
    {
        player.playerStats.ModifyShootProperties(shootType, customHeatPerShot, customDamage, customCooldown);
    }

    public override void RemoveEffect(Player player)
    {
        player.playerStats.ModifyShootProperties(ShootType.DEFAULT, -1, -1, -1);
    }
}
