using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour {

    private void Awake() {
        Services.prefabDatabase = Resources.Load<PrefabDatabase>("Prefab Database");
        Services.sceneManager = new SceneManager<TransitionData>(gameObject, Services.prefabDatabase.Scenes);

        Services.sceneManager.PushScene<MainMenuScene>();
    }
}
