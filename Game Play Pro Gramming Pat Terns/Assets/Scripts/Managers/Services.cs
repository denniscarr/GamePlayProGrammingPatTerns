using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Services : MonoBehaviour {
    public static HitPause hitPause;

    private void Awake() {
        hitPause = FindObjectOfType<HitPause>();
    }
}
