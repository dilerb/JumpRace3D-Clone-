using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody rb;
    internal bool holdingFinger = false;
    [SerializeField] private float gravityFactor = 1;
    [SerializeField] private float jumpFactor = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //GravityEffect();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trampoline"))
        {
            rb.AddForce(Vector3.up * jumpFactor, ForceMode.Force);
        }
    }

    private void GravityEffect()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y - (0.01f * gravityFactor), transform.position.z);
    }
}
