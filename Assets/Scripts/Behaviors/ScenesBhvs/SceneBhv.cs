using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class SceneBhv : MonoBehaviour
{
    public bool Paused;
    public PauseMenuBhv PauseMenu;
    public Instantiator Instantiator;
    public string OnRootPreviousScene = null;
    public bool CanGoPreviousScene = true;

    protected virtual void Init()
    {
        Application.targetFrameRate = 60;
        NavigationService.TrySetCurrentRootScene(SceneManager.GetActiveScene().name);
        Instantiator = GetComponent<Instantiator>();
    }

    public virtual void Pause()
    {
        if (PauseMenu == null)
            return;
        Paused = true;
        PauseMenu.Pause();
    }

    public virtual void Resume()
    {
        if (PauseMenu == null)
            return;
        Paused = false;
        PauseMenu.UnPause();
    }

    protected void GiveUp()
    {
        Instantiator.NewPopupYesNo(Constants.YesNoTitle,
            "You wont be able to recover your progress if you give up now!"
            , Constants.Cancel, Constants.Proceed, OnAcceptGiveUp);

        object OnAcceptGiveUp(bool result)
        {
            if (result)
            {
                Camera.main.gameObject.GetComponent<CameraBhv>().Unfocus();
                Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "GAME OVER", 10.0f, TransitionGiveUp, reverse: true);
                object TransitionGiveUp(bool transResult)
                {
                    NavigationService.NewRootScene(Constants.FeaturesShopScene);
                    return transResult;
                }
            }
            return result;
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

        object OnAcceptExit(bool result)
        {
            if (result)
            {
                Application.Quit();
            }
            return result;
        }
    }
}
