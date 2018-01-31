using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPause : MonoBehaviour {

    public void StartHitPause(float duration) {
        StartCoroutine(HitPauseCoroutine(duration));
    }

    IEnumerator HitPauseCoroutine(float duration) {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        yield return null;
    }
}
