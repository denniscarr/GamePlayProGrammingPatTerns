using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour {

    [SerializeField] GameObject mesh;

    PlayerController playerController;
    PlayerCharge chargeController;


    private void Awake() {
        playerController = GetComponent<PlayerController>();
        chargeController = GetComponent<PlayerCharge>();
    }


    public void Die() {
        mesh.SetActive(false);
        playerController.isAcceptingInput = false;
    }


    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Enemy>() && !chargeController.isCharging) {
            Die();
        }
    }
}
