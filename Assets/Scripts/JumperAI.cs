using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace JumpRace
{
    public class JumperAI : MonoBehaviour
    {
        //[SerializeField] private ParticleSystem drownEffect;
        //[SerializeField] internal GameObject line;
        //[SerializeField] private float forwardSpeed = 1;
        [SerializeField] private float jumpFactor = 1;
        [SerializeField] private float turnRate = 1;
        [SerializeField] internal Animator animator;
        [SerializeField] public string AIName;
        [SerializeField] private int startNode = 0;
        [SerializeField]private Rigidbody rb;

        private int lastJumpNode = 0;
        private Vector3 nextTrampolinePos;
        private GameObject trampolineParent;
        private GameObject finishObj;

        internal bool stopPlayer = false;
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            trampolineParent = GameObject.FindGameObjectWithTag("TrampolineParent");
            finishObj = GameObject.FindGameObjectWithTag("Finish");

            transform.position = new Vector3(
                trampolineParent.transform.GetChild(startNode).transform.position.x,
                trampolineParent.transform.GetChild(startNode).transform.position.y - 0.5f,
                trampolineParent.transform.GetChild(startNode).transform.position.z
                );

            nextTrampolinePos = trampolineParent.transform.GetChild(startNode + 1).position;
            lastJumpNode = trampolineParent.transform.GetChild(startNode).gameObject.GetComponent<Trampoline>().nodeNumber;
        }

        private void LookNextTrampoline()
        {
            transform.rotation = new Quaternion(transform.rotation.x, Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(nextTrampolinePos - transform.position), Time.deltaTime * 10).y, transform.rotation.z, transform.rotation.w);
        }
        IEnumerator MoveForward(Vector3 target)
        {
            LookNextTrampoline();
            yield return transform.DOJump(target, 1, 1, 1.5f).WaitForCompletion();
            rb.useGravity = true;

            //transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
        }
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
                    }
                    else
                    {
                        nextTrampolinePos = new Vector3(finishObj.transform.position.x + UnityEngine.Random.Range(-1f,1f), finishObj.transform.position.y + 5f, finishObj.transform.position.z + UnityEngine.Random.Range(-1f, 1f));
                    }
                }

                lastJumpNode = collision.collider.GetComponent<Trampoline>().nodeNumber;

                int i = UnityEngine.Random.Range(0, 4);
                if (i > 0)
                {
                    rb.useGravity = false;
                    StartCoroutine(MoveForward(nextTrampolinePos));
                }
                else
                {
                    rb.useGravity = true;
                }
            }
            else if (collision.collider.CompareTag("Water"))
            {
                StopFalling();
                //Instantiate(drownEffect, transform.position, Quaternion.identity);
                gameObject.SetActive(false);
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag("Finish"))
            {
                StopFalling();
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
            animator.enabled = true;
        }
        public void StopFalling()
        {
            stopPlayer = true;
            rb.useGravity = false;
            animator.enabled = false;
        }
    }
}