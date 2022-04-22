using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PopupBhv : FrameRateBehavior
{
    private InputControlerBhv _inputControlerBhv;

    protected InputControlerBhv InputControlerBhv
    {
        get
        {
            if (_inputControlerBhv == null)
                _inputControlerBhv = GameObject.Find(Constants.GoInputControler).GetComponent<InputControlerBhv>();
            return _inputControlerBhv;
        }
        set
        {
            _inputControlerBhv = value;
        }
    }

    public virtual void ExitPopup()
    {
        Cache.DecreaseInputLayer();
        Destroy(gameObject);
    }
}
