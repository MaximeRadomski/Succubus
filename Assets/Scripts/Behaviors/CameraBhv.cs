using UnityEngine;

public class CameraBhv : MonoBehaviour
{
    public Camera Camera;
    public bool HasInitiated;
    public bool Paused;

    private Vector3 _originalPosition;
    private Vector3 _targetPosition;

    private float _initialSize;
    private float _bumpSize;
    private bool _isBumbing;
    private bool _isResetBumping;
    private bool _isSliding;
    private bool _isPoundering;
    private bool _isResetingPosition;

    public void Init()
    {
        float unitsPerPixel = Constants.SceneWidth / Screen.width;
        float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;
        if (desiredHalfHeight > Constants.CameraSize)
            Camera.orthographicSize = desiredHalfHeight;
        _originalPosition = transform.position;
        HasInitiated = true;
        _isBumbing = false;
    }

    public void FocusY(float y)
    {
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }

    public void Unfocus()
    {
        transform.position = _originalPosition;
    }

    private void Update()
    {
        if (_isBumbing)
        {
            Bumping();
        }
        else if (_isResetBumping)
        {
            ResetBumping();
        }
        if (_isSliding && !Paused)
        {
            Sliding();
        }
        else if (_isPoundering)
        {
            Poundering();
        }
        else if (_isResetingPosition)
        {
            ResetingPosition();
        }

    }

    public void Bump(int bumpPercent)
    {
        _initialSize = Camera.orthographicSize;
        _bumpSize = _initialSize - (_initialSize * Helper.MultiplierFromPercent(0.0f, bumpPercent));
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
        Camera.orthographicSize = Mathf.Lerp(Camera.orthographicSize, _initialSize, 0.3f);
        if (Helper.FloatEqualsPrecision(Camera.orthographicSize, _initialSize, 0.01f))
        {
            Camera.orthographicSize = _initialSize;
            _isResetBumping = false;
        }
    }

    public void Pounder(int nbPixel)
    {
        _targetPosition = transform.position + new Vector3(0.0f, Constants.Pixel * nbPixel * 3, 0.0f);
        _isResetingPosition = false;
        _isPoundering = true;
    }

    private void Poundering()
    {
        transform.position = Vector3.Lerp(transform.position, _targetPosition, 0.25f);
        if (Helper.FloatEqualsPrecision(transform.position.y, _targetPosition.y, 0.05f))
        {
            _isPoundering = false;
            _isResetingPosition = true;
        }
    }

    private void ResetingPosition()
    {
        transform.position = Vector3.Lerp(transform.position, _originalPosition, 0.001f);
        if (Helper.FloatEqualsPrecision(transform.position.y, _originalPosition.y, 0.001f))
        {
            transform.position = _originalPosition;
            _isResetingPosition = false;
        }
    }

    public void SlideToPosition(Vector3 target)
    {
        _targetPosition = new Vector3(target.x, target.y, transform.position.z);
        _isSliding = true;
    }

    private void Sliding()
    {
        Camera.transform.position = Vector3.Lerp(Camera.transform.position, _targetPosition, 0.15f);
        if (Helper.VectorEqualsPrecision(Camera.transform.position, _targetPosition, 0.01f))
        {
            Camera.transform.position = _targetPosition;
            _isSliding = false;
        }
    }
}
