using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CustomAttribute : Attribute
{
    public string Attribute;
}

public class PrefixeAttribute : CustomAttribute { public PrefixeAttribute(string attribute) { Attribute = attribute; } }

public class SuffixeAttribute : CustomAttribute { public SuffixeAttribute(string attribute) { Attribute = attribute; } }

public class TitleAttribute : CustomAttribute { public TitleAttribute(string attribute) { Attribute = attribute; } }