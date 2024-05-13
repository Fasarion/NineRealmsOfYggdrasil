using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatUIMainWeaponSymbolView : ElementMVC
{
    public float baseScale;
    public float maxAnimatedScale;
    [NonSerialized] public RectTransform mainWeaponSymbolTransform;
    public float duration = 5.0f;

    public void Start()
    {
         mainWeaponSymbolTransform = GetComponent<RectTransform>();
        app.Notify(NotificationMVC.MainWeaponSetup, this, baseScale, maxAnimatedScale, mainWeaponSymbolTransform, duration);
    }
}
