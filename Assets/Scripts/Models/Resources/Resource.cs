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
        return $"{Constants.MaterialHell_4_3}{highlight}{Constants.MaterialEnd}";
    }
}
