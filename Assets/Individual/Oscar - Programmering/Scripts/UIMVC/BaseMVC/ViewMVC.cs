using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewMVC : ElementMVC
{
    public FlashingBoxView flashingBoxView;
    public HealthBarView healthBarView;
    public LevelProgressBarView progressBarView;
    public CombatUIUltIconsView ultIconsView;
    public List<CombatUIUltWeaponReadyHolderView> ultWeaponReadyHolderView;
    public CombatUIWeaponHandlerView weaponHandlerView;
    public CombatUISymbolHolderView symbolHolderView;
    public CombatUIObjectiveCounterView objectiveCounterView;
    public List<CombatUIWeaponSymbolView> combatUIWeaponSymbolViews;
}
