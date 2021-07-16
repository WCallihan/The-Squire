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

    void Update() {
        if(playerController.playerRespawning) {
            playerController.playerRespawning = false;
            RespawnCheckpoint();
        }
    }

    private void RespawnCheckpoint() {
        foreach(var respawnableObject in respawnableObjects) {
            respawnableObject.SetActive(true);
            respawnableObject.GetComponent<Respawner>().Respawn();
        }
    }
}