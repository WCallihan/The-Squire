using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Used by teleportation effect prefab:
 * destroys the effect object after the animation has finished
 */

public class TeleportEffect : MonoBehaviour {

    //called by the effect animation when it is done to despawn the object
    public void TriggerEffectOver() {
        Destroy(gameObject);
    }
}