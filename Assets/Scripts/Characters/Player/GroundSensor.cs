using UnityEngine;
using System.Collections;

/*Used by the GroundSensor objects on the players:
 * detects and returns if the player is touching a collider on the bottom
 * (directly taken from HeroKnight character from Sven Thole)
 */

public class GroundSensor : MonoBehaviour {

    private int collisionCount = 0; //counts the number of colliders the sensor is touching

    private float disableTimer;

    private void OnEnable() {
        collisionCount = 0;
    }

    //returns if the sensor is on the ground, or standing on another collider
    public bool OnGround() {
        if(disableTimer > 0)
            return false;
        return collisionCount > 0;
    }

    void OnTriggerEnter2D(Collider2D other) {
        collisionCount++;
    }

    void OnTriggerExit2D(Collider2D other) {
        collisionCount--;
    }

    //counts down the disableTimer; only important immediately after Disable is called
    void Update() {
        disableTimer -= Time.deltaTime;
    }

    //disbales the on ground sensor for a short duration to guarentee immediate reading of not grounded
    public void Disable(float duration) {
        disableTimer = duration;
    }
}