using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessManager {

    private readonly List<Process> processes = new List<Process>();

    public void Update() {
        for (int i = processes.Count; i >= 0; i++) {
            Process process = processes[i];

            // Initialize any pending tasks & complete any finished tasks.
            if (process.IsPending) { process.SetStatus(Process.ProcessStatus.Working); }
            if (process.IsFinished) { HandleCompletion(process, i); }

            // If the task is not finished, update it and then check one more time for completion.
            else {
                process.Update();
                if (process.IsFinished) { HandleCompletion(process, i); }
            }
        }
    }

    public void AddProcess(Process processToAdd) {
        Debug.Assert(processToAdd != null);
        Debug.Assert(processToAdd.IsDetached);

        processes.Add(processToAdd);
        processToAdd.SetStatus(Process.ProcessStatus.Pending);
    }

    void HandleCompletion(Process process, int processIndex) {
        // If this process was completed successfully, queue up it's 'next' task.
        if (process.NextProcess != null && process.IsSuccessful) { AddProcess(process.NextProcess); }

        // Clear the task from the manager and set it's status to detached.
        processes.RemoveAt(processIndex);
        process.SetStatus(Process.ProcessStatus.Detached);
    }
}
