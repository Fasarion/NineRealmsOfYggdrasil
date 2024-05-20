[System.Serializable]
public struct AudioData
{
    public int AudioEnumValue;
    
    public EventCategoryType eventCategoryType;
    public WeaponTyping weaponType;
    public EnemyTyping enemyTyping;
    public AudioEventType audioEventType;
    public AttackTypeAudio attackType;
}
[System.Serializable]
public enum WeaponTyping
{
    None = 0,
    Sword = 1,
    Hammer = 2,
    Birds = 3,
    Mead = 4,
}

public enum AttackTypeAudio
{
    None = 0,
    Normal = 1,
    Special = 2,
    Ultimate = 3,
    
}

[System.Serializable]
public enum EventCategoryType
{
    None = 0,
    Weapon = 1,
    Player = 2,
    Enemy = 3,
    Environment = 4,
    
}

[System.Serializable]
public enum EnemyTyping
{
    None = 0,
    Grunt = 1,
    Ranger = 2,
    CoolDude = 3,
    
}

[System.Serializable]
public enum AudioEventType
{
    None = 0,
    OnUse = 1,
    OnImpact = 2,
    OnCharge = 3,
}