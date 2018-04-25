using UnityEngine;

[CreateAssetMenu(menuName = "Prefab Database")]
public class PrefabDatabase : ScriptableObject {

    [SerializeField] private GameObject[] _scenes;
    public GameObject[] Scenes {
        get { return _scenes; }
    }

}