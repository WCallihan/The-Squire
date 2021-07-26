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
        UpdateSavePoint(sceneToSave);
        EndScene();
    }

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
    }

    //used by the continue option at the menu
    public void LoadProgress() {
        SceneManager.LoadScene(PlayerPrefs.GetString("LevelToLoad"));
    }

    //used by the new game option at the menu
    public void DeleteProgress() {
        PlayerPrefs.SetString("LevelToLoad", "Opening Cutscene");
            //TO DO: reset player prefs for which levels can be selected specifically
    }
}
