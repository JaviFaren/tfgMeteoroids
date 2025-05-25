using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerPowerUp : MonoBehaviour
{
    [HideInInspector] public Player playerManager;

    private class ActivePowerUp
    {
        public Coroutine coroutine;
        public PowerUpEffect effect;
    }

    private readonly Dictionary<int, ActivePowerUp> activePowerUps = new();

    private void Awake()
    {
        playerManager = GetComponent<Player>();
    }

    private void OnDestroy()
    {
        ClearAllPowerUps();
    }

    public bool TryApplyPowerUpEffect(PowerUpEffect effect)
    {
        int id = effect.id;

        if (effect is ShootTypePowerUp newShootPowerUp)
        {
            // Busca si hay un ShootTypePowerUp activo
            var existingShootPowerUp = activePowerUps.Values.FirstOrDefault(p => p.effect is ShootTypePowerUp);

            if (existingShootPowerUp != null)
            {
                var existing = (ShootTypePowerUp)existingShootPowerUp.effect;

                if (existing.shootType != newShootPowerUp.shootType) return false;
            }
        }

        if (effect is HealingPowerUp healingPowerUp)
        {
            if (playerManager.playerStats.CurrentLives == playerManager.playerStats.MaxLives) return false;
        }

        // Si ya esta aplicado el efecto, se reinicia
        if (activePowerUps.TryGetValue(id, out var active))
        {
            StopCoroutine(active.coroutine);
            active.effect.RemoveEffect(playerManager);
            activePowerUps.Remove(id);
        }

        effect.ApplyEffect(playerManager);

        if (effect.duration > 0f)
        {
            Coroutine c = StartCoroutine(EffectTimer(effect));
            activePowerUps[id] = new ActivePowerUp { coroutine = c, effect = effect };
        }

        playerManager.playerSoundFX.PlayFXSound(playerManager.playerSoundFX.PowerUp);

        return true;
    }

    private IEnumerator EffectTimer(PowerUpEffect effect)
    {
        yield return new WaitForSeconds(effect.duration);
        effect.RemoveEffect(playerManager);
        activePowerUps.Remove(effect.id);
    }

    public void ClearAllPowerUps()
    {
        foreach (var activePowerUp in activePowerUps)
        {
            StopCoroutine(activePowerUp.Value.coroutine);
            activePowerUp.Value.effect.RemoveEffect(playerManager);
        }
        activePowerUps.Clear();
    }
}
