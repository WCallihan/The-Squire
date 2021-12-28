using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Used by all respawnable objects:
 * manages all relevant information pertaining to an object
 * to put back into the state it was in at the last checkpoint
 * reached by the player (including the beginning of levels)
 */

public class Respawner : MonoBehaviour {

    private Vector3 startingPosition;
    private Vector3 respawnPosition;
    private Quaternion startingRotation;
    private SpriteRenderer spriteRenderer;
    private bool spriteFlipped;

    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetCheckpoint();
    }

    //called by LevelManager when the player touches a new checkpoint; also called when the level is loaded
    public void SetCheckpoint() {
        startingPosition = transform.position;
        respawnPosition = startingPosition;
        startingRotation = transform.rotation;
        if(spriteRenderer != null) { //clause for stuff like the breakable bridge
            spriteFlipped = spriteRenderer.flipX;
        }
    }

    //called by LevelManager when the player dies and respawns; puts everything back to where it was at the last checkpoint
    public void Respawn() {
        Rigidbody2D objectRb = gameObject.GetComponent<Rigidbody2D>();
        if(objectRb != null && objectRb.bodyType != RigidbodyType2D.Static) //if the rigidbody on the object is not static, then reset velocity
            objectRb.velocity = Vector3.zero;
        transform.position = respawnPosition;
        transform.rotation = startingRotation;
        if(spriteRenderer != null) { //clause for stuff like the breakable bridge
            spriteRenderer.flipX = spriteFlipped;
        }

        //targets npc enemies that need to be respawned
        HealthManager healthScript = GetComponent<HealthManager>();
        if(healthScript != null) {
            healthScript.Respawn();
        }

        //targets the boulders that were smashed and need to be respawned
        Boulder boulderScript = GetComponent<Boulder>();
        if(boulderScript != null) {
            boulderScript.RespawnBoulder();
        }
    }
}