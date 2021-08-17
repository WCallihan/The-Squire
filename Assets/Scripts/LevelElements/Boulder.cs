using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Used by all destructible boulder objects:
 * manages the destroying and respawning of the boulders
 * when the player destroys the boulder or dies and respawns
 */

public class Boulder : MonoBehaviour {

    [SerializeField] WeaponType weaponNeeded;
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
            boulderAnimator.SetTrigger("Destroy");
            boulderAnimator.SetBool("Destroyed", true);
            audioSource.PlayOneShot(destroySound);
        }
        if(boulderCollider != null)
            boulderCollider.enabled = false;
    }

    //called by Respawner when the player respawns
    public void RespawnBoulder() {
        boulderAnimator.SetBool("Destroyed", false);
        if(boulderCollider != null)
            boulderCollider.enabled = true;
    }
}