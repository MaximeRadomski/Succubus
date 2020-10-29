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
    public GameObject _customParent = null;

    private float _verticalMult;
    private float _horizontalMult;
    private Camera _mainCamera;

    void Start()
    {
        GetMainCamera();
        if (!DontActivateOnStart)
            UpdatePositions();   
    }

    private void GetMainCamera()
    {
        _mainCamera = Helper.GetMainCamera();
    }

    public void UpdatePositions()
    {
        if (_mainCamera == null)
            GetMainCamera();
        if (_mainCamera.GetComponent<CameraBhv>().HasInitiated == false)
            _mainCamera.GetComponent<CameraBhv>().Init();
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
        transform.position = new Vector3(transform.position.x, (_verticalMult * _mainCamera.orthographicSize * (Rotated ? _mainCamera.aspect : 1.0f)) + YOffset, 0.0f);
        if (_customParent == null)
            transform.position += new Vector3(0.0f, _mainCamera.transform.position.y, 0.0f);
        else
            transform.position += new Vector3(0.0f, _customParent.transform.position.y, 0.0f);
    }

    private void AdjustHorizontalPosition()
    {
        transform.position = new Vector3((_horizontalMult * _mainCamera.orthographicSize * (Rotated ? 1.0f : _mainCamera.aspect)) + XOffset, transform.position.y, 0.0f);
        if (_customParent == null)
            transform.position += new Vector3(_mainCamera.transform.position.x, 0.0f, 0.0f);
        else
            transform.position += new Vector3(_customParent.transform.position.x, 0.0f, 0.0f);
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
