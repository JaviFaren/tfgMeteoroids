using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using System.Collections;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    [HideInInspector] public Player playerManager;

    [Header("Componentes")]
    [HideInInspector] public Rigidbody rb;

    [Header("Rotacion")]
    [SerializeField] private Vector2 joystickAxis;
    [SerializeField] private float targetAngle;
    [SerializeField] private float currentAngle;
    [SerializeField] private float maxRotationSpeed;
    [SerializeField] private float rotationAcceleration;

    [Header("Propulsion")]
    [SerializeField] private float maxPropulsionSpeed;

    [Header("Disparo")]
    [SerializeField] private float shotForce;

    [SerializeField] private float defaultShotCooldown;
    private float? shotCooldownOverride = null;
    private float CurrentShotCooldown => shotCooldownOverride ?? defaultShotCooldown;

    private float lastShotTime;

    [Header("Enfriamiento de disparo")]
    [SerializeField] private float maxHeat;
    private float currentHeat;

    [SerializeField] private float defaultHeatPerShot;
    private float? heatPerShotOverride = null;
    private float CurrentHeatPerShot => heatPerShotOverride ?? defaultHeatPerShot;

    [SerializeField] private float heatDecayRate;

    [Header("Audios")]
    public AudioClip shootsound;
    [HideInInspector] public AudioSource audioSource;

    private void Awake()
    {
        playerManager = GetComponent<Player>();
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if (!playerManager.photonView.IsMine) return;
        UpdateHeat();

        if (!playerManager.CanMove) return;
        HandleInput();
    }
    private void FixedUpdate()
    {
        if (!CanControlPlayer()) return;

        HandleRotation();
        HandlePropulsion();
    }

    #region INPUTS
    private void HandleInput()
    {
        joystickAxis = UIGameManager.Instance.virtualJoystick.GetAxis();
        targetAngle = joystickAxis.x >= 0
            ? Vector2.Angle(Vector2.up, joystickAxis)
            : 360 - Vector2.Angle(Vector2.up, joystickAxis);
    }
    #endregion

    #region MOVIMIENTO
    private bool CanControlPlayer() => playerManager.CanMove && playerManager.photonView.IsMine;

    private void HandleRotation()
    {
        if (joystickAxis.magnitude < 0.1f)
        {
            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, rotationAcceleration * Time.fixedDeltaTime);
            return;
        }

        float targetRotation = -targetAngle;
        float currentRotation = transform.eulerAngles.z;
        float angleDifference = Mathf.DeltaAngle(currentRotation, targetRotation);

        float torque = Mathf.Clamp(angleDifference * rotationAcceleration, -maxRotationSpeed, maxRotationSpeed);
        rb.AddTorque(Time.fixedDeltaTime * torque * transform.forward, ForceMode.VelocityChange);
    }

    private void HandlePropulsion()
    {
        rb.AddForce(UIGameManager.Instance.speedSlider.value * Time.fixedDeltaTime * transform.up, ForceMode.VelocityChange);

        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(maxPropulsionSpeed * Time.fixedDeltaTime * transform.up, ForceMode.VelocityChange);
        }

        var propulsionSpeed = rb.velocity.magnitude;
        playerManager.spaceship.SetPropulsion(propulsionSpeed);
        playerManager.photonView.RPC(nameof(RPC_SyncTrail), RpcTarget.All, propulsionSpeed);
    }

    [PunRPC]
    private void RPC_SyncTrail(float propulsionSpeed)
    {
        playerManager.spaceship.DisplayTrail(propulsionSpeed);
    }

    public void Stop()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        UIGameManager.Instance.speedSlider.value = 0;

        playerManager.spaceship.SetPropulsion(rb.velocity.magnitude);
    }
    #endregion

    #region DISPARO
    private bool CanShoot => playerManager.CanShoot &&
                               !playerManager.IsOverheat &&
                               Time.time >= lastShotTime + defaultShotCooldown;

    public void SetHeatPerShotOverride(float value) => heatPerShotOverride = value;
    public void ResetHeatPerShot() => heatPerShotOverride = null;

    public void SetShotCooldownOverride(float value) => shotCooldownOverride = value;
    public void ResetShotCooldownt() => shotCooldownOverride = null;

    public void Fire()
    {
        if (!CanShoot) return;

        playerManager.CanShoot = false;

        playerManager.photonView.RPC(nameof(Shoot), RpcTarget.AllViaServer, rb.rotation);
        lastShotTime = Time.time;

        audioSource.PlayOneShot(shootsound);

        StartCoroutine(ResetShootCooldown());
    }

    private IEnumerator ResetShootCooldown()
    {
        yield return new WaitForSeconds(CurrentShotCooldown);
        playerManager.CanShoot = true;
    }

    private void UpdateHeat()
    {
        if (playerManager.IsOverheat)
        {
            currentHeat -= heatDecayRate * 1.5f * Time.deltaTime;

            if (currentHeat <= 0)
            {
                currentHeat = 0;
                playerManager.IsOverheat = false;
                UIGameManager.Instance.shootBTN.interactable = true;
            }
        }
        else
        {
            currentHeat = Mathf.Max(currentHeat - heatDecayRate * Time.deltaTime, 0);
            if (currentHeat >= maxHeat)
            {
                playerManager.IsOverheat = true;
                UIGameManager.Instance.shootBTN.interactable = false;
            }
        }

        UIGameManager.Instance.shootButtonFill.fillAmount = currentHeat / maxHeat;
    }

    //[PunRPC]
    //public void Shoot(Quaternion rotation, PhotonMessageInfo info)
    //{
    //    float lag = (float)(PhotonNetwork.Time - info.SentServerTime);
    //    Transform spawn = playerManager.spaceship.shotSpawn;

    //    switch (playerManager.playerStats.ShootType)
    //    {
    //        case ShootType.DEFAULT:
    //            CreateShot(spawn.position, rotation * Quaternion.Euler(0, 0, 90), shotForce, lag);
    //            break;

    //        case ShootType.PIERCING:
    //            CreateShot(spawn.position, rotation * Quaternion.Euler(0, 0, 90), shotForce, lag, piercing: true);
    //            break;

    //        case ShootType.MACHINE_GUN:
    //            StartCoroutine(MachineGunBurst(rotation, lag));
    //            break;

    //        case ShootType.SHOTGUN:
    //            FireShotgun(spawn.position, rotation, lag);
    //            break;
    //    }

    //    currentHeat += CurrentHeatPerShot;
    //    playerManager.playerStats.IncrementShotsFired();
    //}

    //private void CreateShot(Vector3 position, Quaternion rotation, float force, float lag, bool piercing = false)
    //{
    //    GameObject shot = Instantiate(playerManager.spaceship.shotPrefab, position, rotation);
    //    playerManager.spaceship.SetCustomizationForShot(shot, UserSession.ShotColor, UserSession.ShotSkin);
    //    var shotScript = shot.GetComponent<PlayerShot>();
    //    shotScript.InitializeBullet(playerManager, force, piercing, Mathf.Abs(lag));
    //}

    [PunRPC]
    public void Shoot(Quaternion rotation, PhotonMessageInfo info)
    {
        float lag = (float)(PhotonNetwork.Time - info.SentServerTime);
        Transform spawn = playerManager.spaceship.shotSpawn;

        int shotID = PhotonNetwork.IsMasterClient
        ? PlayerManager.Instance.GenerateShotID()
        : -1;

        if (PhotonNetwork.IsMasterClient)
        {
            playerManager.photonView.RPC(nameof(CreateShotRPC), RpcTarget.All, 
                spawn.position,
                rotation * Quaternion.Euler(0, 0, 90), 
                shotForce, 
                lag, 
                shotID,
                false);
        }

        //switch (playerManager.playerStats.ShootType)
        //{
        //    case ShootType.DEFAULT:
        //        CreateShot(spawn.position, 
        //            rotation * Quaternion.Euler(0, 0, 90), 
        //            shotForce, 
        //            lag, 
        //            shotID,
        //            piercing: true);
        //        break;

        //    case ShootType.PIERCING:
        //        CreateShot(spawn.position, rotation * Quaternion.Euler(0, 0, 90), shotForce, lag, piercing: true);
        //        break;

        //    case ShootType.MACHINE_GUN:
        //        StartCoroutine(MachineGunBurst(rotation, lag));
        //        break;

        //    case ShootType.SHOTGUN:
        //        FireShotgun(spawn.position, rotation, lag);
        //        break;
        //}

        currentHeat += CurrentHeatPerShot;
        playerManager.playerStats.IncrementShotsFired();
    }

    [PunRPC]
    public void CreateShotRPC(Vector3 position, Quaternion rotation, float force, float lag, int shotID, bool piercing)
    {
        CreateShot(position, rotation, force, lag, shotID, piercing);
    }

    public void CreateShot(Vector3 position, Quaternion rotation, float force, float lag, int shotID, bool piercing)
    {
        GameObject shot = Instantiate(playerManager.spaceship.shotPrefab, position, rotation);
        var shotScript = shot.GetComponent<PlayerShot>();

        shotScript.InitializeBullet(playerManager, force, piercing, Mathf.Abs(lag), shotID);
        playerManager.spaceship.SetCustomizationForShot(shot, UserSession.ShotColor, UserSession.ShotSkin);
    }

    //private IEnumerator MachineGunBurst(Quaternion rotation, float lag)
    //{
    //    Transform spawn = playerManager.spaceship.shotSpawn;
    //    int burstCount = 3;
    //    float interval = 0.05f;

    //    for (int i = 0; i < burstCount; i++)
    //    {
    //        CreateShot(spawn.position, rotation * Quaternion.Euler(0, 0, 90), shotForce * 0.8f, lag);
    //        yield return new WaitForSeconds(interval);
    //    }
    //}

    //private void FireShotgun(Vector3 position, Quaternion rotation, float lag)
    //{
    //    int pellets = 3;
    //    float spreadAngle = 15f;

    //    for (int i = 0; i < pellets; i++)
    //    {
    //        float angleOffset = Random.Range(-spreadAngle, spreadAngle);
    //        Quaternion spreadRot = rotation * Quaternion.Euler(0, 0, angleOffset + 90);
    //        CreateShot(position, spreadRot, shotForce * 0.9f, lag);
    //    }
    //}
    #endregion
}
