﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuBhv : PopupBhv
{
    private System.Func<bool, object> _resumeAction;
    private Instantiator _instantiator;
    //private bool _isRotated;
    //private Vector3 _cameraInitialPosition;
    //private Quaternion _cameraInitialRotation;
    private Camera _mainCamera;

    public void Init(Instantiator instantiator, System.Func<bool, object> resumeAction)
    {
        _instantiator = instantiator;
        _resumeAction = resumeAction;
        //_isRotated = isRotated;
        //if (_isRotated)
        //{
        //    _cameraInitialPosition = Camera.main.transform.position;
        //    Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, _cameraInitialPosition.z);
        //    _cameraInitialRotation = Camera.main.transform.rotation;
        //    Camera.main.transform.rotation = transform.rotation;
        //}
        _mainCamera = Helper.GetMainCamera();
        transform.position = new Vector3(_mainCamera.transform.position.x, _mainCamera.transform.position.y, 0.0f);
        GameObject.Find("ButtonResume").GetComponent<ButtonBhv>().EndActionDelegate = Resume;
        GameObject.Find("ButtonSettings").GetComponent<ButtonBhv>().EndActionDelegate = Settings;
        var buttonGiveUpGameObject = GameObject.Find("ButtonGiveUp");
        buttonGiveUpGameObject.GetComponent<ButtonBhv>().EndActionDelegate = GiveUp;
        if (Constants.CurrentGameMode == GameMode.Ascension || Constants.CurrentGameMode == GameMode.TrueAscension)
        {
            if (SceneManager.GetActiveScene().name == Constants.ClassicGameScene)
                buttonGiveUpGameObject.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = "Abandon";
            else
                buttonGiveUpGameObject.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = "Save & Quit";
        }

    }

    private void Resume()
    {
        //if (_isRotated)
        //{
        //    Camera.main.transform.position = _cameraInitialPosition;
        //    Camera.main.transform.rotation = _cameraInitialRotation;
        //}
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
        _mainCamera.transform.position = new Vector3(0.0f, 0.0f, _mainCamera.transform.position.z);
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
