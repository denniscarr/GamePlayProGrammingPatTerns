using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScene : Scene<TransitionData> {

    public void StartGame() {
        Services.sceneManager.PushScene<GameScene>();
    }
}
