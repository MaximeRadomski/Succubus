using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultySceneBhv : SceneBhv
{
    private List<GameObject> _difficultyButtons;

    public override MusicType MusicType => MusicType.Menu;

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        SetButtons();

        var lastSelectedDifficulty = PlayerPrefsHelper.GetDifficulty();
        SelectDifficulty(lastSelectedDifficulty.GetHashCode());
    }

    private void SetButtons()
    {
        GameObject.Find(Difficulty.Easy.ToString()).GetComponent<ButtonBhv>().EndActionDelegate = () => { SelectDifficulty(0); };
        GameObject.Find(Difficulty.Normal.ToString()).GetComponent<ButtonBhv>().EndActionDelegate = () => { SelectDifficulty(1); };
        GameObject.Find(Difficulty.Hard.ToString()).GetComponent<ButtonBhv>().EndActionDelegate = () => { SelectDifficulty(2); };
        GameObject.Find(Constants.GoButtonBackName).GetComponent<ButtonBhv>().EndActionDelegate = GoToPrevious;
        GameObject.Find(Constants.GoButtonPlayName).GetComponent<ButtonBhv>().EndActionDelegate = Play;
    }

    private void SelectDifficulty(int id)
    {
        for (int i = 0; i < Helper.EnumCount<Difficulty>(); ++i)
        {
            var button = GameObject.Find(((Difficulty)i).ToString());
            if (i == id)
            {
                button.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().color = Constants.ColorPlain;
                button.transform.GetChild(1).GetComponent<SpriteRenderer>().color = Constants.ColorPlain;
            }
            else
            {
                button.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().color = Constants.ColorPlainSemiTransparent;
                button.transform.GetChild(1).GetComponent<SpriteRenderer>().color = Constants.ColorPlainSemiTransparent;
            }
        }
        PlayerPrefsHelper.SaveDifficulty((Difficulty)id);
    }

    private void GoToPrevious()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend, reverse: true);
        object OnBlend(bool result)
        {
            NavigationService.LoadPreviousScene();
            return true;
        }
    }

    private void Play()
    {
        PlayerPrefsHelper.SaveRun(new Run(PlayerPrefsHelper.GetDifficulty()));
        Instantiator.NewOverBlend(OverBlendType.StartLoadingActionEnd, "Ascending", 2, OnBlend);
        object OnBlend(bool result)
        {
            NavigationService.LoadNextScene(Constants.StepsAscensionScene);
            return true;
        }
    }
}
