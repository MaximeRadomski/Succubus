using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSceneBhv : SceneBhv
{
    public override MusicType MusicType => MusicType.Menu;

    private GameObject _treeContainer;

    private List<int> _resources;
    private List<TreeNode> _treeNodes;

    private int _nodeUnitPrice = 3;

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        _treeContainer = GameObject.Find("Tree");
        SetButtons();
        UpdateResources();
        UpdateTreeNodes();
    }

    private void SetButtons()
    {
        _treeNodes = new List<TreeNode>();
        foreach (Transform child in _treeContainer.transform)
        {
            child.GetComponent<ButtonBhv>().EndActionDelegate = SelectNode;
            _treeNodes.Add(new TreeNode(child.name, _nodeUnitPrice, child.gameObject));
        }
        GameObject.Find(Constants.GoButtonBackName).GetComponent<ButtonBhv>().EndActionDelegate = GoToPrevious;
        GameObject.Find(Constants.GoButtonPlayName).GetComponent<ButtonBhv>().EndActionDelegate = Play;
        GameObject.Find("ButtonReset").GetComponent<ButtonBhv>().EndActionDelegate = ResetTreeNodes;
    }

    private void UpdateResources()
    {
        _resources = PlayerPrefsHelper.GetTotalResources();
        for (int i = 0; i < _resources.Count; ++i)
            GameObject.Find($"Resource{i}").transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = _resources[i].ToString();
    }

    private void UpdateTreeNodes()
    {
        var boughtTreeNods = PlayerPrefsHelper.GetBoughtTreeNodes();
        foreach (var node in _treeNodes)
        {
            var nodeRenderer = node.Instance.GetComponent<SpriteRenderer>();
            var altBranchesStr = "012".Remove(node.Branch, 1);
            var altBranches = new List<int>() { int.Parse(altBranchesStr.Substring(0, 1)), int.Parse(altBranchesStr.Substring(1, 1)) };

            if (boughtTreeNods.Contains(node.Name)) //Bought
            {
                node.Bought = true;
                node.Locked = false;
                var iconid = 3 + ((int)node.Realm * 6) + (node.Branch * 2) + 1;
                nodeRenderer.color = Constants.ColorPlain;
                nodeRenderer.sprite = Helper.GetSpriteFromSpriteSheet($"Sprites/TreeIcons_{iconid}");
                node.Instance.GetComponent<ButtonBhv>().SetResetedColor(nodeRenderer.color);
            }
            else if (boughtTreeNods.Contains($"R{(int)node.Realm};B{altBranches[0]};") && boughtTreeNods.Contains($"R{(int)node.Realm};B{altBranches[1]};")) //Locked
            {
                node.Bought = false;
                node.Locked = true;
                var iconid = (int)node.Realm;
                nodeRenderer.color = Constants.ColorPlain;
                nodeRenderer.sprite = Helper.GetSpriteFromSpriteSheet($"Sprites/TreeIcons_{iconid}");
                node.Instance.GetComponent<ButtonBhv>().SetResetedColor(nodeRenderer.color);
            }
            else if (_resources[(int)node.Realm] < node.Price) //Overpriced
            {
                node.Bought = false;
                node.Locked = false;
                var iconid = 3 + ((int)node.Realm * 6) + (node.Branch * 2);
                nodeRenderer.color = Constants.ColorBlackSemiTransparent;
                nodeRenderer.sprite = Helper.GetSpriteFromSpriteSheet($"Sprites/TreeIcons_{iconid}");
                node.Instance.GetComponent<ButtonBhv>().SetResetedColor(nodeRenderer.color);
            }
            else
            {
                node.Bought = false;
                node.Locked = false;
                var iconid = 3 + ((int)node.Realm * 6) + (node.Branch * 2);
                nodeRenderer.color = Constants.ColorPlain;
                nodeRenderer.sprite = Helper.GetSpriteFromSpriteSheet($"Sprites/TreeIcons_{iconid}");
                node.Instance.GetComponent<ButtonBhv>().SetResetedColor(nodeRenderer.color);
            }
            

        }
    }

    private void SelectNode()
    {
        var node = _treeNodes.Find(n => n.Name == Cache.LastEndActionClickedName);
        var firstSpace = node.Description.IndexOf(' ');
        var description = $"{node.Description.Substring(0, firstSpace)}{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)} {node.Description.Substring(firstSpace + 1)}";
        var price = $"\n---\nprice: {Constants.GetMaterial(node.Realm, TextType.succubus3x5, TextCode.c43)}{node.Price} {ResourcesData.Resources[(int)node.Realm].ToLower()}";
        var positive = "Pay";
        var negative = "Cancel";
        var canBuy = true;
        if (node.Locked || node.Bought || _resources[(int)node.Realm] < node.Price)
        {
            canBuy = false;
            positive = "Cancel";
            negative = null;

            if (_resources[(int)node.Realm] < node.Price && !node.Bought && !node.Locked)
                description += price;
            else if (node.Locked)
                description += $"\n---\n{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}locked: {Constants.MaterialEnd}only two branches available per realm.";
        }
        else
            description += price;
        Instantiator.NewPopupYesNo(node.Title, description, negative, positive, OnSelectedNode);

        void OnSelectedNode(bool result)
        {
            if (!result || !canBuy)
                return;
            PlayerPrefsHelper.AlterTotalResource((int)node.Realm, -node.Price);
            PlayerPrefsHelper.AddBoughtTreeNode(node.Name, node.Type);
            UpdateResources();
            UpdateTreeNodes();
        }
    }



    private void ResetTreeNodes()
    {
        var maxRealmResourceAsked = PlayerPrefsHelper.GetRealmBossProgression() + 1; //Starts at -1, increments on boss vainquished
        var hellResourceAsked = maxRealmResourceAsked >= (int)Realm.Hell ? 6 : 0;
        var earthResourceAsked = maxRealmResourceAsked >= (int)Realm.Earth ? 6 : 0;
        var heavenResourceAsked = maxRealmResourceAsked >= (int)Realm.Heaven ? 6 : 0;
        var price = $"{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}\n---\nprice: " +
        $"{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}{hellResourceAsked} {ResourcesData.Resources[(int)Realm.Hell].ToLower()}{Constants.MaterialEnd}, " +
        $"{Constants.GetMaterial(Realm.Earth, TextType.succubus3x5, TextCode.c43)}{earthResourceAsked} {ResourcesData.Resources[(int)Realm.Earth].ToLower()}{Constants.MaterialEnd}, " +
        $"{Constants.GetMaterial(Realm.Heaven, TextType.succubus3x5, TextCode.c43)}{heavenResourceAsked} {ResourcesData.Resources[(int)Realm.Heaven].ToLower()}{Constants.MaterialEnd}";
        var positive = "Pay";
        var negative = "Cancel";
        var canBuy = true;
        if (_resources[0] < hellResourceAsked || _resources[1] < earthResourceAsked || _resources[2] < heavenResourceAsked)
        {
            canBuy = false;
            positive = "Cancel";
            negative = null;
        }
        Instantiator.NewPopupYesNo("Reset Realm Tree", $"would you like to reset the realm tree?{price}", negative, positive, OnResetedNode);

        void OnResetedNode(bool result)
        {
            if (!result || !canBuy)
                return;
            foreach (var node in _treeNodes)
            {
                if (node.Bought)
                    PlayerPrefsHelper.AlterTotalResource((int)node.Realm, node.Price);
            }
            PlayerPrefsHelper.AlterTotalResource((int)Realm.Hell, -hellResourceAsked);
            PlayerPrefsHelper.AlterTotalResource((int)Realm.Earth, -earthResourceAsked);
            PlayerPrefsHelper.AlterTotalResource((int)Realm.Heaven, -heavenResourceAsked);
            PlayerPrefsHelper.ResetBoughtTreeNodes();
            UpdateResources();
            UpdateTreeNodes();
        }
    }

    private void GoToPrevious()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend, reverse: true);
        bool OnBlend(bool result)
        {
            NavigationService.LoadPreviousScene();
            return true;
        }
    }

    private void Play()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);
        bool OnBlend(bool result)
        {
            NavigationService.LoadNextScene(Constants.CharSelScene);
            return true;
        }
    }

    private class TreeNode
    {
        public string Name;
        public Realm Realm;
        public int Branch;
        public int Level;
        public NodeType Type;
        public GameObject Instance;

        public string Title;
        public string Description;
        public int Price;

        public bool Bought;
        public bool Locked;

        public TreeNode(string nodeString, int unitPrice, GameObject instance)
        {
            Name = nodeString;
            var splits = nodeString.Split(';');
            Realm = (Realm)int.Parse(splits[0].Substring(1));
            Branch = int.Parse(splits[1].Substring(1));
            Level = int.Parse(splits[2].Substring(1));
            Type = (NodeType)(((int)Realm * 3) + Branch);
            Instance = instance;

            Title = Type.GetAttribute<TitleAttribute>();
            Description = Type.GetDescription();
            Price = Level * unitPrice;
        }
    }
}
