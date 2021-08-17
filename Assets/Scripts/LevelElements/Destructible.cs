using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Used by all destrucible objects:
 * called by the PlayerController when the player attacks a
 * destructible. this script manages what kind of destructible it is
 * and calls the proper script to carry out the appropriate task
 */

public enum DestructibleType { Boulder, Rope }

public class Destructible : MonoBehaviour {

    [SerializeField] DestructibleType destructibleType;

    public void DestroyObject(WeaponType weaponUsed) {
        if(destructibleType == DestructibleType.Boulder) {
            gameObject.GetComponent<Boulder>().DestroyBoulder(weaponUsed);
        } else if(destructibleType == DestructibleType.Rope) {
            gameObject.GetComponent<Rope>().DestroyRope(weaponUsed);
        }
    }
}