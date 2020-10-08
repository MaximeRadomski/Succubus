using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBhv : MonoBehaviour
{
    public GameObject Shadow;

    public void Start()
    {
        Shadow = transform.GetChild(0).gameObject;
        Shadow.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
        Shadow.GetComponent<SpriteRenderer>().sortingOrder = -2;
    }

    public void CastShadow()
    {
        Shadow.transform.position = transform.position + new Vector3(Constants.Pixel, Constants.Pixel, 0.0f);
    }
}
