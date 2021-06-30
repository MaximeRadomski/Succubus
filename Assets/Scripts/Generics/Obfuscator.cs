using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Obfuscator
{
    private static float _randomFloat = -1;
    private static int _randomInt = -1;
    private static bool _hasInit;

    public static void Initialize()
    {
        if (_hasInit)
            return;
        if (_randomFloat == -1)
            _randomFloat = Random.Range(10000, 99999);
        if (_randomInt == -1)
            _randomInt = Random.Range(10000, 99999);
        _hasInit = true;
    }

    public static int Obfuscate(int originalValue)
    {
        if (!_hasInit)
            Initialize();
        return _randomInt + originalValue;
    }

    public static int Deobfuscate(int obfuscatedValue)
    {
        if (!_hasInit)
            Initialize();
        return obfuscatedValue - _randomInt;
    }

    public static float Obfuscate(float originalValue)
    {
        if (!_hasInit)
            Initialize();
        return _randomFloat + originalValue;
    }

    public static float Deobfuscate(float obfuscatedValue)
    {
        if (!_hasInit)
            Initialize();
        return obfuscatedValue - _randomFloat;
    }

}
