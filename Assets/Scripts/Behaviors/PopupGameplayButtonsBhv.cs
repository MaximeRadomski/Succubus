using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PopupGameplayButtonsBhv : PopupBhv
{
    private Action<bool> _resultAction;

    public void Init(Action<bool> resultAction, Instantiator instantiator)
    {
        _resultAction = resultAction;

        var buttonsContainer = GameObject.Find("GameplayButtonsContainer");
        foreach (Transform child in buttonsContainer.transform)
        {
            child.gameObject.GetComponent<ButtonBhv>().EndActionDelegate = ChooseButton;
            child.gameObject.GetComponent<ButtonBhv>().LongPressActionDelegate = () => { instantiator.NewToast(Helper.GameplayButtonToBinding(child.name).ToLower()); };
        }

        var buttonNegative = transform.Find("ButtonNegative");
        buttonNegative.GetComponent<ButtonBhv>().EndActionDelegate = NegativeDelegate;
    }

    private void ChooseButton()
    {
        Cache.DecreaseInputLayer();
        _resultAction?.Invoke(true);
        Destroy(gameObject);
    }

    private void NegativeDelegate()
    {
        Cache.DecreaseInputLayer();
        _resultAction?.Invoke(false);
        Destroy(gameObject);
    }
}
