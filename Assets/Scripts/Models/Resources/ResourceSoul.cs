using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSoul : Resource
{
    public ResourceSoul()
    {
        Id = 2;
        Name = ResourcesData.Resources[Id];
        Description = $"materialization of humans\ndeepest aspirations.";
        Rarity = Rarity.Common;
    }
}
