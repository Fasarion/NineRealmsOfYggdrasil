using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationMVC : MonoBehaviour
{
    public const string BallHitGround = "ball.hit.ground";
    public const string GameComplete = "game.complete";
    public const string GameStart = "game.start";
    public const string SceneLoad = "scene.load";
    public const string SetFlashingBoxColor = "flashingBox.setColor";
    
    public const string SetHealthModel = "healthBar.setHealth";
    public const string HealthBarLevelsSetView = "healthBar.healthBarLevelsSet";
    
    public const string ExperienceInfoChanged = "levelProgressBar.experienceInfoChanged";

    public const string UltimateUsed = "gameplay.ultimateUsed";
    public const string WeaponSetup = "gameplay.weaponSetup";


}
