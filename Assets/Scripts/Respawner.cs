using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawner : MonoBehaviour {

    private Vector3 startingPosition;
    private Vector3 respawnPosition;

    void Start() {
        startingPosition = transform.position;
        respawnPosition = startingPosition;
    }

    //called by LevelManager when the player dies and respawns; puts everything back to where it was at the last checkpoint
    public void Respawn() {
        transform.position = respawnPosition;

        HealthManager healthScript = GetComponent<HealthManager>();
        if(healthScript != null) {
            healthScript.Respawn();
        }
    }
}