using System.Collections;
using Terresquall;
using UnityEngine;
using UnityEngine.UI;

// ---> Clase con todas las acciones que puede hacer el jugador
public class PlayerActions : MonoBehaviour
{
    public PlayerManager playerManager;

    [Header("Componentes")]
    private Rigidbody rb;

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
    public GameObject shotOBJ;
    public GameObject shotSpawn;
    public GameObject shotStorage;

    [Header("Interfaz")]
    public VirtualJoystick rotationJoystick;
    public Slider velocitySlider;
    public Image heatBar;
    public Button shootButton;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerManager = GetComponent<PlayerManager>();
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
        if (ComprobarPosicion() != null) { TransportarJugador(); }
    }
    private void FixedUpdate()
    {
        if (playerManager.canMove)
        {
            Movement();
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
    void TransportarJugador()
    {
        switch (ComprobarPosicion())
        {
            case "arriba":
                transform.position = new Vector3(transform.position.x, GameController.instance.bottomLeftBorder.y, transform.position.z);
                break;
            case "abajo":
                transform.position = new Vector3(transform.position.x, GameController.instance.topRightBorder.y, transform.position.z);
                break;
            case "derecha":
                transform.position = new Vector3(GameController.instance.bottomLeftBorder.x, transform.position.y, transform.position.z);
                break;
            case "izquierda":
                transform.position = new Vector3(GameController.instance.topRightBorder.x, transform.position.y, transform.position.z);
                break;
            default:
                break;
        }
    }
    string ComprobarPosicion()
    {
        float margin = 5;

        if (transform.position.y > GameController.instance.topRightBorder.y + margin)
        {
            return ("arriba");
        }
        else if (transform.position.y < GameController.instance.bottomLeftBorder.y - margin)
        {
            return ("abajo");
        }
        else if (transform.position.x > GameController.instance.topRightBorder.x + margin)
        {
            return ("derecha");
        }
        else if (transform.position.x < GameController.instance.bottomLeftBorder.x - margin)
        {
            return ("izquierda");
        }
        else { return null; }
    }

    // ---> Disparo 
    public void Fire() // ---> Se asigna al botón de disparar por código en el PlayerManager
    {
        if (playerManager.canShoot && Time.time >= lastShotTime + shotCooldown)
        {
            Shoot();  // Se llama al método Shoot para crear el disparo
            playerManager.canShoot = false;  // Se bloquea el disparo hasta que pase el cooldown
            lastShotTime = Time.time;  // Se guarda el tiempo del disparo
            StartCoroutine(ResetShootCooldown());
        }
    }
    public void Shoot()
    {
        GameObject tempShot = Instantiate(shotOBJ, shotSpawn.transform.position, Quaternion.identity, shotStorage.transform);
        tempShot.GetComponent<PlayerShoot>().Ownername = playerManager.username;
        Rigidbody rb = tempShot.GetComponent<Rigidbody>();
        tempShot.transform.rotation = transform.rotation * Quaternion.Euler(0, 0, 90);
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.AddForce(transform.up * shotForce, ForceMode.Impulse);

        shotHeat += 25;

        Destroy(tempShot, 5f);
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

        //shotHeat -= 33 * Time.deltaTime;
        //if (shotHeat < 0)
        //{
        //    shotHeat = 0;
        //}
    }

    // Código de disparo antiguo
    //public void Disparar()
    //{
    //    if (playerManager.canShoot)
    //    {
    //        //GameObject tempShot = Instantiate(shotOBJ, shotSpawn.transform.position, Quaternion.identity, shotStorage.transform);
    //        //Quaternion tempRotation = this.transform.rotation;
    //        //tempRotation.x = tempRotation.x + 90;
    //        //tempShot.transform.rotation = Quaternion.LookRotation(this.transform.up, this.transform.forward * -1);
    //        //tempShot.GetComponent<Rigidbody>().AddForce(tempShot.gameObject.transform.forward * 80, ForceMode.Impulse);
    //        //shotHeat += 25;
    //        //StartCoroutine(EnfriamientoDisparo());

    //        GameObject tempShot = Instantiate(shotOBJ, shotSpawn.transform.position, Quaternion.identity, shotStorage.transform);
    //        tempShot.GetComponent<PlayerShoot>().Ownername = playerManager.username;
    //        Rigidbody rb = tempShot.GetComponent<Rigidbody>();
    //        tempShot.transform.rotation = transform.rotation * Quaternion.Euler(0, 0, 90);
    //        rb.constraints = RigidbodyConstraints.FreezeRotation;
    //        rb.AddForce(transform.up * 80, ForceMode.Impulse);
    //        shotHeat += 25;
    //        StartCoroutine(EnfriamientoDisparo());
    //    }
    //}
    //public IEnumerator EnfriamientoDisparo()
    //{
    //    playerManager.canShoot = false;
    //    yield return new WaitForSeconds(0.5f);
    //    playerManager.canShoot = true;
    //}
}
