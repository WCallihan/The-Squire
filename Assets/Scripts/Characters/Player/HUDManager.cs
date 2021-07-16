using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* Used by both player characters:
 * manages all the elements on the player's HUD
 */

public class HUDManager : MonoBehaviour {

    [SerializeField] Slider healthBar;
    [SerializeField] GameObject keyIndicator;

    private PlayerController playerController;
    private HealthManager healthManager;

    void Start() {
        playerController = GetComponent<PlayerController>();
        healthManager = GetComponent<HealthManager>();
        healthBar.maxValue = healthManager.maxHealth;
    }

    void LateUpdate() {
        healthBar.value = healthManager.currentHealth;
        keyIndicator.SetActive(playerController.hasKey);
    }
}