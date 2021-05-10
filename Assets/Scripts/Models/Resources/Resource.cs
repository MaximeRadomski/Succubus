using System;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.PlayerLoop;

public abstract class Resource : Loot
{
    public int Id;
    public string Name;
    public string Description;

    public Resource()
    {
        LootType = LootType.Resource;
    }

    protected string Highlight(string highlight)
    {
        return $"{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}{highlight}{Constants.MaterialEnd}";
    }
}
