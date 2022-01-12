using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Used by the BreakableBridge object:
 * detects when the designated breaking object collides with the collapsing
 * bridge and then destroys both objects after playing a sound effect
 */

public class CollapsingBridge : MonoBehaviour {

    [SerializeField] GameObject breakingObject; //object that will trigger the bridge to break
    [SerializeField] AudioClip crashingSound;
    private AudioSource audioSource;

    void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    //destroys both the bridge and breaking object when they collide
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject == breakingObject) {
            collision.gameObject.SetActive(false); //instantly deactivates the breaking object
            StartCoroutine(DelayDeactivation());
        }
    }

    //makes sure the object waits until the sound effect is done playing to deactivate the bridge
    private IEnumerator DelayDeactivation() {
        audioSource.PlayOneShot(crashingSound); //play sound effect
        yield return new WaitWhile(() => audioSource.isPlaying); //wait until the sound has finished
        gameObject.SetActive(false); //deactivate the bridge
    }
}