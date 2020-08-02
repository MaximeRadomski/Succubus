using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollableContainerBhv : InputBhv
{
    public float MinX, MinY;
    public float MaxX, MaxY;
    public bool LockX, LockY;

    private Vector2 _initialTouchPosition;
    private Vector2 _initialPosition;

    public override void SetPrivates()
    {
        base.SetPrivates();
        _initialPosition = transform.position;
    }

    public override void BeginAction(Vector2 initialTouchPosition)
    {
        _initialTouchPosition = initialTouchPosition;
    }

    public override void DoAction(Vector2 touchPosition)
    {
        if (!LockX)
        {
            var newX = touchPosition.x - _initialTouchPosition.x + _initialPosition.x;
            if (newX < MinX)
                newX = MinX;
            else if (newX > MaxX)
                newX = MaxX;
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }
        if (!LockY)
        {
            var newY = touchPosition.y - _initialTouchPosition.y + _initialPosition.y;
            if (newY < MinY)
                newY = MinY;
            else if (newY > MaxY)
                newY = MaxY;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }

    public override void EndAction(Vector2 lastTouchPosition)
    {
        _initialTouchPosition = new Vector2();
        _initialPosition = transform.position;
    }

    public override void CancelAction()
    {
        _initialTouchPosition = new Vector2();
        _initialPosition = transform.position;
    }
}
