using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteOrderBhv : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private TMPro.TextMeshPro _textMesh;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        SetSpriteSortingLayerOrder(Constants.InputLayer);
        _textMesh = GetComponent<TMPro.TextMeshPro>();
        SetTextSortingLayerOrder(Constants.InputLayer);
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
        }
    }
}
