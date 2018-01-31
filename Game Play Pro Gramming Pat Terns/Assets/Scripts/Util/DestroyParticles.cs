using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticles : MonoBehaviour {

    ParticleSystem[] particleSystems;

    private void Awake() {
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    private void Update() {
        int stoppedSystems = 0;
        foreach (ParticleSystem particleSystem in particleSystems) {
            if (GetComponentInChildren<ParticleSystem>().isStopped) {
                stoppedSystems++;
            }
        }
        if (stoppedSystems >= particleSystems.Length) {
            Destroy(gameObject);
        }
    }
}
