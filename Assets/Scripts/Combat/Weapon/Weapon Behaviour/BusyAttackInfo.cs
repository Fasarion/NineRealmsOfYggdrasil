using Patrik;

[System.Serializable]
public struct BusyAttackInfo
{
    public bool Busy;
    public WeaponType WeaponType;
    public AttackType AttackType;

    public BusyAttackInfo(bool busy, WeaponType weaponType, AttackType attackType)
    {
        Busy = busy;
        WeaponType = weaponType;
        AttackType = attackType;
    }
    
    public bool IsBusy(AttackType attackType, WeaponType weaponType)
    {
        bool isBusyWeapon = weaponType == WeaponType;
        bool isBusyAttack = attackType == AttackType;

        // dont consider current attack type and weapon type combo as busy
        if (isBusyWeapon && isBusyAttack)
        {
            return false;
        }
            
        return Busy;
    }
}