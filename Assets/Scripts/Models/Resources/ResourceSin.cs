using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSin : Resource
{
    public ResourceSin()
    {
        Id = 0;
        Name = ResourcesData.Resources[Id];
        Description = $"materialization of humans\ndarkest wills.";
        Rarity = Rarity.Common;
    }
}
