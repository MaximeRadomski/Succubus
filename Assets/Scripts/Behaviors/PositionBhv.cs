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
    public bool Rotated;

    private float _verticalMult;
    private float _horizontalMult;

    void Start()
    {
        if (!DontActivateOnStart)
            UpdatePositions();   
    }

    public void UpdatePositions()
    {
        if (Camera.main.GetComponent<CameraBhv>().HasInitiated == false)
            Camera.main.GetComponent<CameraBhv>().Init();
        if (VerticalSide != CameraVerticalSide.None)
        {
            if (VerticalSide == CameraVerticalSide.MidVertical)
                _verticalMult = 0;
            else
                _verticalMult = VerticalSide == CameraVerticalSide.TopBorder ? 1.0f : -1.0f;
            AdjustVerticalPosition();
        }
        if (HorizontalSide != CameraHorizontalSide.None)
        {
            if (HorizontalSide == CameraHorizontalSide.MidHorizontal)
                _horizontalMult = 0;
            else
                _horizontalMult = HorizontalSide == CameraHorizontalSide.RightBorder ? 1.0f : -1.0f;
            AdjustHorizontalPosition();
        }
    }

    private void AdjustVerticalPosition()
    {
        transform.position = new Vector3(transform.position.x, (_verticalMult * Camera.main.orthographicSize * (Rotated ? Camera.main.aspect : 1.0f)) + YOffset, 0.0f);
        transform.position += new Vector3(0.0f, Camera.main.transform.position.y, 0.0f);
    }

    private void AdjustHorizontalPosition()
    {
        transform.position = new Vector3((_horizontalMult * Camera.main.orthographicSize * (Rotated ? 1.0f : Camera.main.aspect)) + XOffset, transform.position.y, 0.0f);
        transform.position += new Vector3(Camera.main.transform.position.x, 0.0f, 0.0f);
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
