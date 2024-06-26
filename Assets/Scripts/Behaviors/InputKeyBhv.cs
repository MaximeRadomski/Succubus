﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AccountSceneBhv;

public class InputKeyBhv : PopupBhv
{
    public Vector3[] LayoutPositions;
    public Sprite[] Sprites;
    public TMPro.TextMeshPro Target;

    private GameObject _keyboard;
    //private GameObject _keyHover;
    private ButtonBhv _buttonBhv;
    private Identifier _identifier;
    private TMPro.TextMeshPro _textMeshLower;
    private TMPro.TextMeshPro _textMeshUpper;
    private SpriteRenderer _spriteRenderer;
    private string _originalTargetText;
    private string _lowerCase;
    private string _upperCase;
    private int _layoutId;
    private int _pressedDelCount;
    private float _maxWidth;
    private bool _isUpperCase;
    private bool _isPassword;
    private bool _isSigns;

    public void SetPrivates(TMPro.TextMeshPro target, Identifier identifier, bool isPassword, float maxWidth = -1)
    {
        Target = target;
        _originalTargetText = identifier.Text;
        _maxWidth = maxWidth;
        _identifier = identifier;
        _isPassword = isPassword;
        _isSigns = false;
        _keyboard = this.transform.parent.gameObject;
        //_keyHover = _keyboard.transform.Find("KeyHover").gameObject;
        _buttonBhv = GetComponent<ButtonBhv>();
        if (transform.childCount > 0)
        {
            _textMeshLower = transform.GetChild(0).GetComponent<TMPro.TextMeshPro>();
            _textMeshUpper = transform.GetChild(1).GetComponent<TMPro.TextMeshPro>();
            _spriteRenderer = transform.GetChild(2).GetComponent<SpriteRenderer>();
        }
        var name = this.gameObject.name;
        if (name.Contains("Layout"))
        {
            char layoutIdChar = name[Helper.CharacterAfterString(name, "Layout")];
            string layoutIdStr = layoutIdChar.ToString();
            _layoutId = int.Parse(layoutIdStr);
            _buttonBhv.EndActionDelegate = ChangeLayout;
        }
        else if (name.Contains("Shift"))
        {
            _buttonBhv.EndActionDelegate = () => Shift();
        }
        else if (name.Contains("Del"))
        {
            _buttonBhv.BeginActionDelegate = BeginDel;
            _buttonBhv.DoActionDelegate = PressedDel;
            _buttonBhv.EndActionDelegate = EndDel;
        }
        else if (name.Contains("Close"))
        {
            _buttonBhv.EndActionDelegate = ExitPopup;
        }
        else if (name.Contains("Abc!#1"))
        {
            _buttonBhv.EndActionDelegate = NumbersSingsLayout;
        }
        else if (name.Contains("Cancel"))
        {
            _buttonBhv.EndActionDelegate = Cancel;
        }
        else if (name.Contains("Validate"))
        {
            _buttonBhv.EndActionDelegate = Validate;
        }
        else if (name.Contains("Space"))
        {
            _upperCase = " ";
            _lowerCase = " ";
            _textMeshLower.text = _lowerCase;
            _textMeshUpper.text = _upperCase;
            _isUpperCase = false;
            UpdateTextMesh();
            _buttonBhv.EndActionDelegate = () => AddLetter();
        }
        else
        {
            _upperCase = name[Helper.CharacterAfterString(name, "Key")].ToString();
            _lowerCase = name[Helper.CharacterAfterString(name, "Key") + 1].ToString();
            _textMeshLower.text = _lowerCase;
            _textMeshUpper.text = _upperCase;
            _isUpperCase = false;
            UpdateTextMesh();
            _buttonBhv.EndActionDelegate = () => AddLetter();
        }
    }

    #region Layout

    public void ChangeLayout()
    {
        Mock.SetInt(Constants.PpFavKeyboardLayout, _layoutId);
        for (int i = 0; i < _keyboard.transform.childCount; ++i)
        {
            var inputKeyBhv = _keyboard.transform.GetChild(i).GetComponent<InputKeyBhv>();
            if (inputKeyBhv != null)
                inputKeyBhv.GoToLayoutPosition(_layoutId);
        }
    }

    public void NumbersSingsLayout()
    {
        _isSigns = !_isSigns;
        var layout = 3;
        if (_isSigns)
            _upperCase = "ABC";
        else
        {
            _upperCase = "!#1";
            layout = _layoutId;
        }
        for (int i = 0; i < _keyboard.transform.childCount; ++i)
        {
            var inputKeyBhv = _keyboard.transform.GetChild(i).GetComponent<InputKeyBhv>();
            if (inputKeyBhv != null)
                inputKeyBhv.GoToLayoutPosition(layout);
        }
    }

    public void GoToLayoutPosition(int idLayout)
    {
        transform.position = LayoutPositions[idLayout] + _keyboard.transform.position;
    }

    #endregion

    #region Shift

    public void Shift(bool? isUpper = null)
    {
        if (isUpper == null)
            _isUpperCase = !_isUpperCase;
        else
            _isUpperCase = isUpper.Value;
        if (_isUpperCase)
            _spriteRenderer.sprite = Sprites[1];
        else
            _spriteRenderer.sprite = Sprites[0];

        for (int i = 0; i < _keyboard.transform.childCount; ++i)
        {
            var inputKeyBhv = _keyboard.transform.GetChild(i).GetComponent<InputKeyBhv>();
            if (inputKeyBhv != null)
                inputKeyBhv.UpperLower(_isUpperCase);
        }
    }

    public void UpperLower(bool isUpperCase)
    {
        _isUpperCase = isUpperCase;
        UpdateTextMesh();
    }

    #endregion

    #region Del

    private void BeginDel()
    {
        Del();
        _pressedDelCount = 0;
    }

    private void PressedDel()
    {
        ++_pressedDelCount;
        if (_pressedDelCount > 30)
        {
            Del();
            _pressedDelCount -= 4;
        }
    }

    private void EndDel()
    {
        _pressedDelCount = 0;
    }

    public void Del()
    {
        if (_identifier.Text.Length > 0)
        {
            _identifier.Text = _identifier.Text.Substring(0, Target.text.Length - 1);
            if (_isPassword)
                Target.text = ToPassword(_identifier.Text);
            else
                Target.text = _identifier.Text;
        }
    }

    #endregion

    #region Close

    public override void ExitPopup()
    {
        Helper.GetMainCamera().gameObject.GetComponent<CameraBhv>().Unfocus();
        Cache.DecreaseInputLayer();
        Cache.KeyboardUp = false;
        Destroy(_keyboard.gameObject);
    }

    #endregion

    #region Cancel

    public void Cancel()
    {
        _identifier.Text = _originalTargetText;
        if (_isPassword)
            Target.text = ToPassword(_identifier.Text, full: true);
        else
            Target.text = _identifier.Text;
        ExitPopup();
    }

    #endregion

    #region Validate

    public void Validate()
    {
        if (_isPassword)
            Target.text = ToPassword(_identifier.Text, full: true);
        ExitPopup();
    }

    #endregion

    #region Letter

    private void UpdateTextMesh()
    {
        if (_lowerCase == null)
            return;
        _textMeshUpper.enabled = _isUpperCase;
        _textMeshLower.enabled = !_isUpperCase;
    }

    public void AddLetter(string specificLetter = null)
    {
        if (_lowerCase == " " && _identifier.Text.EndsWith(" "))
            return;
        if (specificLetter == null)
            _identifier.Text += _isUpperCase ? _upperCase : _lowerCase;
        else
            _identifier.Text += _isUpperCase ? specificLetter.ToUpper(): specificLetter.ToLower();
        if (_isPassword)
            Target.text = ToPassword(_identifier.Text);
        else
            Target.text = _identifier.Text;
        if (_maxWidth > 0 && Target.renderedWidth > _maxWidth)
            Del();
    }

    #endregion

    private string ToPassword(string text, bool full = false)
    {
        var textPass = "";
        for (int i = 0; i < text.Length; ++i)
        {
            if (i == text.Length - 1 && !full)
                textPass += text[i].ToString();
            else
                textPass += "+";
        }
        return textPass;
    }
}
