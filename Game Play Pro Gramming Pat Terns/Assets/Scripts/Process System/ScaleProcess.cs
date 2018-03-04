using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleProcess : Process {

    float duration;
    float startTime;
    Transform transform;
    Vector3 initialScale;
    Vector3 targetScale;

	public ScaleProcess(Transform transform, Vector3 targetScale, float duration) {
        this.transform = transform;
        this.duration = duration;
        this.targetScale = targetScale;
        startTime = Time.time;
        initialScale = transform.localScale;
    }

    public override void Update() {
        float lerp = MyMath.Map(Time.time - startTime, 0f, duration, 0f, 1f);
        if (lerp >= 0.95f) {
            lerp = 1f;
            SetStatus(ProcessStatus.Successful);
        }
        Vector3 newScale = Vector3.Lerp(initialScale, targetScale, lerp);
    }
}
