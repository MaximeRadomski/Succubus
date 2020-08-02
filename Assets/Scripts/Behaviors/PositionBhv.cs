using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionBhv : MonoBehaviour
{
    public CameraVerticalSide VerticalSide;
    public CameraHorizontalSide HorizontalSide;
    public float XOffset;
    public float YOffset;
    public bool DontActivateOnStart;

    private float _verticalMult;
    private float _horizontalMult;

    void Start()
    {
        if (!DontActivateOnStart)
            UpdatePositions();   
    }

    public void UpdatePositions()
    {
        if (VerticalSide != CameraVerticalSide.None)
        {
            if (VerticalSide == CameraVerticalSide.MidVertical)
                _verticalMult = 0;
            else
                _verticalMult = VerticalSide == CameraVerticalSide.TopBorder ? 1.0f : -1.0f;
            if (!DontActivateOnStart)
                Invoke("AdjustVerticalPosition", 0.0f);
            else
                AdjustVerticalPosition();
        }
        if (HorizontalSide != CameraHorizontalSide.None)
        {
            if (HorizontalSide == CameraHorizontalSide.MidHorizontal)
                _horizontalMult = 0;
            else
                _horizontalMult = HorizontalSide == CameraHorizontalSide.RightBorder ? 1.0f : -1.0f;
            if (!DontActivateOnStart)
                Invoke("AdjustHorizontalPosition", 0.0f);
            else
                AdjustHorizontalPosition();
        }
    }

    private void AdjustVerticalPosition()
    {
        transform.position = new Vector3(transform.position.x, (_verticalMult * Camera.main.orthographicSize) + YOffset, 0.0f);
    }

    private void AdjustHorizontalPosition()
    {
        transform.position = new Vector3((_horizontalMult * Camera.main.orthographicSize * Camera.main.aspect) + XOffset, transform.position.y, 0.0f);
        transform.position += new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);
    }
}

public enum CameraVerticalSide
{
    None,
    TopBorder,
    BotBorder,
    MidVertical
}

public enum CameraHorizontalSide
{
    None,
    LeftBorder,
    RightBorder,
    MidHorizontal
}
