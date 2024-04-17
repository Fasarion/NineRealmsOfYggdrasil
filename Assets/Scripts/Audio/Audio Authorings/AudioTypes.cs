[System.Serializable]
public struct AudioData
{
    public int AudioEnumValue;
    
    public EventCategoryType eventCategoryType;
    public EnvironmentType environmentType;
    public EnemyTyping enemyTyping;
    public AudioEventType audioEventType;
}
[System.Serializable]
public enum EnvironmentType
{
    None = 0,
    Tree = 1,
    Rock = 2,
}

[System.Serializable]
public enum EventCategoryType
{
    None = 0,
    Player = 1,
    Enemy = 2,
    Environment = 3,
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