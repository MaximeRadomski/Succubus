using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public string Letter;
    public float XFromSpawn;
    public float YFromSpawn;
    public bool IsLocked;
    public bool IsAffectedByGravity = true;
    public bool HasBlocksAffectedByGravity;
    public RotationState RotationState = RotationState.O;

    private Vector3 _originalScale;
    private Vector3 _doubleJumpScale = new Vector3(1.3f, 1.3f, 1.0f);
    private bool _isDoubleJumping = false;

    public void HandleOpacityOnLock(float percent)
    {
        foreach (Transform child in transform)
        {
            var tmpColor = child.gameObject.GetComponent<SpriteRenderer>().color;
            child.gameObject.GetComponent<SpriteRenderer>().color = new Color(tmpColor.r, tmpColor.g, tmpColor.b, percent + 0.25f);
        }
    }

    public void SetColor(Color color)
    {
        foreach (Transform child in transform)
        {
            var tmpColor = child.gameObject.GetComponent<SpriteRenderer>().color;
            child.gameObject.GetComponent<SpriteRenderer>().color = color;
        }
    }

    private void Update()
    {
        if (_isDoubleJumping)
        {
            DoubleJumping();
        }
    }

    public void DoubleJump()
    {
        _originalScale = transform.localScale;
        transform.localScale = _doubleJumpScale;
        _isDoubleJumping = true;
    }

    private void DoubleJumping()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _originalScale, 0.2f);
        if (Helper.VectorEqualsPrecision(transform.localScale, _originalScale, 0.01f))
        {
            transform.localScale = _originalScale;
            _isDoubleJumping = false;
        }
    }
}
