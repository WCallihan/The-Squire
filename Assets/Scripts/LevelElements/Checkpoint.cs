using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Used by all Checkpoint objects:
 * contacts the LevelManager to set that checkpoint as the new spawn point
 * for the player and reset all respawn points to their state
 * when the player touches the Checkpoint
 */

public class Checkpoint : MonoBehaviour {

    [SerializeField] LevelManager levelManager;
    private Animator checkpointAnimator;
    private bool checkpointActivated = false;

    void Start() {
        checkpointAnimator = GetComponent<Animator>();
    }

    //the first time the player touches the checkpoint, the flag animation plays and the LevelManager sets the checkpoint for respawn
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("Player")) {
            if(!checkpointActivated) {
                checkpointActivated = true; //can only activate the checkpoint once
                checkpointAnimator.SetTrigger("RaiseFlag"); //play the flag animation
                levelManager.SetCheckpoint(); //sets a new respawn state for all respawnable objects
            }
        }
    }
}