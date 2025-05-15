using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }

    [Header("Personalizacion")]
    public CustomizationDatabase customizationDatabase;

    [Header("Power Ups")]
    public PowerUpDatabase powerUpDatabase;
    public EnemyPowerUpConfigDatabase enemyPowerUpConfigDatabase;

    [Header("Iconos de UI")]
    public UIIconsDatabase UIIconsDatabse;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }
}