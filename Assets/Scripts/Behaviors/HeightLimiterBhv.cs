using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightLimiterBhv : MonoBehaviour
{
    public List<Sprite> RealmSprites;

    private float _x = 4.5f;

    public void Set(int height, Realm realm)
    {
        var customHeight = height - 1;
        transform.position = new Vector3(_x, 0.0f, 0.0f);
        transform.GetChild(0).localPosition = new Vector3(0.0f, customHeight, 0.0f);
        transform.GetChild(1).localPosition = new Vector3(0.0f, customHeight / 2.0f, 0.0f);
        var midRenderer = transform.GetChild(1).GetComponent<SpriteRenderer>();
        midRenderer.size = new Vector2(midRenderer.size.x, customHeight - 0.13f);
        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = RealmSprites[(int)realm * 2];
        midRenderer.sprite = RealmSprites[((int)realm * 2) + 1];
        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = RealmSprites[(int)realm * 2];
    }
}
