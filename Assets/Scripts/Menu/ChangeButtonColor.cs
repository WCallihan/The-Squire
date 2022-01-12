using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/* Used by all button objects with text:
 * uses the Pointer handlers from EventSystems to detect when the mouse touches
 * the button ui and changes the color and plays a sound when it is
 */

public class ChangeButtonColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    private Color defaultColor = new Color(255, 255, 255); //normal color of text is white
    [SerializeField] Color hoveringColor = new Color(255, 224, 0); //default hovering color of text is yellow
    private Text buttonText;
    private AudioSource audioSource;
    [SerializeField] AudioClip hoveringSound;

    void Start() {
        buttonText = GetComponent<Text>();
        audioSource = GetComponent<AudioSource>();
    }

    //when the mouse enter the button's space, change the color and play the sound
    public void OnPointerEnter(PointerEventData eventData) {
        buttonText.color = hoveringColor;
        audioSource.PlayOneShot(hoveringSound);
    }

    //when the mouse exits the button's space, change the color back to normal
    public void OnPointerExit(PointerEventData eventData) {
        buttonText.color = defaultColor;
    }
}