using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {

    public Vector3 relativeSpeed = new Vector3(1f, 1f, 1f);

    [HideInInspector] public bool freezeRotation;

    private void Update() {
        if (!freezeRotation) { transform.Rotate(relativeSpeed * Time.deltaTime); }
    }
}
