using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class SceneChanger : MonoBehaviour {

    [SerializeField] string nextScene = "";
    [SerializeField] string sceneToSave = "";
    [SerializeField] PlayableDirector endLevelTimeline;

    //triggers when the player crosses the level changing threshold
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.CompareTag("Player")) { //double check that it is the player that touched the threshold
            UpdateSavePoint(sceneToSave);
            EndScene();
        }
    }

    // -- ENDING SCENES --

    //called by the trigger collider at the end of the level or by a menu button
    public void EndScene() {
        if(endLevelTimeline != null) {
            endLevelTimeline.Play();
        } else {
            StartNextScene();
        }
    }
    //called by menus or pause screens that are able to load multiple next scenes
    public void EndScene(string givenNextScene) {
        nextScene = givenNextScene;
        EndScene();
    }

    //ONLY called at the end of the LevelFadeOut timeline or by EndScene if there is no fade out; NEVER make an object execute these directly
    public void StartNextScene() {
        if(nextScene != "") {
            SceneManager.LoadScene(nextScene);
        } else {
            Debug.Log("No Next Scene Given");
        }
    }

    //-- SAVE SYSTEM --

    //used at the end of levels to update the latest level to load
    public void UpdateSavePoint(string savedScene) {
        if(savedScene != "") {
            PlayerPrefs.SetString("LevelToLoad", savedScene);
        } else {
            Debug.Log("No Scene To Save Given");
            PlayerPrefs.SetString("LevelToLoad", SceneManager.GetActiveScene().name); //defaults to save the current scene as what should be loaded
        }
        UnlockLevel(SceneManager.GetActiveScene().name); //unlocks the level that was just completed
    }

    //used at the end of levels to unlock the level; called by UpdateSavePoint and WizardController as part of its Die method
    public void UnlockLevel(string unlockedScene) {
        if(unlockedScene.Equals("Level 1"))
            PlayerPrefs.SetInt("Level1Unlocked", 1);
        else if(unlockedScene.Equals("Level 2"))
            PlayerPrefs.SetInt("Level2Unlocked", 1);
        if(unlockedScene.Equals("Level 3"))
            PlayerPrefs.SetInt("Level3Unlocked", 1);
        if(unlockedScene.Equals("Level 4")) //triggered when the wizard dies
            PlayerPrefs.SetInt("Level4Unlocked", 1);
    }

    //used by the continue option at the menu
    public void LoadProgress() {
        SceneManager.LoadScene(PlayerPrefs.GetString("LevelToLoad"));
    }

    //used by the new game option at the menu
    public void DeleteProgress() {
        PlayerPrefs.SetString("LevelToLoad", "Level 1 Opening Cutscene"); //sets the next level to be done as level 1
        PlayerPrefs.SetInt("Level1Unlocked", 0); //resets all saved level completions
        PlayerPrefs.SetInt("Level2Unlocked", 0);
        PlayerPrefs.SetInt("Level3Unlocked", 0);
        PlayerPrefs.SetInt("Level4Unlocked", 0);
    }
}