using UnityEngine;

[CreateAssetMenu(menuName = "PowerUp/Healing")]
public class HealingPowerUp : PowerUpEffect
{
    public int healAmount;

    public override void ApplyEffect(Player player)
    {
        player.playerStats.ModifyLives(healAmount);
    }

    public override void RemoveEffect(Player player)
    {
        
    }
}
