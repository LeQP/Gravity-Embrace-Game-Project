using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Source: https://docs.unity3d.com/ScriptReference/Physics.Raycast.html

public class GravityShift : MonoBehaviour
{
    Transform targetWall;
    public static int oritentNum = 0;
    public static Vector3[] orientVecs = {Vector3.down, Vector3.forward, Vector3.left, Vector3.right, Vector3.back, Vector3.up};
    private Rigidbody rb;
    private RaycastHit hitFront;
    private RaycastHit hitBack;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public static Vector3 getOrientation() {
        return orientVecs[oritentNum];
    }
    

    bool WallCheck() {
        // Approach ground check with using a ray through a RayCast
        float offsetVal = 0.6f;
        bool frontCheck = Physics.Raycast(transform.position + (transform.forward * offsetVal), transform.forward * 1f, out hitFront, 2f);
        bool backCheck = Physics.Raycast(transform.position - (transform.forward * offsetVal), transform.forward * -1f, out hitBack, 2f);
        
        // hit will contain the distance so check if it corresponds to the object on plane
	    if(frontCheck && !backCheck) {
            targetWall = hitFront.transform;
            return true;
        }
	    else 
            return false;
    }

    bool GroundCheck() {
        // Approach ground check with using a ray through a RayCast
	    RaycastHit hit;
        bool groundTest = Physics.Raycast(transform.position + Vector3.up, transform.up * -1f, out hit, 1f);
        //Debug.Log(hit.collider);
        // hit will contain the distance so check if it corresponds to the object on plane
	    if(groundTest) {
            //Debug.Log("Landed");
            return true;
        }
	    else 
            return false;    
    }

    void readWall() {
        if (hitFront.transform.tag == "Floor") {
            oritentNum = 0;
            //rb.constraints = RigidbodyConstraints.None;
            //rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
        else if (hitFront.transform.tag == "Front") {
            oritentNum = 1;
            //rb.constraints = RigidbodyConstraints.None;
            //rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        }
        else if (hitFront.transform.tag == "Left") {
            oritentNum = 2;
            //rb.constraints = RigidbodyConstraints.None;
            //rb.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        }
        else if (hitFront.transform.tag == "Right") {
            oritentNum = 3;
            //rb.constraints = RigidbodyConstraints.None;
            //rb.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        }
        else if (hitFront.transform.tag == "Back") {
            oritentNum = 4;
            //rb.constraints = RigidbodyConstraints.None;
            //rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        }
        else if (hitFront.transform.tag == "Top") {
            oritentNum = 5;
            //rb.constraints = RigidbodyConstraints.None;
            //rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
    }

    void maintainRot() {
        if (oritentNum == 0) {
            transform.rotation = Quaternion.Euler(new Vector3(0, transform.localEulerAngles.y, 0));
        }
        else if (oritentNum == 1) {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, transform.localEulerAngles.z));
        }
        else if (oritentNum == 2) {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0, transform.rotation.y, -90)), 1);
        }
        else if (oritentNum == 3) {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0, transform.rotation.y, 90)), 1);
        }
        else if (oritentNum == 4) {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0, transform.rotation.y, 0)), 1);
        }
        else if (oritentNum == 5) {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(-180, transform.rotation.y, 0)), 1);
        }
    }
    // Update is called once per frame
    void Update() {
        float offsetVal = 0.6f;
        Debug.DrawRay(transform.position + (transform.forward * offsetVal), transform.forward * 2f, Color.red);
        Debug.DrawRay(transform.position - (transform.forward * offsetVal), transform.forward * -2f, Color.red);
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            if (WallCheck() && !GroundCheck()) {
                readWall();
                maintainRot();
                Debug.Log("Can rotate");
            }
            else {
                Debug.Log("Can't rotate");
            }
        }
    }
}
