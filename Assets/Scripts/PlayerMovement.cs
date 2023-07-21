using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    
    private Rigidbody rb;

    private float movX;
    private float movZ;
    private bool initalJump = false;
    public float movSpd = 1f;
    public float jumpSpd;
    public int initalJumpAmt = 4;
    private int jumpCount = 0;
    private bool doubleJump1 = false;
    private bool doubleJump2 = false;
    private bool doubleJump3 = false;

    public float gravity;
    public float timeInterval1 = 0f;
    public float timeInterval2 = 0f;
    public float timeIntervalBetween = 0f;


    public float jumpBoostInterval = 0f;
    private float jumpBoostTotal = 0f;
    private bool allowBoost = false;
    private bool performJump = false;
    Vector3 initPos;
    public float jumpMax;
    private float jumpLimit;
    private bool delayer = false;
    public float turnSpeed = 1f;
    private Vector3 orientVec = Vector3.down;
    Quaternion movRot = Quaternion.identity;

    void Start()
    {
        jumpLimit = initPos.y + jumpMax;
        initPos = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    
    bool GroundCheck() {
        // Approach ground check with using a ray through a RayCast
	    RaycastHit hit;
        bool groundTest = Physics.Raycast(transform.position + (orientVec * -1), orientVec, out hit, 1.012f);
        //Debug.Log(hit.collider);
        // hit will contain the distance so check if it corresponds to the object on plane
	    if(groundTest) {
            //Debug.Log("Landed");
            return true;
        }
	    else 
            return false;    
    }

    /*
    Vector3 orientMove(float movHori, float movVert) {
        //Vector3[2] resVecs;
        float x = 0f;
        float y = 0f;
        float z = 0f;

        if (orientVec == Vector3.down) {
            //resVecs[0] = new Vector(0, 0, movHori);
            //resVecs[1] = new Vector(0, movHori, 0);
            //x = movHori;
            //z = movVert;
        }
        else if (orientVec == Vector3.forward) {
            //y = movVert;
            //x = movHori;
            //resVecs[0] = new Vector(0, 0, movHori);
            //resVecs[1] = new Vector(movHori, 0, 0);
        }
        else if (orientVec == Vector3.left) {
            //y = movVert;
            //z = movHori;
        }
        else if (orientVec == Vector3.right) {
            //y = movHori;
            //z = -movVert;
        }
        else if (orientVec == Vector3.forward) {
            //y = movVert;
            //x = -movHori;
        }
        if (orientVec == Vector3.up) {
            //x = movHori;
            //z = -movVert;
        }
        //return resVecs;
        return new Vector3(x, y, z);
    }
    */
    Vector3 orientMove(float movHori, float movVert) {
        float x = 0f;
        float y = 0f;
        float z = 0f;
        if (orientVec == Vector3.down) {
            x = movHori;
            z = movVert;
        }
        else if (orientVec == Vector3.forward) {
            y = movVert;
            x = movHori;
        }
        else if (orientVec == Vector3.left) {
            y = movVert;
            z = movHori;
        }
        else if (orientVec == Vector3.right) {
            y = movHori;
            z = -movVert;
        }
        else if (orientVec == Vector3.forward) {
            y = movVert;
            x = -movHori;
        }
        if (orientVec == Vector3.up) {
            x = movHori;
            z = -movVert;
        }
        return new Vector3(x, y, z);
    }


    void Update() {
        Debug.DrawRay(transform.position + (orientVec * -1.012f), orientVec, Color.red);
        if (Input.GetKeyDown(KeyCode.Tab)) {
            if (GroundCheck()) {
                Debug.Log("Player is on ground");
            }
            else 
                Debug.Log("Player is not on ground");
        }

        bool isGround = GroundCheck();
        if (isGround) {
            jumpBoostTotal = 0;
            jumpCount = 0;
        }
        /*else if (transform.position.y < 0.01 && !doubleJump1) {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }*/
        if (isGround && Input.GetKeyDown(KeyCode.Space)) {
            StartCoroutine(timer());
            //Debug.Log("DoubleJump1");
            initalJump = true;
            doubleJump1 = true;
        }
        else if (!isGround && Input.GetKeyDown(KeyCode.Space)) {
            if (doubleJump2 == true) {
                StartCoroutine(timer());
                //Debug.Log("DoubleJump3");
                doubleJump3 = true;
            }
        }
        if (Input.GetKeyUp(KeyCode.Space)) {
            if (doubleJump1) {
                //Debug.Log("DoubleJump2");
                doubleJump2 = true;
            }
        }
            
    }


        


    /*
    if (rechargeTime > timeInterval) {
        if (Input.GetKeyDown(KeyCode.Space)) {
            jumpBoostTotal += jumpBoostInterval;
        }
        rechargeTime = rechargeTime - Time.deltaTime;
    }
    else
        rechargeTime = timeInterval;
    */

    void FixedUpdate() {
        bool isGround = GroundCheck();
        if (allowBoost) {
            if (Input.GetKey(KeyCode.Space) && !delayer) {
                jumpBoostTotal += jumpBoostInterval;
                StartCoroutine(delay());
            }
        }
        if (initalJump && jumpCount == 0) {
            if (performJump) {
                initalJump = false;
                jumpCount++;
                //Debug.Log("Jump 1: " + jumpBoostTotal);
                //rb.AddForce(Vector3.up * (initalJumpAmt + jumpBoostTotal) * jumpSpd);
                rb.AddForce(orientVec * -1 * (initalJumpAmt) * jumpSpd);
                jumpBoostTotal = 0;
                allowBoost = false;
                //Debug.Log("Single Jump");
                performJump = false;
            }
        }
        else if (doubleJump1 && doubleJump2 && doubleJump3 && jumpCount == 1) {
            if (performJump) {
                doubleJump1 = false;
                doubleJump2 = false;
                doubleJump3 = false;
                jumpCount++;
                if (jumpBoostTotal > jumpMax)
                    jumpBoostTotal = jumpMax;
                //Debug.Log("Jump 2: " + jumpBoostTotal);
                rb.AddForce(orientVec * -1 * (initalJumpAmt + jumpBoostTotal) * jumpSpd);
                jumpBoostTotal = 0;
                allowBoost = false;
                //Debug.Log("Double Jump");
                performJump = false;
            }
            
        }
        
        //rb.AddForce(orientVec * gravity);
        transform.Translate(transform.forward * Input.GetAxis("Vertical") * movSpd, Space.World);
        transform.rotation = Quaternion.Euler(orientVec * -1 * Input.GetAxis("Horizontal") * turnSpeed);
        //Debug.Log(jumpCount);

        
        Vector3 movVec = orientMove(Input.GetAxis("Horizontal"),  Input.GetAxis("Vertical"));
        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, movVec, turnSpeed * Time.deltaTime, 0f);
        if (Input.GetKey(KeyCode.Escape)) {
            Debug.Log("Desireddorward:" + desiredForward);
            Debug.Log("MoveVec:" + movVec);
        }
        
        
        
    
        
    }

    IEnumerator timer() {
        //Debug.Log("Routine time");
        yield return new WaitForSeconds(timeInterval1);
        allowBoost = true;
        yield return new WaitForSeconds(timeInterval2);
        performJump = true;
    }

    IEnumerator delay() {
        delayer = true;
        yield return new WaitForSeconds(timeIntervalBetween);
        delayer = false;

    }
}



/*
    IEnumerator increaseHeight() {
        Debug.Log("In Ienum");
        jumpMode = false;
        if (performJump == false) {
            yield return new WaitForSeconds(0.1f);
            if (rechargeTime > 0.0f && Input.GetKeyDown(KeyCode.Space)) {
                Debug.Log("jump boost");
                jumpBoostTotal += jumpBoostInterval;
                rechargeTime = rechargeTime - Time.deltaTime;
            }
            else {
                performJump = true;
                jumpMode = true;
            }
                
        }
    }


    void OnMove(InputValue movVal) {
        Vector2 movVec = movVal.Get<Vector2>();
        movX = movVec.x;
        movZ = movVec.y;
    }
    */
    /*
    void OnJump(InputValue movVal) {
        if(jumpCount <  2) {
            rb.AddForce(Vector3.up * jumpAmt, ForceMode.Impulse);
            jumpCount++;  
        }
    }
    */


        /*
        if (GroundCheck()) {
            Vector3 movVec = new Vector3(movX, 0.0f, movZ);
            rb.velocity = movVec * movSpd;
            jumpCount = 0;
        }

        float totalGravity = Physics.gravity.y * gravitySpd;
        movY += totalGravity * Time.deltaTime;

        if (!GroundCheck()) {
            Vector3 movVec = new Vector3(movX, 0.0f, movZ);
            rb.AddForce(movVec * movSpd * 10);
        }
        */