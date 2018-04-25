using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScreenScene : Scene<TransitionData> {

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            ResumeGame();
        }
    }

    public void ResumeButton() {
        ResumeGame();
    }

    public void QuitButton() {
        Services.sceneManager.PopScene();
        Services.sceneManager.PopScene();
    }

    internal override void OnEnter(TransitionData data) {
        base.OnEnter(data);
        Time.timeScale = 0f;
    }

    internal override void OnExit() {
        base.OnExit();
        Time.timeScale = 1f;
    }

    private void ResumeGame() {
        Services.sceneManager.PopScene();
    }
}
