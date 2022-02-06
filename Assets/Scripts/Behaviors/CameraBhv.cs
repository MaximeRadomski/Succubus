using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraBhv : FrameRateBehavior
{
    public Camera Camera;
    public List<PositionBhv> _controlPanels = null;
    public bool HasInitiated;
    public bool Paused;
    public bool IsSliding;

    private float _originalSize;
    private Vector3 _originalPosition;
    private Vector3 _targetPosition;
    private float _targetSideX;

    private float _bumpSize;
    private bool _isBumbing;
    private bool _isResetBumping;
    private bool _isPoundering;
    private bool _isResetingPoundering;
    private bool _isSidePoundering;
    private bool _isResetingSidePoundering;

    private float resetSpeed = 0.1f;

    public void Init()
    {
        float unitsPerPixel = Constants.SceneWidth / Screen.width;
        float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;
        if (desiredHalfHeight > Constants.CameraSize)
            Camera.orthographicSize = desiredHalfHeight;
        _originalSize = Camera.orthographicSize;
        _originalPosition = transform.position;
        HasInitiated = true;
        _isBumbing = false;
        if (SceneManager.GetActiveScene().name == Constants.ClassicGameScene
            || SceneManager.GetActiveScene().name == Constants.TrainingFreeGameScene)
        {
            _controlPanels = new List<PositionBhv>();
            _controlPanels.Add(GameObject.Find("UiPanelLeft").GetComponent<PositionBhv>());
            _controlPanels.Add(GameObject.Find("UiPanelRight").GetComponent<PositionBhv>());
#if UNITY_ANDROID
            if (PlayerPrefsHelper.GetGameplayChoice() == GameplayChoice.Buttons)
            {
                _controlPanels.Add(GameObject.Find("PanelLeft").GetComponent<PositionBhv>());
                _controlPanels.Add(GameObject.Find("PanelRight").GetComponent<PositionBhv>());
            }
            else
            {
                _controlPanels.Add(GameObject.Find("PanelSwipe").GetComponent<PositionBhv>());
            }
#else
            _controlPanels.Add(GameObject.Find("PanelSwipe").GetComponent<PositionBhv>());
#endif
        }
    }

    public void FocusY(float y)
    {
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }

    public void Unfocus()
    {
        transform.position = _originalPosition;
    }

    protected override void FrameUpdate()
    {
        if (_isBumbing)
        {
            Bumping();
        }
        else if (_isResetBumping)
        {
            ResetBumping();
        }
        if (!Paused)
        {
            if (IsSliding)
            {
                Sliding();
            }
            else
            {
                if (_isPoundering)
                {
                    Poundering();
                }
                else if (_isResetingPoundering)
                {
                    ResetingPoundering();
                }
                if (_isSidePoundering)
                {
                    SidePoundering();
                }
                else if (_isResetingSidePoundering)
                {
                    ResetingSidePoundering();
                }
                UpdateControlPanels();
            }
        }
    }

    public void Bump(int bumpPercent)
    {
        _bumpSize = _originalSize - (_originalSize * Helper.MultiplierFromPercent(0.0f, bumpPercent));
        _isBumbing = true;
    }

    private void Bumping()
    {
        Camera.orthographicSize = Mathf.Lerp(Camera.orthographicSize, _bumpSize, 0.3f);
        if (Helper.FloatEqualsPrecision(Camera.orthographicSize, _bumpSize, 0.1f))
        {
            _isBumbing = false;
            _isResetBumping = true;
        }
    }

    private void ResetBumping()
    {
        Camera.orthographicSize = Mathf.Lerp(Camera.orthographicSize, _originalSize, 0.3f);
        if (Helper.FloatEqualsPrecision(Camera.orthographicSize, _originalSize, 0.01f))
        {
            Camera.orthographicSize = _originalSize;
            _isResetBumping = false;
        }
    }

    public void Pounder(float nbPixel, bool hardReset = true)
    {
        if (hardReset == false)
            resetSpeed = 0.01f;
        _targetPosition = transform.position + new Vector3(0.0f, Constants.Pixel * nbPixel * 3, 0.0f);
        _isResetingPoundering = false;
        _isPoundering = true;
    }

    public void SidePounder(float mult = 1.0f)
    {
        _targetSideX = transform.position.x + (Constants.Pixel * 10 * mult);
        _isResetingSidePoundering = false;
        _isSidePoundering = true;
    }

    private void Poundering()
    {
        transform.position = Vector3.Lerp(transform.position, _targetPosition, 0.25f);
        if (Helper.FloatEqualsPrecision(transform.position.y, _targetPosition.y, 0.05f))
        {
            _isPoundering = false;
            _isResetingPoundering = true;
        }
    }

    private void ResetingPoundering()
    {
        transform.position = Vector3.Lerp(transform.position, _originalPosition, resetSpeed);
        if (Helper.FloatEqualsPrecision(transform.position.y, _originalPosition.y, 0.001f))
        {
            transform.position = _originalPosition;
            _isResetingPoundering = false;
        }
    }

    private void SidePoundering()
    {
        var tmpTarget = new Vector3(_targetSideX, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, tmpTarget, 0.1f);
        if (Helper.FloatEqualsPrecision(transform.position.y, tmpTarget.y, 0.01f))
        {
            _isSidePoundering = false;
            _isResetingSidePoundering = true;
        }
    }

    private void ResetingSidePoundering()
    {
        var tmpTarget = new Vector3(_originalPosition.x, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, tmpTarget, 0.1f);
        if (Helper.FloatEqualsPrecision(transform.position.x, _originalPosition.x, 0.001f))
        {
            transform.position = tmpTarget;
            _isResetingSidePoundering = false;
        }
    }

    private void UpdateControlPanels()
    {
        if (_controlPanels == null)
            return;
        foreach (var positionBhv in _controlPanels)
        {
            positionBhv.UpdatePositions();
        }
    }

    public void SlideToPosition(Vector3 target)
    {
        _targetPosition = new Vector3(target.x, target.y, transform.position.z);
        IsSliding = true;
    }

    private void Sliding()
    {
        Camera.transform.position = Vector3.Lerp(Camera.transform.position, _targetPosition, 0.15f);
        if (Helper.VectorEqualsPrecision(Camera.transform.position, _targetPosition, 0.01f))
        {
            Camera.transform.position = _targetPosition;
            IsSliding = false;
        }
    }
}
