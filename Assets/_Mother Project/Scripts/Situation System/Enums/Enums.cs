
public enum GameStates
{
    StartTurn,
    MidTurn,
    MovementGridSelectionTurn,
    TargetSelectionTurn,
    FinishTurn,
    EnemyTurn,
    Simulation,
    GhostPlay,
    SituationOff

}


public enum PlayerType
{
    Player,
    Ally,
    Enemy
}




public enum Direction
{
    up,
    down,
    left,
    right,
    none



}

public enum ActionBranch
{
    Kill,
    Survive,
    Deal,
}

//public enum GridObjectTypes
//{
//    Player,
//    Enemy,
//    Obstacle
//}

public enum Prerequisite
{
    Weapon,
    LevelUp,
    None,
    SinLevelUp,
    VirtueLevelUp,
    HunterLevelUp,
    CasterLevelUp,
    FighterLevelUp,
    PerformerLevelUp,
    TalkerLevelUp,
    Situational,
    Hunter,
    Caster,
    Fighter,
    performer,
    Talker,
    Always

}


public enum MoveType
{
    None,
    Damage,
    Equip,
    ResidualDamage,
    Movement,
    Stun,
    Buff,
    Debuff,
    Condition,
    Counter,
    MoodChange,
    Survival,
    Stealth,
    Heal,
    item,
    DexterityReduction,
    Sin,
    Virtue
}

public enum OffenseModifier
{
    None,
    DexOrStr,
    Dexterity,
    Strength,
    Charisma,
    HighestStat,
    StrOrArc
}

public enum DefenseModifier
{
    None,
    Mind,
    Endurance,
}


public enum TargetType
{
    None,
    SingleTarget,
    SingleTargetAlly,
    SingleTargetEnemy,
    Line,
    AOE,
    Splash,
    Cone,
    WideLine,
    Self
}

public enum Species
{
    None,
    Human,
    Monster,
    Celestial,
    Demonic,
    Undead,
    Myth,
}


public enum CharClass
{
    None,
    Hunter,
    Caster,
    Fighter,
    Performer,
    Talker,
    Other,
}

public enum SubClass
{
    None,
    //TheHunter
    BountyHunter,
    CelestialSentinel,
    DemonicAssassin,
    BeastSlayer,
    PhantomKiller,
    MythicalHero,
    //TheCaster
    Necromancy,
    Valor,
    Nature,
    Mind,
    Time,
    Divination,
    //TheFighter
    KiMaster,
    Guardian,
    Juggernaut,
    Arcanist,
    Exorcist,
    Lathial,
    //Performer
    Bard,
    Acrobat,
    Jester,
    Illusionist,
    Actor,
    //Talker
    Tactician,
    Seductor,
    Napit,
    Storyteller,
    Academic,
}

public enum Mood
{
    None,
    Entertained,
    Intrigued,
    Charmed,
    Stoic,
    Calm,
    Enraged,
    Pissed,
    Ascended,
    Demented,
    Frightened,
    Brave,
    Depressed
}

public enum Presence
{
    None,
    Threatening,
    Calming,
    Intriguing,
    Good,
    Evil,
    Seductive
}

public enum Condition
{
    None,
    Paralyzed,
    Confused,
    Wounded,
    Incapacitated,
    Locked,
    Dazed,
}

public enum Aggression
{
    //aggression is calculated by a float vaule of 1-100. eg, docile is between value 1-20 and care
    //careful is 21-40 and contining on.
    Docile,
    Careful,
    Stoic,
    Irritated,
    Aggressive,
}


public enum ActionType{
    Ranged,
    Melee
}

public enum ActionStance
{
    Defense,
    Offense
}

public enum Mortality
{
    Alive,
    Dead
}

public enum GameType
{
    OneVOne,
    OneVTwo,
    AllOut
}

public enum TeamName
{
    TeamA,
    TeamB,
    TeamC,
    TeamD,
    NullTeam
}

public enum CurrentWeapon
{
    Dagger,
    Sword,
    Talisman,
    BowAndArrow,
    Hammer,
    Axe,
    Spear,
    Staff,
    Spoon
}

public enum HexOrientation
{

    Up,
    Down,
    UpLeft,
    UpRight,
    DownLeft,
    DownRight


}

public enum SceneIndexes { 


    Room2 = 2,
    Room1 = 1,
    PersistantScene=0


}


