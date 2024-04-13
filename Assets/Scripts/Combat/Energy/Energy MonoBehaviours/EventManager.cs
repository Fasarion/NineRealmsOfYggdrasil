using Patrik;
using UnityEngine.Events;

public static class EventManager
{
    public static UnityAction<WeaponBehaviour, bool> OnSetupWeapon;
    public static UnityAction<WeaponBehaviour> OnWeaponSwitch;
}