using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum JoystickInputType
{
    button,
    axis
}

public class JoystickInput
{
    public JoystickInput(int id, string name, string xboxName, string playstationName, JoystickInputType type)
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
    public JoystickInputType Type;
    public float DefaultValue;

    public string DisplayName(ControllerType controllerType) {
        if (controllerType == ControllerType.Xbox)
            return XboxName;
        else if (controllerType == ControllerType.Playstation)
            return PlaystationName;
        else
            return Code;
    }

    public static JoystickInput None = new JoystickInput(-1, "None", "None", "None", JoystickInputType.button);

    public static JoystickInput A_0 = new JoystickInput(0, "Button0", "A", "Square", JoystickInputType.button);
    public static JoystickInput B_1 = new JoystickInput(1, "Button1", "B", "Cross", JoystickInputType.button);
    public static JoystickInput X_2 = new JoystickInput(2, "Button2", "X", "Circle", JoystickInputType.button);
    public static JoystickInput Y_3 = new JoystickInput(3, "Button3", "Y", "Triangle", JoystickInputType.button);
    public static JoystickInput LB_4 = new JoystickInput(4, "Button4", "LB", "L1", JoystickInputType.button);
    public static JoystickInput RB_5 = new JoystickInput(5, "Button5", "RB", "R1", JoystickInputType.button);
    public static JoystickInput Back_6 = new JoystickInput(6, "Button6", "Back", "L2", JoystickInputType.button);
    public static JoystickInput Start_7 = new JoystickInput(7, "Button7", "Start", "R2", JoystickInputType.button);
    public static JoystickInput LS_8 = new JoystickInput(8, "Button8", "LS", "Share", JoystickInputType.button);
    public static JoystickInput RS_9 = new JoystickInput(9, "Button9", "RS", "Option", JoystickInputType.button);
    public static JoystickInput Button10 = new JoystickInput(10, "Button10", "Button10", "L", JoystickInputType.button);
    public static JoystickInput Button11 = new JoystickInput(11, "Button11", "Button11", "R", JoystickInputType.button);
    public static JoystickInput Button12 = new JoystickInput(12, "Button12", "Button12", "Playstation", JoystickInputType.button);
    public static JoystickInput Button13 = new JoystickInput(13, "Button13", "Button13", "Touchpad",JoystickInputType.button);
    public static JoystickInput Button14 = new JoystickInput(14, "Button14", "Button14", "Button14", JoystickInputType.button);
    public static JoystickInput Button15 = new JoystickInput(15, "Button15", "Button15", "Button15", JoystickInputType.button);
    public static JoystickInput Button16 = new JoystickInput(16, "Button16", "Button16", "Button16", JoystickInputType.button);

    public static JoystickInput Axis3p = new JoystickInput(30, "Axis3+", "Axis3+", "R Right", JoystickInputType.axis);
    public static JoystickInput Axis3m = new JoystickInput(31, "Axis3-", "Axis3-", "R Left", JoystickInputType.axis);
    public static JoystickInput RSRight_4p = new JoystickInput(40, "Axis4+", "RS Right", "L2+", JoystickInputType.axis);
    public static JoystickInput RSLeft_4m = new JoystickInput(41, "Axis4-", "RS Left", "L2-", JoystickInputType.axis);
    public static JoystickInput RSDown_5p = new JoystickInput(50, "Axis5+", "RS Down", "R2+", JoystickInputType.axis);
    public static JoystickInput RSUp_5m = new JoystickInput(51, "Axis5-", "RS Up", "R2-", JoystickInputType.axis);
    public static JoystickInput Right_6p = new JoystickInput(60, "Axis6+", "Right", "R Up", JoystickInputType.axis);
    public static JoystickInput Left_6m = new JoystickInput(61, "Axis6-", "Left", "R Down", JoystickInputType.axis);
    public static JoystickInput Up_7p = new JoystickInput(70, "Axis7+", "Up", "Right", JoystickInputType.axis);
    public static JoystickInput Down_7m = new JoystickInput(71, "Axis7-", "Down", "Left", JoystickInputType.axis);
    public static JoystickInput Axis8p = new JoystickInput(80, "Axis8+", "Axis8+", "Up", JoystickInputType.axis);
    public static JoystickInput Axis8m = new JoystickInput(81, "Axis8-", "Axis8-", "Down", JoystickInputType.axis);
    public static JoystickInput LT_9p = new JoystickInput(90, "Axis9+", "LT", "Axis9+", JoystickInputType.axis);
    public static JoystickInput Axis9m = new JoystickInput(91, "Axis9-", "Axis9-", "Axis9-", JoystickInputType.axis);
    public static JoystickInput RT_10p = new JoystickInput(100, "Axis10+", "RT", "Axis10+", JoystickInputType.axis);
    public static JoystickInput Axis10m = new JoystickInput(101, "Axis10-", "Axis10-", "Axis10-", JoystickInputType.axis);
    public static JoystickInput Axis11p = new JoystickInput(110, "Axis11+", "Axis11+", "Axis11+", JoystickInputType.axis);
    public static JoystickInput Axis11m = new JoystickInput(111, "Axis11-", "Axis11-", "Axis11-", JoystickInputType.axis);
    public static JoystickInput Axis12p = new JoystickInput(120, "Axis12+", "Axis12+", "Axis12+", JoystickInputType.axis);
    public static JoystickInput Axis12m = new JoystickInput(121, "Axis12-", "Axis12-", "Axis12-", JoystickInputType.axis);
    public static JoystickInput LSRight_Xp = new JoystickInput(1000, "AxisX+", "LS Right", "L Right", JoystickInputType.axis);
    public static JoystickInput LSLeft_Xm = new JoystickInput(1001, "AxisX-", "LS Left", "L Left", JoystickInputType.axis);
    public static JoystickInput LSDowm_Yp = new JoystickInput(1010, "AxisY+", "LS Down", "L Up", JoystickInputType.axis);
    public static JoystickInput LSUp_Ym = new JoystickInput(1011, "AxisY-", "LS Up", "L Down", JoystickInputType.axis);

    public static List<JoystickInput> JoystickInputs = new List<JoystickInput>() {
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

    public static List<JoystickInput> JoystickButtons = new List<JoystickInput>() {
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

    public static List<JoystickInput> JoystickAxes = new List<JoystickInput>() {
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