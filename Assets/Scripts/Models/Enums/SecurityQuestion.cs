using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using UnityEngine;

public enum SecurityQuestion
{
    [Description("What city were you born in?")]
    City = 0,
    [Description("What is your oldest sibling’s middle name?")]
    Sibbling = 1,
    [Description("What was the first concert you attended?")]
    Concert = 2,
    [Description("What was the make and model of your first car?")]
    Car = 3,
    [Description("What school did you attend for sixth grade?")]
    School = 4,
    [Description("What is your oldest cousin’s first and last name?")]
    Cousin = 5,
    [Description("What is your favorite team?")]
    Team = 6,
    [Description("What is your favorite movie?")]
    Movie = 7
}