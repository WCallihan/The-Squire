using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Used only by the melee enemy characters:
 * manages everything to do with the melee enemy including their movement,
 * attacking, patroling, detecting the player, animations, and sound effects
 */

public class MeleeEnemyController : MonoBehaviour {

    [SerializeField] float speed;
    [SerializeField] Vector2 moveVector;
    [SerializeField] float attackCooldown;
    [SerializeField] float attackDistance; //used to determine if the player is in range to attack

    [SerializeField] AudioClip weaponSwipeSound;

    [SerializeField] GameObject player;
    private HealthManager healthManager;
    private Animator animator;
    private Rigidbody2D enemyRb;
    private AudioSource audioSource;

    //variables used to patrol and denote whether the enemy has detected the player and is following
    [SerializeField] Vector2 patrolLeftPos, patrolRightPos;
    private Vector2 currentWaypoint;
    private bool followingPlayer = false;

    private bool attacking = false;
    private float timeSinceAttack = 0;
    private bool playerInReach = false;
    private int facingDirection = 1;

    //variables that determine the enemy's attack hitbox and its damage
    [SerializeField] Transform attackLeftPos;
    [SerializeField] Transform attackRightPos;
    [SerializeField] float attackRadius = 0.5f;
    [SerializeField] LayerMask enemyLayers; //denotes which layers can be hit
    [SerializeField] float attackDamage = 1;

    void Start() {
        healthManager = GetComponent<HealthManager>();
        animator = GetComponent<Animator>();
        enemyRb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        currentWaypoint = patrolLeftPos; //starts by patrolling to the left
    }

    void Update() {
        if(!healthManager.isDead) {

            // -- MOVEMENT --

            //set moveVector to the next patrol waypoint or towards the player if within patrol range
            if(player != null && //if a player is assigned
                (player.transform.position.x >= patrolLeftPos.x - attackDistance && player.transform.position.x <= patrolRightPos.x + attackDistance) && //if the player is between the patrol points
                (Mathf.Abs(player.transform.position.y - transform.position.y)) <= 3f && //if the player is on the same level or is jumping/falling
                (!player.GetComponent<HealthManager>().isDead)) //if the player is not dead
                {
                moveVector = new Vector2(player.transform.position.x - transform.position.x, 0); //set the moveVector towards the player
                followingPlayer = true;
            } else {
                moveVector = new Vector2(currentWaypoint.x - transform.position.x, 0); //set the moveVector towards thje next patrol waypoint
                followingPlayer = false;
            }

            //set next waypoint if one is reached while patroling
            if(Mathf.Abs(moveVector.x) <= 0.05f) {
                moveVector = new Vector2(0, 0);
                if(currentWaypoint == patrolLeftPos) {
                    currentWaypoint = patrolRightPos;
                } else {
                    currentWaypoint = patrolLeftPos;
                }
            }

            //swap direction of sprite depending on walk direction
            if(moveVector.x > 0) {
                GetComponent<SpriteRenderer>().flipX = true;
                facingDirection = 1; //facing right
            } else if(moveVector.x < 0) {
                GetComponent<SpriteRenderer>().flipX = false;
                facingDirection = -1; //facing left
            }

            //set playerInReach
            if(followingPlayer && Mathf.Abs(moveVector.x) <= attackDistance) {
                playerInReach = true;
            } else {
                playerInReach = false;
            }

            //move
            if(!playerInReach && !attacking) {
                enemyRb.velocity = moveVector.normalized * new Vector2(speed, 0);
            } else {
                enemyRb.velocity = Vector2.zero;
            }

            // -- ANIMATIONS --

            //increase timer that controls attack cooldown
            timeSinceAttack += Time.deltaTime;

            //attack when close to the player
            if(playerInReach && !attacking && timeSinceAttack >= attackCooldown) {
                attacking = true;
                animator.SetTrigger("Attack"); //damage triggered by animation
                timeSinceAttack = 0f; //resets attack timer
            }

            //run if moveVector is not 0 and not near player
            if(Mathf.Abs(moveVector.magnitude) > Mathf.Epsilon && !playerInReach) {
                animator.SetInteger("AnimState", 1);
            }

            //idle if not running or attacking
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

    //called by the attack animation at proper frame; calls Attack to damage any enemies in the weapon's hitbox
    public void AttackTrigger() {
        Attack(facingDirection);
    }
    //called by AttackTrigger to deal damage; animation handled above
    public void Attack(int facingDirection) {
        Collider2D[] enemiesToDamage = null;
        //determine who is in attack hitbox and damage each of them
        if(facingDirection == 1) { //facing right
            enemiesToDamage = Physics2D.OverlapCircleAll(attackRightPos.position, attackRadius, enemyLayers); //makes array of anyone in the enemyLayers layer mask within the created circle (center point, radius, layer)
        } else if(facingDirection == -1) { //facing left
            enemiesToDamage = Physics2D.OverlapCircleAll(attackLeftPos.position, attackRadius, enemyLayers);
        } else {
            Debug.Log("Melee failed");
        }
        foreach(Collider2D enemy in enemiesToDamage) {
            if(enemiesToDamage != null)
                enemy.GetComponent<HealthManager>().TakeDamage(attackDamage); //assumes that any enemy of the enemy has a HealthManager
        }
        audioSource.PlayOneShot(weaponSwipeSound); //play attack sound
    }
    //called at beginning and end of attack animation to signal when it is attacking
    public void SetAttacking(int isAttacking) {
        attacking = (isAttacking > 0); //converts int to bool
    }

    // -- MISC. --

    //called when the enemy dies; despawns the corpse after a few seconds
    IEnumerator Die() {
        SetAttacking(0);
        yield return new WaitForSeconds(5); //waits 5 seconds to let the body linger
        if(healthManager.isDead) { //makes sure it hasn't respawned in that time
            gameObject.SetActive(false);
        }
    }

    //draws representation of the attack area when the object is selected in the editor
    private void OnDrawGizmosSelected() {
        if(attackRightPos != null) {
            Gizmos.DrawWireSphere(attackRightPos.position, attackRadius);
        }
        if(attackLeftPos != null) {
            Gizmos.DrawWireSphere(attackLeftPos.position, attackRadius);
        }
    }
}