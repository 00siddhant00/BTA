using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleBase : MonoBehaviour
{
    protected bool keyEnabled = false;

    public event Action OnKeyEnabled;

    public void CheckIfKeyEnabled()
    {
        if (!keyEnabled) return;

        OnKeyEnabled?.Invoke();
    }
}
