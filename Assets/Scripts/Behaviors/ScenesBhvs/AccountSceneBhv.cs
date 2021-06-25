using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountSceneBhv : SceneBhv
{
    public override MusicType MusicType => MusicType.Account;

    private Identifier _playerNameId;
    private Identifier _password1;
    private Identifier _password2;
    private Identifier _securityAnswer;

    private TMPro.TextMeshPro _securityQuestion;
    private int _idSecurityQuestion;
    private InputControlerBhv _inputControler;

    private GameObject _currentPanel;
    private GameObject _panelConnect;
    private GameObject _panelCreateAccount;
    private GameObject _panelRecovery;

    private int _minPlayerNameCharacters = 3;
    private int _minPasswordCharacters = 10;

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        _inputControler = GameObject.Find(Constants.GoInputControler).GetComponent<InputControlerBhv>();
        InitPanelConnect();
        InitPanelCreateAccount();
        InitPanelRecovery();
        _currentPanel = _panelConnect;
    }

    private void InitPanelConnect()
    {
        _panelConnect = GameObject.Find("PanelConnect");
        _playerNameId = new Identifier("");
        _password1 = new Identifier("");
        var playerNameIdText = GameObject.Find("PlayerNameIdTextConnect");
        playerNameIdText.GetComponent<ButtonBhv>().EndActionDelegate = () => Instantiator.EditViaKeyboard(playerNameIdText, _playerNameId);
        var passwordText = GameObject.Find("PasswordTextConnect");
        passwordText.GetComponent<ButtonBhv>().EndActionDelegate = () => Instantiator.EditViaKeyboard(passwordText, _password1, isPassword: true);
        GameObject.Find("Login").GetComponent<ButtonBhv>().EndActionDelegate = Login;
        GameObject.Find("OrCreateAccount").GetComponent<ButtonBhv>().EndActionDelegate = () => ShowPanel(1);
    }

    private void InitPanelCreateAccount()
    {
        _panelCreateAccount = GameObject.Find("PanelCreateAccount");
        _playerNameId = new Identifier("");
        _password1 = new Identifier("");
        _password2 = new Identifier("");
        _securityAnswer = new Identifier("");
        var playerNameIdText = GameObject.Find("PlayerNameIdTextCreate");
        playerNameIdText.GetComponent<ButtonBhv>().EndActionDelegate = () => Instantiator.EditViaKeyboard(playerNameIdText, _playerNameId);
        var passwordText1 = GameObject.Find("PasswordText1Create");
        passwordText1.GetComponent<ButtonBhv>().EndActionDelegate = () => Instantiator.EditViaKeyboard(passwordText1, _password1, isPassword: true);
        var passwordText2 = GameObject.Find("PasswordText2Create");
        passwordText2.GetComponent<ButtonBhv>().EndActionDelegate = () => Instantiator.EditViaKeyboard(passwordText2, _password2, isPassword: true);
        _securityQuestion = GameObject.Find("SecurityQuestionCreate").GetComponent<TMPro.TextMeshPro>();
        var previousQuestion = GameObject.Find("PreviousQuestion");
        var nextQuestion = GameObject.Find("NextQuestion");
        var SecurityAnswerCreate = GameObject.Find("SecurityAnswerCreate");
        SecurityAnswerCreate.GetComponent<ButtonBhv>().EndActionDelegate = () => Instantiator.EditViaKeyboard(SecurityAnswerCreate, _securityAnswer);

        GameObject.Find("Create").GetComponent<ButtonBhv>().EndActionDelegate = CreateAccount;
        GameObject.Find("BackToConnectCreate").GetComponent<ButtonBhv>().EndActionDelegate = () => ShowPanel(0);
    }

    private void InitPanelRecovery()
    {
        _panelRecovery = GameObject.Find("PanelRecovery");
    }

    private void ShowPanel(int param)
    {
        var newPanel = _panelConnect;
        if (param == 1)
            newPanel = _panelCreateAccount;
        else if (param == 2)
            newPanel = _panelRecovery;
        newPanel.transform.position = _currentPanel.transform.position;
        _currentPanel.transform.position = new Vector3(50.0f + (param * 20.0f), 50.0f, 0.0f);
        _currentPanel = newPanel;
        _inputControler.ResetMenuSelector();
    }

    private void Login()
    {
        if (string.IsNullOrEmpty(_playerNameId.Text)) { Instantiator.NewPopupYesNo("Player Name", "you must enter a player name.", null, "Ok", null); return; }
        if (string.IsNullOrWhiteSpace(_playerNameId.Text) || _playerNameId.Text.Length < _minPlayerNameCharacters) { Instantiator.NewPopupYesNo("Player Name", $"your player name must contains at least {_minPlayerNameCharacters} characters.", null, "Ok", null); return; }
        if (string.IsNullOrEmpty(_password1.Text)) { Instantiator.NewPopupYesNo("Password", "you must enter a password.", null, "Ok", null); return; }
        if (string.IsNullOrWhiteSpace(_password1.Text) || _password1.Text.Length < _minPasswordCharacters) { Instantiator.NewPopupYesNo("Password", $"your password name must contains at least {_minPasswordCharacters} characters.", null, "Ok", null); return; }

        Instantiator.NewLoading();
        AccountService.GetAccount(_playerNameId.Text, (account) =>
        {
            Helper.ResumeLoading();
            if (account == null)
            {
                Instantiator.NewPopupYesNo("Error", $"no account found with this player name.", null, "Ok", null);
                return;
            }
            else if (EncryptedPlayerPrefs.Md5WithKey(_password1.Text) != account.Key_Password)
            {
                Instantiator.NewPopupYesNo("Error", $"incorrect password.", "Forgot?", "Ok", OnIncorrectPassword);
                return;
            }
            else
                Instantiator.NewPopupYesNo("Connected", $"welcome back {account.Id_PlayerName.ToLower()}.", null, "Ok", null);
        });
    }

    private object OnIncorrectPassword(bool result)
    {
        if (result)
            return true;
        ShowPanel(2);
        return false;
    }

    private void CreateAccount()
    {

    }

    public class Identifier
    {
        public string Text;

        public Identifier(string text)
        {
            Text = text;
        }
    }
}
