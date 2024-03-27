using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMakeDamage
{
    public void UpdateDamage(int deltaDamage);
}

public interface IHasCooldown
{
    public void UpdateCooldown(float deltaCooldown);
}

public interface IHasSize
{
    public void UpdateSize(float deltaSize);
}

public interface IHasSpeed
{
    public void UpdateSpeed(float deltaSpeed);
}

public interface IHasForce
{
    public void UpdateForce(float deltaForce);
}

public interface IHasArea
{
    public void UpdateArea(float deltaArea);
}

public interface IHasTrustBool
{
    public void UpdateTrustBool(float deltaTrustBool);
}

public interface IHasLifeSteal
{
    public void UpdateLifeSteal(float deltaLifeSteal);
}

public interface IHasAmount
{
    public void UpdateAmount(float deltaAmount);
}

public interface IHasPierce
{
    public void UpdatePierce(float deltaPierce);
}
