using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Used by the pickups objects:
 * assigned a pickup type in a public enumerator and
 * bobs up and down until it is destroyed by the player picking it up
 */

public enum PickupType { None, Key }

public class Pickups : MonoBehaviour {

    public PickupType pickupType;
    public AudioClip pickupSound;

    private float startPos;
    private float topPos;
    private float bottomPos;
    private float speed = 0.2f;
    private bool movingUp = true;

    void Start() {
        startPos = transform.position.y;
        topPos = startPos + 0.08f;
        bottomPos = startPos - 0.08f;
    }

    void Update() {
        //constantly bobs up and down slightly
        if(movingUp) {
            transform.Translate(Vector2.up * Time.deltaTime * speed);
        } else {
            transform.Translate(Vector2.down * Time.deltaTime * speed);
        }
        if(transform.position.y >= topPos) {
            movingUp = false;
        } else if(transform.position.y <= bottomPos) {
            movingUp = true;
        }
    }
}
