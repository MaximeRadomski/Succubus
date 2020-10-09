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
    public bool IsMimic;

    private Vector3 _originalScale;
    private Vector3 _doubleJumpScale = new Vector3(1.3f, 1.3f, 1.0f);
    private bool _isDoubleJumping = false;
    private bool _atLeastOneShadowSet = false;

    public void HandleOpacityOnLock(float percent)
    {
        foreach (Transform child in transform)
        {
            var tmpColor = child.gameObject.GetComponent<SpriteRenderer>().color;
            var max = 1.0f;
            if (Vector2.Distance(child.position, transform.position) > 4.0f)
                max = 0.25f;
            child.gameObject.GetComponent<SpriteRenderer>().color = new Color(tmpColor.r, tmpColor.g, tmpColor.b, percent > max ? max : percent);
        }
    }

    public int GetNbBlocksMimicked()
    {
        int count = 0;
        foreach (Transform child in transform)
        {
            if (child.GetComponent<BlockBhv>()?.IsMimicked ?? false)
                ++count;
        }
        return count;
    }

    public void SetColor(Color color)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.GetComponent<SpriteRenderer>().color = color;
        }
    }

    private void Update()
    {
        if (_isDoubleJumping)
        {
            DoubleJumping();
        }
        if (!IsLocked || !_atLeastOneShadowSet)
        {
            _atLeastOneShadowSet = true;
            foreach (Transform block in transform)
            {
                block.GetComponent<BlockBhv>()?.CastShadow();
            }
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
    public void CheckAndSetMimicStatus()
    {
        var nbMimibBlocks = 0;
        foreach (Transform block in transform)
        {
            if (Vector2.Distance(block.position, transform.position) > 4.0f)
                ++nbMimibBlocks;
        }
        if (nbMimibBlocks == 0)
        {
            HasBlocksAffectedByGravity = IsMimic = false;
            IsAffectedByGravity = true;
        }
        else
        {
            HasBlocksAffectedByGravity = IsMimic = true;
            IsAffectedByGravity = false;
        }
    }
}
