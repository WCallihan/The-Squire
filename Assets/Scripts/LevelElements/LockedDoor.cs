using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Used by the door objects:
 * detects if the player is touching the door and if they
 * interact with it holding a key, it unlocks and disappears
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
        if(touchingPlayer && Input.GetKeyDown(KeyCode.E) && playerController.hasKey == true) {
            playerController.hasKey = false;
            StartCoroutine(UnlockDoor());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.CompareTag("Player")) {
            playerController = collision.gameObject.GetComponent<PlayerController>();
            touchingPlayer = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision) {
        if(collision.gameObject.CompareTag("Player")) {
            touchingPlayer = false;
        }
    }

    IEnumerator UnlockDoor() {
        audioSource.PlayOneShot(unlockSound);
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
    }
}
