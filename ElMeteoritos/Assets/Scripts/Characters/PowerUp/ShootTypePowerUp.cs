using UnityEngine;

[CreateAssetMenu(menuName = "PowerUp/ShootType")]
public class ShootTypePowerUp : PowerUpEffect
{
    public ShootType shootType;
    public float customHeatPerShot = -1f; // Valor negativo indica usar el default
    public int customDamage = -1; // Valor negativo indica usar el default

    public override void ApplyEffect(Player player)
    {
        player.playerStats.ModifyShootProperties(shootType, customHeatPerShot, customDamage);
    }

    public override void RemoveEffect(Player player)
    {
        player.playerStats.ModifyShootProperties(ShootType.DEFAULT, -1f, -1);
    }
}
