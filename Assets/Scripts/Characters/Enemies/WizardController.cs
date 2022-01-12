using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardController : MonoBehaviour {

    public float attackCooldown; //public to allow the health manager to change it when the wizard gets hit
    [SerializeField] Vector2 vectorToPlayer;

    [SerializeField] AudioClip attackSound;

    [SerializeField] GameObject player;
    private HealthManager healthManager;
    private Animator animator;
    private AudioSource audioSource;

    private bool attacking = false;
    private float timeSinceAttack = 0f;
    private bool playerInRange = false;
    private int facingDirection = 1;

    //variables that determine the attack range, it's hitboxes, and its damage
    [SerializeField] float fireRange;
    [SerializeField] Transform attackLeftPos;
    [SerializeField] Transform attackRightPos;
    [SerializeField] float attackRadius = 0.5f;
    [SerializeField] LayerMask enemyLayers; //denotes which layers can be hit
    [SerializeField] float fireDamage;

    [SerializeField] GameObject teleportEffectPrefab; //prefab of the teleport effect
    [SerializeField] GameObject[] teleportationLocations; //array of game objects of the teleportation locations
    [SerializeField] GameObject safeTeleportationLocation; //game object of the safe teleportation location
    [SerializeField] AudioClip teleportSound;
    [SerializeField] int maxTeleportTimes = 0; //times the wizard teleports before going to safety

    private GameObject currentTeleportLocation;
    private bool fleeing = true; //denotes when the wizard will teleport away from the player
    private bool teleporting = false; //makes sure that the teleport is only called once at a time
    private int timesTeleported = 0;
    private float nextHealthThreshold = 20;

    [SerializeField] GameObject platformsGrid;

    void Start() {
        healthManager = GetComponent<HealthManager>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        currentTeleportLocation = safeTeleportationLocation;
        fleeing = true; //start as fleeing when the level starts and when respawned
    }

    void Update() {
        if(!healthManager.isDead) {

            // -- ORIENTATION --

            //set vectorToPlayer and check if the player is within range
            if(player != null) {
                vectorToPlayer = player.transform.position - transform.position;
                //checks if the player is in range and makes sure the player isn't dead
                if(vectorToPlayer.magnitude <= fireRange && !player.GetComponent<HealthManager>().isDead) {
                    playerInRange = true;
                } else if(player.GetComponent<HealthManager>().isDead) { //if the player is dead and going to be respawned, then go back to fleeing and reset health threshold
                    fleeing = true; //reset fleeing
                    nextHealthThreshold = 20; //reset health threshold
                    playerInRange = false;
                } else {
                    playerInRange = false;
                }
            }

            //swap direction of sprite depending on direction to player
            if(playerInRange && vectorToPlayer.x > 0) {
                GetComponent<SpriteRenderer>().flipX = true;
                facingDirection = 1; //facing right
            } else if(playerInRange && vectorToPlayer.x < 0) {
                GetComponent<SpriteRenderer>().flipX = false;
                facingDirection = -1; //facing left
            }

            // -- PLAYER IN RANGE --

            //if the wizard is not fleeing and drops below a certain amount of health, then start fleeing
            if(!fleeing && healthManager.currentHealth <= nextHealthThreshold) {
                fleeing = true;
                nextHealthThreshold -= 10;
                StartCoroutine(DeletePlatforms()); //removes platforms
            }

            //increase timer that controls attack cooldown
            timeSinceAttack += Time.deltaTime;

            //attack or teleport when the player is in range
            if(playerInRange) {
                //try to attack the player if the wizard is not fleeing
                if(!fleeing) {
                    //attack the player if possible
                    if(!attacking && timeSinceAttack >= attackCooldown) {
                        attacking = true;
                        attackCooldown = 0; //sets the cooldown back to 0 after the stun
                        animator.SetTrigger("Attack"); //damage triggered by animation
                        timeSinceAttack = 0f; //resets attack timer
                    }
                //or teleport away from the player if the wizard isn't already
                } else if(!teleporting) {
                    teleporting = true; //set to true so that the wizard doesn't double teleport
                    RandomTeleport(); //randomly teleport to another location
                }
            }

            // -- IDLE --

            //idle if they player is not in range
            else {
                animator.SetInteger("AnimState", 0);
            }
        } else if(healthManager.isDead) {
            StartCoroutine(Die()); //start Die function if the character dies
        } else {
            animator.SetInteger("AnimState", 0); //idle when the player dies/game is not running
        }
    }

    // -- ATTACKING --

    //called by the attack animation at proper frame; calls Attack to damage any enemies in the fire's hitbox
    public void AttackTrigger() {
        Attack(facingDirection);
    }
    //called by AttackTrigger to deal damage; animation handled above
    public void Attack(int facingDirection) {
        Collider2D[] enemiesToDamage = null;
        //determine who is in fire hitbox and damage each of them
        if(facingDirection == 1) { //facing right
            enemiesToDamage = Physics2D.OverlapCircleAll(attackRightPos.position, attackRadius, enemyLayers); //makes array of anyone in the enemyLayers layer mask within the created circle (center point, radius, layer)
        } else if(facingDirection == -1) { //facing left
            enemiesToDamage = Physics2D.OverlapCircleAll(attackLeftPos.position, attackRadius, enemyLayers);
        } else {
            Debug.Log("Melee failed");
        }
        foreach(Collider2D enemy in enemiesToDamage) {
            if(enemiesToDamage != null)
                enemy.GetComponent<HealthManager>().TakeDamage(fireDamage); //assumes that any enemy of the wizard has a HealthManager
        }
        if(audioSource != null) {
            audioSource.PlayOneShot(attackSound); //play attack sound
        }
    }
    //called at beginning and end of attack animation to signal when it is attacking
    public void SetAttacking(int isAttacking) {
        attacking = (isAttacking > 0); //converts int to bool
    }

    // -- TELEPORTING --

    //teleport to random teleport location out of the given list until the wizard has teleported the max amount of times and then teleport to safety and stop fleeing
    public void RandomTeleport() {
        //increment timeTeleported and check if it has teleported to max amount of times
        timesTeleported++;
        if(timesTeleported >= maxTeleportTimes) {
            //teleport to safety as its last teleport
            SafeTeleport();
            fleeing = false; //stop fleeing so the wizard can attack the player
            timesTeleported = 0; //reset teleport count
        } else {
            //randomize next location until it is not the current one
            GameObject nextLocation = null;
            do {
                int randInt = Random.Range(0, teleportationLocations.Length - 1);
                nextLocation = teleportationLocations[randInt];
            } while(nextLocation == currentTeleportLocation);
            //move to that location
            TeleportHelper(nextLocation);
        }
        //set teleporting back to false to allow for it to teleport again
        teleporting = false;
    }
    //teleport to the designated safe location
    public void SafeTeleport() {
        TeleportHelper(safeTeleportationLocation);
        StartCoroutine(SummonPlatforms());
    }
    //helper function called by RandomTeleport and SafeTeleport; teleports the wizard to the given location
    private void TeleportHelper(GameObject destination) {
        //spawn teleport effects at the current location and the destination
        Instantiate(teleportEffectPrefab, transform.position + new Vector3(0, 1.75f, 0), teleportEffectPrefab.transform.rotation);
        Instantiate(teleportEffectPrefab, destination.transform.position + new Vector3(0, 1.75f, 0), teleportEffectPrefab.transform.rotation);
        //play teleport sound
        audioSource.PlayOneShot(teleportSound);
        //move the wizard
        transform.position = destination.transform.position;
    }

    // -- PLATFORMS --

    //teleports in platforms to reach the wizard
    public IEnumerator SummonPlatforms() {
        yield return new WaitForSeconds(2); //wait for 2 seconds after teleporting to safety to summon
        //summon platforms to get to the wizard
        platformsGrid.transform.position = new Vector3(0, 0, 0);
        //spawn teleport effects at the platform locations
        Instantiate(teleportEffectPrefab, new Vector3(5.25f,16.45f,-1f), teleportEffectPrefab.transform.rotation);
        Instantiate(teleportEffectPrefab, new Vector3(27.45f,16.45f,-1f), teleportEffectPrefab.transform.rotation);
        //play teleport sound
        audioSource.PlayOneShot(teleportSound);
    }
    //teleports out platforms
    public IEnumerator DeletePlatforms() {
        yield return new WaitForSeconds(2); //wait for 2 seconds after teleporting away to delete
        //deactivate platforms
        platformsGrid.transform.position = new Vector3(0, 20, 0); //some random position out of the level
        //spawn teleport effects at the platform locations
        Instantiate(teleportEffectPrefab, new Vector3(5.25f, 16.45f, -1f), teleportEffectPrefab.transform.rotation);
        Instantiate(teleportEffectPrefab, new Vector3(27.45f, 16.45f, -1f), teleportEffectPrefab.transform.rotation);
        //play teleport sound
        audioSource.PlayOneShot(teleportSound);
    }

    // -- MISC. --

    //called when the wizard dies; triggers the end of the scene
    public IEnumerator Die() {
        SetAttacking(0);
        yield return new WaitForSeconds(3); //waits 3 seconds to let the body linger
        if(healthManager.isDead) { //makes sure it hasn't respawned in that time
            FindObjectOfType<SceneChanger>().UnlockLevel("Level 4"); //unlocks level 4
            FindObjectOfType<SceneChanger>().EndScene(); //ends scene
        }
    }

    //draws representation of the attack area when the object is selected in the editor
    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, fireRange);
        if(attackRightPos != null) {
            Gizmos.DrawWireSphere(attackRightPos.position, attackRadius);
        }
        if(attackLeftPos != null) {
            Gizmos.DrawWireSphere(attackLeftPos.position, attackRadius);
        }
    }
}