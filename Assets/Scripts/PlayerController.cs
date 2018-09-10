using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float MoveSpeed = 15;
    private Vector3 moveDir;
    private Rigidbody rb;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + transform.TransformDirection(moveDir) * MoveSpeed * Time.deltaTime);
    }
}
