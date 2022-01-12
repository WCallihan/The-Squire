using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Used by all destructible boulder objects:
 * manages the destroying and respawning of the boulders
 * when the player destroys the boulder or dies and respawns
 */

public class Boulder : MonoBehaviour {

    [SerializeField] WeaponType weaponNeeded = WeaponType.Club; //weapon type needed to destroy the boulder
    [SerializeField] AudioClip destroySound;
    private Animator boulderAnimator;
    private PolygonCollider2D boulderCollider;
    private AudioSource audioSource;

    void Start() {
        boulderAnimator = GetComponent<Animator>();
        boulderCollider = GetComponent<PolygonCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    //called by Destructible when the player hits the boulder
    public void DestroyBoulder(WeaponType weaponUsed) {
        if(weaponUsed == weaponNeeded) {
            //plays the destroying animation and preps the animator to respawn the boulder if it needs to
            boulderAnimator.SetTrigger("Destroy");
            boulderAnimator.SetBool("Destroyed", true);
            //play destroying sound
            audioSource.PlayOneShot(destroySound);
            //turns off the collider to allow the player to walk through it
            if(boulderCollider != null)
                boulderCollider.enabled = false;
        }
    }

    //called by Respawner when the player respawns
    public void RespawnBoulder() {
        //triggers the animator to remake the boulder, making it look like normal
        boulderAnimator.SetBool("Destroyed", false);
        //turns the collider back on
        if(boulderCollider != null)
            boulderCollider.enabled = true;
    }
}