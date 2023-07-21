using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Source: https://docs.unity3d.com/ScriptReference/Physics.Raycast.html

public class GravityShift : MonoBehaviour
{
    Transform targetWall;
    public static int oritentNum = 0;
    private Rigidbody rb;
    private RaycastHit hitFront;
    private RaycastHit hitBack;
    public PlayerMove pm;
    private Vector3 orientVec;
    private Vector3[] refVecs = {Vector3.down, Vector3.forward, Vector3.left, Vector3.right, Vector3.back,  Vector3.up};
    public Transform vCamera;

    void Start()
    {
        orientVec = Vector3.down;
        rb = GetComponent<Rigidbody>();
    }

    public Vector3 getOrientation() {
        //Vector3 estVec = Vector3.Cross(transform.forward, transform.right * -1);
        //orientVec = new Vector3(Mathf.Round(estVec.x), Mathf.Round(estVec.y), Mathf.Round(estVec.z));
        return orientVec;
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


    void changeGravity() {
        pm.keyLock();
        //Set Vertical Axis
        bool xUsed = false;
        bool yUsed = false;
        bool zUsed = false;

        if (orientVec.x != 0) {
            pm.setAxisState(0, ((int) (orientVec.x * 0.5 + 2.5)));
            xUsed = true;
        }
        else if (orientVec.y != 0) {
            pm.setAxisState(1, ((int) (orientVec.y * 0.5 + 2.5)));
            yUsed = true;
        }
        else if (orientVec.z != 0) {
            pm.setAxisState(2, ((int) (orientVec.z * 0.5 + 2.5)));
            zUsed = true;
        }

        //Set Horizontal Axis
        float[] compareVecDist = new float[6];
        int index = 0;
        for (int i = 0; i < 6; i++) {
            compareVecDist[i] = (refVecs[i] - transform.forward).sqrMagnitude;
        }
        for (int i = 1; i < 6; i++) {
            if (compareVecDist[i] < compareVecDist[index]) 
                index = i;
        }
        Vector3 rightMov = Vector3.Cross(-1 * orientVec, refVecs[index]);
        if (rightMov.x != 0) {
            pm.setAxisState(0, ((int) (rightMov.x * -0.5 + 0.5)));
            xUsed = true;
        }
        else if (rightMov.y != 0) {
            pm.setAxisState(1, ((int) (rightMov.y *  -0.5 + 0.5)));
            yUsed = true;
        }
        else if (rightMov.z != 0) {
            pm.setAxisState(2, ((int) (rightMov.z *  -0.5 + 0.5)));
            zUsed = true;
        }
        if (!xUsed) {
            pm.setAxisState(0, 4);
        }
            
        else if (!yUsed) {
            pm.setAxisState(1, 4);
        }
            
        else if (!zUsed) {
            pm.setAxisState(2, 4);
        }
        orientVec = refVecs[index];
        pm.keyUnlock();
        pm.pushMov();
    }

    // Update is called once per frame
    void Update() {
        float offsetVal = 0.6f;
        Debug.DrawRay(transform.position + (transform.forward * offsetVal), transform.forward * 2f, Color.red);
        Debug.DrawRay(transform.position - (transform.forward * offsetVal), transform.forward * -2f, Color.red);
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            if (WallCheck() && !pm.GroundCheck()) {
                changeGravity();
            }
            else {
                Debug.Log("Can't rotate");
            }
        }
    }
}
