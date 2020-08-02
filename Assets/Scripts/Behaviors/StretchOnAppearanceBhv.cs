using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StretchOnAppearanceBhv : MonoBehaviour
{
    private bool _isStretching;
    private Vector3 _resetedScale;
    private Vector3 _stretchedScale;

    void Start()
    {
        SetPrivates();
    }

    public void SetPrivates()
    {
        _isStretching = false;
        _resetedScale = new Vector3(1.0f, 1.0f, 1.0f);
        if (gameObject.name.Contains("Popup"))
            _stretchedScale = new Vector3(0.9f, 0.9f, 1.0f);
        else
            _stretchedScale = new Vector3(0.75f, 0.75f, 1.0f);
        Stretch();
    }

    public void Stretch()
    {
        _isStretching = true;
        transform.localScale = _stretchedScale;
    }

    private void Update()
    {
        if (_isStretching)
            StretchOnBegin();
    }

    private void StretchOnBegin()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _resetedScale, 0.4f);
        if (transform.localScale == _resetedScale)
            _isStretching = false;
    }
}
