using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [SerializeField] float movementSpeed;

    [HideInInspector] public bool isAcceptingInput = true;
    public static Transform m_Transform;

    Vector3 directionalInput;
    Rigidbody m_Rigidbody;


    private void Start() {
        m_Transform = transform;
        m_Rigidbody = GetComponent<Rigidbody>();
    }


    private void Update() {
        RecieveInput();
    }


    private void FixedUpdate() {
        Move();
    }


    void RecieveInput() {
        if (!isAcceptingInput) { return; }
        directionalInput = Vector3.zero;
        directionalInput.x = Input.GetAxisRaw("Horizontal");
        directionalInput.y = Input.GetAxisRaw("Vertical");
    }


    private void Move() {
        if (!isAcceptingInput) { return; }

        Vector3 newPosition = transform.position + directionalInput.normalized * movementSpeed * Time.fixedDeltaTime;
        transform.LookAt(newPosition);
        m_Rigidbody.MovePosition(newPosition);
    }
}
