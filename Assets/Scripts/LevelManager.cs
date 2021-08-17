using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    [SerializeField] GameObject player;
    private PlayerController playerController;

    public GameObject[] respawnableObjects;

    void Start() {
        playerController = player.GetComponent<PlayerController>();
    }

    //constantly checks if the player is respawning, and then tells all other objects to respawn if able
    void Update() {
        if(playerController.playerRespawning) {
            playerController.playerRespawning = false;
            RespawnCheckpoint();
        }
    }

    //called by Checkpoint when the player touches it for the first time
    public void SetCheckpoint() {
        foreach(var respawnObject in respawnableObjects) {
            if(respawnObject.activeInHierarchy) {
                respawnObject.GetComponent<Respawner>().SetCheckpoint();
            }
        }
    }

    //finds all respawnable objects and tells them to respawn
    private void RespawnCheckpoint() {
        foreach(var respawnableObject in respawnableObjects) {
            respawnableObject.SetActive(true);
            respawnableObject.GetComponent<Respawner>().Respawn();
        }
    }
}