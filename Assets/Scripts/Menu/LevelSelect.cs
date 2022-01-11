using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Used by the Level Select Canvas object in Main Menu:
 * checks if each level is unlocked and handles activating the
 * level button to load that specific level
 */

public class LevelSelect : MonoBehaviour {

    [SerializeField] GameObject level1Unlocked;
    [SerializeField] GameObject level2Unlocked;
    [SerializeField] GameObject level3Unlocked;
    [SerializeField] GameObject level4Unlocked;

    void Awake() {
        //activates each level's button if the player prefs have them unlocked
        if(PlayerPrefs.GetInt("Level1Unlocked", 0) != 0)
            level1Unlocked.SetActive(true);
        else
            level1Unlocked.SetActive(false);

        if(PlayerPrefs.GetInt("Level2Unlocked", 0) != 0)
            level2Unlocked.SetActive(true);
        else
            level2Unlocked.SetActive(false);

        if(PlayerPrefs.GetInt("Level3Unlocked", 0) != 0)
            level3Unlocked.SetActive(true);
        else
            level3Unlocked.SetActive(false);

        if(PlayerPrefs.GetInt("Level4Unlocked", 0) != 0)
            level4Unlocked.SetActive(true);
        else
            level4Unlocked.SetActive(false);
    }
}