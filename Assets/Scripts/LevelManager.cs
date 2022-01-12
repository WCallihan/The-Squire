using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Used by the LevelManager objects:
 * manages the level checkpoints and the respawning of all
 * respawnable objects in the scene
 */

public class LevelManager : MonoBehaviour {

    [SerializeField] GameObject player;
    private PlayerController playerController;

    public GameObject[] respawnableObjects; //array of all respawnable game objects in the scene

    void Start() {
        playerController = player.GetComponent<PlayerController>();
    }

    //constantly checks if the player is respawning, and then tells all other objects to respawn if able
    void Update() {
        if(playerController.playerRespawning) {
            playerController.playerRespawning = false; //set to false to avoid constantly respawning
            RespawnCheckpoint();
        }
    }

    //called by Checkpoint when the player touches it for the first time; sets the new checkpoint for all respawnable objects
    public void SetCheckpoint() {
        foreach(var respawnObject in respawnableObjects) {
            if(respawnObject.activeInHierarchy) { //does not effect deactivated objects like enemies that have already been killed
                respawnObject.GetComponent<Respawner>().SetCheckpoint(); //each respawnable object must have a Respawner script
            }
        }
    }

    //finds all respawnable objects and tells them to respawn
    private void RespawnCheckpoint() {
        foreach(var respawnableObject in respawnableObjects) {
            respawnableObject.SetActive(true); //activates all respawnable objects to allow their Respawner scripts to be used
            respawnableObject.GetComponent<Respawner>().Respawn(); //each respawnable object must have a Respawner script
        }
    }
}