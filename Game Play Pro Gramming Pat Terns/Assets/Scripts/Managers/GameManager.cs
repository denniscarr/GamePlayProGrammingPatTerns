using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static Vector2 groundHalfExtents {
        get {
            Transform ground = GameObject.Find("Ground").transform;
            return new Vector2(
                    ground.localScale.x * 0.5f,
                    ground.localScale.y * 0.5f
                );
        }
    }
}
