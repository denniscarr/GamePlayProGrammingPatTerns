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


    private void OnEnable() {
        GetComponent<Collider>().enabled = true;
    }


    private void Update() {
        maxInvincibilityFrames -= Time.deltaTime;
    }

    
    public void ActivateInvincibility() {
        currentInvincibilityFrames = maxInvincibilityFrames;
    }


    public void Die() {
        if (maxInvincibilityFrames > 0) { return; }
        transform.localPosition = new Vector3(0f, 0f, transform.localPosition.z);
        GetComponent<Collider>().enabled = false;
        Services.enemyManager.DestroyAllExistingEnemies();
        Services.sceneManager.PushScene<GameOverScreenScene>(new TransitionData(FindObjectOfType<ScoreDisplay>().score));
    }


    private void OnTriggerEnter(Collider other) {
        bool isEnemy = other.GetComponent<Enemy>() | LayerMask.LayerToName(other.gameObject.layer) == "Enemy";
        if (isEnemy && !chargeController.isCharging) {
            Die();
        }
    }
}
