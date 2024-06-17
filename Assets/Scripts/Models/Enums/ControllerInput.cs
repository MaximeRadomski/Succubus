using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControllerInputType
{
    button,
    axis
}

public class ControllerInput
{
    public ControllerInput(int id, string name, string xboxName, string playstationName, ControllerInputType type)
    {
        Id = id;
        Code = name;
        XboxName = xboxName;
        PlaystationName = playstationName;
        Type = type;
    }

    public int Id;
    public string Code;
    public string XboxName;
    public string PlaystationName;
    public ControllerInputType Type;
    public float DefaultValue;

    public string DisplayName(ControllerType controllerType) {
        if (controllerType == ControllerType.Xbox)
            return XboxName;
        else if (controllerType == ControllerType.Playstation)
            return PlaystationName;
        else
            return Code;
    }

    public static ControllerInput None = new ControllerInput(-1, "None", "None", "None", ControllerInputType.button);

    public static ControllerInput A_0 = new ControllerInput(0, "Button0", "A", "Square", ControllerInputType.button);
    public static ControllerInput B_1 = new ControllerInput(1, "Button1", "B", "Cross", ControllerInputType.button);
    public static ControllerInput X_2 = new ControllerInput(2, "Button2", "X", "Circle", ControllerInputType.button);
    public static ControllerInput Y_3 = new ControllerInput(3, "Button3", "Y", "Triangle", ControllerInputType.button);
    public static ControllerInput LB_4 = new ControllerInput(4, "Button4", "LB", "L1", ControllerInputType.button);
    public static ControllerInput RB_5 = new ControllerInput(5, "Button5", "RB", "R1", ControllerInputType.button);
    public static ControllerInput Back_6 = new ControllerInput(6, "Button6", "Back", "L2", ControllerInputType.button);
    public static ControllerInput Start_7 = new ControllerInput(7, "Button7", "Start", "R2", ControllerInputType.button);
    public static ControllerInput LS_8 = new ControllerInput(8, "Button8", "LS", "Share", ControllerInputType.button);
    public static ControllerInput RS_9 = new ControllerInput(9, "Button9", "RS", "Option", ControllerInputType.button);
    public static ControllerInput Button10 = new ControllerInput(10, "Button10", "Button10", "L", ControllerInputType.button);
    public static ControllerInput Button11 = new ControllerInput(11, "Button11", "Button11", "R", ControllerInputType.button);
    public static ControllerInput Button12 = new ControllerInput(12, "Button12", "Button12", "Playstation", ControllerInputType.button);
    public static ControllerInput Button13 = new ControllerInput(13, "Button13", "Button13", "Touchpad",ControllerInputType.button);
    public static ControllerInput Button14 = new ControllerInput(14, "Button14", "Button14", "Button14", ControllerInputType.button);
    public static ControllerInput Button15 = new ControllerInput(15, "Button15", "Button15", "Button15", ControllerInputType.button);
    public static ControllerInput Button16 = new ControllerInput(16, "Button16", "Button16", "Button16", ControllerInputType.button);

    public static ControllerInput Axis3p = new ControllerInput(30, "Axis3+", "Axis3+", "R Right", ControllerInputType.axis);
    public static ControllerInput Axis3m = new ControllerInput(31, "Axis3-", "Axis3-", "R Left", ControllerInputType.axis);
    public static ControllerInput RSRight_4p = new ControllerInput(40, "Axis4+", "RS Right", "L2+", ControllerInputType.axis);
    public static ControllerInput RSLeft_4m = new ControllerInput(41, "Axis4-", "RS Left", "L2-", ControllerInputType.axis);
    public static ControllerInput RSDown_5p = new ControllerInput(50, "Axis5+", "RS Down", "R2+", ControllerInputType.axis);
    public static ControllerInput RSUp_5m = new ControllerInput(51, "Axis5-", "RS Up", "R2-", ControllerInputType.axis);
    public static ControllerInput Right_6p = new ControllerInput(60, "Axis6+", "Right", "R Down", ControllerInputType.axis);
    public static ControllerInput Left_6m = new ControllerInput(61, "Axis6-", "Left", "R Up", ControllerInputType.axis);
    public static ControllerInput Up_7p = new ControllerInput(70, "Axis7+", "Up", "Right", ControllerInputType.axis);
    public static ControllerInput Down_7m = new ControllerInput(71, "Axis7-", "Down", "Left", ControllerInputType.axis);
    public static ControllerInput Axis8p = new ControllerInput(80, "Axis8+", "Axis8+", "Up", ControllerInputType.axis);
    public static ControllerInput Axis8m = new ControllerInput(81, "Axis8-", "Axis8-", "Down", ControllerInputType.axis);
    public static ControllerInput LT_9p = new ControllerInput(90, "Axis9+", "LT", "Axis9+", ControllerInputType.axis);
    public static ControllerInput Axis9m = new ControllerInput(91, "Axis9-", "Axis9-", "Axis9-", ControllerInputType.axis);
    public static ControllerInput RT_10p = new ControllerInput(100, "Axis10+", "RT", "Axis10+", ControllerInputType.axis);
    public static ControllerInput Axis10m = new ControllerInput(101, "Axis10-", "Axis10-", "Axis10-", ControllerInputType.axis);
    public static ControllerInput Axis11p = new ControllerInput(110, "Axis11+", "Axis11+", "Axis11+", ControllerInputType.axis);
    public static ControllerInput Axis11m = new ControllerInput(111, "Axis11-", "Axis11-", "Axis11-", ControllerInputType.axis);
    public static ControllerInput Axis12p = new ControllerInput(120, "Axis12+", "Axis12+", "Axis12+", ControllerInputType.axis);
    public static ControllerInput Axis12m = new ControllerInput(121, "Axis12-", "Axis12-", "Axis12-", ControllerInputType.axis);
    public static ControllerInput LSRight_Xp = new ControllerInput(1000, "AxisX+", "LS Right", "L Right", ControllerInputType.axis);
    public static ControllerInput LSLeft_Xm = new ControllerInput(1001, "AxisX-", "LS Left", "L Left", ControllerInputType.axis);
    public static ControllerInput LSDowm_Yp = new ControllerInput(1010, "AxisY+", "LS Down", "L Down", ControllerInputType.axis);
    public static ControllerInput LSUp_Ym = new ControllerInput(1011, "AxisY-", "LS Up", "L Up", ControllerInputType.axis);

    public static List<ControllerInput> JoystickInputs = new List<ControllerInput>() {
        None,
        A_0,
        B_1,
        X_2,
        Y_3,
        LB_4,
        RB_5,
        Back_6,
        Start_7,
        LS_8,
        RS_9,
        Button10,
        Button11,
        Button12,
        Button13,
        Button14,
        Button15,
        Button16,

        Axis3p,
        Axis3m,
        RSRight_4p,
        RSLeft_4m,
        RSDown_5p,
        RSUp_5m,
        Right_6p,
        Left_6m,
        Up_7p,
        Down_7m,
        Axis8p,
        Axis8m,
        LT_9p,
        Axis9m,
        RT_10p,
        Axis10m,
        Axis11p,
        Axis11m,
        Axis12p,
        Axis12m,
        LSRight_Xp,
        LSLeft_Xm,
        LSDowm_Yp,
        LSUp_Ym,
    };

    public static List<ControllerInput> JoystickButtons = new List<ControllerInput>() {
        A_0,
        B_1,
        X_2,
        Y_3,
        LB_4,
        RB_5,
        Back_6,
        Start_7,
        LS_8,
        RS_9,
        Button10,
        Button11,
        Button12,
        Button13,
        Button14,
        Button15,
        Button16,
    };

    public static List<ControllerInput> JoystickAxes = new List<ControllerInput>() {
        Axis3p,
        Axis3m,
        RSRight_4p,
        RSLeft_4m,
        RSDown_5p,
        RSUp_5m,
        Right_6p,
        Left_6m,
        Up_7p,
        Down_7m,
        Axis8p,
        Axis8m,
        LT_9p,
        Axis9m,
        RT_10p,
        Axis10m,
        Axis11p,
        Axis11m,
        Axis12p,
        Axis12m,
        LSRight_Xp,
        LSLeft_Xm,
        LSDowm_Yp,
        LSUp_Ym,
    };
}