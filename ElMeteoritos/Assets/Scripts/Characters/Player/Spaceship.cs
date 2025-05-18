using UnityEngine;

public class Spaceship : MonoBehaviour
{
    [HideInInspector] public Player playerManager;

    [Header("Nave")]
    [HideInInspector] public SpriteRenderer spaceshipSR;
    [HideInInspector] public Animator spaceshipAnim;

    [Header("Propulsion")]
    public SpriteRenderer propulsionSR;
    public Animator propulsionAnim;

    [Header("Trail")]
    public ParticleSystem trailPS;

    [Header("Disparo")]
    public GameObject shotPrefab;
    public Transform shotSpawn;

    private void Awake()
    {
        playerManager = GetComponentInParent<Player>();
        spaceshipSR = GetComponent<SpriteRenderer>();
        spaceshipAnim = GetComponent<Animator>();
    }

    #region INITIALIZATION
    public void Initialize()
    {
        SetCustomizationForSpaceship(UserSession.SpaceshipColor, UserSession.SpaceshipSkin);
        SetCustomizationForPropulsion(UserSession.PropulsionColor, UserSession.PropulsionSkin);
        SetCustomizationForTrail(UserSession.TrailColor, UserSession.TrailSkin);
    }

    public void SetCustomizationForSpaceship(string colorHex, int skinID)
    {
        spaceshipSR.color = ConvertColor(colorHex);

        var skin = DatabaseManager.Instance.customizationDatabase.GetShipSkinById(skinID);
        if (skin != null)
        {
            spaceshipSR.sprite = skin.sprite;
            spaceshipAnim.runtimeAnimatorController = skin.animator;
        }
    }

    public void SetCustomizationForPropulsion(string colorHex, int skinID)
    {
        propulsionSR.color = ConvertColor(colorHex);

        var skin = DatabaseManager.Instance.customizationDatabase.GetPropulsionSkinById(skinID);
        if (skin != null)
        {
            propulsionAnim.runtimeAnimatorController = skin.animator;
        }
    }

    public void SetCustomizationForTrail(string colorHex, int skinID)
    {
        // Color
        var main = trailPS.main;
        main.startColor = ConvertColor(colorHex);

        // Skin
        var trailSkin = DatabaseManager.Instance.customizationDatabase.GetTrailSkinById(skinID);
        if (trailSkin == null)
        {
            Debug.LogWarning("TrailSkin not found");
            return;
        }

        main.startSize = trailSkin.startSize;

        var textureSheet = trailPS.textureSheetAnimation;
        textureSheet.RemoveSprite(0);

        foreach (var sprite in trailSkin.sprites)
        {
            textureSheet.AddSprite(sprite);
        }

        
    }

    public void SetCustomizationForShot(GameObject shot, string colorHex, int skinID)
    {
        SpriteRenderer shotSR = shot.GetComponent<SpriteRenderer>();
        Animator shotAnim = shot.GetComponent<Animator>();

        shotSR.color = ConvertColor(colorHex);

        var skin = DatabaseManager.Instance.customizationDatabase.GetShotSkinById(skinID);
        if (skin != null)
        {
            shotSR.sprite = skin.sprite;
            shotAnim.runtimeAnimatorController = skin.animator;
        }
        //SpriteRenderer shotSR = shotPrefab.GetComponent<SpriteRenderer>();
        //Animator shotAnim = shotPrefab.GetComponent<Animator>();

        //// Color
        //string shotColorHex = UserSession.ShotColor;
        //shotSR.color = ConvertColor(shotColorHex);

        //// Skin
        //int shotSkinID = UserSession.ShotSkin;
        //var shotSkin = DatabaseManager.Instance.customizationDatabase
        //    .GetShotSkinById(shotSkinID);

        //// Animator
        //if (shotSkin != null)
        //{
        //    shotSR.sprite = shotSkin.sprite;
        //    shotAnim.runtimeAnimatorController = shotSkin.animator;
        //}
    }

    public Color ConvertColor(string hexColor)
    {
        if (ColorUtility.TryParseHtmlString("#" + hexColor, out Color color))
            return color;

        Debug.LogError("Color hexadecimal no valido: " + hexColor);
        return Color.white;
    }
    #endregion

    #region ANIMATIONS
    public void PlayTargetAnimation(string targetAnimation)
    {
        //animator.CrossFade(targetAnimation, 0.2f);
        spaceshipAnim.Play(targetAnimation);
    }

    public void SetPropulsion(float speed)
    {
        propulsionAnim.SetFloat("Speed", speed);
    }
    #endregion

    #region TRAIL
    public void DisplayTrail(float speed)
    {
        if (speed > 5 && !trailPS.isPlaying)
        {
            trailPS.Play();
        }
        else if (speed <= 5 && trailPS.isPlaying)
        {
            trailPS.Stop();
        }
    }
    #endregion
}
