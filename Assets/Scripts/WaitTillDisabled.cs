using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitTillDisabledEnable : MonoBehaviour
{

    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void PlayeState(string stateName)
    {
        anim.Play(stateName);
    }

    public void StopState()
    {
        StartCoroutine(Stop());
    }

    IEnumerator Stop()
    {
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        print("Stop");
        anim.StopPlayback();
    }
}

