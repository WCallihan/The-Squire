using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                checkpointActivated = true;
                checkpointAnimator.SetTrigger("RaiseFlag");
                levelManager.SetCheckpoint();
            }
        }
    }
}