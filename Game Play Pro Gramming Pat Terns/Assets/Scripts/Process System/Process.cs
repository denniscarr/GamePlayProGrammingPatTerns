using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Process {

	public enum ProcessStatus {
        Detached,   // Not yet assigned to a manager.
        Pending,    // Assigned to a manager but not yet initialized.
        Working,    
        Successful,
        Failed,
        Aborted
    }

    public ProcessStatus Status { get; private set; }

    // Convenience methods for checking status.
    public bool IsDetached { get { return Status == ProcessStatus.Detached; } }
    public bool IsAttached { get { return Status != ProcessStatus.Detached; } }
    public bool IsPending { get { return Status == ProcessStatus.Pending; } }
    public bool IsWorking { get { return Status == ProcessStatus.Working; } }
    public bool IsSuccessful { get { return Status == ProcessStatus.Successful; } }
    public bool IsFailed { get { return Status == ProcessStatus.Failed; } }
    public bool IsAborted { get { return Status == ProcessStatus.Aborted; } }
    public bool IsFinished { get { return Status == ProcessStatus.Aborted || Status == ProcessStatus.Successful || Status == ProcessStatus.Failed; } }

    // Used for setting up sequences of processes.
    public Process NextProcess { get; private set; }

    // A convenience method to allow other classes to abort this task.
    public void Abort() {
        SetStatus(ProcessStatus.Aborted);
    }
    
    internal void SetStatus(ProcessStatus newStatus) {
        if (Status == newStatus) { return; }

        Status = newStatus;

        switch (newStatus) {
            case ProcessStatus.Working:
                // If this task has just been set to working, initialize it.
                Initialize();
                break;

            case ProcessStatus.Successful:
                OnSuccess();
                CleanUp();
                break;

            case ProcessStatus.Aborted:
                OnAbort();
                CleanUp();
                break;

            case ProcessStatus.Failed:
                OnFail();
                CleanUp();
                break;

            // These states are only relevant for the process manager, so we can ignore them here.
            case ProcessStatus.Detached:
            case ProcessStatus.Pending:
                break;

            default:
                throw new ArgumentOutOfRangeException(newStatus.ToString(), newStatus, null);
        }
    }

    protected virtual void Initialize() { }
    public virtual void Update() { }
    protected virtual void CleanUp() { }

    protected virtual void OnSuccess() { }
    protected virtual void OnAbort() { }
    protected virtual void OnFail() { }


    // Used to chain sequences of processes.
    public Process Then(Process process) {
        Debug.Assert(!process.IsAttached);
        NextProcess = process;
        return process;
    }
}
