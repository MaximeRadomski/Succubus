using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
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
    public Color ActualColor = Constants.ColorPlain;

    private Vector3 _originalScale = new Vector3(1.0f, 1.0f, 1.0f);
    private Vector3 _doubleJumpScale = new Vector3(1.5f, 1.5f, 1.0f);
    private bool _isDoubleJumping = false;
    private bool _atLeastOneShadowSet = false;
    private bool _disableAsked = false;
    private bool _canMimicAlterBlocksAffectedByGravity = true;

    public void HandleOpacityOnLock(float percent)
    {
        foreach (Transform child in transform)
        {
            var tmpColor = child.gameObject.GetComponent<SpriteRenderer>().color;
            var maxOpacity = ActualColor.a;
            if (Vector2.Distance(child.position, transform.position) > 4.0f && IsMimic)
                maxOpacity = 0.25f;
            child.gameObject.GetComponent<SpriteRenderer>().color = new Color(tmpColor.r, tmpColor.g, tmpColor.b, percent > maxOpacity ? maxOpacity : percent);
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
        ActualColor = color;
        foreach (Transform child in transform)
        {
            child.gameObject.GetComponent<SpriteRenderer>().color = color;
            if (child.childCount > 0)
            {
                var grandChildColor = child.GetChild(0).GetComponent<SpriteRenderer>().color;
                grandChildColor = new Color(grandChildColor.r, grandChildColor.g, grandChildColor.b, color.a);
                child.GetChild(0).GetComponent<SpriteRenderer>().color = grandChildColor;
            }
        }
    }

    private void Update()
    {
        if (_isDoubleJumping)
        {
            DoubleJumping();
        }
        else if (_disableAsked)
        {
            this.enabled = false;
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
        foreach (Transform block in transform)
        {
            block.localScale = _doubleJumpScale;
        }
        _isDoubleJumping = true;
    }

    private void DoubleJumping()
    {
        if (transform.childCount <= 0)
            return;
        foreach (Transform block in transform)
        {
            block.localScale = Vector3.Lerp(block.localScale, _originalScale, 0.2f);
        }
        
        if (Helper.VectorEqualsPrecision(transform.GetChild(0).localScale, _originalScale, 0.01f))
        {
            foreach (Transform block in transform)
            {
                transform.localScale = _originalScale;
            }
            _isDoubleJumping = false;
        }
    }

    public void AskDisable()
    {
        _disableAsked = true;
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
            IsMimic = false;
            IsAffectedByGravity = true;
            if (_canMimicAlterBlocksAffectedByGravity)
                HasBlocksAffectedByGravity = false;
        }
        else
        {
            IsMimic = true;
            IsAffectedByGravity = false;
            if (_canMimicAlterBlocksAffectedByGravity)
                HasBlocksAffectedByGravity = true;
        }
    }

    public void AlterBlocksAffectedByGravity(bool value)
    {
        HasBlocksAffectedByGravity = value;
        if (value)
            _canMimicAlterBlocksAffectedByGravity = false;
        else
            _canMimicAlterBlocksAffectedByGravity = true;
    }

    public void AddRandomBlocks(Realm realm, int nbBlocks, Instantiator instantiator, Transform ghost)
    {
        int remainingBlocks = nbBlocks;
        var id = Random.Range(0, transform.childCount);
        var initialRandomBlockId = id;
        while (id < 10)
        {
            if (id == transform.childCount)
                id = 0;
            if (remainingBlocks <= 0)
                break;
            for (int i = 0; i < 4; ++i)
            {
                var child = transform.GetChild(id);
                int x = i == 0 || i == 2 ? 0 : (i == 1 ? 1 : -1);
                int y = i == 1 || i == 3 ? 0 : (i == 0 ? 1 : -1);
                int roundedChildX = Mathf.RoundToInt(child.position.x);
                int roundedChildY = Mathf.RoundToInt(child.position.y);

                if (!AnyChildWithPosition(roundedChildX + x, roundedChildY + y))
                {
                    instantiator.NewPieceBlock(realm.ToString(), new Vector3(roundedChildX + x, roundedChildY + y, 0.0f), transform);
                    instantiator.NewPieceBlock(realm + "Ghost", new Vector3(roundedChildX + x, roundedChildY + y, 0.0f), ghost);
                    AddRandomBlocks(realm, --nbBlocks, instantiator, ghost);
                    return;
                }
            }
            ++id;
            if (id == initialRandomBlockId)
                break;
        }
    }

    private bool AnyChildWithPosition(int x, int y)
    {
        foreach (Transform child in transform)
        {
            int roundedX = Mathf.RoundToInt(child.position.x);
            int roundedY = Mathf.RoundToInt(child.position.y);

            if (child.transform.position.x == x && child.transform.position.y == y)
                return true;
        }
        return false;
    }

    public int GetWidth()
    {
        var minX = 99;
        var maxX = -99;
        var minY = 99;
        foreach (Transform child in piece.transform)
        {
            var blockBhv = child.GetComponent<BlockBhv>();
            if (blockBhv != null)
                blockBhv.Spread(0.5f);
            if (child.transform.position.x < minX)
                minX = Mathf.RoundToInt(child.transform.position.x);
            if (child.transform.position.x > maxX)
                maxX = Mathf.RoundToInt(child.transform.position.x);
            if (child.transform.position.y < minY)
                minY = Mathf.RoundToInt(child.transform.position.y);
        }
        minY--;
        minX = minX < 0 ? 0 : minX;
        maxX = maxX > 9 ? 9 : maxX;
    }
}
