using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreenScene : Scene<TransitionData> {

    [SerializeField] Text scoreNumberText;

    internal override void OnEnter(TransitionData data) {
        base.OnEnter(data);
        Debug.Log("Doing this " + data.score);
        scoreNumberText.text = data.score.ToString();
    }

    public void RestartButton() {
        Services.sceneManager.PopScene();
    }

    public void QuitButton() {
        Services.sceneManager.PopScene();
        Services.sceneManager.PopScene();
    }
}
