using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupTrainingFreeLevelBhv : PopupBhv
{
    private System.Action<int> _resultAction;

    public void Init(System.Action<int> resultAction)
    {
        _resultAction = resultAction;

        int i = 0;
        var levelsContainer = GameObject.Find("LevelsContainer");
        foreach (Transform child in levelsContainer.transform)
        {
            var buttonBhv = child.gameObject.GetComponent<ButtonBhv>();
            buttonBhv.EndActionDelegate = ChooseLevel;
            if (i == 0)
                buttonBhv.IsMenuSelectorResetButton = true;
            ++i;
        }

        var buttonNegative = transform.Find("ButtonNegative");
    }

    private void ChooseLevel()
    {
        var lastClickedButton = GameObject.Find(Cache.LastEndActionClickedName);
        var buttonId = int.Parse(lastClickedButton.gameObject.name.Substring(lastClickedButton.gameObject.name.Length - 2, 2));
        Cache.DecreaseInputLayer();
        _resultAction?.Invoke(buttonId);
        Destroy(gameObject);
    }

    public override void ExitPopup()
    {
        _resultAction?.Invoke(1);
        base.ExitPopup();
    }
}
