using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleFuckerUpper : MonoBehaviour {

    [SerializeField] Vector3 maxChange = new Vector3(0f, 0f, 0f);
    Vector3 originalScale;

    private void Awake() {
        originalScale = transform.localScale;
    }

    private void Update() {
        transform.localScale = new Vector3(
                originalScale.x + Random.Range(-maxChange.x, maxChange.x),
                originalScale.y + Random.Range(-maxChange.y, maxChange.y),
                originalScale.z + Random.Range(-maxChange.z, maxChange.z)
            );
    }
}
