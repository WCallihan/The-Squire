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
    [SerializeField] GameObject[] weaponSelectedIndicators;
    [SerializeField] AudioClip[] weaponSelectedSounds;
    private WeaponType currentWeaponIndicator = WeaponType.Sword;

    private PlayerController playerController;
    private HealthManager healthManager;
    private AudioSource audioSource;

    void Start() {
        playerController = GetComponent<PlayerController>();
        healthManager = GetComponent<HealthManager>();
        audioSource = GetComponent<AudioSource>();
        healthBar.maxValue = healthManager.maxHealth;
    }

    void LateUpdate() {
        healthBar.value = healthManager.currentHealth;
        keyIndicator.SetActive(playerController.hasKey);
    }

    //updates the indicator and plays the equip sfx of the currently selected weapon; gets called by PlayerCOntroller when weapons are changed
    public void UpdateWeaponSelected(WeaponType newWeapon) {
        if(currentWeaponIndicator != newWeapon) {
            weaponSelectedIndicators[(int)currentWeaponIndicator].SetActive(false); //deactivates old indicator
            currentWeaponIndicator = newWeapon;
            weaponSelectedIndicators[(int)currentWeaponIndicator].SetActive(true); //activates new indicator
            audioSource.PlayOneShot(weaponSelectedSounds[(int)currentWeaponIndicator], 0.2f);
        }
    }
}