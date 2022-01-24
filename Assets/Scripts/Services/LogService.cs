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

            var context = "";
            var currentScene = GameObject.Find(Constants.GoSceneBhvName).GetComponent<SceneBhv>();
            if (currentScene is ClassicGameSceneBhv gameScene)
            {
                var gameplayControler = GameObject.Find(Constants.GoSceneBhvName).GetComponent<GameplayControler>();
                var run = PlayerPrefsHelper.GetRun();
                context = $"\n    [CONTEXT] Opponent: {gameScene.CurrentOpponent?.Name}, Character: {gameScene.Character?.Name}, Item: {gameplayControler?.CharacterItem?.Name}, CacheOpponentAttackId: {Cache.CurrentOpponentAttackId}, Difficulty: {run.Difficulty}";
            }
            else if (currentScene is LoreSceneBhv loreScene)
            {
                context = $"\n    [CONTEXT] NavigationService.SceneParameter: {JsonUtility.ToJson(NavigationService.SceneParameter)}";
            }

            var tmpLog = $"[{type.ToString().ToUpper()} - {SceneManager.GetActiveScene().name}] {condition}{context}\n    {stackTrace.Replace("\n", "\n    ")}";
            if (tmpLog.Substring(tmpLog.Length - 4) == "    ")
                tmpLog = tmpLog.Substring(0, tmpLog.Length - 4);
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
