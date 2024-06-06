using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewAbilityUnlockTutorialSegment : TutorialSegment
{

    [SerializeField]private bool hasUnlockedRightClick;

    public bool hasUnlockedSecondWeapon;

    public bool hasFilledUpUltimate;
    // Start is called before the first frame update
    void Start()
    {
        hasUnlockedRightClick = false;
        hasUnlockedSecondWeapon = false;
        hasFilledUpUltimate = false;

    }

    public void RightClickUnlocked(bool hasUnlocked)
    {
        hasUnlockedRightClick = hasUnlocked;
        CheckIfAllConditionsMet();
    }

    public void SecondWeaponUnlocked(bool hasUnlocked)
    {
        hasUnlockedSecondWeapon = hasUnlocked;
        CheckIfAllConditionsMet();
    }

    public void UltimateReady(bool ultimateReady)
    {
        hasFilledUpUltimate = ultimateReady;
        CheckIfAllConditionsMet();
    }

    public void CheckIfAllConditionsMet()
    {
        if (hasFilledUpUltimate && hasUnlockedRightClick && hasUnlockedSecondWeapon)
        {
            SegmentCompleted();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
