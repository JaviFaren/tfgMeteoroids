using UnityEngine;

public class Spaceship : MonoBehaviour
{
    [HideInInspector] public Player playerManager;

    [Header("Nave")]
    [HideInInspector] public SpriteRenderer spaceshipSR;
    [HideInInspector] public Animator spaceshipAnim;

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
        SetCustomizationForSpaceship();
        //SetCustomizationForPropulsion();
        //SetCustomizationForTrail();
    }

    private void SetCustomizationForSpaceship()
    {
        // Color
        string spaceshipColorHex = UserSession.SpaceshipColor;
        spaceshipSR.color = ConvertColor(spaceshipColorHex);

        // Skin
        int spaceshipSkinID = UserSession.SpaceshipSkin;
        var spaceshipSkin = DatabaseManager.Instance.customizationDatabase
            .GetShipSkinById(spaceshipSkinID);

        // Animator
        if (spaceshipSkin != null)
        {
            spaceshipSR.sprite = spaceshipSkin.sprite;
            spaceshipAnim.runtimeAnimatorController = spaceshipSkin.animator;
        }
    }

    private void SetCustomizationForPropulsion()
    {
        // Color
        string propulsionColorHex = UserSession.PropulsionColor;


        // Skin
        int propulsionSkinID = UserSession.PropulsionSkin;
        var propulsionSkin = DatabaseManager.Instance.customizationDatabase
            .GetPropulsionSkinById(propulsionSkinID);

        // Animator
    }

    private void SetCustomizationForTrail() // En funcion de como se haga el trail habra que cambiarlo
    {
        // Color
        string trailColorHex = UserSession.TrailColor;

        // Skin
        int trailSkinID = UserSession.TrailSkin;
        var trailSkin = DatabaseManager.Instance.customizationDatabase
            .GetTrailSkinById(trailSkinID);

        // Animator
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
    #endregion
}
