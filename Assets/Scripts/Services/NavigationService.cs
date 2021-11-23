using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class NavigationService
{
    public static NavigationParameter SceneParameter;
    public static string Path;

    public static void TrySetCurrentRootScene(string name)
    {
        if (string.IsNullOrEmpty(Path))
            Path = "/" + name;
    }

    public static void NewRootScene(string name)
    {
        Path = string.Empty;
        LoadNextScene(name);
    }

    public static void LoadBackUntil(string name)
    {
        Path = Path.Substring(0, Path.IndexOf(name) + name.Length);
        SceneManager.LoadScene(name);
    }

    public static void LoadNextScene(string name, NavigationParameter parameter = null)
    {
        SceneParameter = parameter;
        if (name == SceneManager.GetActiveScene().name)
            return;
        Cache.NameLastScene = SceneManager.GetActiveScene().name;
        Path += "/" + name;
        //Debug.Log("    [DEBUG]    Path = " + _path);
        SceneManager.LoadScene(name);
    }

    public static void OverBlendPreviousScene(string onRootPreviousScene = null)
    {
        var instantiator = GameObject.Find(Constants.GoSceneBhvName).GetComponent<Instantiator>();
        if (instantiator != null)
            instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, string.Empty, 10.0f, OnToPreviousScene, true);
        else
            LoadPreviousScene(onRootPreviousScene);

        object OnToPreviousScene(bool result)
        {
            LoadPreviousScene(onRootPreviousScene);
            return result;
        }
    }

    public static bool IsRootScene()
    {
        Cache.NameLastScene = SceneManager.GetActiveScene().name;
        var lastSeparator = Path.LastIndexOf('/');
        return string.IsNullOrEmpty(Path) || lastSeparator == 0;
    }

    public static void LoadPreviousScene(string onRootPreviousScene = null)
    {
        Cache.NameLastScene = SceneManager.GetActiveScene().name;
        var lastSeparator = Path.LastIndexOf('/');
        if (string.IsNullOrEmpty(Path) || lastSeparator == 0)
        {
            if (!string.IsNullOrEmpty(onRootPreviousScene))
                NewRootScene(onRootPreviousScene);
            else
                Debug.Log("    [DEBUG]    Root");
            return;
        }
        Path = Path.Substring(0, lastSeparator);
        lastSeparator = Path.LastIndexOf('/');
        var previousScene = Path.Substring(lastSeparator + 1);
        //Debug.Log("    [DEBUG]    Path = " + _path);
        SceneManager.LoadScene(previousScene);
    }

    public static void ReloadScene()
    {
        //Debug.Log("    [DEBUG]    Path = " + _path);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

public class NavigationParameter
{
    public bool BoolParam0;
    public int IntParam0;
    public string StringParam0;
}
