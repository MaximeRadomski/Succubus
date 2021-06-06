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
    public bool IsHollowed = false;
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

    public void Lock(Instantiator instantiator)
    {
        IsLocked = true;
        EnableRotationPoint(false, instantiator);
        //foreach (Transform child in transform)
        //{
        //    int roundedX = Mathf.RoundToInt(child.position.x);
        //    int roundedY = Mathf.RoundToInt(child.position.y);
        //    child.gameObject.name = $"{roundedX},{roundedY}";
        //}
    }

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

    public void SetColor(Color color, bool overVisionBlock = false)
    {
        ActualColor = color;
        foreach (Transform child in transform)
        {
            child.gameObject.GetComponent<SpriteRenderer>().color = color;
            if (overVisionBlock)
                child.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 100;
            if (child.childCount > 0)
            {
                var grandChildSpriteRenderer = child.GetChild(0).GetComponent<SpriteRenderer>();
                if (grandChildSpriteRenderer == null)
                    continue;
                var grandChildColor = grandChildSpriteRenderer.color;
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

    public void AddRandomBlocks(Realm realm, int nbBlocks, Instantiator instantiator, Transform ghost, Color ghostColor)
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
                    ghost.transform.position = transform.position;
                    instantiator.NewPieceBlock(realm.ToString(), new Vector3(roundedChildX + x, roundedChildY + y, 0.0f), transform);
                    var shadowBlock = instantiator.NewPieceBlock(realm + "Ghost", new Vector3(roundedChildX + x, roundedChildY + y, 0.0f), ghost);
                    shadowBlock.GetComponent<SpriteRenderer>().color = Constants.IsffectAttackInProgress == AttackType.Intoxication ? Constants.ColorPlainTransparent : ghostColor;
                    AddRandomBlocks(realm, --nbBlocks, instantiator, ghost, ghostColor);
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

    public int[] GetRangeX()
    {
        var minX = 99;
        var maxX = -99;
        foreach (Transform child in transform)
        {
            if (child.transform.position.x < minX)
                minX = Mathf.RoundToInt(child.transform.position.x);
            if (child.transform.position.x > maxX)
                maxX = Mathf.RoundToInt(child.transform.position.x);
        }
        minX = minX < 0 ? 0 : minX;
        maxX = maxX > 9 ? 9 : maxX;
        return new int[] { minX, maxX };
    }

    public int[] GetRangeY()
    {
        var minY = 99;
        var maxY = -99;
        foreach (Transform child in transform)
        {
            if (child.transform.position.y < minY)
                minY = Mathf.RoundToInt(child.transform.position.y);
            if (child.transform.position.y > maxY)
                maxY = Mathf.RoundToInt(child.transform.position.y);
        }
        minY = minY < Constants.HeightLimiter ? Constants.HeightLimiter : minY;
        maxY = maxY > 99 ? 99 : maxY;
        return new int[] { minY, maxY };
    }

    public void EnableRotationPoint(bool enable, Instantiator instantiator)
    {
        var child0 = transform.GetChild(0);
        if (child0 == null)
            return;
        var rotationPoint = child0.Find(Constants.GoRotationPoint);
        if (enable == true)
        {
            if (rotationPoint != null)
                rotationPoint.GetComponent<SpriteRenderer>().enabled = true;
            else
                instantiator.NewRotationPoint(this.gameObject);
        }
        else if (rotationPoint != null)
        {
            rotationPoint.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
