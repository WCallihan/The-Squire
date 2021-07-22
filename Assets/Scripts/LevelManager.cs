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

    //finds all respawnable objects and tells them to respawn
    private void RespawnCheckpoint() {
        foreach(var respawnableObject in respawnableObjects) {
            respawnableObject.SetActive(true);
            respawnableObject.GetComponent<Respawner>().Respawn();
        }
    }
}