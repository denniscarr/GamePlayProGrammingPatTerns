using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionProcess : Process {

    private readonly Action action;

    public ActionProcess(Action action) {
        this.action = action;
    }

    protected override void Initialize() {
        SetStatus(ProcessStatus.Successful);
        action();
    }
}
