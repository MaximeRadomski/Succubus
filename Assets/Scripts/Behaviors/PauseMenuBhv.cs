using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class PauseMenuBhv : PopupBhv
{
    private Action<bool> _resumeAction;
    private Instantiator _instantiator;
    private bool _isHorizontal;
    private Camera _mainCamera;

    public void Init(Instantiator instantiator, Action<bool> resumeAction, GameSceneBhv callingScene, bool isHorizontal)
    {
        _instantiator = instantiator;
        _resumeAction = resumeAction;
        _isHorizontal = isHorizontal;
#if UNITY_ANDROID
        if (_isHorizontal)
        {
            if (Cache.HorizontalCameraInitialPosition == null)
            {
                Cache.HorizontalCameraInitialPosition = Camera.main.transform.position;
                Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Cache.HorizontalCameraInitialPosition.Value.z);
            }
            if (Cache.HorizontalCameraInitialRotation == null)
                Cache.HorizontalCameraInitialRotation = Camera.main.transform.rotation;
            Camera.main.transform.rotation = transform.rotation;
        }
#endif
        _mainCamera = Helper.GetMainCamera();
        transform.position = new Vector3(_mainCamera.transform.position.x, _mainCamera.transform.position.y, 0.0f);
        GameObject.Find("ButtonResume").GetComponent<ButtonBhv>().EndActionDelegate = Resume;
        GameObject.Find("ButtonSettings").GetComponent<ButtonBhv>().EndActionDelegate = Settings;
        var buttonGiveUpGameObject = GameObject.Find("ButtonGiveUp");
        buttonGiveUpGameObject.GetComponent<ButtonBhv>().EndActionDelegate = GiveUp;
        if (Cache.CurrentGameMode == GameMode.Ascension || Cache.CurrentGameMode == GameMode.TrueAscension)
        {
            if (SceneManager.GetActiveScene().name == Constants.ClassicGameScene)
                buttonGiveUpGameObject.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = "Abandon";
            else
            {
                buttonGiveUpGameObject.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = "Save & Quit";
                buttonGiveUpGameObject.GetComponent<ButtonBhv>().EndActionDelegate = SaveAndQuit;
            }
        }

        var buttonInfo = transform.Find(Constants.GoButtonInfoName);
        if (callingScene != null)
            buttonInfo.GetComponent<ButtonBhv>().EndActionDelegate = () => { PauseToInfo(callingScene); };
        else
            Destroy(buttonInfo.gameObject);
    }

    private void PauseToInfo(GameSceneBhv callingScene)
    {
        var inputControlerBhv = GameObject.Find(Constants.GoInputControler).GetComponent<InputControlerBhv>();
        Cache.DecreaseInputLayer();
        inputControlerBhv.InitMenuKeyboardInputs();
        callingScene.Info();
        Destroy(this.gameObject);
    }

    private void Resume()
    {
#if UNITY_ANDROID
        if (_isHorizontal)
        {
            if (Cache.HorizontalCameraInitialPosition != null)
            {
                Camera.main.transform.position = Cache.HorizontalCameraInitialPosition.Value;
                Cache.HorizontalCameraInitialPosition = null;
            }
            if (Cache.HorizontalCameraInitialRotation != null)
            {
                Camera.main.transform.rotation = Cache.HorizontalCameraInitialRotation.Value;
                Cache.HorizontalCameraInitialRotation = null;
            }
        }
#endif
        Cache.DecreaseInputLayer();
        _resumeAction.Invoke(true);
    }

    public override void ExitPopup()
    {
        Resume();
    }

    private void Settings()
    {
        Cache.DecreaseInputLayer();
        transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        _mainCamera.transform.position = new Vector3(0.0f, 0.0f, _mainCamera.transform.position.z);
        _instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);
        bool OnBlend(bool result)
        {
            NavigationService.LoadNextScene(Constants.SettingsScene);
            return true;
        }
    }

    private void GiveUp()
    {
        _instantiator.NewPopupYesNo("Wait!", "do you really want to give up ?", "No", "Yes", OnGiveUp);

        void OnGiveUp(bool result)
        {
            if (!result)
                return;
            Cache.DecreaseInputLayer();
            _resumeAction.Invoke(false);
        }
    }

    private void SaveAndQuit()
    {
        Cache.DecreaseInputLayer();
        _resumeAction.Invoke(false);
    }
}
