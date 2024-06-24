using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ButtonBhv;

public class TextCheckerBhv : FrameRateBehavior
{
    public bool CheckAttacks;
    public bool CheckImmunities;
    public bool CheckWeaknesses;

    private Instantiator _instantiator;
    private TMPro.TextMeshPro _textMesh;
    void Start()
    {
        CheckText();
    }

    private void CheckText()
    {
        _textMesh = GetComponent<TMPro.TextMeshPro>();
        _instantiator = GameObject.Find(Constants.GoSceneBhvName).GetComponent<Instantiator>();
        if (_textMesh == null || _instantiator == null)
            return;
        if (CheckAttacks)
            DoCheckAttacks();
        if (CheckImmunities)
            DoCheckImmunities();
        if (CheckWeaknesses)
            DoCheckWeaknesses();
    }

    private void DoCheckAttacks()
    {
        var attackTypeCount = AttackType.AttackTypes.Count;
        for (int i = 1; i <= attackTypeCount; ++i)
        {
            var attack = AttackType.FromId(i);
            var name = attack.Name.ToLower();   
            if (_textMesh.text.Contains($" {name}"))
            {
                AddCheckPossibility(() =>
                {
                    _instantiator.NewToast($"{name}: {attack.Description.ToLower()}", duration: 3.0f);
                });
                break;
            }
        }
    }

    private void DoCheckImmunities()
    {
        var immunitiesCount = Immunity.Immunities.Count;
        for (int i = 1; i <= immunitiesCount; ++i)
        {
            var immunity = Immunity.FromId(i);
            var name = immunity.Name.ToLower();
            if (_textMesh.text.Contains($"{name}"))
            {
                AddCheckPossibility(() =>
                {
                    _instantiator.NewToast($"{name} immunity: {immunity.Description.ToLower()}", duration: 3.0f);
                });
                break;
            }
        }
    }

    private void DoCheckWeaknesses()
    {
        var weaknessCount = Weakness.Weaknesses.Count;
        for (int i = 1; i <= weaknessCount; ++i)
        {
            var weakness = Weakness.FromId(i);
            var name = weakness.Name.ToLower();
            if (_textMesh.text.Contains($"{name}"))
            {
                AddCheckPossibility(() =>
                {
                    _instantiator.NewToast($"{name} weakness: {weakness.Description.ToLower()}", duration: 3.0f);
                });
                break;
            }
        }
    }

    private void AddCheckPossibility(ActionDelegate action)
    {
        var boxCollider = _textMesh.gameObject.GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            boxCollider = _textMesh.gameObject.AddComponent<BoxCollider2D>();
            boxCollider.size = new Vector2(_textMesh.rectTransform.sizeDelta.x, _textMesh.rectTransform.sizeDelta.y);
        }
        var buttonBhv = _textMesh.gameObject.GetComponent<ButtonBhv>();
        if (buttonBhv == null)
            buttonBhv = _textMesh.gameObject.AddComponent<ButtonBhv>();
        if (buttonBhv.EndActionDelegate != null)
            buttonBhv.LongPressActionDelegate = action;
        else
            buttonBhv.EndActionDelegate = action;
    }
}
