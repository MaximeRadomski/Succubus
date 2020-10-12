using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBhv : MonoBehaviour
{
    public GameObject Shadow;
    public bool IsMimicked;

    public void Start()
    {
        Shadow = transform.GetChild(0).gameObject;
        Shadow.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
        Shadow.GetComponent<SpriteRenderer>().sortingOrder = -2;
    }

    public void CastShadow()
    {
        if (Shadow != null)
            Shadow.transform.position = transform.position + new Vector3(Constants.Pixel, Constants.Pixel, 0.0f);
    }

    public void SetMimicAppearance()
    {
        IsMimicked = true;
        GetComponent<SpriteRenderer>().color = Constants.ColorPlainQuarterTransparent;
        Shadow.GetComponent<SpriteRenderer>().color = Constants.ColorPlainSemiTransparent;
    }

    public void UnsetMimicAppearance()
    {
        IsMimicked = false;
        GetComponent<SpriteRenderer>().color = Constants.ColorPlain;
        Shadow.GetComponent<SpriteRenderer>().color = Constants.ColorBlack;
    }

    public void PreventYAxisShif()
    {
        int roundedY = Mathf.RoundToInt(transform.position.y);
        transform.position = new Vector3(transform.position.x, roundedY, 0.0f);
    }
}
