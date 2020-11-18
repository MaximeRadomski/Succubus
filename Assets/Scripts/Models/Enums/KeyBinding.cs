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
    [Description("Back / Pause")]
    BackPause = 9,
    [Description("None")]
    None = 99
}
