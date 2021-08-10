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
    private bool followPlayer = true;
    private float fallRespawnTimer = 3f;

    void FixedUpdate() {
        //follows player at determined point above their head with set boundaries left and right
        if(followPlayer) {
            transform.position = followPoint.gameObject.transform.position + new Vector3(0, 0, -10);
            if(transform.position.x <= leftBound) {
                transform.position = new Vector3(leftBound, transform.position.y, transform.position.z);
            } else if(transform.position.x >= rightBound) {
                transform.position = new Vector3(rightBound, transform.position.y, transform.position.z);
            }
        } else {
            //counts down the fall respawn timer while its not following the player
            fallRespawnTimer -= Time.deltaTime;
            if(fallRespawnTimer <= 0) {
                followPlayer = true;
                GameObject.FindObjectOfType<PlayerController>().BeginRespawn(); //triggers universal respawning through the LevelManager
            }
        }
    }


    //detects when the player falls off the map, triggering a respawn
    public void playerFall() {
        followPlayer = false;
        fallRespawnTimer = 3f;
    }
}
