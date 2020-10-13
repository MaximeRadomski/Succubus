using UnityEngine;
using System.Collections;
using UnityEditor;

public class AttackLineBhv : MonoBehaviour
{
    private Realm _realm;
    private Instantiator _instantiator;
    private Vector3 _target;
    private float _distance;
    private float _distanceToAdd;

    public void Init(Vector3 source, Vector3 target, Realm realm, Instantiator instantiator)
    {
        _target = target;
        transform.position = source + new Vector3(0.0f, -2.0f, 0.0f);
        if (transform.position.y < 0)
            transform.position = new Vector3(transform.position.x, 0.0f, 0.0f);
        _realm = realm;
        _instantiator = instantiator;
        _distance = _distanceToAdd = 0.05f;
    }

    void Update()
    {
        if (_target == null)
            return;
        var fadeBlock = _instantiator.NewFadeBlock(_realm, transform.position, 4, 0);
        fadeBlock.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
        transform.position = Vector3.Lerp(transform.position, _target, _distance);
        if (Helper.VectorEqualsPrecision(transform.position, _target, 0.1f))
            Destroy(gameObject);
        _distance += _distanceToAdd;
    }
}