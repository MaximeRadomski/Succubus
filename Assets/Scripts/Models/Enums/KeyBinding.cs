using System.ComponentModel;

public enum KeyBinding
{
    [Description("Hard Drop")]
    HardDrop = 0,
    [Description("Soft Drop")]
    SoftDrop = 1,
    [Description("Left")]
    Left = 2,
    [Description("Right")]
    Right = 3,
    [Description("Clockwise Rotation")]
    Clock = 4,
    [Description("Counter Clockwise Rotation")]
    AntiClock = 5,
    [Description("Hold")]
    Hold = 6,
    [Description("Item")]
    Item = 7,
    [Description("Special")]
    Special = 8,
    [Description("Rotation 180")]
    Rotation180 = 9,
    [Description("Sonic Drop")]
    SonicDrop = 10,
    [Description("Menu Up")]
    MenuUp = 11,
    [Description("Menu Down")]
    MenuDown = 12,
    [Description("Menu Left")]
    MenuLeft = 13,
    [Description("Menu Right")]
    MenuRight = 14,
    [Description("Menu Select")]
    MenuSelect = 15,
    [Description("Back / Pause")]
    BackPause = 16,
    [Description("Restart Training")]
    Restart = 17,
    [Description("None")]
    None = 99
}
