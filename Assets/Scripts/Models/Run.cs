using UnityEngine;

public class Run
{
    public Realm CurrentRealm;
    public int RealmLevel;
    public int MaxSteps;
    public string Steps;
    // Character starts at X50Y50
    // X = X
    // Y = Y
    // S = Realm Skin ID 
    // D = Discovered 0/1
    // V LandLordVision 0/1
    // I = Item ID (-1 = none) || T = Tattoo ID (-1 = none)
    // O = Realm Opponents IDs
    // ; = Step End
    // Example (without spaces) = X00 Y00 S00 D0 V0 I00 O00 01 02 03 04 05 06 07 08 ;
    // 1110
    public int X, Y;

    public Run()
    {

    }
}
