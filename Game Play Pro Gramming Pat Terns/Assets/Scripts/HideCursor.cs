using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideCursor : MonoBehaviour {

    [SerializeField] bool hideOnStart = true;

    private void Start() {
        if (hideOnStart) {
            Hide();
        }
    }

    public void Hide() {
        Cursor.visible = false;
    }

    public void Show() {
        Cursor.visible = true;
    }
}
