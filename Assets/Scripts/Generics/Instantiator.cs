﻿using UnityEngine;

public class Instantiator : MonoBehaviour
{
    void Start()
    {
        Init();
    }

    public void Init()
    {
    }

    public GameObject NewPiece(string pieceLetter, string realm, Vector3 spawnerPosition, bool keepSpawnerX = false)
    {
        var tmpPieceObject = Resources.Load<GameObject>("Prefabs/" + pieceLetter + "-" + realm);
        var pieceModel = tmpPieceObject.GetComponent<Piece>();
        var tmpPieceInstance = Instantiate(tmpPieceObject, spawnerPosition + new Vector3(keepSpawnerX ? 0.0f : pieceModel.XFromSpawn, pieceModel.YFromSpawn, 0.0f), tmpPieceObject.transform.rotation);
        return tmpPieceInstance;
    }

    public void EditViaKeyboard()
    {
        var target = GameObject.Find(Constants.LastEndActionClickedName);
        if (target == null)
            return;
        ShowKeyboard(target.GetComponent<TMPro.TextMeshPro>(), target.GetComponent<BoxCollider2D>().size.x);
    }

    public void ShowKeyboard(TMPro.TextMeshPro target, float maxWidth = -1)
    {
        var tmpKeyboardObject = Resources.Load<GameObject>("Prefabs/Keyboard");
        var tmpKeyboardInstance = Instantiate(tmpKeyboardObject, tmpKeyboardObject.transform.position, tmpKeyboardObject.transform.rotation);
        Constants.IncreaseInputLayer(tmpKeyboardInstance.name);
        for (int i = 0; i < tmpKeyboardInstance.transform.childCount; ++i)
        {
            var inputKeyBhv = tmpKeyboardInstance.transform.GetChild(i).GetComponent<InputKeyBhv>();
            if (inputKeyBhv != null)
                inputKeyBhv.SetPrivates(target, maxWidth);
        }
        tmpKeyboardInstance.transform.Find("InputKeyLayout" + PlayerPrefs.GetInt(Constants.PpFavKeyboardLayout, Constants.PpFavKeyboardLayoutDefault)).GetComponent<InputKeyBhv>().ChangeLayout();
        if (target.transform.position.y < -Camera.main.orthographicSize + Constants.KeyboardHeight)
            Camera.main.gameObject.GetComponent<CameraBhv>().FocusY(target.transform.position.y + (Camera.main.orthographicSize - Constants.KeyboardHeight));
    }

    public void NewPopupYesNo(string title, string content, string negative, string positive,
        System.Func<bool, object> resultAction)
    {
        var tmpPopupObject = Resources.Load<GameObject>("Prefabs/PopupYesNo");
        var tmpPopupInstance = Instantiate(tmpPopupObject, tmpPopupObject.transform.position, tmpPopupObject.transform.rotation);
        Constants.IncreaseInputLayer(tmpPopupInstance.name);
        tmpPopupInstance.GetComponent<PopupYesNoBhv>().SetPrivates(title, content, negative, positive, resultAction);
    }

    public void NewOverBlend(OverBlendType overBlendType, string message, float? constantLoadingSpeed,
        System.Func<bool, object> resultAction, bool reverse = false)
    {
        var tmpOverBlendObject = Resources.Load<GameObject>("Prefabs/OverBlend");
        var tmpOverBlendInstance = Instantiate(tmpOverBlendObject, tmpOverBlendObject.transform.position, tmpOverBlendObject.transform.rotation);
        tmpOverBlendInstance.GetComponent<OverBlendBhv>().SetPrivates(overBlendType, message, constantLoadingSpeed, resultAction, reverse);
    }

    public void NewSnack(string content, float duration = 2.0f)
    {
        var tmpSnackObject = Resources.Load<GameObject>("Prefabs/Snack");
        var tmpSnackInstance = Instantiate(tmpSnackObject, tmpSnackObject.transform.position, tmpSnackObject.transform.rotation);
        tmpSnackInstance.GetComponent<SnackBhv>().SetPrivates(content, duration);
    }

    public PauseMenuBhv NewPauseMenu()
    {
        var tmpPauseMenuObject = Resources.Load<GameObject>("Prefabs/PauseMenu");
        var tmpPauseMeuInstance = Instantiate(tmpPauseMenuObject, tmpPauseMenuObject.transform.position, tmpPauseMenuObject.transform.rotation);
        var pauseMenuBhv = tmpPauseMeuInstance.GetComponent<PauseMenuBhv>();
        pauseMenuBhv.SetPrivates();
        return pauseMenuBhv;
    }

    public void PopText(string text, Vector2 position, TextType type, TextThickness thickness = TextThickness.Thick)
    {
        var tmpPoppingTextObject = Resources.Load<GameObject>("Prefabs/PoppingText");

        var tmpTexts = GameObject.FindGameObjectsWithTag(Constants.TagPoppingText);
        var nbTextsOnThisPosition = 0;
        foreach (var tmpText in tmpTexts)
        {
            if (Helper.VectorEqualsPrecision(tmpText.GetComponent<PoppingTextBhv>().StartingPosition, position, 0.01f))
                ++nbTextsOnThisPosition;
        }

        var tmpPoppingTextInstance = Instantiate(tmpPoppingTextObject, position, tmpPoppingTextObject.transform.rotation);
        tmpPoppingTextInstance.GetComponent<PoppingTextBhv>().SetPrivates(text, position + new Vector2(0.0f, -0.3f * nbTextsOnThisPosition), type, thickness);
    }

    public void PopIcon(Sprite sprite, Vector2 position)
    {
        var tmpPoppingIconObject = Resources.Load<GameObject>("Prefabs/PoppingIcon");
        var tmpPoppingIconInstance = Instantiate(tmpPoppingIconObject, position, tmpPoppingIconObject.transform.rotation);
        tmpPoppingIconInstance.GetComponent<PoppingIconBhv>().SetPrivates(sprite, position + new Vector2(0.0f, +0.3f));
    }
}
