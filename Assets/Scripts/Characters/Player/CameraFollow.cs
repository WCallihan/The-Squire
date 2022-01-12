using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Used by Main Camaera object in all levels:
 * is assigned a Camera Position object attached the to player and follows it at all times
 * to keep the player in frame. bounds are set so the camera never shows anything past the
 * bounds of the environment but still keeps the player in frame.
 */

public class CameraFollow : MonoBehaviour {

    [SerializeField] float leftBound; //bounds make sure the camera stays within the bounds of the environment; manually assigned on a per level basis
    [SerializeField] float rightBound;
    [SerializeField] GameObject followPoint; //an object childed to the player
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
        //followPlayer is only false after the player has fallen through a fall cutoff and playerFall has been called
        } else {
            //counts down the fall respawn timer while its not following the player
            fallRespawnTimer -= Time.deltaTime;
            if(fallRespawnTimer <= 0) { //timer is done
                followPlayer = true;
                GameObject.FindObjectOfType<PlayerController>().BeginRespawn(); //triggers universal respawning through the LevelManager
            }
        }
    }

    //called by PlayerController when it collides with the fall cutoff; triggers a respawn of everything
    public void playerFall() {
        followPlayer = false;
        fallRespawnTimer = 3f;
    }
}