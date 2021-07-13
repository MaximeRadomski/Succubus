using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionService : MonoBehaviour
{
    private static readonly string TableProgressions = "Progressions";

    public static void PutProgression(ProgressionDto progression, string playerName, Action onResolved)
    {
        RestClient.Put<ProgressionDto>(DatabaseService.SetTableAndId(TableProgressions, playerName), progression).Then(r =>
        {
            onResolved.Invoke();
        });
    }

    public static void GetProgression(string playerNameId, Action<ProgressionDto> resultAction)
    {
        RestClient.Get(DatabaseService.SetTableAndId(TableProgressions, playerNameId)).Then(returnValue =>
        {
            ProgressionDto progression = null;
            if (!string.IsNullOrEmpty(returnValue.Text) && returnValue.Text != "null")
                progression = JsonUtility.FromJson<ProgressionDto>(returnValue.Text);
            resultAction.Invoke(progression);
        });
    }
}
