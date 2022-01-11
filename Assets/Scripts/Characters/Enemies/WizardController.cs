using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class WizardController : MonoBehaviour {

    public float attackCooldown; //public to allow the health manager to change it when the wizard gets hit
    [SerializeField] Vector2 vectorToPlayer;

    [SerializeField] AudioClip attackSound;

    [SerializeField] GameObject player;
    private HealthManager healthManager;
    private Animator animator;
    private Rigidbody2D enemyRb;
    private AudioSource audioSource;

    private bool attacking = false;
    private float timeSinceAttack = 0f;
    private bool playerInRange = false;
    private int facingDirection = 1;

    [SerializeField] float fireRange;
    [SerializeField] Transform attackLeftPos;
    [SerializeField] Transform attackRightPos;
    [SerializeField] float attackRadius = 0.5f;
    [SerializeField] LayerMask enemyLayers;
    [SerializeField] float fireDamage;

    [SerializeField] GameObject teleportEffectPrefab;
    [SerializeField] GameObject[] teleportationLocations;
    [SerializeField] GameObject safeTeleportationLocation;
    [SerializeField] AudioClip teleportSound;
    [SerializeField] int maxTeleportTimes = 0; //times the wizard teleports before going to safety

    private GameObject currentTeleportLocation;
    private bool fleeing = true; //denotes when the wizard will teleport away from the player
    private static Semaphore teleporting = new Semaphore(1, 1); //makes sure that the teleport is only called once at a time
    private int timesTeleported = 0;
    private float nextHealthThreshold = 20;

    [SerializeField] GameObject platformsGrid;

    void Start() {
        healthManager = GetComponent<HealthManager>();
        animator = GetComponent<Animator>();
        enemyRb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        //audioSource.volume = PlayerPrefs.GetFloat("EffectsVolume", 1);
        currentTeleportLocation = safeTeleportationLocation;
        fleeing = true; //start as fleeing when the level starts and when respawned
    }

    void Update() {
        if(!healthManager.isDead) {
            //Set vectorToPlayer and check if they are within range
            if(player != null) {
                vectorToPlayer = player.transform.position - transform.position;
                //checks if the player is in range and makes sure the player isn't dead
                if(vectorToPlayer.magnitude <= fireRange && !player.GetComponent<HealthManager>().isDead) {
                    playerInRange = true;
                } else if(player.GetComponent<HealthManager>().isDead) { //if the player is dead and going to be respawned, then go back to fleeing and reset health threshold
                    fleeing = true;
                    nextHealthThreshold = 20;
                    playerInRange = false;
                } else {
                playerInRange = false;
            }
            }

            //swap direction of sprite depending on direction to player
            if(playerInRange && vectorToPlayer.x > 0) { //facing right
                GetComponent<SpriteRenderer>().flipX = true;
                facingDirection = 1;
            } else if(playerInRange && vectorToPlayer.x < 0) { //facing left
                GetComponent<SpriteRenderer>().flipX = false;
                facingDirection = -1;
            }

            //if the wizard is not fleeing and drops below a certain amount of health, then start fleeing
            if(!fleeing && healthManager.currentHealth <= nextHealthThreshold) {
                fleeing = true;
                nextHealthThreshold -= 10;
                StartCoroutine(DeletePlatforms()); //removes platforms
            }

            timeSinceAttack += Time.deltaTime;
            //Attack or teleport when the player is in range
            if(playerInRange) {
                //Attack the player if the wizard is not fleeing and if the wizard can attack
                if(!fleeing) {
                    //attack the player if possible
                    if(!attacking && timeSinceAttack >= attackCooldown) {
                        attacking = true;
                        attackCooldown = 0; //sets the cooldown back to 0 after the stun
                        animator.SetTrigger("Attack"); //actual attack triggered by animation
                        timeSinceAttack = 0f; //resets attack timer
                    }
                //Or teleport away from the player if the wizard isn't already
                } else if(teleporting.WaitOne(0)) { //if the semaphore is available, then teleport
                    RandomTeleport();
                }
            }
            //Idle if they player is not in range
            else {
                animator.SetInteger("AnimState", 0);
            }
        } else if(healthManager.isDead) {
            StartCoroutine(Die());
        } else {
            animator.SetInteger("AnimState", 0); //idle when the player dies/game is not running
        }
    }

    // -- ATTACKING --

    //called by the attack animation at proper frame
    public void AttackTrigger() {
        Attack(facingDirection);
    }
    //deal damage depending on facingDirection
    public void Attack(int facingDirection) {
        Collider2D[] enemiesToDamage = null;
        if(facingDirection == 1) { //facing right
            enemiesToDamage = Physics2D.OverlapCircleAll(attackRightPos.position, attackRadius, enemyLayers); //makes array of anyone in the enemyLayers layer mask within the created circle (center point, radius, layer)
        } else if(facingDirection == -1) { //facing left
            enemiesToDamage = Physics2D.OverlapCircleAll(attackLeftPos.position, attackRadius, enemyLayers);
        } else {
            Debug.Log("Melee failed");
        }
        foreach(Collider2D enemy in enemiesToDamage) {
            if(enemiesToDamage != null)
                enemy.GetComponent<HealthManager>().TakeDamage(fireDamage);
        }
        if(audioSource != null) {
            audioSource.PlayOneShot(attackSound);
        }
    }
    //called at beginning and end of attack animation to signal when it is attacking
    public void SetAttacking(int isAttacking) {
        attacking = (isAttacking > 0);
    }

    // -- TELEPORTING --

    //teleport to random teleport location out of the given list until the wizard has teleported the max amount of times and then teleport to safety and stop fleeing
    public void RandomTeleport() {
        //increment timeTeleported and check if it has teleported to max amount of times
        timesTeleported++;
        if(timesTeleported >= maxTeleportTimes) {
            //teleport to safety as its last teleport
            SafeTeleport();
            fleeing = false;
            timesTeleported = 0;
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
        //release teleporting semaphore to allow it to be called again
        teleporting.Release();
    }
    //teleport to the designated safe location
    public void SafeTeleport() {
        TeleportHelper(safeTeleportationLocation);
        StartCoroutine(SummonPlatforms());
    }
    //helper function to teleport the wizard to the given location
    private void TeleportHelper(GameObject destination) {
        //spawn teleport effects at the current location and the destination
        Instantiate(teleportEffectPrefab, transform.position + new Vector3(0, 1.75f, 0), teleportEffectPrefab.transform.rotation);
        Instantiate(teleportEffectPrefab, destination.transform.position + new Vector3(0, 1.75f, 0), teleportEffectPrefab.transform.rotation);
        //play teleport sound
        audioSource.PlayOneShot(teleportSound);
        //move the wizard
        transform.position = destination.transform.position;
    }

    //teleports in platforms to reach the boss
    public IEnumerator SummonPlatforms() {
        yield return new WaitForSeconds(2); //wait for 2 seconds after teleporting to safety to summon
        //summon platforms to get to boss
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
        yield return new WaitForSeconds(5);
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