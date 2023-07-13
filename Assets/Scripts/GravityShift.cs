using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Source: https://docs.unity3d.com/ScriptReference/Physics.Raycast.html

public class GravityShift : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public Vector3 getOrientation() {
        return Vector3.up;
    }
    Transform targetWall;

    bool WallCheck() {
        // Approach ground check with using a ray through a RayCast
	    RaycastHit hitFront;
        RaycastHit hitBack;
        float offsetVal = 0.6f;
        bool frontCheck = Physics.Raycast(transform.position + (transform.forward * offsetVal), transform.forward * 1f, out hitFront, 1.6f);
        bool backCheck = Physics.Raycast(transform.position - (transform.forward * offsetVal), transform.forward * -1f, out hitBack, 1.6f);
        
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

    // Update is called once per frame
    void Update()
    {
        float offsetVal = 0.6f;
        Debug.DrawRay(transform.position + (transform.forward * offsetVal), transform.forward * 1f, Color.red);
        Debug.DrawRay(transform.position - (transform.forward * offsetVal), transform.forward * -1f, Color.red);
        Debug.DrawRay(transform.position + Vector3.up, transform.up * -1f, Color.red);
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            if (WallCheck() && !GroundCheck()) {
                Debug.Log("Can rotate");
                Quaternion newAngle = Quaternion.Euler(targetWall.rotation.x, transform.rotation.y, transform.rotation.z);
                transform.rotation = newAngle;
            }
            else {
                Debug.Log("Can't rotate");
            }
        }
    }
}
