using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Services : MonoBehaviour {
    public static HitPause hitPause;
    public static EnemyManager enemyManager;

    public static void LocateAll() {
        hitPause = FindObjectOfType<HitPause>();
        enemyManager = FindObjectOfType<EnemyManager>();
    }
}
