using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Used by the arrow projectile prefabs:
 * moves forward at the spawned angle until it exits the range
 * collider or it hits another collider
 * damages the player if the arrow hits them
 */

public class Projectile : MonoBehaviour {

    [SerializeField] float arrowSpeed;
    public float arrowDamage; //set by ArcherEnemyController

    void Update() {
        transform.Translate(Vector3.right * Time.deltaTime * arrowSpeed);
    }

    //used to destroy the arrow when it reaches the max range set by the circle trigger collider
    private void OnTriggerExit2D(Collider2D collision) {
        Destroy(gameObject);
    }

    //used to destroy the arrow if it hits anything before exiting the range
    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.CompareTag("Player")) {
            collision.gameObject.GetComponent<HealthManager>().TakeDamage(arrowDamage);
        }
        Destroy(gameObject);
    }
}