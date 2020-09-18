public class Run
{
    public Realm CurrentRealm;
    public int RealmLevel;
    public int MaxSteps;
    public string Steps;
    public int X, Y;
    // Character starts at X50 Y50 because coordinates are stored in two digits and coordinates under zero could mean up to 3 digits
    // X = X
    // Y = Y
    // R = Realm
    // S = Step Type ID
    // D = Discovered 0/1
    // V LandLordVision 0/1
    // I = Item ID (-1 = none) || T = Tattoo ID (-1 = none) || C = Character ID (-1 = none)
    // O = Realm Opponents IDs
    // ; = Step End
    // Example (without spaces) = X00 Y00 R0 T00 D0 V0 I00 O00 01 02 03 04 05 06 07 08 ;
    // 1110

    public bool CharacterEncounterAvailability;
    public int CharEncounterPercent = 5;
    public int ItemLootPercent = 30;

    public Run()
    {
        CurrentRealm = Realm.Heaven;
        RealmLevel = 1;
    }
}
