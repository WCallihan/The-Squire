using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupWindow : MonoBehaviour {

    [SerializeField] GameObject popupWindow;

    public void ShowPopup() {
        popupWindow.SetActive(true);
    }

    public void HidePopup() {
        popupWindow.SetActive(false);
    }
}
