using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Transition Data is the data which scenes pass back and forth as they transition.
public class SceneManager<TTransitionData> {

    // The Scene Manager does two main things:
    // 1. Managing the stack of scenes
    // 2. Forwarding events to the current scene

    internal GameObject SceneRoot { get; private set; }

    // A dictionary to store the managed scenes. We will be using prefabs.
    private readonly Dictionary<Type, GameObject> _scenes = new Dictionary<Type, GameObject>();

    // The currently active scenes are organized in a stack whose depth represents history.
    private readonly Stack<Scene<TTransitionData>> _sceneStack = new Stack<Scene<TTransitionData>>();

    // CurrentScene returns the topmost scene on the stack.
    public Scene<TTransitionData> CurrentScene {
        get {
            return _sceneStack.Count != 0 ? _sceneStack.Peek() : null;
        }
    }


    public SceneManager(GameObject root, IEnumerable<GameObject> scenePrefabs) {
        SceneRoot = root;

        // Go through the list of scenes and enter them into the _scnenes dictionary. This we we can look them up by type.
        foreach(var prefab in scenePrefabs) {
            var scene = prefab.GetComponent<Scene<TTransitionData>>();
            Debug.Assert(scene != null, "Could not find scene script in prefab used to initialize Scene Manager");
            _scenes.Add(scene.GetType(), prefab);
        }
    }


    // Removes the current scene from the stack.
    public void PopScene(TTransitionData data = default(TTransitionData)) {

        // We'll need to store references to the previous and next scenes if they exist so that
        // we can call OnEnter/Exit functions on them.
        Scene<TTransitionData> previousScene = null;
        Scene<TTransitionData> nextScene = null;

        // Store the topmost scene as previousScene and then remove it.
        if (_sceneStack.Count != 0) {
            previousScene = _sceneStack.Peek();
            _sceneStack.Pop();
        }

        // If there is still another scene in the stack, store it as nextScene.
        if (_sceneStack.Count != 0) {
            nextScene = _sceneStack.Peek();
        }

        // Call OnEnter on the new current scene.
        if (nextScene != null) {
            nextScene._OnEnter(data);
        }

        // Call OnExit on the previous scene.
        if (previousScene != null) {
            UnityEngine.Object.Destroy(previousScene.Root);
            previousScene._OnExit();
        }
    }


    // Creates a scene and puts it on top of the active scene stack.
    public void PushScene<T>(TTransitionData data = default (TTransitionData)) where T : Scene<TTransitionData> {

        var previousScene = CurrentScene;
        var nextScene = GetScene<T>();
            
        _sceneStack.Push(nextScene);
        nextScene._OnEnter(data);

        // Call _OnExit on the scene which is being pushed down.
        if (previousScene != null) {
            previousScene._OnExit();
            previousScene.Root.SetActive(false);
        }
    }


    // This method replaces (and destroys) the top scene in the stack rather than pushing it down.
    // This is useful when it wouldn't make sense to ever go back to a scene (ie, loading screens)
    public void Swap<T>(TTransitionData data = default(TTransitionData)) where T : Scene<TTransitionData> {

        // Store a reference to the prefious scene and then pop it.
        Scene<TTransitionData> previousScene = null;
        if (_sceneStack.Count > 0) {
            previousScene = _sceneStack.Peek();
            _sceneStack.Pop();
        }

        // Get a reference to the next scene and push it onto the stack. Also call OnEnter().
        var nextScene = GetScene<T>();
        _sceneStack.Push(nextScene);
        nextScene._OnEnter(data);

        // Call OnExit on the previous scene and then destroy it.
        if (previousScene != null) {
            previousScene._OnExit();
            UnityEngine.Object.Destroy(previousScene);
        }
    }


    // A helper method for creating a scene of a given type and adding it to the stack.
    private T GetScene<T>() where T : Scene<TTransitionData> {

        // Get a reference to the prefab of the given type.
        GameObject prefab;
        _scenes.TryGetValue(typeof(T), out prefab);
        Debug.Assert(prefab != null, "Could not find scene prefab for scene type: " + typeof(T).Name);

        // Instantiate that prefab as a game object, parent it to the scene root game object, and return a reference to it.
        var sceneObject = UnityEngine.Object.Instantiate(prefab);
        sceneObject.name = typeof(T).Name;
        sceneObject.transform.SetParent(SceneRoot.transform, false);
        return sceneObject.GetComponent<T>();
    }
}
