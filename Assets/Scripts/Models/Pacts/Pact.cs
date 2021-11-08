using System;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.PlayerLoop;

public abstract class Pact : Loot
{
    public int Id;
    public string Name;
    public string Pros;
    public string Cons;
    public string ShortDescription;
    public int MaxFight;
    public int NbFight = 0;

    public abstract void ApplyPact(Character character);

    public Pact()
    {
        LootType = LootType.Pact;
    }

    public string FullDescription()
    {
        return $"{Pros}\n--- but ---\n{Cons}\n---\nduration {Highlight($"{MaxFight} fight{(MaxFight > 1 ? "s" : string.Empty)}")}.";
    }

    protected string Highlight(string highlight)
    {
        return $"{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}{highlight}{Constants.MaterialEnd}";
    }
}
