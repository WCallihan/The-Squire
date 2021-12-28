using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Used by the BreakableBridge object:
 * detects when the designated breaking object collides with the
 * collapsing bridge and then it destroys both objects after
 * playing a sound effect
 */

public class CollapsingBridge : MonoBehaviour {

    [SerializeField] GameObject breakingObject;
    [SerializeField] AudioClip crashingSound;
    private CompositeCollider2D compositeCollider;
    private AudioSource audioSource;

    void Start() {
        compositeCollider = GetComponent<CompositeCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    //destroys both the bridge and breaking object when they collide
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject == breakingObject) {
            collision.gameObject.SetActive(false);
            StartCoroutine(DelayDeactivation());
        }
    }

    //makes sure the object waits until the sound effect is done playing to set the bridge as not active
    private IEnumerator DelayDeactivation() {
        audioSource.PlayOneShot(crashingSound);
        yield return new WaitWhile(() => audioSource.isPlaying);
        gameObject.SetActive(false);
    }
}