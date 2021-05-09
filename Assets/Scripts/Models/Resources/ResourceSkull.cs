using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSkull : Resource
{
    public ResourceSkull()
    {
        Id = 1;
        Name = ResourcesData.Resources[Id];
        Description = $"shell of humans conscience.";
        Rarity = Rarity.Common;
    }
}
