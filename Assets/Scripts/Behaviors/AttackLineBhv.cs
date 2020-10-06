using UnityEngine;
using System.Collections;

public class AttackLineBhv : MonoBehaviour
{
    private Realm _realm;
    private Instantiator _instantiator;
    private Vector3 _target;

    public void Init(GameObject target, Realm realm, Instantiator instantiator)
    {
        _target = target.transform.position;
        _realm = realm;
        _instantiator = instantiator;
    }

    void Update()
    {
        if (_target == null)
            return;
        _instantiator.NewFadeBlock(_realm, Vector3.Lerp(transform.position, _target, 0.1f), 4, -1);
        if (Helper.VectorEqualsPrecision(transform.position, _target, 0.1f))
            Destroy(gameObject);
    }
}