using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour {

    [SerializeField] GameObject mesh;

    [SerializeField] float maxInvincibilityFrames = 0.4f;
    float currentInvincibilityFrames;

    PlayerController playerController;
    PlayerCharge chargeController;


    private void Awake() {
        playerController = GetComponent<PlayerController>();
        chargeController = GetComponent<PlayerCharge>();
    }


    private void Update() {
        maxInvincibilityFrames -= Time.deltaTime;
    }

    
    public void ActivateInvincibility() {
        currentInvincibilityFrames = maxInvincibilityFrames;
    }


    public void Die() {
        if (maxInvincibilityFrames > 0) { return; }
        mesh.SetActive(false);
        playerController.isAcceptingInput = false;
    }


    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Enemy>() && !chargeController.isCharging) {
            Die();
        }
    }
}
