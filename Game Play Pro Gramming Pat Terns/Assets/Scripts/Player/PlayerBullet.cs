using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour {

    [SerializeField] float speed = 30f;
    [SerializeField] float spread = 10f;

    Plane movementPlane;
    Vector3 direction;

    Rigidbody m_Rigidbody;

    private void Awake() {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void Start() {
        // Get direction based on mouse position.
        movementPlane = new Plane(transform.position, transform.position + transform.up, transform.position + transform.right);
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        float planeRaycastDistance;
        if (movementPlane.Raycast(mouseRay, out planeRaycastDistance)) {
            Vector3 mousePoint = mouseRay.origin + mouseRay.direction * planeRaycastDistance;
            direction = Vector3.Normalize(mousePoint - transform.position);
            direction = Quaternion.Euler(0, 0, Random.Range(-spread, spread)) * direction;
        }

        // If the player somehow clicked off the screen, get destroyed.
        else {
            Debug.LogError("Bullet tried to get aimed off screen.");
            Destroy(gameObject);
        }
    }

    private void FixedUpdate() {
        // Move to new position.
        Vector3 newPosition = transform.position + direction * speed * Time.fixedDeltaTime;
        m_Rigidbody.MovePosition(newPosition);

        // Delete if too far from level.
        if (Vector3.Distance(Vector3.zero, transform.position) >= 50f) {
            Destroy(gameObject);
        }
    }
}
