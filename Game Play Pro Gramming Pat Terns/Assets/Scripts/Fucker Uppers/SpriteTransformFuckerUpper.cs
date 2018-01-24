using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteTransformFuckerUpper : MonoBehaviour {

    [SerializeField] Vector3 scaleMax = new Vector3(0.1f, 0.1f, 0.1f);
    [SerializeField] bool shouldRotate;

    Vector3 originalScale;

    private void Awake() {
        originalScale = transform.localScale;
    }

    private void Update() {
        transform.localScale = new Vector3(
                originalScale.x + Random.Range(-scaleMax.x, scaleMax.x),
                originalScale.y + Random.Range(-scaleMax.y, scaleMax.z),
                originalScale.z + Random.Range(-scaleMax.z, scaleMax.z)
            );

        if (shouldRotate) {
            transform.Rotate(Vector3.forward, Random.Range(-180f, 180f));
        }
    }
}
