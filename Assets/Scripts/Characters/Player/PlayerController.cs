using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Used by both player characters:
 * manages everything to do with the player including their inputs, movement,
 * ground sensor, attacking, animations, and sound effects
 */

//public enum to denote the weapon types, used by PlayerController, HUDManager, Destructible, Boulder, and Rope
public enum WeaponType { Sword, Claymore, Spear, Club }

public class PlayerController : MonoBehaviour {

    [SerializeField] float speed = 4f;
    [SerializeField] float jumpForce = 7.5f;

    [SerializeField] AudioClip[] weaponSwingSounds;
    public WeaponType currentWeapon = WeaponType.Sword; //unchanging on Knight
    [SerializeField] bool multipleWeapons = true; //true on Squire, false on Knight

    //various scripts and components that PlayerController uses and calls
    private HealthManager healthManager;
    private HUDManager hudManager;
    private Animator animator;
    private Rigidbody2D playerRb;
    private AudioSource audioSource;
    private GroundSensor groundSensor;

    private float timeSinceAttack = 0f;
    private float attackCooldown = 0.25f;
    private bool attacking = false;
    private float delayToIdle = 0f;
    private Collider2D touchingPickup;
    private bool grounded = false;
    private int facingDirection = 1;

    public bool hasKey; //public to be used by the LockerDoor object

    //the positions and radiuses that make up the weapon hit boxes when attacking
    [SerializeField] Transform swordAttackLeftPos;
    [SerializeField] Transform swordAttackRightPos;
    [SerializeField] float swordAttackRadius = 0.5f;
    [SerializeField] Transform spearAttackLeftPos;
    [SerializeField] Transform spearAttackRightPos;
    [SerializeField] float spearAttackRadius = 0.5f;
    [SerializeField] Transform claymoreClubAttackLeftPos;
    [SerializeField] Transform claymoreClubAttackRightPos;
    [SerializeField] float claymoreClubAttackRadius = 0.5f;
    //denotes which layers can be hit by the player
    [SerializeField] LayerMask enemyLayers;

    public bool playerRespawning = false; //public to be used by LevelManager to trigger everything to respawn

    void Start() {
        healthManager = GetComponent<HealthManager>();
        hudManager = GetComponent<HUDManager>();
        animator = GetComponent<Animator>();
        playerRb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        groundSensor = transform.Find("GroundSensor").GetComponent<GroundSensor>();
    }

    void Update() {
        if(!healthManager.isDead) {

            // -- MOVEMENT --

            //controls the grounded variable and sets the animator boolean
            grounded = groundSensor.OnGround();
            animator.SetBool("Grounded", grounded);

            //take in movement input
            float inputX = 0;
            if(!attacking || !grounded) { //cannot move if the player is attacking and is grounded
                inputX = Input.GetAxis("Horizontal");
            }

            //swap direction of sprite depending on walk direction
            if(inputX > 0) {
                GetComponent<SpriteRenderer>().flipX = false;
                facingDirection = 1; //facing right
            } else if(inputX < 0) {
                GetComponent<SpriteRenderer>().flipX = true;
                facingDirection = -1; //facing left
            }

            //move side to side
            playerRb.velocity = new Vector2(inputX * speed, playerRb.velocity.y);

            // -- WEAPONS --

            //change weapons when not attacking
            if(multipleWeapons && !attacking) {
                //1 = sword
                if(Input.GetKeyDown(KeyCode.Alpha1)) {
                    currentWeapon = WeaponType.Sword;
                    hudManager.UpdateWeaponSelected(WeaponType.Sword);
                //2 = claymore
                } else if(Input.GetKeyDown(KeyCode.Alpha2)) {
                    currentWeapon = WeaponType.Claymore;
                    hudManager.UpdateWeaponSelected(WeaponType.Claymore);
                //3 = spear
                } else if(Input.GetKeyDown(KeyCode.Alpha3)) {
                    currentWeapon = WeaponType.Spear;
                    hudManager.UpdateWeaponSelected(WeaponType.Spear);
                //4 = club
                } else if(Input.GetKeyDown(KeyCode.Alpha4)) {
                    currentWeapon = WeaponType.Club;
                    hudManager.UpdateWeaponSelected(WeaponType.Club);
                }
            }

            // -- ANIMATIONS --

            //set AirSpeedY in animator to denote if the player is falling
            animator.SetFloat("AirSpeedY", playerRb.velocity.y);

            //increase timer that controls attack cooldown
            timeSinceAttack += Time.deltaTime;

            //attack if the attack cooldown has passed
            if(Input.GetMouseButtonDown(0) && timeSinceAttack >= attackCooldown) {
                if(multipleWeapons)
                    animator.SetInteger("WeaponType", (int)currentWeapon); //parameter only on the Squire
                animator.SetTrigger("Attack"); //damage triggered by animation
                timeSinceAttack = 0.0f; //resets attack timer
            }

            //jump if grounded and not attacking
            else if((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && grounded && !attacking) {
                animator.SetTrigger("Jump");
                playerRb.velocity = new Vector2(playerRb.velocity.x, jumpForce); //set the player's y velocity to the jump force
                groundSensor.Disable(0.2f); //sets onGround to false for 0.2 seconds
            }

            //run if not jumping or attacking
            else if(Mathf.Abs(inputX) > Mathf.Epsilon) {
                // Reset timer
                delayToIdle = 0.05f;
                animator.SetInteger("AnimState", 1);
            }

            //idle if not running, jumping, or attacking
            else {
                //prevents flickering transitions to idle
                delayToIdle -= Time.deltaTime;
                if(delayToIdle < 0)
                    animator.SetInteger("AnimState", 0);
            }

            // -- PICKUPS --

            //pick up the pickup that the player is touching if there is one
            if(Input.GetKeyDown(KeyCode.E) && touchingPickup != null) {
                PickupType currentPickup = touchingPickup.gameObject.GetComponent<Pickups>().pickupType; //takes the pickup's type
                audioSource.PlayOneShot(touchingPickup.gameObject.GetComponent<Pickups>().pickupSound); //plays the pickup's sound effect
                //if the pickup if a key, then give the player the key
                if(currentPickup == PickupType.Key) {
                    hasKey = true;
                    touchingPickup.gameObject.SetActive(false); //deactivates the pickup so it can't be picked up again
                }
            }
        }
    }

    // -- COLLISIONS --

    //pickup and obstacle collisions
    private void OnTriggerEnter2D(Collider2D collision) {
        //detects a pickup
        if(collision.CompareTag("Pickup")) {
            touchingPickup = collision;
        //detects collision with an obstacle (spikes) and deals damage to kill the player
        } else if(collision.CompareTag("Obstacle")) {
            healthManager.TakeDamage(100);
        //goes through a falling cutoff; tell the camera follow to stop following so it will trigger a respawn
        } else if(collision.CompareTag("Falling Cutoff")) {
            CameraFollow mainCameraFollow = GameObject.FindObjectOfType<CameraFollow>();
            if(mainCameraFollow != null) {
                mainCameraFollow.playerFall(); //triggers everything to respawn as if the player died
            }
        }
    }
    //exiting the pickup collider
    private void OnTriggerExit2D(Collider2D collision) {
        //no longer touching the pickup
        if(collision.CompareTag("Pickup")) {
            touchingPickup = null;
        }
    }

    // -- ATTACKING --

    //called by the attack animation at proper frame in attack animation; calls Attack to damage any enemies in the weapon's hitbox
    public void AttackTrigger() {
        Attack(currentWeapon, facingDirection);
    }
    //called by AttackTrigger to deal damage; animation handled above
    public void Attack(WeaponType currentWeapon, int facingDirection) {
        Collider2D[] enemiesToDamage = null;
        //initialize required variables to useless values
        Transform attackPos = null;
        float attackRadius = 0f;
        float damage = 0;
        //set attackPos, radius, and damage depending on the current weapon and the facingDirection
        //sword
        if(currentWeapon == WeaponType.Sword) {
            if(facingDirection == 1) //facing right
                attackPos = swordAttackRightPos;
            else if(facingDirection == -1) //facing left
                attackPos = swordAttackLeftPos;
            attackRadius = swordAttackRadius;
            damage = 1;
        //claymore or club
        } else if(currentWeapon == WeaponType.Claymore || currentWeapon == WeaponType.Club) {
            //claymore and club share weapon hitboxes
            if(facingDirection == 1) //facing right
                attackPos = claymoreClubAttackRightPos;
            else if(facingDirection == -1) //facing left
                attackPos = claymoreClubAttackLeftPos;
            attackRadius = claymoreClubAttackRadius;
            //claymore and club do different damage
            if(currentWeapon == WeaponType.Claymore)
                damage = 3;
            else if(currentWeapon == WeaponType.Club)
                damage = 2;
        //spear
        } else if(currentWeapon == WeaponType.Spear) {
            if(facingDirection == 1) //facing right
                attackPos = spearAttackRightPos;
            else if(facingDirection == -1) //facing left
                attackPos = spearAttackLeftPos;
            attackRadius = spearAttackRadius;
            damage = 0.5f;
        }
        
        //determine who is in attack hitbox and damage each of them
        enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRadius, enemyLayers); //makes array of anyone in the enemyLayers layer mask within the created circle (center point, radius, layer)
        foreach(Collider2D enemy in enemiesToDamage) {
            //if the target is an enemy, make them take damage
            if(enemy.CompareTag("Enemy"))
                enemy.GetComponent<HealthManager>().TakeDamage(damage);
            //if the target is a destructible, attempt to destroy it
            else if(enemy.CompareTag("Destructible"))
                enemy.GetComponent<Destructible>().DestroyObject(currentWeapon);
        }
        audioSource.PlayOneShot(weaponSwingSounds[(int)currentWeapon]); //play weapon's attack sound
    }
    //called at beginning and end of attack animation to signal when it is attacking
    public void SetAttacking(int isAttacking) {
        attacking = (isAttacking > 0); //converts int to bool
    }

    // -- MISC. --

    //called at end of death animation to trigger everything else respawning
    public void BeginRespawn() {
        playerRespawning = true;
        hasKey = false;
    }

    //draws representation of the attack area when selected in the editor
    private void OnDrawGizmosSelected() {
        //draw sword hitboxes
        if(swordAttackRightPos != null) {
            Gizmos.DrawWireSphere(swordAttackRightPos.position, swordAttackRadius);
        }
        if(swordAttackLeftPos != null) {
            Gizmos.DrawWireSphere(swordAttackLeftPos.position, swordAttackRadius);
        }

        //draw claymore and club hitboxes
        if(claymoreClubAttackRightPos != null) {
            Gizmos.DrawWireSphere(claymoreClubAttackRightPos.position, claymoreClubAttackRadius);
        }
        if(claymoreClubAttackLeftPos != null) {
            Gizmos.DrawWireSphere(claymoreClubAttackLeftPos.position, claymoreClubAttackRadius);
        }

        //draw spear hitboxes
        if(spearAttackRightPos != null) {
            Gizmos.DrawWireSphere(spearAttackRightPos.position, spearAttackRadius);
        }
        if(spearAttackLeftPos != null) {
            Gizmos.DrawWireSphere(spearAttackLeftPos.position, spearAttackRadius);
        }
    }
}