using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Used by all objects that are hanging from another object(usually a rope):
 * manages the object's rigidbody and if the tether point is destroyed (set as inactive)
 * then the hanging object is released by setting the rigidbody to dynamic and letting it fall
 */

public class HangingObject : MonoBehaviour {

    [SerializeField] GameObject tetherPoint; //the object that is holding the hanging object in place
    private Rigidbody2D hangingRb;

    void Start() {
        hangingRb = GetComponent<Rigidbody2D>();
    }

    void Update() {
        //sets the hanging object to static if the tether point is not destroyed
        //used primarily for when objects are respawned
        if(tetherPoint.activeInHierarchy && hangingRb.bodyType != RigidbodyType2D.Kinematic) {
            hangingRb.bodyType = RigidbodyType2D.Kinematic;
        }

        //sets the hanging object to dynamic if the tether point is destroyed
        if(!tetherPoint.activeInHierarchy && hangingRb.bodyType != RigidbodyType2D.Dynamic) {
            hangingRb.bodyType = RigidbodyType2D.Dynamic;
        }
    }
}