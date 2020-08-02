using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PopupBhv : MonoBehaviour
{
    public virtual void ExitPopup()
    {
        Constants.DecreaseInputLayer();
        Destroy(gameObject);
    }
}
