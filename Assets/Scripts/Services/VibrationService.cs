using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VibrationService
{

#if UNITY_ANDROID && !UNITY_EDITOR
    public static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    public static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    public static AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#else
    public static AndroidJavaClass unityPlayer;
    public static AndroidJavaObject currentActivity;
    public static AndroidJavaObject vibrator;
#endif

    private static bool _hasInit;
    private static bool? _vibrationEnabled;

    private static void Init(bool force = false)
    {
        if (!force && _hasInit)
            return;
        _vibrationEnabled = PlayerPrefsHelper.GetVibrationEnabled();
        _hasInit = true;
    }

    public static void Vibrate()
    {
        if (!_hasInit || _vibrationEnabled == null)
            Init(force: true);
        if (!_vibrationEnabled.HasValue || !_vibrationEnabled.Value)
            return;
        long defaultVibrationTime = 50;
        if (isAndroid())
            vibrator.Call("vibrate", defaultVibrationTime);
#if UNITY_ANDROID
        else
            Handheld.Vibrate();
#endif
    }


    public static void Vibrate(long milliseconds)
    {
        if (!_hasInit || _vibrationEnabled == null)
            Init(force: true);
        if (!_vibrationEnabled.HasValue || !_vibrationEnabled.Value)
            return;
        if (isAndroid())
            vibrator.Call("vibrate", milliseconds);
#if UNITY_ANDROID
        else
            Handheld.Vibrate();
#endif
    }

    public static void Vibrate(long[] pattern, int repeat)
    {
        if (!_hasInit || _vibrationEnabled == null)
            Init(force: true);
        if (!_vibrationEnabled.HasValue || !_vibrationEnabled.Value)
            return;
        if (isAndroid())
            vibrator.Call("vibrate", pattern, repeat);
#if UNITY_ANDROID
        else
            Handheld.Vibrate();
#endif
    }

    public static bool HasVibrator()
    {
        return isAndroid();
    }

    public static void Cancel()
    {
        if (isAndroid())
            vibrator.Call("cancel");
    }

    private static bool isAndroid()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
	return true;
#else
        return false;
#endif
    }
}