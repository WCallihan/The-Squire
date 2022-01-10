using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Used by the ground and background objects in the Ending scene:
 * moves the object constantly to the left and resets the position
 * halfway through the movement to create an infinite scrolling
 * effect.
 */

public class RepeatingBackground : MonoBehaviour {

    [SerializeField] float speed = 20;
    private Vector3 startPos;
    private float repeatWidth;

    void Start() {
        startPos = transform.position; //sets start position as where the background is before running
        repeatWidth = GetComponent<BoxCollider2D>().size.x / 2; //sets half way point based on the box collider's dimensions
    }

    void Update() {
        //constantly moves the object to the left
        transform.Translate(Vector3.left * Time.deltaTime * speed);

        //resets background half way through to create an infinite scrolling background
        if(transform.position.x <= startPos.x - repeatWidth) {
            transform.position = startPos;
        }
    }
}