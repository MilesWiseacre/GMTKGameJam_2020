using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_Axis
{
    public float roll;
    public float pitch;
    public float yaw;
    public float thrust;
}

public class Player : MonoBehaviour
{
    public Movement_Axis axis;

    float axisRoll, axisPitch, axisYaw, axisThrust;

    private GameManager gm;

    private Rigidbody rb;

    private GameObject cam;

    private UIManager ui;

    private float rollWeight, pitchWeight, yawWeight, thrustWeight;

    public float adjustInput_01 = 20;

    public float adjustInput_02 = 30;

    public float adjustInput_03 = 40;

    private float zRotation, yRotation, xRotation;

    private Quaternion currentRotation;

    private float lStickInputH, lStickInputV, rStickInputH, rStickInputV, dPadH, dPadV, faceH, faceV, bumpersInput, triggersInput;

    private string lSideInput, rSideInput, rollInput, pitchInput, yawInput, thrustInput;

    private string currentRoll, currentPitch, currentYaw, currentThrust;

    private float rolling, pitching, yawing, thrusting;

    private float runTime;

    public AudioClip warning;

    [SerializeField]
    private GameObject environment = null;

    void Start()
    {
        axis = new Movement_Axis();
        axis.roll = 0f;
        axis.pitch = 0f;
        axis.yaw = 0f;
        axis.thrust = 10f;

        rb = GetComponent<Rigidbody>();

        gm = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        cam = Camera.main.gameObject;
        ui = GameObject.FindWithTag("UI").GetComponent<UIManager>();
    }

    public void ResetGame()
    {
        rb.useGravity = false;
        ui.SetUI(0);
        StopCoroutine(ShuffleInput());
        runTime = 0.00f;
        transform.position = new Vector3(transform.position.x, 0, 0);
        Quaternion reset = Quaternion.Euler(0, 0, 0);
        transform.rotation = reset;
        if (environment != null)
        {
            environment.transform.position = new Vector3(90, 0, 0);
        }
    }

    public void StartGame()
    {
        StartCoroutine(ShuffleInput());
    }

    void Update()
    {
        if (gm.IsGameOver == false)
        {
            if (rb.useGravity == false)
            {
                rb.useGravity = true;
            }
            currentRotation = transform.rotation;
            runTime += Time.deltaTime;
            ui.SetScore(runTime);
            if (currentRoll != null && currentPitch != null && currentYaw != null && currentThrust != null)
            {
                PassInputs(currentRoll, currentPitch, currentYaw, currentThrust);
            }
            EstablishAxis();
            MovementByAxis();
        } else
        {
            rb.velocity = transform.right * 10 * Time.deltaTime;
        }
    }
    // Plugs the input in using the strings
    void PassInputs(string input1, string input2, string input3, string input4)
    {
        rolling = Input.GetAxis(input1);
        pitching = Input.GetAxis(input2);
        yawing = Input.GetAxis(input3);
        thrusting = Input.GetAxis(input4);
    }

    // Prevents excessive rotation and throws on the delta time to the movement axis
    void EstablishAxis()
    {
        if ((currentRotation.x >= .5 && rolling > 0) || (currentRotation.x <= -.5 && rolling < 0))
        {
            rolling = 0;
        }
        axis.roll = rolling * adjustInput_03 * rollWeight * Time.deltaTime;
        if (transform.position.y >= 1)
        {
            if ((currentRotation.z >= 0 && pitching > 0))
            {
                pitching = 0;
            }
        } else
        {
            if ((currentRotation.z >= .3 && pitching > 0) || (currentRotation.z <= -.3 && pitching < 0))
            {
                pitching = 0;
            }
        }
        axis.pitch = (pitching - .5f) * adjustInput_01 * pitchWeight * Time.deltaTime;
        if ((currentRotation.y >= .5 && yawing > 0) || (currentRotation.y <= -.5 && yawing < 0))
        {
            yawing = 0;
        }
        axis.yaw = yawing * adjustInput_02 * yawWeight * Time.deltaTime;
        if ((axis.thrust >= 20 && thrusting > 0.1f) || (axis.thrust <= 5 && thrusting < -0.1f))
        {
            thrusting = 0;
        }
        axis.thrust += thrusting * thrustWeight * Time.deltaTime;
    }

    // Translates the movement from the movement axis into the rigidbody
    void MovementByAxis()
    {
        rb.velocity = transform.right * axis.thrust;
        //rb.AddRelativeTorque(axis.roll, axis.yaw, axis.pitch);
        rb.AddTorque(axis.roll, axis.yaw, axis.pitch);
    }
    // Reconfigures which movement axis is applied to which force
    IEnumerator ShuffleInput()
    {
        while (gm.IsGameOver == false)
        {
            if (runTime >= 10f)
            {
                ClearInput();

                DecideSide();

                DecideInput();

                DecideWeight();

                AudioSource.PlayClipAtPoint(warning, transform.position);
                //Debug.Log("Controls shuffled");
                yield return new WaitForSeconds(10.0f);
            }
            else
            {
                ClearInput();

                currentRoll = "LStick_H";
                currentPitch = "RStick_V";
                currentYaw = "Bumpers";
                currentThrust = "Triggers";
                Debug.Log("Default controls");
                yield return new WaitForSeconds(10.0f);
            }
        }
    }

    // Resets the inputs so they can be set again
    void ClearInput()
    {
        currentRoll = null;
        currentPitch = null;
        currentYaw = null;
        currentThrust = null;
        rollWeight = 1;
        pitchWeight = 1;
        yawWeight = 1;
        thrustWeight = 1;
    }

    // Decides what direction the sticks, dpad, or face buttons will have to be used
    void DecideSide()
    {
        // If harder difficulty, determine wheter LStick/Dpad or RStick/Face will be used
        bool lSide = false;
        bool rSide = false;
        if (gm.Hard)
        {
            float lSideDecide = Random.Range(0, 1);
            if (lSideDecide == 0)
            {
                lSide = false;
            }
            else if (lSideDecide == 1)
            {
                lSide = true;
            }
            float rSideDecide = Random.Range(0, 1);
            if (rSideDecide == 0)
            {
                rSide = false;
            }
            else if (rSideDecide == 1)
            {
                rSide = true;
            }
        }

        // Determine whether horizontal or vertical inputs will be used
        float lDecide = Random.Range(0, 1);
        if (lDecide == 0)
        {
            if (!lSide)
            {
                lSideInput = "LStick_H";
            }
            else
            {
                lSideInput = "DPad_H";
            }
        }
        else if (lDecide == 1)
        {
            if (!lSide)
            {
                lSideInput = "LStick_V";
            }
            else
            {
                lSideInput = "DPad_V";
            }
        }
        float rDecide = Random.Range(0, 1);
        if (rDecide == 0)
        {
            if (!rSide)
            {
                rSideInput = "RStick_H";
            }
            else
            {
                rSideInput = "Face_H";
            }
        }
        else if (rDecide == 1)
        {
            if (!rSide)
            {
                rSideInput = "RStick_V";
            }
            else
            {
                rSideInput = "Face_V";
            }
        }
    }

    // Determine what input is going in to LStick, RStick, Bumpers, or Triggers
    void DecideInput()
    {
        int axisDecide = 4;
        while (axisDecide > 0)
        {
            float randomInput = Random.Range(1, axisDecide);
            switch (axisDecide)
            {
                case 4:
                    if (randomInput == 1)
                    {
                        currentRoll = lSideInput;
                    }
                    else if (randomInput == 2)
                    {
                        currentPitch = lSideInput;
                    }
                    else if (randomInput == 3)
                    {
                        currentYaw = lSideInput;
                    }
                    else if (randomInput == 4)
                    {
                        currentThrust = lSideInput;
                    }
                    break;
                case 3:
                    if (randomInput == 1)
                    {
                        if (currentRoll == null)
                        {
                            currentRoll = rSideInput;
                        }
                        else
                        {
                            currentThrust = rSideInput;
                        }
                    }
                    else if (randomInput == 2)
                    {
                        if (currentPitch == null)
                        {
                            currentPitch = rSideInput;
                        }
                        else
                        {
                            currentThrust = rSideInput;
                        }
                    }
                    else if (randomInput == 3)
                    {
                        if (currentYaw == null)
                        {
                            currentYaw = rSideInput;
                        }
                        else
                        {
                            currentThrust = rSideInput;
                        }
                    }
                    break;
                case 2:
                    if (randomInput == 1)
                    {
                        if (currentRoll == null)
                        {
                            currentRoll = "Bumpers";
                        }
                        else
                        {
                            if (currentPitch == null)
                            {
                                currentPitch = "Bumpers";
                            }
                            else
                            {
                                if (currentYaw == null)
                                {
                                    currentYaw = "Bumpers";
                                }
                            }
                        }
                    }
                    else if (randomInput == 2)
                    {
                        if (currentYaw == null)
                        {
                            currentYaw = "Bumpers";
                        }
                        else
                        {
                            if (currentThrust == null)
                            {
                                currentThrust = "Bumpers";
                            }
                            else
                            {
                                if (currentPitch == null)
                                {
                                    currentPitch = "Bumpers";
                                }
                            }
                        }
                    }
                    break;
                case 1:
                    if (currentRoll == null)
                    {
                        currentRoll = "Triggers";
                    }
                    else if (currentPitch == null)
                    {
                        currentPitch = "Triggers";
                    }
                    else if (currentYaw == null)
                    {
                        currentYaw = "Triggers";
                    }
                    else if (currentThrust == null)
                    {
                        currentThrust = "Triggers";
                    }
                    break;
                default:
                    break;
            }
            axisDecide--;
        }
    }

    // Determine which axis has a Weight on it
    void DecideWeight()
    {
        float weightDecide = Random.Range(0, 3);
        switch (weightDecide)
        {
            case 0:
                rollWeight = 2;
                float rollflip = Random.Range(0, 1);
                if (rollflip == 0)
                {
                    rollWeight = rollWeight * -1;
                    ui.SetUI(4);
                }
                break;
            case 1:
                pitchWeight = -.5f;
                ui.SetUI(3);
                break;
            case 2:
                yawWeight = 2;
                float yawflip = Random.Range(0, 1);
                if (yawflip == 0)
                {
                    yawWeight = yawWeight * -1;
                    ui.SetUI(1);
                }
                break;
            case 3:
                thrustWeight = -2;
                ui.SetUI(2);
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Fail")
        {
            gm.GameOver();
            ResetGame();
            ClearInput();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
