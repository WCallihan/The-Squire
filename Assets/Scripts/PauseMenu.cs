using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

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
            musicPlayer.Play();
        if(activeTimeline != null)
            activeTimeline.Play();

        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        gamePaused = false;
    }

    //pauses the game when it is playing and the pause button is pressed
    public void Pause() {
        if(musicPlayer != null)
            musicPlayer.Pause();
        if(activeTimeline != null)
            activeTimeline.Pause();

        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        gamePaused = true;
    }

    //loads the main menu scene; called by the menu button on pause menu
    public void ReturnToMenu() {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }

    //ends the game; called by the quit button on pause menu
    public void QuitGame() {
        if(UnityEditor.EditorApplication.isPlaying)
            UnityEditor.EditorApplication.isPlaying = false;
        else
            Application.Quit();
    }
}
