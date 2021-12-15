using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LogService
{
    private static string _currentSessionLogs;
    private static string _path = $"{Application.persistentDataPath}/Logs.txt";

    public static void Init()
    {
        Application.logMessageReceived += LogCallback;
    }

    private static void LogCallback(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Assert
            || type == LogType.Error
            || type == LogType.Exception
            || type == LogType.Warning)
        {
            var path = $"{Application.persistentDataPath}/Logs.txt";
            if (string.IsNullOrEmpty(_currentSessionLogs))
            {
                _currentSessionLogs = $"----------------------------\nSession: {Helper.DateFormat(DateTime.Now)}\n----------------------------\n";
                File.AppendAllText(path, _currentSessionLogs);
            }
            var tmpLog = $"[{type.ToString().ToUpper()} - {SceneManager.GetActiveScene().name}] {condition}\n\t{stackTrace.Replace("\n", "\n\t")}";
            if (tmpLog[tmpLog.Length - 1] == '\t')
                tmpLog = tmpLog.Substring(0, tmpLog.Length - 1);
            _currentSessionLogs = tmpLog;
            File.AppendAllText(path, _currentSessionLogs);
        }
    }

    public static void TrySendLogs()
    {
        if (!File.Exists(_path))
            return;
        var content = File.ReadAllText(_path);
        var lastCredentials = PlayerPrefsHelper.GetLastSavedCredentials();
        var title = $"Logs_{Helper.DateFormat(DateTime.Now).Replace(" ", "_")}";
        if (lastCredentials != null && !string.IsNullOrEmpty(lastCredentials.PlayerName))
        {
            content = content.Insert(0, $"Player: {lastCredentials.PlayerName}\n");
            title += $"_{lastCredentials.PlayerName}";
        }
        DatabaseService.SendLog(title, new Dto { Checksum = content }, () =>
        {
            _currentSessionLogs = null;
            File.Delete(_path);
        });
    }
}
