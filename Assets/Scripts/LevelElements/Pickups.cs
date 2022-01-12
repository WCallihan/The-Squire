using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Used by the key objects:
 * assigned a pickup type in a public enumerator and
 * bobs up and down until it is destroyed by the player picking it up
 */

//technically not necessary because there is only one type of pickup, but it is imported from another project of mine
public enum PickupType { None, Key }

public class Pickups : MonoBehaviour {

    public PickupType pickupType; //public to be checked by PlayerController
    public AudioClip pickupSound; //public because the player object plays the sound effect

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
            transform.Translate(Vector2.up * Time.deltaTime * speed); //move up
        } else {
            transform.Translate(Vector2.down * Time.deltaTime * speed); //move down
        }
        //if the object reaches the top position, then stop moving up
        if(transform.position.y >= topPos) {
            movingUp = false;
        //if the object reaches the bottom position, then start moving up
        } else if(transform.position.y <= bottomPos) {
            movingUp = true;
        }
    }
}