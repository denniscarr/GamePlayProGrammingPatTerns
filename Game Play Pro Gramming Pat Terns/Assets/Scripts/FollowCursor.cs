﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCursor : MonoBehaviour {

    private void Update() {
        Plane movementPlane = new Plane(transform.position, transform.position + transform.up, transform.position + transform.right);
        transform.position = MouseUtil.ProjectMousePositionOntoPlane(movementPlane);
    }
}
