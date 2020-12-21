using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterDelay : MonoBehaviour
{
    public float CustomDelay = 1.0f;

    void Start()
    {
        Invoke(nameof(ExecuteDestroyAfterDelay), CustomDelay);
    }

    private void ExecuteDestroyAfterDelay()
    {
        Destroy(this.gameObject);
    }
}
