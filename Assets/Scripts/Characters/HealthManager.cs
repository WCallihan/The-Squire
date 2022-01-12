using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Used by all player and enemy characters:
 * manages the character's maximum and current health and has
 * a generic function that can take damage when the character is hit.
 * also handles the stunning of the wizard.
 */

public class HealthManager : MonoBehaviour {

    public float maxHealth;
    public float currentHealth;

    [SerializeField] AudioClip hurtSound;

    private Animator animator;
    private AudioSource audioSource;

    public bool isDead = false;

    void Start() {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    //called by an Attack function when the gameObject is hit by an attack
    public void TakeDamage(float damage) {
        if(!isDead) {
            //take damage if it isn't already dead
            UpdateHealth(-damage);
            animator.SetTrigger("Hurt"); //all characters that use this script must have same trigger name
            if(hurtSound != null)
                audioSource.PlayOneShot(hurtSound); //play hurt sound

            //handles death of characters; enemies have an extra function in their controllers
            if(currentHealth <= 0) {
                isDead = true;
                animator.SetBool("Dead", true); //all characters that use this script must have same boolean name
            }

            //stuns the wizard, and only the wizard, when he takes damage
            WizardController wizCont = GetComponent<WizardController>();
            if(wizCont != null) {
                wizCont.attackCooldown = 4; //stun the wizard for 4 seconds
            }
        }
    }

    //called when health is changed by damage or healing
    public void UpdateHealth(float healthChange) {
        currentHealth += healthChange;
    }

    //called when respawning the player and enemies after the player dies; sets the health back to max and sets dead values to false
    public void Respawn() {
        isDead = false;
        currentHealth = maxHealth;
        animator.SetBool("Dead", false);
    }
}