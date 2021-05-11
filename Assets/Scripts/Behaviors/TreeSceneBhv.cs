using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSceneBhv : SceneBhv
{
    public override MusicType MusicType => MusicType.Menu;

    private GameObject _treeContainer;

    private List<int> _resources;
    private List<TreeNode> _treeNodes;

    private int _nodeUnitPrice = 5;

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        _treeContainer = GameObject.Find("CharacterButtons");
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
        var resources = PlayerPrefsHelper.GetTotalResources();
        for (int i = 0; i < resources.Count; ++i)
        {
            if (resources[i] <= 0)
                GameObject.Find($"Resource{i}").SetActive(false);
            else
                GameObject.Find($"Resource{i}").transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = resources[i].ToString();
        }
    }

    private void UpdateTreeNodes()
    {
        var boughtTreeNods = PlayerPrefsHelper.GetBoughtTreeNodes();
        foreach (var node in _treeNodes)
        {
            var nodeRenderer = node.Instance.GetComponent<SpriteRenderer>();
            var altBranchesStr = "012".Remove(node.Branch);
            var altBranches = new List<int>() { int.Parse(altBranchesStr.Substring(0, 1)), int.Parse(altBranchesStr.Substring(1, 1)) };

            if (boughtTreeNods.Contains(node.Name)) //Bought
            {
                node.Bought = true;
                var iconid = 3 + (node.Realm.GetHashCode() * 3) + node.Branch + 1;
                nodeRenderer.sprite = Helper.GetSpriteFromSpriteSheet($"Sprites/TreeIcons_{iconid}");
            }
            else if (boughtTreeNods.Contains($"R{node.Realm};B{altBranches[0]};") && boughtTreeNods.Contains($"R{node.Realm};B{altBranches[1]};")) //Locked
            {
                node.Locked = true;
                var iconid = node.Realm.GetHashCode();
                nodeRenderer.sprite = Helper.GetSpriteFromSpriteSheet($"Sprites/TreeIcons_{iconid}");
            }
            else if (_resources[node.Realm.GetHashCode()] < node.Price)
                nodeRenderer.color = Constants.ColorBlack;
            

        }
    }

    private void SelectNode()
    {
        var lastClickedButton = GameObject.Find(Constants.LastEndActionClickedName);
        
    }

    private void ResetTreeNodes()
    {
        
    }

    private void GoToPrevious()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend, reverse: true);
        object OnBlend(bool result)
        {
            NavigationService.LoadPreviousScene();
            return true;
        }
    }

    private void Play()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);
        object OnBlend(bool result)
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
        public int Id;
        public int Price;
        public GameObject Instance;

        public bool Bought;
        public bool Locked;

        public TreeNode(string nodeString, int unitPrice, GameObject instance)
        {
            Name = nodeString;
            var splits = nodeString.Split(';');
            Realm = (Realm)int.Parse(splits[0].Substring(1));
            Branch = int.Parse(splits[1].Substring(1));
            Id = int.Parse(splits[2].Substring(1));
            Price = Id * unitPrice;
            Instance = instance;
        }
    }
}
