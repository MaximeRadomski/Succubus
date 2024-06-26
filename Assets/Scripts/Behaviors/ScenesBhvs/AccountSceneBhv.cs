using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountSceneBhv : SceneBhv
{
    public override MusicType MusicType => MusicType.Account;

    private Identifier _playerName;
    private Identifier _password1;
    private Identifier _password2;
    private Identifier _securityAnswer;
    private AccountDto _tmpUser;

    private TMPro.TextMeshPro _securityQuestion;
    private int _idSecurityQuestion;
    private InputControlerBhv _inputControler;

    private GameObject _currentPanel;
    private GameObject _panelConnect;
    private GameObject _panelCreateAccount;
    private GameObject _panelRecovery;
    private GameObject _panelNewPassword;
    private GameObject _panelConnected;

    private List<List<TMPro.TextMeshPro>> _resetsLists;

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        _inputControler = GameObject.Find(Constants.GoInputControler).GetComponent<InputControlerBhv>();
        _resetsLists = new List<List<TMPro.TextMeshPro>>();
        InitPanelConnect();
        InitPanelCreateAccount();
        InitPanelsRecovery();
        InitPanelConnected();
        _currentPanel = _panelConnect;
        _playerName = new Identifier("");
        _password1 = new Identifier("");
        _password2 = new Identifier("");
        _securityAnswer = new Identifier("");
        GameObject.Find(Constants.GoButtonBackName).GetComponent<ButtonBhv>().EndActionDelegate = () => GoToPrevious(true);

        AccountService.CheckAccount(this.Instantiator, (account) =>
        {
            if (account == null)
                return;
            GameObject.Find("PlayerNamePseudo").GetComponent<TMPro.TextMeshPro>().text = account.PlayerName;
            _tmpUser = account;
            ShowPanel(4);
        });
    }

    private void InitPanelConnect()
    {
        var resetList = new List<TMPro.TextMeshPro>();
        _panelConnect = GameObject.Find("PanelConnect");
        var playerNameIdText = GameObject.Find("PlayerNameIdTextConnect");
        playerNameIdText.GetComponent<ButtonBhv>().EndActionDelegate = () => Instantiator.EditViaKeyboard(playerNameIdText, _playerName);
        resetList.Add(playerNameIdText.GetComponent<TMPro.TextMeshPro>());
        var passwordText = GameObject.Find("PasswordTextConnect");
        passwordText.GetComponent<ButtonBhv>().EndActionDelegate = () => Instantiator.EditViaKeyboard(passwordText, _password1, isPassword: true);
        resetList.Add(passwordText.GetComponent<TMPro.TextMeshPro>());
        GameObject.Find("Login").GetComponent<ButtonBhv>().EndActionDelegate = Login;
        GameObject.Find("OrCreateAccount").GetComponent<ButtonBhv>().EndActionDelegate = () => ShowPanel(1);
        _resetsLists.Add(resetList);
    }

    private void InitPanelCreateAccount()
    {
        var resetList = new List<TMPro.TextMeshPro>();
        _panelCreateAccount = GameObject.Find("PanelCreateAccount");
        var playerNameIdText = GameObject.Find("PlayerNameIdTextCreate");
        playerNameIdText.GetComponent<ButtonBhv>().EndActionDelegate = () => Instantiator.EditViaKeyboard(playerNameIdText, _playerName);
        resetList.Add(playerNameIdText.GetComponent<TMPro.TextMeshPro>());
        var passwordText1 = GameObject.Find("PasswordText1Create");
        passwordText1.GetComponent<ButtonBhv>().EndActionDelegate = () => Instantiator.EditViaKeyboard(passwordText1, _password1, isPassword: true);
        resetList.Add(passwordText1.GetComponent<TMPro.TextMeshPro>());
        var passwordText2 = GameObject.Find("PasswordText2Create");
        passwordText2.GetComponent<ButtonBhv>().EndActionDelegate = () => Instantiator.EditViaKeyboard(passwordText2, _password2, isPassword: true);
        resetList.Add(passwordText2.GetComponent<TMPro.TextMeshPro>());
        _securityQuestion = GameObject.Find("SecurityQuestionCreate").GetComponent<TMPro.TextMeshPro>();
        _idSecurityQuestion = -1;
        AlterSecurityQuestion(1);
        GameObject.Find("PreviousQuestion").GetComponent<ButtonBhv>().EndActionDelegate = () => AlterSecurityQuestion(1);
        GameObject.Find("NextQuestion").GetComponent<ButtonBhv>().EndActionDelegate = () => AlterSecurityQuestion(-1);
        var securityAnswerCreate = GameObject.Find("SecurityAnswerCreate");
        securityAnswerCreate.GetComponent<ButtonBhv>().EndActionDelegate = () => Instantiator.EditViaKeyboard(securityAnswerCreate, _securityAnswer);
        resetList.Add(securityAnswerCreate.GetComponent<TMPro.TextMeshPro>());
        GameObject.Find("Create").GetComponent<ButtonBhv>().EndActionDelegate = CreateAccount;
        GameObject.Find("BackToConnectCreate").GetComponent<ButtonBhv>().EndActionDelegate = () => ShowPanel(0);
        _resetsLists.Add(resetList);
    }

    private void AlterSecurityQuestion(int i)
    {
        _idSecurityQuestion += i;
        if (_idSecurityQuestion < 0)
            _idSecurityQuestion = Helper.EnumCount<SecurityQuestion>() - 1;
        else if (_idSecurityQuestion >= Helper.EnumCount<SecurityQuestion>())
            _idSecurityQuestion = 0;
        _securityQuestion.text = ((SecurityQuestion)_idSecurityQuestion).GetDescription().ToLower();
    }

    private void InitPanelsRecovery()
    {
        var resetList = new List<TMPro.TextMeshPro>();
        _panelRecovery = GameObject.Find("PanelRecovery");
        var securityAnswerRecovery = GameObject.Find("SecurityAnswerRecovery");
        securityAnswerRecovery.GetComponent<ButtonBhv>().EndActionDelegate = () => Instantiator.EditViaKeyboard(securityAnswerRecovery, _securityAnswer);
        resetList.Add(securityAnswerRecovery.GetComponent<TMPro.TextMeshPro>());
        GameObject.Find("ResetRecovery").GetComponent<ButtonBhv>().EndActionDelegate = VerifySecurityQuestion;
        GameObject.Find("BackToConnectRecovery").GetComponent<ButtonBhv>().EndActionDelegate = () => ShowPanel(0);
        _resetsLists.Add(resetList);

        resetList.Clear();
        _panelNewPassword = GameObject.Find("PanelNewPassword");
        var passwordText1 = GameObject.Find("PasswordText1NewPassword");
        passwordText1.GetComponent<ButtonBhv>().EndActionDelegate = () => Instantiator.EditViaKeyboard(passwordText1, _password1, isPassword: true);
        resetList.Add(passwordText1.GetComponent<TMPro.TextMeshPro>());
        var passwordText2 = GameObject.Find("PasswordText2NewPassword");
        passwordText2.GetComponent<ButtonBhv>().EndActionDelegate = () => Instantiator.EditViaKeyboard(passwordText2, _password2, isPassword: true);
        resetList.Add(passwordText2.GetComponent<TMPro.TextMeshPro>());
        GameObject.Find("ResetNewPassword").GetComponent<ButtonBhv>().EndActionDelegate = PutNewPassword;
        GameObject.Find("BackToConnectNewPassword").GetComponent<ButtonBhv>().EndActionDelegate = () => ShowPanel(0);
        _resetsLists.Add(resetList);
    }

    private void InitPanelConnected()
    {
        _panelConnected = GameObject.Find("PanelConnected");
        _panelConnected.transform.Find("UploadProgression").GetComponent<ButtonBhv>().EndActionDelegate = UploadProgression;
        _panelConnected.transform.Find("DownloadProgression").GetComponent<ButtonBhv>().EndActionDelegate = DownloadProgression;
        GameObject.Find("Disconnect").GetComponent<ButtonBhv>().EndActionDelegate = () =>
        {
            PlayerPrefsHelper.SaveLastSavedCredentials(null);
            ShowPanel(0);
        };
    }

    private void ShowPanel(int param)
    {
        var newPanel = _panelConnect;
        if (param == 1)
            newPanel = _panelCreateAccount;
        else if (param == 2)
            newPanel = _panelRecovery;
        else if (param == 3)
            newPanel = _panelNewPassword;
        else if (param == 4)
            newPanel = _panelConnected;
        newPanel.transform.position = _currentPanel.transform.position;
        _currentPanel.transform.position = new Vector3(50.0f + (param * 20.0f), 50.0f, 0.0f);
        _currentPanel = newPanel;
        _playerName.Text = "";
        _password1.Text = "";
        _password2.Text = "";
        _securityAnswer.Text = "";
        if (param < _resetsLists.Count)
        {
            foreach (var textMesh in _resetsLists[param])
                textMesh.text = "";
        }
        _inputControler.ResetMenuSelector();
        //StartCoroutine(Helper.ExecuteAfterDelay(0.05f, () => { _inputControler.ResetMenuSelector(); return true; }));
    }

    private void Login()
    {
        if (string.IsNullOrEmpty(_playerName.Text)) { Instantiator.NewPopupYesNo("Player Name", "you must enter a player name.", null, "Ok", null); return; }
        if (string.IsNullOrWhiteSpace(_playerName.Text) || _playerName.Text.Length < AccountService.MinPlayerNameCharacters) { Instantiator.NewPopupYesNo("Player Name", $"your player name must contains at least {AccountService.MinPlayerNameCharacters} characters.", null, "Ok", null); return; }
        if (string.IsNullOrEmpty(_password1.Text)) { Instantiator.NewPopupYesNo("Password", "you must enter a password.", null, "Ok", null); return; }
        if (string.IsNullOrWhiteSpace(_password1.Text) || _password1.Text.Length < AccountService.MinPasswordCharacters) { Instantiator.NewPopupYesNo("Password", $"your password name must contains at least {AccountService.MinPasswordCharacters} characters.", null, "Ok", null); return; }

        Instantiator.NewLoading();
        AccountService.GetAccount(_playerName.Text, (account) =>
        {
            Helper.ResumeLoading();
            if (account == null)
            {
                Instantiator.NewPopupYesNo("Error", $"no account found with this player name.", null, "Ok", null);
                return;
            }
            else if (Mock.Md5WithKey(_password1.Text, account.Type) != account.Password)
            {
                Instantiator.NewPopupYesNo("Error", $"incorrect password.", "Forgot?", "Ok", (result) =>
                {
                    if (result)
                        return;
                    _tmpUser = account;
                    GameObject.Find("SecurityQuestionRecovery").GetComponent<TMPro.TextMeshPro>().text = _tmpUser.SecretQuestion.ToLower();
                    ShowPanel(2);
                });
                return;
            }
            else
            {
                PlayerPrefsHelper.SaveLastSavedCredentials(new AccountDto(_playerName.Text, _password1.Text, null, null, account.Type, account.Checksum));
                Instantiator.NewPopupYesNo("Connected", $"welcome back {account.PlayerName.ToLower()}.", null, "Ok", (result) =>
                {
                    GameObject.Find("PlayerNamePseudo").GetComponent<TMPro.TextMeshPro>().text = account.PlayerName;
                    _tmpUser = account;
                    ShowPanel(4);
                });
            }
        });
    }

    private void CreateAccount()
    {
        if (string.IsNullOrEmpty(_playerName.Text)) { Instantiator.NewPopupYesNo("Player Name", "you must enter a player name.", null, "Ok", null); return; }
        if (string.IsNullOrWhiteSpace(_playerName.Text) || _playerName.Text.Length < AccountService.MinPlayerNameCharacters) { Instantiator.NewPopupYesNo("Player Name", $"your player name must contains at least {AccountService.MinPlayerNameCharacters} characters.", null, "Ok", null); return; }
        if (string.IsNullOrWhiteSpace(_playerName.Text.Substring(0, 1)) || string.IsNullOrWhiteSpace(_playerName.Text.Substring(_playerName.Text.Length - 1))) { Instantiator.NewPopupYesNo("Player Name", $"your player name can't start or end with whitespace characters.", null, "Ok", null); return; }
        if (string.IsNullOrEmpty(_password1.Text)) { Instantiator.NewPopupYesNo("Password", "you must enter a password.", null, "Ok", null); return; }
        if (string.IsNullOrWhiteSpace(_password1.Text) || _password1.Text.Length < AccountService.MinPasswordCharacters) { Instantiator.NewPopupYesNo("Password", $"your password name must contains at least {AccountService.MinPasswordCharacters} characters.", null, "Ok", null); return; }
        if (string.IsNullOrEmpty(_password2.Text)) { Instantiator.NewPopupYesNo("Password", "you must enter a password.", null, "Ok", null); return; }
        if (string.IsNullOrWhiteSpace(_password2.Text) || _password2.Text.Length < AccountService.MinPasswordCharacters) { Instantiator.NewPopupYesNo("Password", $"your password name must contains at least {AccountService.MinPasswordCharacters} characters.", null, "Ok", null); return; }
        if (_password1.Text != _password2.Text) { Instantiator.NewPopupYesNo("Password", $"your passwords do not match.", null, "Ok", null); return; }

        Instantiator.NewLoading();
        AccountService.GetAccount(_playerName.Text, (account) =>
        {
            if (account != null && account.PlayerName == _playerName.Text)
            {
                Helper.ResumeLoading();
                Instantiator.NewPopupYesNo("Error", $"already existing account with this player name.", null, "Ok", null);
                return;
            }
            var type = Random.Range(0, Mock.test.Count);
            AccountService.PutAccount(new AccountDto(_playerName.Text, Mock.Md5WithKey(_password1.Text, type), ((SecurityQuestion)_idSecurityQuestion).GetDescription(), Mock.Md5WithKey(_securityAnswer.Text.ToLower(), type), type, Mock.Md5WithKey(_playerName.Text.ToLower(), type)), () =>
            {
                Helper.ResumeLoading();
                ShowPanel(0);
                Instantiator.NewPopupYesNo("Success", $"your account was successfully created.", null, "Ok", null);
            });
        });
    }

    private void VerifySecurityQuestion()
    {
        if (string.IsNullOrEmpty(_securityAnswer.Text)) { Instantiator.NewPopupYesNo("Security Answer", "you must enter an answer.", null, "Ok", null); return; }
        var encryptedAnswer = Mock.Md5WithKey(_securityAnswer.Text.ToLower(), _tmpUser.Type);
        if (encryptedAnswer != _tmpUser.SecretAnswer) { Instantiator.NewPopupYesNo("Security Answer", "your answer is different from the one you set up.", null, "Ok", null); return; }

        if (encryptedAnswer == _tmpUser.SecretAnswer)
            ShowPanel(3);
    }

    private void PutNewPassword()
    {
        if (string.IsNullOrEmpty(_password1.Text)) { Instantiator.NewPopupYesNo("Password", "you must enter a password.", null, "Ok", null); return; }
        if (string.IsNullOrWhiteSpace(_password1.Text) || _password1.Text.Length < AccountService.MinPasswordCharacters) { Instantiator.NewPopupYesNo("Password", $"your password name must contains at least {AccountService.MinPasswordCharacters} characters.", null, "Ok", null); return; }
        if (string.IsNullOrEmpty(_password2.Text)) { Instantiator.NewPopupYesNo("Password", "you must enter a password.", null, "Ok", null); return; }
        if (string.IsNullOrWhiteSpace(_password2.Text) || _password2.Text.Length < AccountService.MinPasswordCharacters) { Instantiator.NewPopupYesNo("Password", $"your password name must contains at least {AccountService.MinPasswordCharacters} characters.", null, "Ok", null); return; }
        if (_password1.Text != _password2.Text) { Instantiator.NewPopupYesNo("Password", $"your passwords do not match.", null, "Ok", null); return; }

        Instantiator.NewLoading();
        AccountService.PutAccount(new AccountDto(_tmpUser.PlayerName, Mock.Md5WithKey(_password1.Text, _tmpUser.Type), _tmpUser.SecretQuestion, _tmpUser.SecretAnswer, _tmpUser.Type, _tmpUser.Checksum), () =>
        {
            Helper.ResumeLoading();
            ShowPanel(0);
            Instantiator.NewPopupYesNo("Success", $"your password was successfully modified.", null, "Ok", null);
        });
    }

    private void UploadProgression()
    {
        var progression = new ProgressionDto()
        {
            UnlockedCharacters = PlayerPrefsHelper.GetUnlockedCharactersString(),
            UnlockedSkins = PlayerPrefsHelper.GetUnlockedSkinsString(),
            RealmTree = Mock.GetString(Constants.PpRealmTree, Constants.PpSerializeDefault),
            BoughtTreeNodes = PlayerPrefsHelper.GetBoughtTreeNodes(),
            BonusRarePercent = PlayerPrefsHelper.GetBonusRarePercent(),
            BonusLegendaryPercent = PlayerPrefsHelper.GetBonusLegendaryPercent(),
            RealmBossProgression = PlayerPrefsHelper.GetRealmBossProgression()
        };
        var dark = Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32);
        var light = Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43);
        var content = $"would you like to upload and save your local progress?";
        content += $"\n{dark}unlocked characters: {light}{progression.UnlockedCharacters.CountChar('1')}";
        content += $"\n{dark}tree nodes bought: {light}{PlayerPrefsHelper.GetRealmTree().NodesBought()}";
        content += $"\n{dark}rare + legendary bonus: {light}{progression.BonusRarePercent}% {dark}+{light} {progression.BonusLegendaryPercent}%";
        content += $"\n{dark}completed realm: {light}{((Realm)progression.RealmBossProgression).ToString().ToLower()}";
        this.Instantiator.NewPopupYesNo("Upload", content, "No", "Yes", (result) =>
        {
            if (!result)
                return;
            ProgressionService.PutProgression(progression, _tmpUser.PlayerName, () =>
            {
                
            });
        });
    }

    private void DownloadProgression()
    {
        ProgressionService.GetProgression(_tmpUser.PlayerName, (progression) =>
        {
            if (progression == null)
            {
                this.Instantiator.NewPopupYesNo("Error", "you currently have no progress saved online.", null, "Ok", (result) =>
                {
                    return;
                });
                return;
            }
            var dark = Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32);
            var light = Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43);
            var realmTree = JsonUtility.FromJson<RealmTree>(progression.RealmTree);
            if (realmTree == null)
                realmTree = new RealmTree();
            var content = $"would you like to download and override your local progress?";
            content += $"\n{dark}unlocked characters: {light}{progression.UnlockedCharacters.CountChar('1')}";
            content += $"\n{dark}tree nodes bought: {light}{realmTree.NodesBought()}";
            content += $"\n{dark}rare + legendary bonus: {light}{progression.BonusRarePercent}% {dark}+{light} {progression.BonusLegendaryPercent}%";
            content += $"\n{dark}completed realm: {light}{((Realm)progression.RealmBossProgression).ToString().ToLower()}";
            this.Instantiator.NewPopupYesNo("Download", content, "No", "Yes", (result) =>
            {
                if (!result)
                    return;
                ProgressionService.ApplyProgression(progression);
            });
        });
    }

    private object GoToPrevious(bool result)
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend, reverse: true);
        bool OnBlend(bool result)
        {
            NavigationService.LoadPreviousScene();
            return true;
        }
        return true;
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
