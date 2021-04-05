using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRogueSceneBhv : SceneBhv
{
    private GameplayControler _gameplayControler;

    public override MusicType MusicType => MusicType.None;

    void Start()
    {
        Init();
        _gameplayControler = GetComponent<GameplayControler>();
    }
}
