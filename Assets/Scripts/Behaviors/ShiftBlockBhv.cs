using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShiftBlockBhv : MonoBehaviour
{
    private Realm _realm;
    private int _nbRows;
    private Color _coloredTransparent;
    private int _framesBeforeFade = 60;

    public void Init(int nbRows, Realm opponentRealm)
    {
        _realm = opponentRealm;
        _nbRows = nbRows;
        var background = transform.Find("Background");
        background.localScale = new Vector3(background.localScale.x, background.localScale.y * _nbRows, 1.0f);
        var realmColor = (Color)Constants.GetColorFromRealm(_realm, 4);
        var bgColor = new Color(realmColor.r, realmColor.g, realmColor.b, 0.2f);
        _coloredTransparent = new Color(realmColor.r, realmColor.g, realmColor.b, 0.0f);
        background.GetComponent<SpriteRenderer>().color = bgColor;
        foreach (Transform child in transform)
        {
            if (child.name.Contains("Corner") || child.name.Contains("Mid"))
            {
                float floatHeight = (float)(_nbRows - 1);
                if (child.name.Contains("Top"))
                    child.transform.position += new Vector3(0.0f, floatHeight / 2, 0.0f);
                else if (child.name.Contains("Bot"))
                    child.transform.position += new Vector3(0.0f, -floatHeight / 2, 0.0f);
                child.GetComponent<SpriteRenderer>().color = realmColor;
            }
        }
    }

    private void Update()
    {
        if (_framesBeforeFade-- > 0)
            return;
        foreach (Transform child in transform)
        {
            var spriteRenderer = child.GetComponent<SpriteRenderer>();
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, _coloredTransparent, 0.1f);
        }
        if (transform.GetChild(0).GetComponent<SpriteRenderer>().color == _coloredTransparent)
            Destroy(this.gameObject);
    }
}
