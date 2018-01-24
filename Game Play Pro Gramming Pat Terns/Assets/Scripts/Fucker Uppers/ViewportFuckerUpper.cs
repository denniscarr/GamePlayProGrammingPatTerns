using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewportFuckerUpper : MonoBehaviour {

    [SerializeField] FloatRange viewportRange = new FloatRange(0, 0);

    private Camera m_Camera;


    private void Start() {
        m_Camera = GetComponent<Camera>();
    }


    private void Update() {
        m_Camera.rect = new Rect(
                new Vector2(viewportRange.Random, viewportRange.Random),
                new Vector2(1f, 1f)
            );
    }
}
