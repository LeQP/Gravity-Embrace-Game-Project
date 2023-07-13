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
        Physics.Raycast(transform.position + Vector3.up, transform.up * -1f, out hit, 1);
        // hit will contain the distance so check if it corresponds to the object on plane
	    if(Physics.Raycast(transform.position + transform.up, transform.up * -1f, out hit, 1)) {
            //Debug.Log("Landed");
            return true;
        }
	    else 
            return false;
		    
    }

    void Update() {
        Debug.DrawRay(transform.position + Vector3.up, transform.up * -1f, Color.green);
        bool isGround = GroundCheck();
        if (isGround) {
            jumpBoostTotal = 0;
            jumpCount = 0;
        }
        if (isGround && Input.GetKeyDown(KeyCode.Space)) {
            StartCoroutine(timer());
            initalJump = true;
            doubleJump1 = true;
        }
        else if (!isGround && Input.GetKeyDown(KeyCode.Space)) {
            if (doubleJump2 == true) {
                StartCoroutine(timer());
                doubleJump3 = true;
            }
        }
        if (Input.GetKeyUp(KeyCode.Space)) {
            if (doubleJump1) {
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
                rb.AddForce(Vector3.up * (initalJumpAmt) * jumpSpd);
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
                Debug.Log("Jump 2: " + jumpBoostTotal);
                rb.AddForce(Vector3.up * (initalJumpAmt + jumpBoostTotal) * jumpSpd);
                jumpBoostTotal = 0;
                allowBoost = false;
                //Debug.Log("Double Jump");
                performJump = false;
            }
            
        }
        else
            rb.AddForce(Vector3.down * gravity);

        //Debug.Log(jumpCount);
        Vector3 movVec = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));

        // Adjust rotation
        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, movVec, turnSpeed * Time.deltaTime, 0f);
        movRot = Quaternion.LookRotation(desiredForward);
        rb.MovePosition(rb.position + (movVec * movSpd));
        rb.MoveRotation(movRot);
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