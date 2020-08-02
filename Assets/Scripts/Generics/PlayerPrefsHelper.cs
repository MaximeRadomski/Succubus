using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class PlayerPrefsHelper : MonoBehaviour
{

    public static void SaveRun(Run run)
    {
        PlayerPrefs.SetString(Constants.PpRun, JsonUtility.ToJson(run));
    }

    public static Run GetRun()
    {
        var run = JsonUtility.FromJson<Run>(PlayerPrefs.GetString(Constants.PpRun, Constants.PpSerializeDefault));
        if (run == null)
            return null;
        return run;
    }
}
