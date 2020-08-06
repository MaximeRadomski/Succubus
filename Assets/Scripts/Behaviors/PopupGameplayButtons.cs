using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupGameplayButtons : PopupBhv
{
    private System.Func<bool, object> _resultAction;

    public void Init(System.Func<bool, object> resultAction)
    {
        _resultAction = resultAction;

        var buttonsContainer = GameObject.Find("GameplayButtonsContainer");
        foreach (Transform child in buttonsContainer.transform)
        {
            child.gameObject.GetComponent<ButtonBhv>().EndActionDelegate = ChooseButton;
        }

        var buttonNegative = transform.Find("ButtonNegative");
        buttonNegative.GetComponent<ButtonBhv>().EndActionDelegate = NegativeDelegate;
    }

    private void ChooseButton()
    {
        Constants.DecreaseInputLayer();
        _resultAction?.Invoke(true);
        Destroy(gameObject);
    }

    private void NegativeDelegate()
    {
        Constants.DecreaseInputLayer();
        _resultAction?.Invoke(false);
        Destroy(gameObject);
    }
}
