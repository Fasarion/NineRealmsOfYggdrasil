using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatUIMainWeaponSymbolController : BaseControllerMVC
{

    
  
    
    public override void OnNotification(string p_event_path, Object p_target, params object[] p_data)
    {
        switch (p_event_path)
        {
            case NotificationMVC.MainWeaponSetup:
            {
                var mainWeaponSymbolModel = app.model.mainWeaponSymbolModel;
                var baseScale = (float)p_data[0];
                var maxAnimatedScale = (float)p_data[1];
                var mainWeaponSymbolTransform = (RectTransform)p_data[2];
                var duration = (float)p_data[3];
                mainWeaponSymbolTransform.localScale = new Vector3(baseScale, baseScale, baseScale);
                mainWeaponSymbolModel.duration = duration;
                mainWeaponSymbolModel.maxAnimatedScale = maxAnimatedScale;
                mainWeaponSymbolModel.mainWeaponSymbolTransform = mainWeaponSymbolTransform;
                mainWeaponSymbolModel.baseScale = baseScale;
                mainWeaponSymbolModel.animatedScale = baseScale;
                break;
            }
        }
    }
}
