using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Used by both player characters:
 * manages everything to do with the player including their
 * inputs, movement, ground sensor, attacking, and sound
 * effects
 */

public enum WeaponType { Sword, Claymore, Spear, Club }

public class PlayerController : MonoBehaviour {

    [SerializeField] float speed = 4f;
    [SerializeField] float jumpForce = 7.5f;

    [SerializeField] AudioClip[] weaponSwingSounds;
    public WeaponType currentWeapon = WeaponType.Sword; //unchanging on HeroKnight
    [SerializeField] bool multipleWeapons = true; //true on Squire, false on HeroKnight

    private HealthManager healthManager;
    private HUDManager hudManager;
    private Animator animator;
    private Rigidbody2D playerRb;
    private AudioSource audioSource;
    private GroundSensor groundSensor;

    private float timeSinceAttack = 0f;
    private float attackCooldown = 0.25f;
    private float delayToIdle = 0f;
    private Collider2D touchingPickup;
    private bool grounded = false;
    private int facingDirection = 1;

    public bool hasKey;

    [SerializeField] Transform attackLeftPos;
    [SerializeField] Transform attackRightPos;
    [SerializeField] float attackRange;
    [SerializeField] LayerMask enemyLayers;

    public bool playerRespawning = false;

    void Start() {
        healthManager = GetComponent<HealthManager>();
        hudManager = GetComponent<HUDManager>();
        animator = GetComponent<Animator>();
        playerRb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        groundSensor = transform.Find("GroundSensor").GetComponent<GroundSensor>();
    }

    void Update() {
        if(/*gameManager.gameRunning &&*/ !healthManager.isDead) {
            // Increase timer that controls attack combo
            timeSinceAttack += Time.deltaTime;

            //controls the grounded variable and sets the animator boolean
            grounded = groundSensor.OnGround();
            animator.SetBool("Grounded", grounded);

            //Horizontal Input
            float inputX = Input.GetAxis("Horizontal");

            //Swap direction of sprite depending on walk direction
            if(inputX > 0) {
                GetComponent<SpriteRenderer>().flipX = false;
                facingDirection = 1; //facing right
            } else if(inputX < 0) {
                GetComponent<SpriteRenderer>().flipX = true;
                facingDirection = -1; //facing left
            }

            //Move
            playerRb.velocity = new Vector2(inputX * speed, playerRb.velocity.y);

            //Set AirSpeed in animator
            animator.SetFloat("AirSpeedY", playerRb.velocity.y);

            // -- Handle Animations --
            //Change Weapons
            if(multipleWeapons) {
                if(Input.GetKeyDown(KeyCode.Alpha1)) {
                    currentWeapon = WeaponType.Sword;
                    hudManager.UpdateWeaponSelected(WeaponType.Sword);
                } else if(Input.GetKeyDown(KeyCode.Alpha2)) {
                    currentWeapon = WeaponType.Claymore;
                    hudManager.UpdateWeaponSelected(WeaponType.Claymore);
                } else if(Input.GetKeyDown(KeyCode.Alpha3)) {
                    currentWeapon = WeaponType.Spear;
                    hudManager.UpdateWeaponSelected(WeaponType.Spear);
                } else if(Input.GetKeyDown(KeyCode.Alpha4)) {
                    currentWeapon = WeaponType.Club;
                    hudManager.UpdateWeaponSelected(WeaponType.Club);
                }
            }
            //Attack
            if(Input.GetMouseButtonDown(0) && timeSinceAttack >= attackCooldown) {
                if(multipleWeapons)
                    animator.SetInteger("WeaponType", (int)currentWeapon); //parameter only on the Squire
                animator.SetTrigger("Attack"); //actual attack triggered by animation
                timeSinceAttack = 0.0f; //resets attack timer
            }

            //Jump
            else if((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && grounded) {
                animator.SetTrigger("Jump");
                playerRb.velocity = new Vector2(playerRb.velocity.x, jumpForce);
                groundSensor.Disable(0.2f); //sets onGround to false for 0.2 seconds
            }

            //Run
            else if(Mathf.Abs(inputX) > Mathf.Epsilon) {
                // Reset timer
                delayToIdle = 0.05f;
                animator.SetInteger("AnimState", 1);
            }

            //Idle
            else {
                // Prevents flickering transitions to idle
                delayToIdle -= Time.deltaTime;
                if(delayToIdle < 0)
                    animator.SetInteger("AnimState", 0);
            }

            //Picking up pickups
            if(Input.GetKeyDown(KeyCode.E) && touchingPickup != null) {
                PickupType currentPickup = touchingPickup.gameObject.GetComponent<Pickups>().pickupType;
                audioSource.PlayOneShot(touchingPickup.gameObject.GetComponent<Pickups>().pickupSound);
                if(currentPickup == PickupType.Key) {
                    hasKey = true;
                    touchingPickup.gameObject.SetActive(false);
                }
            }
        }
    }

    //Pickup and Obstacle Collisions
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.CompareTag("Pickup")) {
            touchingPickup = collision;
        } else if(collision.CompareTag("Obstacle")) {
            healthManager.TakeDamage(100);
        } else if(collision.CompareTag("Falling Cutoff")) {
            CameraFollow mainCameraFollow = GameObject.FindObjectOfType<CameraFollow>();
            if(mainCameraFollow != null) {
                mainCameraFollow.playerFall();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision) {
        if(collision.CompareTag("Pickup")) {
            touchingPickup = null;
        }
    }

    //called by the animation at proper moment in attack animation; calls Attack and launches projectiles
    public void AttackTrigger() {
        Attack(currentWeapon, facingDirection);
    }
    //called by AttackTrigger to deal damage, animation handled above
    public void Attack(WeaponType currentWeapon, int facingDirection) {
        /*Collider2D[] enemiesToDamage = null;
        if(facingDirection == 1) { //facing right
            enemiesToDamage = Physics2D.OverlapCircleAll(attackRightPos.position, attackRange, enemyLayers); //makes array of anyone in the enemyLayers layer mask within the created circle (center point, radius, layer)
        } else if(facingDirection == -1) { //facing left
            enemiesToDamage = Physics2D.OverlapCircleAll(attackLeftPos.position, attackRange, enemyLayers);
        } else {
            Debug.Log("Melee failed");
        }
        float damage = 1;
        if(currentWeapon == WeaponType.Sword)
            damage = 1;
        else if(currentWeapon == WeaponType.Claymore)
            damage = 3;
        else if(currentWeapon == WeaponType.Spear)
            damage = 0.5f;
        else if(currentWeapon == WeaponType.Club)
            damage = 2;
        foreach(Collider2D enemy in enemiesToDamage) {
            enemy.GetComponent<HealthManager>().TakeDamage(damage);
        }*/
        audioSource.PlayOneShot(weaponSwingSounds[(int)currentWeapon]);
    }

    //called at end of death animation to trigger everything else respawning
    public void BeginRespawn() {
        playerRespawning = true;
        hasKey = false;
    }


    //draws representation of the attack area when selected in the editor
    private void OnDrawGizmosSelected() {
        if(attackRightPos != null) {
            Gizmos.DrawWireSphere(attackRightPos.position, attackRange);
        }
        if(attackLeftPos != null) {
            Gizmos.DrawWireSphere(attackLeftPos.position, attackRange);
        }
    }
}