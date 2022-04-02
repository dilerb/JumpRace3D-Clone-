using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpRace
{
    public class Player : MonoBehaviour
    {
        private bool holdingFinger = false;
        private float touchPosX = 0f;
        private int lastJumpNode = 0;

        [SerializeField] private float gravityFactor = 1;
        [SerializeField] private float jumpFactor = 1;
        [SerializeField] private float forwardSpeed = 1;
        [SerializeField] private float turnRate = 1;
        [SerializeField] internal GameObject line;
        [SerializeField] internal Animator animator;
        [SerializeField] private ParticleSystem drownEffect;
        [SerializeField] private Camera _cam;
        [SerializeField] private Rigidbody rb;
        private float swerveAmount = 0;
        private bool lookTrampoline = false;
        private Vector3 nextTrampolinePos;
        private GameObject finishObj;

        internal bool stopPlayer = false;
        private void Start()
        {
            finishObj = GameObject.FindGameObjectWithTag("Finish");
        }
        private void Update()
        {
            if (stopPlayer)
                return;

            if (Input.touchCount > 0)
            {
                lookTrampoline = false;

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
                else if (touch.phase == TouchPhase.Moved)
                {
                    swerveAmount = touch.position.x - touchPosX;
                    //RotatePlayer(swerveAmount);

                    if (Math.Abs(swerveAmount) > 1f)
                    {
                        if (swerveAmount < 0)
                            RotatePlayer(-1);
                        else
                            RotatePlayer(1);
                    }
                }
                else if (touch.phase == TouchPhase.Stationary)
                {
                    touchPosX = touch.position.x;
                }
            }

            //GravityEffect();
        }
        private void FixedUpdate()
        {

            if (holdingFinger)
            {
                MoveForward();
            }
            else
            {
                if (lookTrampoline)
                    LookNextTrampoline();
            }

        }

        private void RotatePlayer(float d)
        {
            transform.Rotate(Vector3.up * d * turnRate * Time.deltaTime);
            //transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, d * turnRate * Time.deltaTime, transform.localEulerAngles.z);
        }
        private void LookNextTrampoline()
        {
            transform.rotation = new Quaternion(transform.rotation.x, Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(nextTrampolinePos - transform.position), Time.deltaTime * 2).y,transform.rotation.z,transform.rotation.w);
        }
        private void MoveForward()
        {
            transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
        }

        //private void GravityEffect()
        //{
        //    transform.position = new Vector3(transform.position.x, transform.position.y - (0.01f * gravityFactor), transform.position.z);
        //}

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Trampoline"))
            {
                PlayerAnimation();
           
                if (collision.collider.GetComponent<Trampoline>()._type == Trampoline.TrampolineType.LongJump)
                {
                    rb.Sleep();
                    rb.AddForce(Vector3.up * jumpFactor * 2, ForceMode.Force);
                }
                else
                {
                    rb.Sleep();
                    rb.AddForce(Vector3.up * jumpFactor, ForceMode.Force);

                    if (collision.collider.GetComponent<Trampoline>().nodeNumber > 1)
                    {
                        nextTrampolinePos = collision.collider.transform.parent.GetChild(collision.collider.transform.GetSiblingIndex() + 1).position;
                        lookTrampoline = true;
                    }
                    else
                    {
                        nextTrampolinePos = new Vector3(finishObj.transform.position.x, finishObj.transform.position.y + 5f, finishObj.transform.position.z);
                        lookTrampoline = true;
                    }
                }

                if (lastJumpNode == 0)
                {
                    lastJumpNode = collision.collider.GetComponent<Trampoline>().nodeNumber;
                    GameManager.Instance.maxTrampolineCount = lastJumpNode;
                }
                else if (collision.collider.GetComponent<Trampoline>().nodeNumber < lastJumpNode)
                {
                    if (lastJumpNode - collision.collider.GetComponent<Trampoline>().nodeNumber > 1)
                    {
                        //long jump!
                        GameManager.Instance.OpenLongJumpText();
                    }
                    else if (GetComponent<DrawLine>().inGreen)
                    {
                        //perfect!
                        GameManager.Instance.OpenPerfectText();
                    }

                    lastJumpNode = collision.collider.GetComponent<Trampoline>().nodeNumber;
                }

                GameManager.Instance.SetLevelBar(lastJumpNode);
            }
            else if (collision.collider.CompareTag("Water"))
            {
                stopPlayer = true;
                Instantiate(drownEffect, transform.position, Quaternion.identity);
                _cam.transform.SetParent(null);
                gameObject.SetActive(false);
                GameManager.Instance.StopGame();
                GameManager.Instance.OpenLosePanel();
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag("Finish"))
            {
                stopPlayer = true;
                GameManager.Instance.StopGame();
                GameManager.Instance.OpenWinPanel();
                animator.enabled = true;
                animator.Play("Idle");
            }
        }

        private void PlayerAnimation()
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
        }
        public void StartFalling()
        {
            stopPlayer = false;
            rb.useGravity = true;
            line.SetActive(true);
            animator.enabled = true;
        }
        public void StopFalling()
        {
            stopPlayer = true;
            rb.useGravity = false;
            line.SetActive(false);
            animator.enabled = false;
        }
    }
}