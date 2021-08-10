using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

/*Used by the TimelineManager objects in cutscene scenes:
 * allows the user to skip the cutscene by double tapping space.
 * shows an indicator prompting the user to tap twice that fades in
 * and out.
 */

public class SkipCutscene : MonoBehaviour {

    [SerializeField] PlayableDirector activeTimeline;
    [SerializeField] Animation skipIndicatorAnimation;

    private bool canSkip = false;
    private float fadeOutTimer = 3f;

    void Update() {
        //if the player has pressed space, it fades in the indicator or, if it's already there, skips to the ends of the timeline
        if(Input.GetKeyDown(KeyCode.Space)) {
            if(canSkip) {
                activeTimeline.time = activeTimeline.duration;
                skipIndicatorAnimation.gameObject.SetActive(false);
            } else {
                FadeIn();
            }
        }
        //if the indicator is visible, counts down a few seconds to fade it out
        if(canSkip) {
            fadeOutTimer -= Time.deltaTime;
            if(fadeOutTimer <= 0)
                FadeOut();
        }
    }

    //fades the indicator into visibility; gives the ability to skip the scene
    public void FadeIn() {
        canSkip = true;
        skipIndicatorAnimation.Play("Fade In");
        fadeOutTimer = 3f;
    }

    //fades the indicator out of visibility; removes the ability to skip the scene
    public void FadeOut() {
        canSkip = false;
        skipIndicatorAnimation.Play("Fade Out");
    }
}
