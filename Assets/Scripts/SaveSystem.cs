using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*Used by the SaveSystem object:
 * used only in the opening cutscene but set to don't destroy on
 * load so that it can always be found and used. when the game's
 * progress is reset, the old game object is destoryed and the new
 * one in the loaded opening cutscene is used.
 */

public class SaveSystem : MonoBehaviour {
    
    //keeps save system object throughout game and instantly sets LevelToLoad as the beginning of the game
    void Start() {
        DontDestroyOnLoad(gameObject);
        PlayerPrefs.SetString("LevelToLoad", "Opening Cutscene");
    }

    void Update() {

    }

    //used at the end of levels to update the latest level to load
    public void UpdateSavePoint(string savedScene) {
        PlayerPrefs.SetString("LevelToLoad", savedScene);
    }

    //used by the continue option at the menu
    public void LoadProgress() {
        SceneManager.LoadScene(PlayerPrefs.GetString("LevelToLoad"));
    }

    //used by the new game option at the menu; deletes old saved level and the new object in opening cutscene sets it back to level 1
    public void DeleteProgress() {
        Destroy(gameObject);
    }
}