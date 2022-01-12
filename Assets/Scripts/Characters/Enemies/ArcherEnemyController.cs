using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Used only by the archer enemy characters:
 * manages everything to do with the archer enemy including their attacking,
 * look direction, detecting the player's location, animations,
 * spawning their arrow projectiles, and sound effects
 */

public class ArcherEnemyController : MonoBehaviour {

    [SerializeField] float attackCooldown;
    [SerializeField] Vector2 vectorToPlayer;

    [SerializeField] AudioClip shootArrowSound;

    [SerializeField] GameObject player;
    private HealthManager healthManager;
    private Animator animator;
    private AudioSource audioSource;

    private bool attacking = false;
    private float timeSinceAttack = 0f;
    private bool playerInRange = false;
    private int facingDirection = 1;
    private int bowDirection = 1;

    //variables that determine the spawn locations of the arrows depending on where it is trying to shoot
    [SerializeField] Transform sideLeftArrowSpawn;
    [SerializeField] Transform sideRightArrowSpawn;
    [SerializeField] Transform downLeftArrowSpawn;
    [SerializeField] Transform downRightArrowSpawn;
    [SerializeField] GameObject arrowPrefab; //arrow prefab to be spawned when attacking
    [SerializeField] float arrowRange;
    [SerializeField] GameObject arrowRangeCollider; //collider used to stop arrows from exceeding their range
    [SerializeField] LayerMask enemyLayers; //denotes which layers can be hit
    [SerializeField] float arrowDamage;

    private Transform arrowSpawnPos;
    private float arrowAngle;
    private string attackAnimation;

    void Start() {
        healthManager = GetComponent<HealthManager>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update() {
        if(!healthManager.isDead) {

            // -- ORIENTATION --

            //set vectorToPlayer if they are within range and on level or below the archer
            if(player != null) {
                vectorToPlayer = player.transform.position - transform.position;
                //checks if the player is in range then if they are not too far above the archer's level; also makes sure the player isn't dead
                if(vectorToPlayer.magnitude <= arrowRange && vectorToPlayer.y <= 2 && !player.GetComponent<HealthManager>().isDead) {
                    if(vectorToPlayer.y >= -1.5f) { //"on level" with or above the arrow, then shoot sideways
                        bowDirection = 1; //sideways
                        attackAnimation = "AttackSide";
                    } else { //below the arrow, then shoot down
                        bowDirection = -1; //down
                        attackAnimation = "AttackDown";
                    }
                    playerInRange = true;
                } else {
                    playerInRange = false;
                }
            }

            //swap direction of sprite depending on direction to player
            if(playerInRange && vectorToPlayer.x > 0) {
                GetComponent<SpriteRenderer>().flipX = true;
                facingDirection = 1;  //facing right
            } else if(playerInRange && vectorToPlayer.x < 0) {
                GetComponent<SpriteRenderer>().flipX = false;
                facingDirection = -1;  //facing left
            }

            // -- ANIMATIONS --

            //increase timer that controls attack cooldown
            timeSinceAttack += Time.deltaTime;

            //attack when the player is in range
            if(playerInRange && !attacking && timeSinceAttack >= attackCooldown) {
                attacking = true;
                animator.SetTrigger(attackAnimation); //projectile spawning triggered by animation, which is determined by the player's position
                timeSinceAttack = 0f; //resets attack timer
            }

            //idle if not attacking
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

    //called by attack animation; spawns an arrow that follows the vectorToPlayer
    public void SpawnArrow() {
        SetArrowSpawnPos(); //sets the arrow's spawn point
        //spawns the arrow in the set position and at the set angle and assigns it it's damage and range collider
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPos.transform.position, arrowPrefab.transform.rotation * Quaternion.Euler(0f, 0f, arrowAngle));
        arrow.GetComponent<Projectile>().arrowDamage = arrowDamage;
        arrow.GetComponent<Projectile>().arrowRangeCollider = arrowRangeCollider;
        audioSource.PlayOneShot(shootArrowSound); //play attack sound
    }
    //called by SpawnArrow to set the spawn position and angle of the arrow depending on where the player is
    private void SetArrowSpawnPos() {
        //gets angle to the player
        arrowAngle = Vector2.SignedAngle(Vector2.right, vectorToPlayer);

        //finds which direction the bow is facing and if it is up or down and clamps the arrow's angle between set values that make sense for the archer
        if(bowDirection == 1) { //aiming sideways
            if(facingDirection == 1) { //facing right
                arrowSpawnPos = sideRightArrowSpawn;
                arrowAngle = Mathf.Clamp(arrowAngle, -10f, 10f);
            } else if(facingDirection == -1) { //facing left
                arrowSpawnPos = sideLeftArrowSpawn;
                arrowAngle = Mathf.Clamp(arrowAngle, 170f, 190f);
            }
        } else if(bowDirection == -1) { //aiming down
            if(facingDirection == 1) { //facing right
                arrowSpawnPos = downRightArrowSpawn;
                arrowAngle = Mathf.Clamp(arrowAngle, -60f, -20f);
            } else if(facingDirection == -1) { //facing left
                arrowSpawnPos = downLeftArrowSpawn;
                arrowAngle = Mathf.Clamp(arrowAngle, -160f, -120f);
            }
        }
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

    //draws representations of the arrow range and arrow spawn positions when the object is selected in the editor
    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, arrowRange); //draws arrow range circle
        //draw very small circles for the arrow spawn positions
        if(sideLeftArrowSpawn != null) {
            Gizmos.DrawWireSphere(sideLeftArrowSpawn.transform.position, 0.1f);
        }
        if(sideRightArrowSpawn != null) {
            Gizmos.DrawWireSphere(sideRightArrowSpawn.transform.position, 0.1f);
        }
        if(downLeftArrowSpawn != null) {
            Gizmos.DrawWireSphere(downLeftArrowSpawn.transform.position, 0.1f);
        }
        if(downRightArrowSpawn != null) {
            Gizmos.DrawWireSphere(downRightArrowSpawn.transform.position, 0.1f);
        }
    }
}