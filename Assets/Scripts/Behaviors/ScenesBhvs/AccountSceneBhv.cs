using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountSceneBhv : SceneBhv
{
    public override MusicType MusicType => MusicType.Account;

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        InitPanelConnect();
        InitPanelCreateAccount();
        InitPanelRecovery();
    }

    private void InitPanelConnect()
    {

    }

    private void InitPanelCreateAccount()
    {

    }

    private void InitPanelRecovery()
    {

    }
}
