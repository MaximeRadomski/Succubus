using UnityEngine;

public class CameraBhv : MonoBehaviour
{
    public Camera Camera;

    private Vector3 _beforeFocusPosition;

    void Start()
    {
        float unitsPerPixel = Constants.SceneWidth / Screen.width;
        float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;
        if (desiredHalfHeight > Constants.CameraSize)
            Camera.orthographicSize = desiredHalfHeight;
        _beforeFocusPosition = transform.position;
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
