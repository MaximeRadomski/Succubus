using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResourcesData
{
    public static string[] Resources = { "Sin", "Skull", "Soul" };

    //DEBUG
    public static bool DebugEnabled = Constants.ResourcesDebug;
    public static Resource DebugResource = GetResourceFromName("Sin");

    public static Resource GetResourceFromName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;
        var cleanName = name.Replace(" ", "");
        cleanName = cleanName.Replace("'", "");
        cleanName = cleanName.Replace("-", "");
        var instance = Activator.CreateInstance(Type.GetType("Resource" + cleanName));
        return (Resource)instance;
    }
}
