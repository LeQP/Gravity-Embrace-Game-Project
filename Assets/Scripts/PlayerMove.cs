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
    //public float jumpHeight2;


    private float movHor;
    private float movVer;
    Vector3 movVec;

    private Vector3 orientVec = Vector3.down;

    private bool doJump = false;
    private bool stopJump = false;
    private bool startTimer = false;
    public float jumpTimer = 0.5f;
    private float timer = 0f;
    
    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        gravityAmt = gravity;
        timer = jumpTimer;
    }

    public void OnMove(InputValue input) {
        Vector2 inputVec = input.Get<Vector2>();
        movVec = orientMove(inputVec.x, inputVec.y);
    }

    Vector3 orientMove(float movHor, float movVer) {
        float x = 0f;
        float y = 0f;
        float z = 0f;
        if (orientVec == Vector3.down) {
            x = movHor;
            z = movVer;
        }
        else if (orientVec == Vector3.forward) {
            y = movVer;
            x = movHor;
        }
        else if (orientVec == Vector3.left) {
            y = movVer;
            z = movHor;
        }
        else if (orientVec == Vector3.right) {
            y = movHor;
            z = -movVer;
        }
        else if (orientVec == Vector3.forward) {
            y = movVer;
            x = -movHor;
        }
        if (orientVec == Vector3.up) {
            x = movHor;
            z = -movVer;
        }
        return new Vector3(x, y, z);
    }



    void performJump() {
        gravityAmt = 0f;
        rb.AddForce(orientVec * -1 * jumpHeight, ForceMode.Impulse);
    }
    void releaseJump() {
        gravityAmt = gravity;
    }
    void jump() {

        if (Input.GetKeyDown(KeyCode.Space)) {  
            Debug.Log("Jump");
            doJump = true;
        }

        if (Input.GetKeyUp(KeyCode.Space)) {
            Debug.Log("Gravity (Key Up)");
            stopJump = true;
        }

        if (startTimer) {
            Debug.Log("Timer Decrements");
            timer -= Time.deltaTime;
            if (timer <= 0) {
                Debug.Log("Gravity (Timer Ran Out)");
                stopJump = true;
            }
        }
    }

    bool GroundCheck() {
        // Approach ground check with using a ray through a RayCast
	    RaycastHit hit;
        bool groundTest = Physics.Raycast(transform.position + (orientVec * -1), orientVec, out hit, 1.13f);
	    if(groundTest) 
            return true;
	    else 
            return false;    
    }

    void Update() {
        Debug.DrawRay(transform.position + (orientVec * -1.012f), orientVec, Color.red);
        jump();
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            if (GroundCheck())
                Debug.Log("IsGrounded");
            else
                Debug.Log("Not Grounded");
        }
        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            /*
            Debugger("x+ and y+", Vector3.right, Vector3.up);
            Debugger("y+ and x+", Vector3.up, Vector3.right);
            Debug.Log("---");
            Debugger("y+ and z+", Vector3.up, Vector3.forward);
            Debugger("z+ and y+", Vector3.forward, Vector3.up);
            Debug.Log("---");
            Debugger("z+ and x+", Vector3.forward, Vector3.right);
            Debugger("x+ and z+", Vector3.right, Vector3.forward);
            Debug.Log("---");
            Debug.Log("---");
            Debugger("y+ and x+", Vector3.up, Vector3.right);
            Debugger("y+ and x-", Vector3.up, Vector3.left);
            Debug.Log("---");
            Debugger("y+ and z+", Vector3.up, Vector3.forward);
            Debugger("y+ and z-", Vector3.up, Vector3.back);
            Debug.Log("---");
            Debugger("z+ and x+", Vector3.forward, Vector3.right);
            Debugger("z+ and x-", Vector3.forward, Vector3.left);
            */
            /*
            Debugger("Floor = z+ and x-", Vector3.forward, Vector3.left);
            Debugger("Front = y+ and x-", Vector3.up, Vector3.left);
            Debugger("Back = y+ and x+", Vector3.up, Vector3.right);
            Debugger("Left = y+ and z-", Vector3.up, Vector3.back);
            Debugger("Right = y+ and z+", Vector3.up, Vector3.forward);
            */
            /*
            Debugger("y- and z+", Vector3.down, Vector3.forward);
            Debugger("y- and z-", Vector3.down, Vector3.back);
            Debugger("y- and x+", Vector3.down, Vector3.right);
            Debugger("y- and x-", Vector3.down, Vector3.left);
            */
            Debugger("(floor -> front orient) -> top: ", Vector3.forward, Vector3.up);
            Debugger("(floor -> front orient) -> floor", Vector3.forward, Vector3.down);
            Debugger("(floor -> front orient) -> left: ", Vector3.forward, Vector3.left);
            Debugger("(floor -> front orient) -> right", Vector3.forward, Vector3.right);


        }


    }

    void Debugger(string quote, Vector3 a, Vector3 b) {
        Debug.Log(quote + ": " + Vector3.Cross(a, b));
    }
    
    // Update is called once per frame
    void FixedUpdate() {
        bool isGround = GroundCheck();
        float modSpd = 0f;
        if (isGround) {
            modSpd = groundSpd;
        }
        else
            modSpd = airSpd;
        orientVec = GravityShift.getOrientation();
        Vector3 newDir = Vector3.RotateTowards(transform.forward, movVec, turnSpd * Time.deltaTime, 0f);
        rb.AddForce(movVec * movSpd * modSpd);
        transform.rotation = Quaternion.LookRotation(newDir, orientVec * -1);
        
        if (doJump && isGround) {
            //Debug.Log("Jump");
            performJump();
            Debug.Log("Start Timer");
            startTimer = true;
            doJump = false;
        }

        if (stopJump) {
            releaseJump();
            timer = jumpTimer;
            //if (startTimer)
            //Debug.Log("Stop Timer");
            startTimer = false;
            stopJump = false;
        }
        rb.AddForce(orientVec * gravityAmt);
    }


}
