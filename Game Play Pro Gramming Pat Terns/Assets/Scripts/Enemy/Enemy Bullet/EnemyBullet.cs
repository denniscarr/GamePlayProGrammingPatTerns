using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour {

    [SerializeField] float speed;

    [HideInInspector] public Vector3 direction;

    private void Awake() {
        transform.parent = GameObject.Find("Bullets").transform;
    }

    private void FixedUpdate() {
        Vector3 newPosition = transform.position;
        newPosition += direction * speed * Time.fixedDeltaTime;
        transform.position = newPosition;

        // If I'm way outside of the level, delete me.
        if (transform.position.magnitude > 50f) { Destroy(gameObject); }
    }
}
