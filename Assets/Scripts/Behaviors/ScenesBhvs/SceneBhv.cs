using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class SceneBhv : MonoBehaviour
{
    public bool Paused;
    public PauseMenuBhv PauseMenu;
    public Instantiator Instantiator;
    public CameraBhv CameraBhv;
    public string OnRootPreviousScene = null;
    public bool CanGoPreviousScene = true;

    protected MusicControlerBhv _musicControler;

    protected virtual void Init()
    {
        Application.targetFrameRate = 60;
        CameraBhv = Camera.main.gameObject.GetComponent<CameraBhv>();
        NavigationService.TrySetCurrentRootScene(SceneManager.GetActiveScene().name);
        Instantiator = GetComponent<Instantiator>();
        _musicControler = GameObject.Find(Constants.GoMusicControler)?.GetComponent<MusicControlerBhv>();
    }

    public virtual void PauseOrPrevious()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, Previous, reverse: true);
        object Previous(bool result)
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

        object OnAcceptGiveUp(bool result)
        {
            if (result)
            {
                CameraBhv.Unfocus();
                Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "GAME OVER", 10.0f, TransitionGiveUp, reverse: true);
                object TransitionGiveUp(bool transResult)
                {
                    NavigationService.NewRootScene(Constants.MainMenuScene);
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
