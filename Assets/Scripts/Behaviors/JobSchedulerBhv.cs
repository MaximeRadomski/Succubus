using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class JobSchedulerBhv : MonoBehaviour
{
    private float _retryDelay = 60.0f * 10.0f;

    void Start()
    {
        var schedulerGameObjects = GameObject.FindGameObjectsWithTag(Constants.TagJobScheduler);
        if (schedulerGameObjects.Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        Init();
    }

    private void Init()
    {
        DontDestroyOnLoad(transform.gameObject);
        Jobs();
    }

    private void Jobs()
    {
        LogService.TrySendLogs();
        Invoke(nameof(Jobs), _retryDelay);
    }
}