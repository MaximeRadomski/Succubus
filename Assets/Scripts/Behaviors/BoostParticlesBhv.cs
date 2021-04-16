using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostParticlesBhv : MonoBehaviour
{
    private float _y = 3.463f;

    public void Boost(Realm realm, float duration)
    {
        transform.localPosition = new Vector3(0.0f, -_y, 0.0f);
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        var main = GetComponent<ParticleSystem>().main;
        main.duration = duration;
        main.startColor = (Color)Constants.GetColorFromRealm(realm, 4);
        GetComponent<ParticleSystem>().Play();
    }

    public void Malus(Realm realm, float duration)
    {
        Boost(realm, duration);
        transform.localPosition = new Vector3(0.0f, _y, 0.0f);
        transform.localScale = new Vector3(1.0f, -1.0f, 1.0f);
    }
}
