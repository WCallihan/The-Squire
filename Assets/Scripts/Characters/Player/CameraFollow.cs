using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Used by Main Camera
 * Is assigned an object attached the to player and follows it at all times.
 * Bounds are set so the camera never shows anything past the bounds of the environment but still keeps the player in frame.
 */

public class CameraFollow : MonoBehaviour {

    [SerializeField] float leftBound;
    [SerializeField] float rightBound;
    [SerializeField] GameObject followPoint;

    void LateUpdate() {
        //follows player at determined point above their head with set boundaries left and right
        transform.position = followPoint.gameObject.transform.position + new Vector3(0, 0, -10);
        if(transform.position.x <= leftBound) {
            transform.position = new Vector3(leftBound, transform.position.y, transform.position.z);
        } else if(transform.position.x >= rightBound) {
            transform.position = new Vector3(rightBound, transform.position.y, transform.position.z);
        }
    }
}
