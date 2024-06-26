﻿using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class SceneBhv : FrameRateBehavior
{
    public bool Paused;
    public PauseMenuBhv PauseMenu;
    public Instantiator Instantiator;
    public CameraBhv CameraBhv;
    public string OnRootPreviousScene = null;
    public bool CanGoPreviousScene = true;

    public abstract MusicType MusicType { get; }

    protected MusicControlerBhv _musicControler;

    protected virtual void Init()
    {
        Application.targetFrameRate = 60;
        CameraBhv = Helper.GetMainCamera().gameObject.GetComponent<CameraBhv>();
        NavigationService.TrySetCurrentRootScene(SceneManager.GetActiveScene().name);
        Instantiator = GetComponent<Instantiator>();
        _musicControler = GameObject.Find(Constants.GoMusicControler)?.GetComponent<MusicControlerBhv>();
        if (!Cache.HasStartedBySplashScreen)
            NavigationService.NewRootScene(Constants.SplashScreenScene);
    }

    public virtual void PauseOrPrevious()
    {
        if (!NavigationService.IsRootScene())
            Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, Previous, reverse: true);
        bool Previous(bool result)
        {
            NavigationService.LoadPreviousScene();
            return result;
        }
    }

    public virtual void Resume()
    {

    }

    protected void GiveUp()
    {
        Instantiator.NewPopupYesNo(Constants.YesNoTitle,
            "You wont be able to recover your progress if you give up now!"
            , Constants.Cancel, Constants.Proceed, OnAcceptGiveUp);

        void OnAcceptGiveUp(bool result)
        {
            if (result)
            {
                CameraBhv.Unfocus();
                Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "GAME OVER", 10.0f, TransitionGiveUp, reverse: true);
                bool TransitionGiveUp(bool transResult)
                {
                    NavigationService.NewRootScene(Constants.MainMenuScene);
                    return transResult;
                }
            }
        }
    }

    protected virtual void Settings()
    {
        
    }

    protected virtual void Exit()
    {
        Instantiator.NewPopupYesNo(Constants.YesNoTitle,
            "Are you sure you want to quit the game?"
            , Constants.Cancel, Constants.Proceed, OnAcceptExit);

        void OnAcceptExit(bool result)
        {
            if (result)
            {
                Application.Quit();
            }
        }
    }
}
