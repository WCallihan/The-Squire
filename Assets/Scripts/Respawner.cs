using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawner : MonoBehaviour {

    private Vector3 startingPosition;
    private Vector3 respawnPosition;
    private SpriteRenderer spriteRenderer;
    private bool spriteFlipped;

    void Start() {
        startingPosition = transform.position;
        respawnPosition = startingPosition;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteFlipped = spriteRenderer.flipX;
    }

    //called by LevelManager when the player dies and respawns; puts everything back to where it was at the last checkpoint
    public void Respawn() {
        transform.position = respawnPosition;
        spriteRenderer.flipX = spriteFlipped;

        HealthManager healthScript = GetComponent<HealthManager>();
        if(healthScript != null) {
            healthScript.Respawn();
        }
    }
}