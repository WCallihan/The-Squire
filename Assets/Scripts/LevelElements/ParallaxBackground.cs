using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Used by the Background Sky object:
 * follows the camera with a parallax effect that makes it move slower
 * to create effect of the background being far away
 */

public class ParallaxBackground : MonoBehaviour {

    [SerializeField] Camera mainCamera;
    [SerializeField] float parallaxFactor;
    private Vector3 startPos;

    void Start() {
        startPos = transform.position;
    }

    //moves the background depending on the parallaxFactor and the camera's position
    void LateUpdate() {
        Vector3 distance = (mainCamera.transform.position * parallaxFactor) - startPos; //determines the distance to move the background per frame depending on the background's position
        Vector3 newPosition = new Vector3(startPos.x + distance.x, startPos.y + distance.y, startPos.z);
        transform.position = newPosition; //moves the background to its new position
    }
}