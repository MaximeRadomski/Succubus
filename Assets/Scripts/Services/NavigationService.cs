using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class NavigationService
{
    public static NavigationParameter SceneParameter;
    public static List<string> Path;

    public static void TrySetCurrentRootScene(string name)
    {
        if (Path == null || Path.Count == 0)
            Path = new List<string>() { name };
    }

    public static void NewRootScene(string name)
    {
        Path.Clear();
        LoadNextScene(name);
    }

    public static void LoadBackUntil(string name)
    {
        for (int i = Path.Count - 1; Path.Count > 0 && Path[i] != name; --i)
            Path.RemoveAt(i);
        SceneManager.LoadScene(name);
    }

    public static void LoadNextScene(string name, NavigationParameter parameter = null)
    {
        SceneParameter = parameter;
        if (name == SceneManager.GetActiveScene().name)
            return;
        Cache.NameLastScene = SceneManager.GetActiveScene().name;
        Path.Add(name);
        SceneManager.LoadScene(name);
    }

    public static void OverBlendPreviousScene(string onRootPreviousScene = null)
    {
        var instantiator = GameObject.Find(Constants.GoSceneBhvName).GetComponent<Instantiator>();
        if (instantiator != null)
            instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, string.Empty, 10.0f, OnToPreviousScene, true);
        else
            LoadPreviousScene();

        bool OnToPreviousScene(bool result)
        {
            LoadPreviousScene();
            return result;
        }
    }

    public static bool IsRootScene()
    {
        if (Path == null || Path.Count <= 1)
            return true;
        Cache.NameLastScene = SceneManager.GetActiveScene().name;
        return false;
    }

    public static void LoadPreviousScene()
    {
        Cache.NameLastScene = SceneManager.GetActiveScene().name;
        if (Path == null || Path.Count == 0)
        {
            Debug.Log("    [DEBUG]    Root");
            return;
        }
        Path.RemoveAt(Path.Count - 1);   
        SceneManager.LoadScene(Path[Path.Count - 1]);
    }

    public static void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

public class NavigationParameter
{
    public bool BoolParam0; 
    public int IntParam0;
    public string StringParam0;
}
