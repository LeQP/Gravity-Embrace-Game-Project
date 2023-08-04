using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{

    private Rigidbody rb;
    public float movSpd = 1f;
    public float groundSpd = 10f;
    public float gravity = 5f;
    private float gravityAmt = 0f;
    public float airSpd = 2f;
    public float turnSpd = 1f;
    public float jumpHeight = 10f;
    private float movHor;
    private float movVer;
    Vector3 movVec;

    private Vector3 orientVec = Vector3.down;
    public GravityShift gs;

    private bool doJump = false;
    private bool stopJump = false;
    private bool startTimer = false;
    public float jumpTimer = 0.5f;
    private float timer = 0f;
    private bool doForce = false;
    private bool noForce = true;

    public float forceMass = 1;
    private float forceFact = 1f;
    private float norMass;

    private bool key = true;
    public Transform vCamera;
    private bool shiftCamera = false;

    /*
    axisState is an array used to decide the player's movement in regards to the world's axis values.
    Since the player can walk on walls, it changes how the player moves along the 3 axises with the controls.
    To keep track of which axises and direction (+ or -), each index of the array corresponds to the three axises (x, y, and z)
    and have a value 0-4 to indicate how that axis moves.

    Indexes
    0 = Correspond to the x axis
    1 = Correspond to the y axis
    2 = Correspond to the z axis

    Values
    0 = the player's position increases in axis value when the player presses up (Horizontal movement).
    1 = the player's position decreases in axis value when the player presses right (Horizontal movement).
    2 = the player's position increases in axis value when the player presses up (Vertical movement).
    3 = the player's position decreases in axis value when the player presses right (Vertical movement).
    4 = the axis is not in used

    */
    private int[] axisState;
    private bool[] axisFreezeRot;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        norMass = rb.mass;
        gravityAmt = gravity;
        timer = jumpTimer;
        orientVec = gs.getOrientation();
        axisState = new int[3];
        axisState[0] = 0;
        axisState[1] = 4;
        axisState[2] = 2;
        axisFreezeRot = new bool[3];
        axisFreezeRot[0] = true;
        axisFreezeRot[1] = false;
        axisFreezeRot[2] = true;
        //rotVec = new Vector3(0f, 0.0001f, 0f);

    }

    public void pushMov()
    {
        movVec = orientMove(0f, 0.1f);
        shiftCamera = true;
    }

    public void OnMove(InputValue input)
    {
        Vector2 inputVec = input.Get<Vector2>();
        movVec = orientMove(inputVec.x, inputVec.y);
    }

    public void setAxisState(int index, int state)
    {
        axisState[index] = state;
        if (state == 4)
        {
            axisFreezeRot[index] = false;
        }
        else
        {
            axisFreezeRot[index] = true;
        }
    }

    public void keyLock()
    {
        key = false;
    }

    public void keyUnlock()
    {
        key = true;
    }

    public void readAxisState()
    {
        Debug.Log("X = " + axisState[0]);
        Debug.Log("Y = " + axisState[1]);
        Debug.Log("Z = " + axisState[2]);
    }

    private float readAxisState(int axisNum, float movHor, float movVer)
    {
        if (axisNum == 0)
            return movHor;
        else if (axisNum == 1)
            return -movHor;
        else if (axisNum == 2)
            return movVer;
        else if (axisNum == 3)
            return -movVer;
        else
            return 0f;
    }
    Vector3 orientMove(float movHor, float movVer)
    {
        float x = readAxisState(axisState[0], movHor, movVer);
        float y = readAxisState(axisState[1], movHor, movVer);
        float z = readAxisState(axisState[2], movHor, movVer);
        return new Vector3(x, y, z);
    }

    void performJump()
    {
        gravityAmt = 0f;
        rb.AddForce(orientVec * -1 * jumpHeight, ForceMode.Impulse);
    }
    void releaseJump()
    {
        gravityAmt = gravity;
    }
    void jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            doJump = true;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            stopJump = true;
        }
        if (startTimer)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                stopJump = true;
            }
        }
    }
    void forceGravity()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            doForce = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            doForce = false;
        }
    }

    public bool GroundCheck()
    {
        // Approach ground check with using a ray through a RayCast
        RaycastHit hit;
        bool groundTest = Physics.Raycast(transform.position + (orientVec * -1), orientVec, out hit, 1.13f);
        if (groundTest)
            return true;
        else
            return false;
    }

    void Update()
    {
        jump();
        forceGravity();
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            readAxisState();
            Debug.Log("movVec = " + movVec);
            Debug.Log("orientVec * -1 = " + orientVec * -1);
        }
    }

    void Debugger(string quote, Vector3 a, Vector3 b)
    {
        Debug.Log(quote + ": " + Vector3.Cross(a, b));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (key)
        {
            bool isGround = GroundCheck();
            float modSpd = 0f;

            if (isGround)
                modSpd = groundSpd;
            else
                modSpd = airSpd;
            orientVec = gs.getOrientation();
            rb.AddForce(movVec * movSpd * modSpd * rb.mass * forceFact);
            Debug.Log(movSpd * modSpd * rb.mass * forceFact);
            if (movVec != Vector3.zero)
            {
                Quaternion newRot = Quaternion.LookRotation(movVec, orientVec * -1);
                transform.rotation = newRot;
            }
            if (shiftCamera)
            {
                vCamera.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
                shiftCamera = false;
            }
            if (doJump && isGround)
            {
                performJump();
                startTimer = true;
                doJump = false;
            }
            if (stopJump)
            {
                releaseJump();
                timer = jumpTimer;
                startTimer = false;
                stopJump = false;
            }
            if (doForce)
            {
                forceFact = 1.7f;
                rb.mass = forceMass;
                rb.AddForce(orientVec * gravity * forceMass * 3);
                //doForce = false;
            }
            if (!doForce)
            {
                forceFact = 1f;
                rb.mass = norMass;
            }
            rb.AddForce(orientVec * gravityAmt);
        }
    }


}
