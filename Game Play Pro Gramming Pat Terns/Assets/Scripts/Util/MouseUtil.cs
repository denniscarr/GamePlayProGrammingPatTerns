using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseUtil {

	public static Vector3 ProjectMousePositionOntoPlane(Plane plane) {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        float planeRaycastDistance;
        if (plane.Raycast(mouseRay, out planeRaycastDistance)) {
            return mouseRay.origin + mouseRay.direction * planeRaycastDistance;
        }
        else {
            Debug.LogError("Mouse position could not projected onto given plane.");
            return Vector3.zero;
        }
    }
}
