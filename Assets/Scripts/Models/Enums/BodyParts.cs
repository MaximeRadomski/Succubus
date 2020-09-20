﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum BodyParts
{
    [Description("Left Arm")]
    LeftArm = 0,
    [Description("Right Arm")]
    RightArm = 1,
    [Description("Left Shoulder")]
    LeftShoulder = 2,
    [Description("Right Shoulder")]
    RightShoulder = 3,
    [Description("Left Thigh")]
    LeftThigh = 4,
    [Description("Right Thigh")]
    RightThigh = 5,
    [Description("Left Calf")]
    LeftCalf = 6,
    [Description("Right Calf")]
    RightCalf = 7,
    [Description("Chest")]
    Chest = 8,
    [Description("Abdomen")]
    Abdomen = 9,
    [Description("Back")]
    Back = 10,
    [Description("Neck")]
    Neck = 11
}
