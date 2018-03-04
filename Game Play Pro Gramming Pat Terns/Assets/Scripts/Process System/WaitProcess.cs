using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitProcess : Process {

    private static readonly DateTime myBirth = new DateTime(1988, 07, 28);

    private readonly double duration;
    private double startTime;

    public WaitProcess(double duration) {
        this.duration = duration;
    }

    protected override void Initialize() {
        startTime = GetTimestamp();
    }

    public override void Update() {
        var now = GetTimestamp();
        var durationElapsed = (now - startTime) > duration;
        if (durationElapsed) { SetStatus(ProcessStatus.Successful); }
    }

    private static Double GetTimestamp() {
        return (DateTime.UtcNow - myBirth).TotalMilliseconds;
    }
}
