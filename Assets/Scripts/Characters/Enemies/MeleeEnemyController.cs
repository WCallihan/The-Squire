using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Used only by the melee enemy characters:
 * manages everything to do with the melee enemy including
 * their movement, attacking, patroling, detecting the player
 * and sound effects
 */

public class MeleeEnemyController : MonoBehaviour {

    [SerializeField] float speed;
    [SerializeField] Vector2 moveVector;
    [SerializeField] float attackCooldown;
    [SerializeField] float attackDistance;

    [SerializeField] AudioClip swordSwipeSound;

    [SerializeField] GameObject player;
    private HealthManager healthManager;
    private Animator animator;
    private Rigidbody2D enemyRb;
    private AudioSource audioSource;

    public Vector2 patrolLeftPos, patrolRightPos;
    private Vector2 currentWaypoint;
    private bool followingPlayer = false;

    private bool attacking = false;
    private float timeSinceAttack = 0;
    private bool playerInReach = false;
    private int facingDirection = 1;

    [SerializeField] Transform attackLeftPos;
    [SerializeField] Transform attackRightPos;
    [SerializeField] float attackRange = 0.5f;
    [SerializeField] LayerMask enemyLayers;
    [SerializeField] float attackDamage = 1;

    void Start() {
        healthManager = GetComponent<HealthManager>();
        animator = GetComponent<Animator>();
        enemyRb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        //audioSource.volume = PlayerPrefs.GetFloat("EffectsVolume", 1);
        currentWaypoint = patrolLeftPos;
    }

    void Update() {
        if(/*gameManager.gameRunning &&*/ !healthManager.isDead) {
            //Set moveVector to the next patrol waypoint or player if within patrol range
            if(player != null && //if a player enemy is assigned
                (player.transform.position.x >= patrolLeftPos.x && player.transform.position.x <= patrolRightPos.x) && //if the player is between the patrol points
                (Mathf.Abs(player.transform.position.y - transform.position.y)) <= 4f) //if the player is on the same level or is jumping/falling
                {
                moveVector = new Vector2(player.transform.position.x - transform.position.x, 0);
                followingPlayer = true;
            } else {
                moveVector = new Vector2(currentWaypoint.x - transform.position.x, 0);
                followingPlayer = false;
            }

            //Set currentWaypoint if one is reached while patroling
            if(Mathf.Abs(moveVector.x) <= 0.05f) {
                moveVector = new Vector2(0, 0);
                if(currentWaypoint == patrolLeftPos) {
                    currentWaypoint = patrolRightPos;
                } else {
                    currentWaypoint = patrolLeftPos;
                }
            }

            //Swap direction of sprite depending on walk direction
            if(moveVector.x > 0) {
                GetComponent<SpriteRenderer>().flipX = true;
                facingDirection = 1;
            } else if(moveVector.x < 0) {
                GetComponent<SpriteRenderer>().flipX = false;
                facingDirection = -1;
            }

            //Set playerInReach
            if(followingPlayer && Mathf.Abs(moveVector.x) <= attackDistance) {
                playerInReach = true;
            } else {
                playerInReach = false;
            }
            // Move
            if(!playerInReach && !attacking) {
                enemyRb.velocity = moveVector.normalized * new Vector2(speed, 0);
            } else {
                enemyRb.velocity = Vector2.zero;
            }

            timeSinceAttack += Time.deltaTime;
            //Attack when close to the player
            if(playerInReach && !attacking && timeSinceAttack >= attackCooldown) {
                attacking = true;
                animator.SetTrigger("Attack"); //actual attack triggered by animation
                timeSinceAttack = 0f; //resets attack timer
            }

            //Run
            if(Mathf.Abs(moveVector.magnitude) > Mathf.Epsilon && !playerInReach) {
                animator.SetInteger("AnimState", 1);
            }

            //Idle
            else {
                animator.SetInteger("AnimState", 0);
            }
        } else if(healthManager.isDead) {
            StartCoroutine(Die());
        } else {
            animator.SetInteger("AnimState", 0); //idle when the player dies/game is not running
        }
    }

    //called by the attack animation at proper frame
    public void AttackTrigger() {
        Attack(facingDirection);
    }
    //deal damage depending on facingDirection
    public void Attack(int facingDirection) {
        Collider2D[] enemiesToDamage = null;
        if(facingDirection == 1) { //facing right
            enemiesToDamage = Physics2D.OverlapCircleAll(attackRightPos.position, attackRange, enemyLayers); //makes array of anyone in the enemyLayers layer mask within the created circle (center point, radius, layer)
        } else if(facingDirection == -1) { //facing left
            enemiesToDamage = Physics2D.OverlapCircleAll(attackLeftPos.position, attackRange, enemyLayers);
        } else {
            Debug.Log("Melee failed");
        }
        foreach(Collider2D enemy in enemiesToDamage) {
            if(enemiesToDamage != null)
                enemy.GetComponent<HealthManager>().TakeDamage(attackDamage);
        }
        audioSource.PlayOneShot(swordSwipeSound);
    }
    //called at beginning and end of attack animation to signal when it is attacking
    public void SetAttacking(int isAttacking) {
        attacking = (isAttacking > 0) ? true : false ;
    }

    //called when the enemy dies; despawns the corpse after a few seconds
    IEnumerator Die() {
        SetAttacking(0);
        yield return new WaitForSeconds(5);
        if(healthManager.isDead) { //makes sure it hasn't respawned in that time
            gameObject.SetActive(false);
        }
    }

    //draws representation of the attack area when the object is selected in the editor
    private void OnDrawGizmosSelected() {
        if(attackRightPos != null) {
            Gizmos.DrawWireSphere(attackRightPos.position, attackRange);
        }
        if(attackLeftPos != null) {
            Gizmos.DrawWireSphere(attackLeftPos.position, attackRange);
        }
    }
}