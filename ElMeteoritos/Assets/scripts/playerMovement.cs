using System.Collections;
using System.Collections.Generic;
using Terresquall;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class playerMovement : MonoBehaviour
{
    public GameObject player;
    private Rigidbody rb;
    public VirtualJoystick joystick;
    public Vector2 axis;
    public float angle;

    public float propulsionSpeed;
    public float MaxRotationSpeed;
    public float acceleration;
    public float currentSpeed;
    public Slider sliderVelocity;

    public float currentAngle;

    [Range(0f, 100f)]
    public float shotHeat;
    public bool isOverheat;
    public bool shotCooldown;
    public Image heatBar;
    public GameObject shotOBJ;
    public GameObject shotSpawn;
    public GameObject shotStorage;

    private void Awake()
    {
        rb = player.GetComponent<Rigidbody>();
    }
    void Start()
    {
        propulsionSpeed = 25;
        MaxRotationSpeed = 8;
        acceleration = 4;
        currentSpeed = 0;
        isOverheat = false;
        shotHeat = 0;
        shotCooldown = false;
    }

    void Update()
    {
        axis = joystick.GetAxis();
        if (axis.x >= 0)
        {
            angle = Vector2.Angle(Vector2.up, axis);
        }
        else
        {
            angle = 360 - Vector2.Angle(Vector2.up, axis);
        }

        //disparo();
        SobrecalentamientoDisparo();

        TransportarJugador();
    }

    private void FixedUpdate()
    {
        movement();
    }



    public void movement()
    {
        PCMovement();

        mobileMovement();
        propulsion();
    }

    public void PCMovement()
    {
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(player.transform.up * propulsionSpeed, ForceMode.Acceleration);
        }
    }

    public void mobileMovement()
    {
        if(angle != 0)
        {
            currentAngle = transform.eulerAngles.z;
            float angleDifference = Mathf.DeltaAngle(currentAngle, -angle);
            if (Mathf.Abs(angleDifference) > 0.1f)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, MaxRotationSpeed, acceleration);
            }
            else
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, acceleration);
            }

            float rotationStep = Mathf.Sign(angleDifference) * currentSpeed;

            if (Mathf.Abs(rotationStep) > Mathf.Abs(angleDifference))
            {
                rotationStep = angleDifference;
            }
            rb.AddTorque(this.transform.forward * rotationStep, ForceMode.Acceleration);
        }
    }
    public void propulsion()
    {
        rb.AddForce(player.transform.up * sliderVelocity.value, ForceMode.Acceleration);
    }

    // ---> Límites de pantalla
    void TransportarJugador()
    {
        switch (ComprobarPosicion())
        {
            case "arriba":
                player.transform.position = new Vector3(player.transform.position.x, controlPartida.instance.bottomLeft.y, player.transform.position.z);
                break;
            case "abajo":
                player.transform.position = new Vector3(player.transform.position.x, controlPartida.instance.topRight.y, player.transform.position.z);
                break;
            case "derecha":
                player.transform.position = new Vector3(controlPartida.instance.bottomLeft.x, player.transform.position.y, player.transform.position.z);
                break;
            case "izquierda":
                player.transform.position = new Vector3(controlPartida.instance.topRight.x, player.transform.position.y, player.transform.position.z);
                break;
            default:
                break;
        }
    }
    string ComprobarPosicion ()
    {
        float margin = 5;

        if (player.transform.position.y > controlPartida.instance.topRight.y + margin)
        {
            return ("arriba");
        }
        else if (player.transform.position.y < controlPartida.instance.bottomLeft.y - margin)
        {
            return ("abajo");
        }
        else if (player.transform.position.x > controlPartida.instance.topRight.x + margin)
        {
            return ("derecha");
        }
        else if (player.transform.position.x < controlPartida.instance.bottomLeft.x - margin)
        {
            return ("izquierda");
        }
        else { return null; }
    }

    //NO FUNCIONA EL ADDFORCE
    public void disparo()
    {
        if (!shotCooldown)
        {
            GameObject tempShot = Instantiate(shotOBJ, shotSpawn.transform.position, Quaternion.identity, shotStorage.transform);
            Quaternion tempRotation = this.transform.rotation;
            tempRotation.x = tempRotation.x + 90;
            tempShot.transform.rotation = Quaternion.LookRotation(this.transform.up, this.transform.forward * -1);
            tempShot.GetComponent<Rigidbody>().AddForce(tempShot.gameObject.transform.forward*80, ForceMode.Impulse);
            shotHeat += 25;
            StartCoroutine(shotCD());
        }
    }

    void SobrecalentamientoDisparo()
    {
        shotHeat -= 33 * Time.deltaTime;
        heatBar.fillAmount = shotHeat / 100f;

        if (shotHeat < 0)
        {
            shotHeat = 0;
        }
    }

    public IEnumerator shotCD()
    {
        shotCooldown = true;
        yield return new WaitForSeconds(0.5f);
        shotCooldown = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "enemigo")
        {
            Debug.Log("COSORRO, ME IDENTIFICO COMO CADAVER");
        }
    }
}
