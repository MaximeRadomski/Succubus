﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PopupYesNoBhv : PopupBhv
{
    private System.Func<bool, object> _resultAction;
    private ButtonBhv _buttonPositive;
    private ButtonBhv _buttonNegative;

    public void Init(string title, string content, string negative, string positive,
        System.Func<bool, object> resultAction, Sprite sprite = null)
    {
        //transform.position = Camera.main.transform.position;
        transform.Find("Title").GetComponent<TMPro.TextMeshPro>().text = title;
        transform.Find("Content").GetComponent<TMPro.TextMeshPro>().text = content;
        _resultAction = resultAction;

        _buttonPositive = transform.Find("ButtonPositive").GetComponent<ButtonBhv>();
        _buttonPositive.EndActionDelegate = PositiveDelegate;
        _buttonPositive.transform.Find("ButtonPositiveText").GetComponent<TMPro.TextMeshPro>().text = positive;
        if (string.IsNullOrEmpty(negative))
            _buttonPositive.transform.position = new Vector3(_buttonPositive.transform.parent.position.x, _buttonPositive.transform.position.y, 0.0f);

        _buttonNegative = transform.Find("ButtonNegative").GetComponent<ButtonBhv>();
        _buttonNegative.EndActionDelegate = NegativeDelegate;
        _buttonNegative.transform.Find("ButtonNegativeText").GetComponent<TMPro.TextMeshPro>().text = negative;
        if (string.IsNullOrEmpty(negative))
            _buttonNegative.gameObject.SetActive(false);
        if (sprite != null)
        {
            var mainPicture = transform.Find("MainPicture");
            mainPicture.GetComponent<SpriteRenderer>().sprite = sprite;
            foreach (Transform child in mainPicture)
                child.GetComponent<SpriteRenderer>().enabled = true;
            if ((SceneManager.GetActiveScene().name == Constants.TrainingFreeGameScene
                || SceneManager.GetActiveScene().name == Constants.ClassicGameScene)
                && PlayerPrefsHelper.GetOrientation() == Direction.Horizontal)
            {
                mainPicture.transform.localPosition = new Vector3(0.0f, 9.5f, 0.0f);
                transform.position += new Vector3(0.0f, -2.3f, 0.0f);
            }
        }
    }

    private void PositiveDelegate()
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

    public override void ExitPopup()
    {
        Constants.DecreaseInputLayer();
        Destroy(_buttonPositive.gameObject);
        Destroy(_buttonNegative.gameObject);
        _resultAction?.Invoke(false);
        Destroy(gameObject);
    }
}
