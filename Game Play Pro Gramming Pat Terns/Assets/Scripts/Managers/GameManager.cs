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


    private void Awake() {
        Services.LocateAll();
        Services.enemyManager.Initialize();
    }


    private void Update() { 
        Services.enemyManager.Run();
    }


    private void FixedUpdate() {
        Services.enemyManager.FixedRun();
    }


    public static Vector3 GetPointInArena(Vector2 margins) {
        return new Vector3(
                Random.Range(-groundHalfExtents.x + margins.x, groundHalfExtents.x - margins.x),
                Random.Range(-groundHalfExtents.y + margins.y, groundHalfExtents.y - margins.y),
                0f
            );
    }
}
