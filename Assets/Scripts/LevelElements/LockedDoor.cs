using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Used by the door objects:
 * detects if the player is touching the door and if they
 * interact with it while holding a key, it unlocks and disappears
 */

public class LockedDoor : MonoBehaviour {

    [SerializeField] AudioClip unlockSound;

    private PlayerController playerController;
    private AudioSource audioSource;
    private bool touchingPlayer = false;

    void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    void Update() {
        //if the player is touching the door, they press e, and they have the key, then take the key and open the door
        if(touchingPlayer && Input.GetKeyDown(KeyCode.E) && playerController.hasKey == true) {
            playerController.hasKey = false;
            StartCoroutine(UnlockDoor()); //call function to unlock and deactivate the door
        }
    }

    //if something collides with the door, check if it is the player
    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.CompareTag("Player")) {
            playerController = collision.gameObject.GetComponent<PlayerController>();
            touchingPlayer = true; //player is touching the door
        }
    }
    //if something stops colliding with the door, check if it is the player
    private void OnCollisionExit2D(Collision2D collision) {
        if(collision.gameObject.CompareTag("Player")) {
            touchingPlayer = false; //player is no longer touching the door
        }
    }

    //deactivates the door after a short delay
    IEnumerator UnlockDoor() {
        audioSource.PlayOneShot(unlockSound); //play sound effect
        yield return new WaitForSeconds(1); //wait 1 second to have the door linger
        gameObject.SetActive(false); //deactivate the door
    }
}