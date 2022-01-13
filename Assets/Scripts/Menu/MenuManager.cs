using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Used by the MenuManager object in the Main Menu scene:
 * manages which canvas is currently in use and handles changing
 * which screen is active. also handles the popup window for the
 * new game button
 */

public class MenuManager : MonoBehaviour {

    [SerializeField] GameObject mainMenuCanvas;
    [SerializeField] GameObject levelSelectCanvas;
    [SerializeField] GameObject creditsCanvas;
    private GameObject currentCanvas;

    [SerializeField] GameObject popupWindow;

    private void Start() {
        currentCanvas = mainMenuCanvas; //main menu begins active, nothing else does
    }

    // -- SCREEN CHANGE --

    //goes to the main menu screen
    public void GoToMenu() {
        CanvasChanger(mainMenuCanvas);
    }

    //goes to the level select screen
    public void GoToLevelSelect() {
        CanvasChanger(levelSelectCanvas);
        HidePopup();
    }

    //goes to the credits screen
    public void GoToCredits() {
        CanvasChanger(creditsCanvas);
        HidePopup();
    }

    //helper function for the other functions to change the canvas to the requested one
    private void CanvasChanger(GameObject nextCanvas) {
        currentCanvas.SetActive(false);
        nextCanvas.SetActive(true);
        currentCanvas = nextCanvas;
    }

    // -- POPUP WINDOW --

    //called by the new game button; shows the new game popup window
    public void ShowPopup() {
        popupWindow.SetActive(true);
    }

    //called by the screen changing functions and the cancel button in the popup window; closes the new game popup window
    public void HidePopup() {
        popupWindow.SetActive(false);
    }

    // -- MISC. --

    //ends the game; called by the quit button
    public void QuitGame() {
        Application.Quit();
    }
}