<<<<<<< HEAD:Assets/TFT/Heros/Scripts/HeroEnum.cs
﻿public enum HeroStatus
=======
﻿public enum GameStatus
{
    Readying,
    Playing,
    Comping,
    Transiting
}
public enum HeroStatus
>>>>>>> parent of 58de95d... U:Assets/TFT/Script/EnumManager.cs
{
    Standby,
    Fight,
    Dead
}

public enum HeroClass
{
    None,
    Assassin,
    Brawler,
    Ranger,
    Supporter,
    Warrior,
    Wizard
}

public enum HeroRace
{
    None,
    Demon,
    Divinity,
    Human,
    Rebel,
    Spirit,
    Wild,
    Dwarf
}

public enum BuffLevel
{
    Newbie,
    Bronze,
    Sliver,
    Golden
}

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

public enum HeroLevel
{
    Level1,
    Level2,
    Level3
}

public enum HeroState
{
    Idle,
    Fight
}