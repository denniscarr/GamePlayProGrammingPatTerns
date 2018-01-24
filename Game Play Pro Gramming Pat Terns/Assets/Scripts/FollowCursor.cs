using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCursor : MonoBehaviour {

    private void Update() {
        Plane movementPlane = new Plane(transform.position, transform.position + transform.up, transform.position + transform.right);
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        float planeRaycastDistance;
        if (movementPlane.Raycast(mouseRay, out planeRaycastDistance)) {
            Vector3 mousePoint = mouseRay.origin + mouseRay.direction * planeRaycastDistance;
            transform.position = mousePoint;
        }
    }
}
