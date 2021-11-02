using System;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.PlayerLoop;

public abstract class Pact : Loot
{
    public int Id;
    public string Name;
    public string Description;
    public string ShortDescription;
    public int NbFight;

    public abstract bool ApplyPact();

    public Pact()
    {
        LootType = LootType.Pact;
    }

    protected string Highlight(string highlight)
    {
        return $"{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}{highlight}{Constants.MaterialEnd}";
    }
}
