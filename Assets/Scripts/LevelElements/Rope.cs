using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Used by all destructible rope objects:
 * manages the destroying of the rope when the player hits
 * it as well as the destruction of any connected ropes
 */

public class Rope : MonoBehaviour {

    [SerializeField] WeaponType weaponNeeded;
    [SerializeField] AudioClip destroySound;
    [SerializeField] GameObject[] connectedRopes;
    private AudioSource audioSource;

    void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    //called by Destructible when the player hits the rope
    public void DestroyRope(WeaponType weaponUsed) {
        if(weaponUsed == weaponNeeded) {
            if(connectedRopes != null)
                foreach(var connection in connectedRopes)
                    connection.gameObject.SetActive(false);
            StartCoroutine(DelayDeactivation());
        }
    }
    //makes sure the object waits until the sound effect is done playing to set the rope as not active
    private IEnumerator DelayDeactivation() {
        audioSource.PlayOneShot(destroySound);
        yield return new WaitWhile(() => audioSource.isPlaying);
        gameObject.SetActive(false);
    }

    //respawn completely handled by LevelManager
}