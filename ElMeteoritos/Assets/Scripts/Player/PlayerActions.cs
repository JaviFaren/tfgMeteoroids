using Photon.Pun;
using System.Collections;
using Terresquall;
using UnityEngine;
using UnityEngine.UI;

// ---> Clase con todas las acciones que puede hacer el jugador
public class PlayerActions : MonoBehaviour
{
    [HideInInspector] public PlayerManager playerManager;

    [Header("Componentes")]
    private Rigidbody rb;
    public PhotonView view;

    [Header("Rotacion")]
    public Vector2 joystickAxis;
    public float targetAngle;
    public float currentAngle;
    public float maxRotationSpeed;
    public float currentRotationSpeed;

    [Header("Propulsion")]
    public float propulsionSpeed;
    public float acceleration;

    [Header("Disparo")]
    public float shotForce;  // Fuerza del disparo
    public float shotCooldown;
    private float lastShotTime;
    [Range(0f, 100f)]
    public float shotHeat;
    public bool isOverheat = false; // ---> De momento no se usa.
    //public bool shotCooldown = false;

    [Header("Interfaz")]
    public VirtualJoystick rotationJoystick;
    public Slider velocitySlider;
    public Image heatBar;
    public Button shootButton;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerManager = GetComponent<PlayerManager>();
        view = GetComponent<PhotonView>();
    }
    private void Start()
    {

    }
    private void Update()
    {
        if (playerManager.canMove)
        {
            joystickAxis = rotationJoystick.GetAxis();
            if (joystickAxis.x >= 0)
            {
                targetAngle = Vector2.Angle(Vector2.up, joystickAxis);
            }
            else
            {
                targetAngle = 360 - Vector2.Angle(Vector2.up, joystickAxis);
            }
        }

        SobrecalentamientoDisparo();
        if (GameController.instance.CheckPosition(gameObject) != null) { TransportarJugador(); }
    }
    private void FixedUpdate()
    {
        if (playerManager.canMove && view.IsMine)
        {
            Movement();
        }
        else
        {
            
        }
    }

    public void Movement()
    {
        PCMovement();

        Rotation();
        Propulsion();
    }

    // ---> Rotación de la nave
    public void Rotation()
    {
        if (targetAngle != 0)
        {
            currentAngle = transform.eulerAngles.z;
            float angleDifference = Mathf.DeltaAngle(currentAngle, -targetAngle);
            if (Mathf.Abs(angleDifference) > 0.1f)
            {
                currentRotationSpeed = Mathf.MoveTowards(currentRotationSpeed, maxRotationSpeed, acceleration);
            }
            else
            {
                currentRotationSpeed = Mathf.MoveTowards(currentRotationSpeed, 0f, acceleration);
            }

            float rotationStep = Mathf.Sign(angleDifference) * currentRotationSpeed;

            if (Mathf.Abs(rotationStep) > Mathf.Abs(angleDifference))
            {
                rotationStep = angleDifference;
            }
            rb.AddTorque(this.transform.forward * rotationStep, ForceMode.Acceleration);

            if (Mathf.Abs(angleDifference) < 0.5f)
            {
                rb.angularVelocity = Vector3.zero;
            }
        }
        
    }

    // ---> Propulsión de la nave
    public void Propulsion()
    {
        rb.AddForce(transform.up * velocitySlider.value, ForceMode.Acceleration);
    }
    public void PCMovement()
    {
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(transform.up * propulsionSpeed, ForceMode.Acceleration);
        }
    }

    // ---> Límites de pantalla
    [PunRPC]
    void Teleport(Vector3 newPosition)
    {
        var photonTransformView = GetComponent<PhotonTransformView>();
        if (photonTransformView != null)
            photonTransformView.enabled = false;

        transform.position = newPosition;

        if (photonTransformView != null)
            photonTransformView.enabled = true;
    }

    void TransportarJugador()
    {
        if (!playerManager.phView.IsMine) return; // Solo el jugador local debe hacer la comprobación

        Vector3 newPosition = transform.position;

        switch (GameController.instance.CheckPosition(gameObject))
        {
            case "above":
                newPosition = new Vector3(transform.position.x, GameController.instance.bottomLeftBorder.y, transform.position.z);
                break;
            case "below":
                newPosition = new Vector3(transform.position.x, GameController.instance.topRightBorder.y, transform.position.z);
                break;
            case "right":
                newPosition = new Vector3(GameController.instance.bottomLeftBorder.x, transform.position.y, transform.position.z);
                break;
            case "left":
                newPosition = new Vector3(GameController.instance.topRightBorder.x, transform.position.y, transform.position.z);
                break;
            default:
                return; // No cambio, salimos
        }

        playerManager.phView.RPC("Teleport", RpcTarget.All, newPosition);
    }

    // ---> Disparo 
    public void Fire() // ---> Se asigna al botón de disparar por código en el PlayerManager
    {
        if (playerManager.canShoot && Time.time >= lastShotTime + shotCooldown)
        {
            playerManager.phView.RPC("Shoot", RpcTarget.All);  // Se llama al método Shoot para crear el disparo
            playerManager.canShoot = false;  // Se bloquea el disparo hasta que pase el cooldown
            lastShotTime = Time.time;  // Se guarda el tiempo del disparo
            StartCoroutine(ResetShootCooldown());
        }
    }

    [PunRPC]
    public void Shoot()
    {
        GameObject tempShot = Instantiate(playerManager.spaceship.shotPrefab, playerManager.spaceship.shotSpawn.position, Quaternion.identity, GameController.instance.shot_Container.transform);
        tempShot.GetComponent<PlayerShoot>().damage = playerManager.playerStats.shootDamage; // Se asigna el dano del disparo 
        tempShot.GetComponent<PlayerShoot>().playerID = playerManager.userID; // Se asigna el id del usuario que ha realizado el disparo
        Rigidbody rb = tempShot.GetComponent<Rigidbody>();
        tempShot.transform.rotation = transform.rotation * Quaternion.Euler(0, 0, 90);
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.AddForce(transform.up * shotForce, ForceMode.Impulse);

        shotHeat += 25;

        if (tempShot)
        {
            Destroy(tempShot, 4f);
        }
    }

    IEnumerator ResetShootCooldown()
    {
        yield return new WaitForSeconds(shotCooldown);
        playerManager.canShoot = true;
    }
    public void SobrecalentamientoDisparo()
    {
        shotHeat = Mathf.Max(shotHeat - 33 * Time.deltaTime, 0);
        heatBar.fillAmount = shotHeat / 100f;
    }
    [PunRPC]
    public void setupPlayer(Vector3 pos)
    {
        transform.position = pos;
        //PlayerUIManager.instance.InitializePlayersPanel();
    }
    [PunRPC]
    public void addPlayer()
    {
        GameController.instance.AddPlayerToPlayersList(playerManager);
    }
}
