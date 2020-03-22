public enum HeroAttribute
{
    Attack,
    Attack_Speed,
    Critical_Cahnce,
    Magic_Defense,
    Mana,
    Max_Health,
    Skill_Damage,
    Physic_Defense
}
public enum ItemType
{
    Axe,
    Belt,
    Bow,
    Cloak,
    Shield,
    Shoulder,
    Staff,
    Sword
}
public enum OpponentType
{
    Player,
    Monster
}
public enum GameStatus
{
    Readying,
    Playing,
    Comping,
    Transiting
}
public enum SyncHeroMethod
{
    AddHero,
    RemoveHero,
    HeroUpgrade
}

public enum SyncMoveHero
{
    Undefined,
    AddGameboard,
    RemoveGameboard,
    MoveHero
}
public enum HeroStatus
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
    Nothing,
    Idle,
    Fight,
    Walking
}