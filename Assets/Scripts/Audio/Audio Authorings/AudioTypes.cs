[System.Serializable]
public struct AudioData
{
    public int AudioEnumValue;
    
    public EventCategoryType eventCategoryType;
    public WeaponTyping weaponType;
    public EnemyTyping enemyTyping;
    public AudioEventType audioEventType;
}
[System.Serializable]
public enum WeaponTyping
{
    None = 0,
    Sword = 1,
    Hammer = 2,
    Mead = 3,
    Birds = 4,
}

[System.Serializable]
public enum EventCategoryType
{
    None = 0,
    Player = 1,
    Enemy = 2,
    Environment = 3,
    Weapon = 4,
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
    HitAudio = 1
}