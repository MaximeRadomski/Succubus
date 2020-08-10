using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuBhv : PopupBhv
{
    private System.Func<bool, object> _resumeAction;
    private Instantiator _instantiator;
    private bool _isRotated;
    private Vector3 _cameraInitialPosition;

    public void Init(Instantiator instantiator, System.Func<bool, object> resumeAction, bool isRotated)
    {
        _instantiator = instantiator;
        _resumeAction = resumeAction;
        _isRotated = isRotated;
        if (_isRotated)
        {
            _cameraInitialPosition = Camera.main.transform.position;
            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, _cameraInitialPosition.z);
            Camera.main.transform.Rotate(0.0f, 0.0f, -90.0f);
        }
        transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0.0f);
        GameObject.Find("ButtonResume").GetComponent<ButtonBhv>().EndActionDelegate = Resume;
        GameObject.Find("ButtonSettings").GetComponent<ButtonBhv>().EndActionDelegate = Settings;
        GameObject.Find("ButtonGiveUp").GetComponent<ButtonBhv>().EndActionDelegate = GiveUp;
    }

    private void Resume()
    {
        if (_isRotated)
        {
            Camera.main.transform.position = _cameraInitialPosition;
            Camera.main.transform.Rotate(0.0f, 0.0f, 90.0f);
        }
        Constants.DecreaseInputLayer();
        _resumeAction.Invoke(true);
    }

    public override void ExitPopup()
    {
        Resume();
    }

    private void Settings()
    {
        Constants.DecreaseInputLayer();
        transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        Camera.main.transform.position = new Vector3(0.0f, 0.0f, Camera.main.transform.position.z);
        _instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);
        object OnBlend(bool result)
        {
            NavigationService.LoadNextScene(Constants.SettingsScene);
            return true;
        }
    }

    private void GiveUp()
    {
        Constants.DecreaseInputLayer();
        _resumeAction.Invoke(false);
    }
}
