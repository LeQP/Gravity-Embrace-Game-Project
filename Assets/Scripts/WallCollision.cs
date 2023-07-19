using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollision : MonoBehaviour
{

    bool setInitialCollision = true;
    Vector3 initPos;

    Rigidbody playerRb;
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name == "Sample Model") {
            Debug.Log("Colliding");
            if (setInitialCollision) {
                Debug.Log("Set");
                
                setInitialCollision = false;
            }
            //initPos = other.transform.position + (Vector3.forward * -50);
            //other.rigidbody.MovePosition(initPos);
        }
    }
    
    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.name == "Sample Model") {
            Debug.Log("No Longer Colliding");
            setInitialCollision = true;
        }
    }
}
