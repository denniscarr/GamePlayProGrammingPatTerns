using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	[HideInInspector] public Vector2 levelExtents;

    private void Start() {
        Transform ground = GameObject.Find("Ground").transform;
    }
}
