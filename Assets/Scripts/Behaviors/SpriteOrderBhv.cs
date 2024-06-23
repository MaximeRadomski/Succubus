using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ButtonBhv;

public class SpriteOrderBhv : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private TMPro.TextMeshPro _textMesh;
    private Instantiator _instantiator;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        SetSpriteSortingLayerOrder(Cache.InputLayer);
        _textMesh = GetComponent<TMPro.TextMeshPro>();
        SetTextSortingLayerOrder(Cache.InputLayer);
    }

    private void SetSpriteSortingLayerOrder(int hundred)
    {
        if (_spriteRenderer != null)
        {
            var currentOrder = _spriteRenderer.sortingOrder;
            //int toSubstract = currentOrder / 100;
            //int decimals = currentOrder - (toSubstract * 100);
            //_spriteRenderer.sortingOrder = (hundred * 100) + decimals;
            _spriteRenderer.sortingOrder = currentOrder + (hundred * 1000);
        }
    }

    private void SetTextSortingLayerOrder(int hundred)
    {
        if (_textMesh != null)
        {
            var currentOrder = _textMesh.sortingOrder;
            //int toSubstract = currentOrder / 100;
            //int decimals = currentOrder - (toSubstract * 100);
            //_textMesh.sortingOrder = (hundred * 100) + decimals;
            _textMesh.sortingOrder = currentOrder + (hundred * 1000);
            if (_textMesh.transform.childCount > 0)
            {
                var child = _textMesh.transform.GetChild(0);
                if (child != null)
                {
                    var subMesh = child.GetComponent<TMPro.TMP_SubMesh>();
                    if (subMesh != null)
                    {
                        subMesh.renderer.sortingOrder = currentOrder + (hundred * 1000);
                        subMesh.renderer.sortingLayerID = _textMesh.sortingLayerID;
                    }
                }
            }
            var attackTypeCount = AttackType.AttackTypes.Count;
            for (int i = 1; i < attackTypeCount; ++i)
            {
                var attack = AttackType.FromId(i);
                var name = attack.Name.ToLower();
                if (_textMesh.text.Contains($" {name}")) {
                    var boxCollider = _textMesh.gameObject.GetComponent<BoxCollider2D>();
                    if (boxCollider == null)
                    {
                        boxCollider = _textMesh.gameObject.AddComponent<BoxCollider2D>();
                        boxCollider.size = new Vector2(_textMesh.rectTransform.sizeDelta.x, _textMesh.rectTransform.sizeDelta.y);
                    }
                    var buttonBhv = _textMesh.gameObject.GetComponent<ButtonBhv>();
                    if (buttonBhv == null)
                        buttonBhv = _textMesh.gameObject.AddComponent<ButtonBhv>();
                    if (_instantiator == null)
                        _instantiator = GameObject.Find(Constants.GoSceneBhvName).GetComponent<Instantiator>();
                    ActionDelegate action = () =>
                    {
                        _instantiator.NewToast($"{name}: {attack.Description.ToLower()}", duration: 2.5f);
                    };
                    if (buttonBhv.EndActionDelegate != null)
                        buttonBhv.LongPressActionDelegate = action;
                    else
                        buttonBhv.EndActionDelegate = action;
                    break;
                }
            }
        }
    }
}
