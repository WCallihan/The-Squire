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
    private bool attacking = false;
    private float delayToIdle = 0f;
    private Collider2D touchingPickup;
    private bool grounded = false;
    private int facingDirection = 1;

    public bool hasKey;

    [SerializeField] Transform swordAttackLeftPos;
    [SerializeField] Transform swordAttackRightPos;
    [SerializeField] float swordAttackRadius = 0.5f;
    [SerializeField] Transform spearAttackLeftPos;
    [SerializeField] Transform spearAttackRightPos;
    [SerializeField] float spearAttackRadius = 0.5f;
    [SerializeField] Transform claymoreClubAttackLeftPos;
    [SerializeField] Transform claymoreClubAttackRightPos;
    [SerializeField] float claymoreClubAttackRadius = 0.5f;
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
        if(!healthManager.isDead) {
            // Increase timer that controls attack combo
            timeSinceAttack += Time.deltaTime;

            //controls the grounded variable and sets the animator boolean
            grounded = groundSensor.OnGround();
            animator.SetBool("Grounded", grounded);

            //Horizontal Input
            float inputX = 0;
            if(!attacking || !grounded) { //can move if not attacking unless in the air
                inputX = Input.GetAxis("Horizontal");
            }

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
            if(multipleWeapons && !attacking) {
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
            else if((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && grounded && !attacking) {
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
        Collider2D[] enemiesToDamage = null;
        Transform attackPos = swordAttackRightPos;
        float attackRadius = swordAttackRadius;
        float damage = 1;
        if(currentWeapon == WeaponType.Sword) {
            if(facingDirection == 1) //facing right
                attackPos = swordAttackRightPos;
            else if(facingDirection == -1) //facing left
                attackPos = swordAttackLeftPos;
            attackRadius = swordAttackRadius;
            damage = 1;
        } else if(currentWeapon == WeaponType.Claymore) {
            if(facingDirection == 1) //facing right
                attackPos = claymoreClubAttackRightPos;
            else if(facingDirection == -1) //facing left
                attackPos = claymoreClubAttackLeftPos;
            attackRadius = claymoreClubAttackRadius;
            damage = 3;
        } else if(currentWeapon == WeaponType.Spear) {
            if(facingDirection == 1) //facing right
                attackPos = spearAttackRightPos;
            else if(facingDirection == -1) //facing left
                attackPos = spearAttackLeftPos;
            attackRadius = spearAttackRadius;
            damage = 0.5f;
        } else if(currentWeapon == WeaponType.Club) {
            if(facingDirection == 1) //facing right
                attackPos = claymoreClubAttackRightPos;
            else if(facingDirection == -1) //facing left
                attackPos = claymoreClubAttackLeftPos;
            attackRadius = claymoreClubAttackRadius;
            damage = 2;
        }
        
        enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRadius, enemyLayers); //makes array of anyone in the enemyLayers layer mask within the created circle (center point, radius, layer)
        foreach(Collider2D enemy in enemiesToDamage) {
            if(enemy.CompareTag("Enemy"))
                enemy.GetComponent<HealthManager>().TakeDamage(damage);
            else if(enemy.CompareTag("Destructible"))
                enemy.GetComponent<Destructible>().DestroyObject(currentWeapon);
        }
        audioSource.PlayOneShot(weaponSwingSounds[(int)currentWeapon]);
    }
    //called at beginning and end of attack animation to signal when it is attacking
    public void SetAttacking(int isAttacking) {
        attacking = (isAttacking > 0) ? true : false;
    }

    //called at end of death animation to trigger everything else respawning
    public void BeginRespawn() {
        playerRespawning = true;
        hasKey = false;
    }


    //draws representation of the attack area when selected in the editor
    private void OnDrawGizmosSelected() {
        if(swordAttackRightPos != null) {
            Gizmos.DrawWireSphere(swordAttackRightPos.position, swordAttackRadius);
        }
        if(swordAttackLeftPos != null) {
            Gizmos.DrawWireSphere(swordAttackLeftPos.position, swordAttackRadius);
        }

        if(spearAttackRightPos != null) {
            Gizmos.DrawWireSphere(spearAttackRightPos.position, spearAttackRadius);
        }
        if(spearAttackLeftPos != null) {
            Gizmos.DrawWireSphere(spearAttackLeftPos.position, spearAttackRadius);
        }

        if(claymoreClubAttackRightPos != null) {
            Gizmos.DrawWireSphere(claymoreClubAttackRightPos.position, claymoreClubAttackRadius);
        }
        if(claymoreClubAttackLeftPos != null) {
            Gizmos.DrawWireSphere(claymoreClubAttackLeftPos.position, claymoreClubAttackRadius);
        }
    }
}