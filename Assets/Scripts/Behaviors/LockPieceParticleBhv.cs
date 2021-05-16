using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPieceParticleBhv : MonoBehaviour
{
    public void Init(Realm realm)
    {
        if (GetComponent<ParticleSystem>().isPlaying)
            GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = GetComponent<ParticleSystem>().main;
        main.startColor = (Color)Constants.GetColorFromRealm(realm, 2);
        GetComponent<ParticleSystem>().Play();
        transform.GetChild(0).GetComponent<FadeOnAppearanceBhv>().Init(0.075f, (Color)Constants.GetColorFromRealm(realm, 2));
    }
}
