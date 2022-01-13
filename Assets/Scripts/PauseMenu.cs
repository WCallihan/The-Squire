using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

/* Used by the LevelManager objects:
 * manages the pause menu in each scene including actiivating and deactivating it and
 * pausing the game when it is active
 */

public class PauseMenu : MonoBehaviour {

    [SerializeField] GameObject pauseMenu;
    [SerializeField] AudioSource musicPlayer;
    [SerializeField] PlayableDirector activeTimeline;

    private bool gamePaused = false;

    //toggle the pause with the escape button
    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if(gamePaused) {
                Resume();
            } else {
                Pause();
            }
        }
    }

    //resumes the game when paused; also called by the resume button
    public void Resume() {
        if(musicPlayer != null)
            musicPlayer.Play(); //resumes the music
        if(activeTimeline != null)
            activeTimeline.Play(); //resumes the cutscene (if applicable)

        pauseMenu.SetActive(false); //deactivates the pause menu
        Time.timeScale = 1f; //resumes the scene
        gamePaused = false;
    }

    //pauses the game when it is playing and the pause button is pressed
    public void Pause() {
        if(musicPlayer != null)
            musicPlayer.Pause(); //pauses the music
        if(activeTimeline != null)
            activeTimeline.Pause(); //pauses the cutscene (if applicable)

        pauseMenu.SetActive(true); //activates the pause menu
        Time.timeScale = 0f; //pauses the scene
        gamePaused = true;
    }

    //loads the main menu scene; called by the menu button on pause menu
    public void ReturnToMenu() {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }

    //ends the game; called by the quit button on pause menu
    public void QuitGame() {
        Application.Quit();
    }
}