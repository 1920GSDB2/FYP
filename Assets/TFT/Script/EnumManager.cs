public enum HeroAttribute
{
    Attack,
    Attack_Speed,
    Critical_Cahnce,
    Magic_Defense,
    Mana,
    Health,
    maxHp,
    Skill_Damage,
    Physic_Defense,
    Critical_Damage,
    Shield,
    Heal_Recovery_Rate,
    Mana_Recovery_Rate
}
public enum ItemType
{
    Axe,
    Belt,
    Bow,
    Cloak,
    Crystal,
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
    Walking,
    Die,
    Control
}
public enum Award
{
    Equipment,
    Money
}
public enum ControlSkillType {
    No,
    Stun,
    Fear
}
public enum DamageType {
    No,
    Magic,
    Physical,
    TrueDamage,
    CriticalDamage,
    Heal
}
public enum WaveType
{
    Monster,
    Player,
    Shadow
}