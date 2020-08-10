using UnityEngine;

public class CameraBhv : MonoBehaviour
{
    public Camera Camera;
    public bool HasInitiated;

    private Vector3 _beforeFocusPosition;

    public void Init()
    {
        float unitsPerPixel = Constants.SceneWidth / Screen.width;
        float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;
        if (desiredHalfHeight > Constants.CameraSize)
            Camera.orthographicSize = desiredHalfHeight;
        _beforeFocusPosition = transform.position;
        HasInitiated = true;
    }

    public void FocusY(float y)
    {
        _beforeFocusPosition = transform.position;
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }

    public void Unfocus()
    {
        transform.position = _beforeFocusPosition;
    }
}
