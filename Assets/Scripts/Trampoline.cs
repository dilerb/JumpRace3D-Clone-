using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Trampoline : MonoBehaviour
{
    public enum TrampolineType
    {
        Normal,
        Fragile,
        LongJump,
        Movable
    }

    [SerializeField] public TrampolineType _type = TrampolineType.Normal;
    [SerializeField] private ParticleSystem bounceEffect;
    [SerializeField] private TextMesh numberText;
    [SerializeField] public int nodeNumber = 0;
    [SerializeField] private Animator _animator;
    void Start()
    {
        if (_type != TrampolineType.LongJump)
        {
            numberText.text = nodeNumber.ToString();
        }

        if (_type == TrampolineType.Movable)
        {
            _animator.Play("LeftRightMove");
            //MoveAnimation();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            bounceEffect.Play();

            if (_type == TrampolineType.LongJump)
            {
                DestroyTrampoline();
            }
        }
    }

    IEnumerator DestroyTrampoline()
    {
        yield return transform.DOMoveY(transform.position.y - 1, 1f);
        transform.DOKill();
        gameObject.SetActive(false);
    }

    private void MoveAnimation()
    {
        Vector3[] path = new Vector3[2];
        path[0] = new Vector3(transform.position.x + 1f, transform.position.y, transform.position.z);
        path[1] = new Vector3(transform.position.x - 1f, transform.position.y, transform.position.z);
        //path[2] = new Vector3(transform.position.x + 1f, transform.position.y, transform.position.z);

        transform.position = path[1];
        transform.DOPath(path, 2.5f)
            //.SetEase(Ease.Linear)
            .SetLoops(-1);
    }
}