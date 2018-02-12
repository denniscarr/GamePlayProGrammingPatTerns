using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharge : MonoBehaviour {

    [SerializeField] GameObject enemyHitParticles;

    [SerializeField] float chargeSpeed = 200f;
    [SerializeField] float chargeLength = 3f;
    [SerializeField] float chargeCooldown = 1f;
    [SerializeField] GameObject chargingMesh;
    [SerializeField] GameObject normalMesh;
    [SerializeField] AudioSource m_AudioSource;

    [HideInInspector] public bool isCharging;

    [HideInInspector] public bool chargeBuffered = false;
    float cooldownTimer;
    Vector3 lastChargeDirection;
    Coroutine chargeCoroutine;

    Rigidbody m_Rigidbody;


    private void Awake() {
        m_Rigidbody = GetComponent<Rigidbody>();
        cooldownTimer = chargeCooldown;
    }


    private void Update() {

        if (chargeBuffered) {
            Vector3 directionalInput = Vector2.zero;
            directionalInput.x = Input.GetAxisRaw("Horizontal");
            directionalInput.y = Input.GetAxisRaw("Vertical");

            if (directionalInput != Vector3.zero && directionalInput != lastChargeDirection) {
                Vector3 chargeDirection = transform.position + directionalInput.normalized;
                transform.LookAt(chargeDirection);
                StopCoroutine(chargeCoroutine);
                chargeCoroutine = StartCoroutine(Charge());
                chargeBuffered = false;
            }
        }
        
        else {
            cooldownTimer += Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.Space) && cooldownTimer >= chargeCooldown) {
                chargeCoroutine = StartCoroutine(Charge());
                cooldownTimer = 0f;
            }
        }
    }


    void CheckForEndOfChargeExtension() {
        if (!chargeBuffered) { return; }

        StopCoroutine(chargeCoroutine);
        chargeCoroutine = StartCoroutine(Charge());
        chargeBuffered = false;
    }
    

    IEnumerator Charge() {
        GetComponent<PlayerController>().isAcceptingInput = false;
        lastChargeDirection = transform.forward;
        m_Rigidbody.velocity = Vector3.zero;
        isCharging = true;
        float distanceCharged = 0f;
        normalMesh.SetActive(false);
        chargingMesh.SetActive(true);
        m_AudioSource.Play();

        yield return new WaitUntil(() => {
            if (chargeLength - distanceCharged <= 0.1f) {
                return true;
            } else {
                Vector3 newPosition = transform.position + transform.forward * chargeSpeed * Time.deltaTime;
                distanceCharged += Vector3.Distance(transform.position, newPosition);
                m_Rigidbody.MovePosition(newPosition);
                return false;
            }
        });

        isCharging = false;
        GetComponent<PlayerController>().isAcceptingInput = true;
        GetComponent<PlayerHealthController>().ActivateInvincibility();

        chargingMesh.SetActive(false);
        normalMesh.SetActive(true);

        CheckForEndOfChargeExtension();

        yield return null;
    }


    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Enemy>() && isCharging) {
            Vector3 hitLocation = other.ClosestPoint(transform.position);
            Instantiate(enemyHitParticles, hitLocation, Quaternion.identity);
            other.GetComponent<Enemy>().GetHitByCharge();
        }
    }
}
