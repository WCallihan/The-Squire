using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class SceneChanger : MonoBehaviour {

    [SerializeField] string nextScene = "";
    [SerializeField] PlayableDirector endLevelTimeline;

    //triggers when the player crosses the level changing threshold
    private void OnTriggerEnter2D(Collider2D collision) {
        EndScene();
    }

    //called by the trigger collider at the end of the level
    public void EndScene() {
        if(endLevelTimeline != null) {
            endLevelTimeline.Play();
        } else {
            StartNextScene();
        }
    }

    //called at the end of the fade out timeline or by EndScene if there is no fade out
    public void StartNextScene() {
        if(nextScene != "") {
            SceneManager.LoadScene(nextScene);
        } else {
            Debug.Log("No Next Scene Given");
        }
    }
}
