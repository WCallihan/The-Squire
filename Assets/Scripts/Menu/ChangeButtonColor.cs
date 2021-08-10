using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/* Used by all button objects:
 * uses the IPointer handlers from EventSystems to detect when the mouse touches
 * the button ui and changes the color and plays a sound when it is
 */

public class ChangeButtonColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    private Color defaultColor = new Color(255, 255, 255);
    [SerializeField] Color hoveringColor = new Color(255, 224, 0);
    private Text buttonText;
    private AudioSource audioSource;
    [SerializeField] AudioClip hoveringSound;

    void Start() {
        buttonText = GetComponent<Text>();
        audioSource = GetComponent<AudioSource>();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        buttonText.color = hoveringColor;
        audioSource.PlayOneShot(hoveringSound);
    }

    public void OnPointerExit(PointerEventData eventData) {
        buttonText.color = defaultColor;
    }
}
