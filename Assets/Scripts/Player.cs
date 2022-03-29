using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody rb;
    private Animator animator;
    private bool holdingFinger = false;
    private float touchPosX = 0f;
    [SerializeField] private float gravityFactor = 1;
    [SerializeField] private float jumpFactor = 1;
    [SerializeField] private float forwardSpeed = 1;
    [SerializeField] private float turnRate = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                touchPosX = touch.position.x;
                holdingFinger = true;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                holdingFinger = false;
            }
            else
            {
                if (touch.position.x - touchPosX > 0)
                {
                    RotatePlayer(1);
                }
                else
                {
                    RotatePlayer(-1);
                }
            }
        }
        
        if (holdingFinger)
        {
            MoveForward();
        }

        //GravityEffect();
    }

    private void RotatePlayer(int d)
    {
        transform.Rotate(Vector3.up * d * Time.deltaTime * turnRate);
    }

    private void MoveForward()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + (forwardSpeed * Time.deltaTime));
    }

    private void GravityEffect()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y - (0.01f * gravityFactor), transform.position.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Trampoline"))
        {
            int i = UnityEngine.Random.Range(0, 2);

            if (i == 0)
            {
                animator.Play("Flip1");
            }
            else
            {
                animator.Play("Flip2");
            }

            rb.AddForce(Vector3.up * jumpFactor, ForceMode.Force);
        }
    }
}
