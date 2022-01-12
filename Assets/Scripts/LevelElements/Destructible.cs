using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Used by all destrucible objects:
 * called by the PlayerController when the player attacks a
 * destructible. this script manages what kind of destructible the object is
 * and calls the proper script to carry out the appropriate task
 */

//public enum to list the types of destructible objects
public enum DestructibleType { Boulder, Rope }

public class Destructible : MonoBehaviour {

    [SerializeField] DestructibleType destructibleType;

    //called by PlayerController when the player attacks the destructible object; calls the appropriate script to handle the destruction
    public void DestroyObject(WeaponType weaponUsed) {
        //call boulder script for a boulder
        if(destructibleType == DestructibleType.Boulder) {
            gameObject.GetComponent<Boulder>().DestroyBoulder(weaponUsed);
        //call rope script for a rope
        } else if(destructibleType == DestructibleType.Rope) {
            gameObject.GetComponent<Rope>().DestroyRope(weaponUsed);
        }
    }
}