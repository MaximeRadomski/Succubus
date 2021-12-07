using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupCharacterSkinsBhv : PopupBhv
{
    private System.Action<int> _resultAction;
    private List<int> _unlockedSkins;
    private int _currentSelectedSkin;

    public void Init(int charId, List<int> unlockedSkins, int currentSelectedSkin, System.Action<int> resultAction)
    {
        _resultAction = resultAction;
        _unlockedSkins = unlockedSkins;
        _currentSelectedSkin = currentSelectedSkin;

        int i = 0;
        var skinsContainer = GameObject.Find("SkinsContainer");
        foreach (Transform child in skinsContainer.transform)
        {
            var buttonBhv = child.gameObject.GetComponent<ButtonBhv>();
            child.TryGetComponent<SpriteRenderer>(out var spriteRenderer);
            spriteRenderer.sprite = Helper.GetCharacterSkin(charId, i);
            if (unlockedSkins[i] == 0)
                spriteRenderer.color = new Color(0.0f, 0.0f, 0.0f, 0.5f);
            buttonBhv.EndActionDelegate = ChooseSkin;
            if (i == currentSelectedSkin)
                buttonBhv.IsMenuSelectorResetButton = true;
            ++i;
        }

        var buttonNegative = transform.Find("ButtonNegative");
        buttonNegative.GetComponent<ButtonBhv>().EndActionDelegate = NegativeDelegate;
    }

    private void ChooseSkin()
    {
        var lastClickedButton = GameObject.Find(Cache.LastEndActionClickedName);
        var buttonId = int.Parse(lastClickedButton.gameObject.name.Substring(lastClickedButton.gameObject.name.Length - 1, 1));
        if (_unlockedSkins[buttonId] == 0)
            return;
        Cache.DecreaseInputLayer();
        _resultAction?.Invoke(buttonId);
        Destroy(gameObject);
    }

    private void NegativeDelegate()
    {
        Cache.DecreaseInputLayer();
        _resultAction?.Invoke(_currentSelectedSkin);
        Destroy(gameObject);
    }
}
