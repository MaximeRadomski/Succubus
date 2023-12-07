using System;

public static class TattoosData
{
    public static int LegendaryTattooAppearancePercent = 1;
    public static int RareTattooAppearancePercent = 15;

    public static string[] Tattoos =
    /* 0  */ { "Tribal", "Trinity", "Cleaver", "Broken Clock", "Knuckle", "Fox", "Black Mirror", "Kinky Boot", "Thermal Goggles", "D20",
    /* 10 */ "Broken Sword", "Wooden Wings", "T Worship", "I Worship", "Backspace Key", "X-Ray Glasses", "Spicy Lollipop", "Trash Bin", "Hedgehog", "Handbrake",
    /* 20 */ "Eclipse Glasses", "Crown Of Thorns", "Perfect Score", "Heavy Weight", "Ouroboros", "SOP Sign", "Full Black", "Hell Suppo", "Earth Child", "Heaven Icon",
    /* 30 */ "Simp Shield", "Permutation", "Fire Stone", "Earth Stone", "Water Stone", "Wind Stone", "Forgotten Dream", "Quad Damage", "Double Edged", "Damocles",
    /* 40 */ "Brand of Sacrifice", "Beer", "Loot Box", "Family Tree", "Star", "Infinite Stairs", "Blood Eye", "Water Eye", "Insulating Foam", "Sickle", 
    /* 50 */ "Hammer", "God Hand", "Map", "Target", "Last Stand", "Slumbering Dragoncrest", "Social Pyramid", "Farm", "Owl", "Pentagram",
    /* 60 */ "Diamond", "Scissor Jack", "Basketball Hoop", "Slav Wheel", "Devil's Contract", "Bass Guitar", "Instant Noodles", "Black Hole", "Butterfly Knife", "Holy Mantle",
    /* 70 */ "Logic Gate", "Ying", "Yang", "Vanilla Ice Cream", "Chocolate Ice Cream", "No Ragrets", "Sword Tip", "Whetstone", "Paper Cut", "Double Ended Dog Toy",
    /* 80 */ "S-R-S", "Knives Juggling", "Top Hat", "Ace of Clubs", "Cookie", "Witch Hat", "Ugly Sneaker", "Seed of Light", "Seed of Darkness", "Gatling Gun",
    /* 90 */ "Podium", "Whac-A-Mole", "Dark Sign", "Gamegirl", "Stairway to Heaven", "Brobot", "King of Clubs" };

    public static string[] CommonTattoosNames = { Tattoos[00], Tattoos[01], Tattoos[04], Tattoos[07], Tattoos[09], Tattoos[10], Tattoos[11], Tattoos[12], Tattoos[13], Tattoos[17], Tattoos[20], Tattoos[21], Tattoos[31], Tattoos[37], Tattoos[39], Tattoos[41], Tattoos[43], Tattoos[44], Tattoos[48], Tattoos[49], Tattoos[50], Tattoos[57], Tattoos[58], Tattoos[60], Tattoos[61], Tattoos[64], Tattoos[65], Tattoos[71], Tattoos[72], Tattoos[76], Tattoos[77], Tattoos[82], Tattoos[83], Tattoos[84], Tattoos[86], Tattoos[87], Tattoos[88], Tattoos[89], Tattoos[90], Tattoos[91], Tattoos[92], Tattoos[93], Tattoos[95], Tattoos[96] };
    public static string[] RareTattoosNames =      { Tattoos[02], Tattoos[03], Tattoos[05], Tattoos[08], Tattoos[14], Tattoos[15], Tattoos[16], Tattoos[18], Tattoos[19], Tattoos[24], Tattoos[30], Tattoos[32], Tattoos[33], Tattoos[34], Tattoos[35], Tattoos[36], Tattoos[38], Tattoos[40], Tattoos[42], Tattoos[46], Tattoos[47], Tattoos[53], Tattoos[59], Tattoos[62], Tattoos[63], Tattoos[66], Tattoos[68], Tattoos[69], Tattoos[75], Tattoos[78], Tattoos[79], Tattoos[81], Tattoos[85] };
    public static string[] LegendaryTattoosNames = { Tattoos[06], Tattoos[22], Tattoos[23], Tattoos[25], Tattoos[26], Tattoos[27], Tattoos[28], Tattoos[29], Tattoos[45], Tattoos[51], Tattoos[52], Tattoos[54], Tattoos[55], Tattoos[56], Tattoos[67], Tattoos[73], Tattoos[74], Tattoos[80], Tattoos[94] };

    // DEBUG //
    public static bool DebugEnabled = Constants.TattoosDebug;
    public static bool DebugMultitude = false;
    public static Tattoo DebugTattoo = GetTattooFromName("King of Clubs");

    public static Tattoo GetRandomTattoo()
    {
        var rareBonus = PlayerPrefsHelper.GetBonusRarePercent();
        if (rareBonus > Constants.MaxRarePercent)
            rareBonus = Constants.MaxRarePercent;
        var rarityRare = RareTattooAppearancePercent + rareBonus;

        var legendaryBonus = PlayerPrefsHelper.GetBonusLegendaryPercent();
        if (legendaryBonus > Constants.MaxLegendaryPercent)
            legendaryBonus = Constants.MaxLegendaryPercent;
        var rarityLegendary = LegendaryTattooAppearancePercent + legendaryBonus;

        int rarityPercent = UnityEngine.Random.Range(0, 100);
        if (rarityPercent < rarityLegendary)
            return GetRandomTattooFromRarity(Rarity.Legendary);
        else if (rarityPercent < rarityRare)
            return GetRandomTattooFromRarity(Rarity.Rare);
        else
            return GetRandomTattooFromRarity(Rarity.Common);
    }

    public static Tattoo GetRandomTattooFromRarity(Rarity rarity)
    {
        if (rarity == Rarity.Legendary)
            return GetTattooFromNames(LegendaryTattoosNames, UnityEngine.Random.Range(0, LegendaryTattoosNames.Length));
        else if (rarity == Rarity.Rare)
            return GetTattooFromNames(RareTattoosNames, UnityEngine.Random.Range(0, RareTattoosNames.Length));
        else
            return GetTattooFromNames(CommonTattoosNames, UnityEngine.Random.Range(0, CommonTattoosNames.Length));
    }

    public static Tattoo GetTattooFromNames(string[] names, int id, int loopId = -1)
    {
        if (id >= names.Length)
            id = 0;
        if (string.IsNullOrEmpty(names[id]))
            return null;
        var cleanName = names[id].Replace(" ", "");
        cleanName = cleanName.Replace("'", "");
        cleanName = cleanName.Replace("-", "");
        var instance = (Tattoo)Activator.CreateInstance(Type.GetType("Tattoo" + cleanName, true));
        if (PlayerPrefsHelper.GetMaxedOutTattoos().Contains((instance).Id) && loopId != id)
        {
            if (loopId == -1)
                loopId = id;
            return GetTattooFromNames(names, ++id, loopId);
        }
        return instance;
    }

    public static Tattoo GetTattooFromName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;
        var cleanName = name.Replace(" ", "");
        cleanName = cleanName.Replace("'", "");
        cleanName = cleanName.Replace("-", "");
        var instance = (Tattoo)Activator.CreateInstance(Type.GetType("Tattoo" + cleanName));
        return instance;
    }
}
