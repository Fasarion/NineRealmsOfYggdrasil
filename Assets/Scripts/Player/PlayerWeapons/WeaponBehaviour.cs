using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using Unity.VisualScripting;

public abstract class WeaponBehaviour : MonoBehaviour
{
    public List<ActionSequenceObject> attackActions = new List<ActionSequenceObject>();
    public List<ActionSequenceObject> swingActions = new List<ActionSequenceObject>();
    public List<ActionSequenceObject> impactActions = new List<ActionSequenceObject>();
    public List<ActionSequenceObject> hitActions = new List<ActionSequenceObject>();
    public List<ActionSequenceObject> retractActions = new List<ActionSequenceObject>();
    public List<ActionSequenceObject> powerActions = new List<ActionSequenceObject>();
    public List<ActionSequenceObject> endActions = new List<ActionSequenceObject>();

    [SerializeField] Animator animator;

    public WeaponBehaviour weapon;

    //public StatCounterHandler statCounterHandler;
    
    public virtual void RunAttackSequence()
    {
        
        foreach (var sequence in attackActions)
        {
            Timing.RunCoroutine(sequence.ExecuteSequence());
        }
    }

    public virtual void RunSwingSequence()
    {
        foreach (var sequence in swingActions)
        {
            Timing.RunCoroutine(sequence.ExecuteSequence());
        }
    }

    public virtual void RunImpactSequence()
    {
        
        foreach (var sequence in impactActions)
        {
            Timing.RunCoroutine(sequence.ExecuteSequence());
        }
    }

    public virtual void RunHitSequence()
    {
        foreach (var sequence in hitActions)
        {
            Timing.RunCoroutine(sequence.ExecuteSequence());
        }
    }

    public virtual void RunRetractSequence()
    {
        foreach (var sequence in retractActions)
        {
            Timing.RunCoroutine(sequence.ExecuteSequence());
        }
    }

    public virtual void RunPowerSequence()
    {
        foreach (var sequence in powerActions)
        {
            Timing.RunCoroutine(sequence.ExecuteSequence());
        }
    }

    public virtual void RunEndSequence()
    {
        foreach (var sequence in endActions)
        {
            Timing.RunCoroutine(sequence.ExecuteSequence());
        }
    }

    public virtual IEnumerator<float> AttackLoop(float cooldown)
    {
        yield return 0;
    }

    public virtual void StartAttackLoop()
    {
        
    }

    public virtual void StopAttackLoop()
    {
        Timing.KillCoroutines("AttackLoop");
    }

    public Animator GetAnimator()
    {
        return animator;
    }

    public virtual BaseWeaponType GetWeaponType()
    {
        return BaseWeaponType.Bow;
    }

    public virtual string GetAnimatorKeywords()
    {
        return null;
    }

    public virtual void RunAttackCalculations()
    {
        
    }
    
    public virtual void SortUpgradePackage(UpgradeObject upgrade)
    {
        foreach (var mod in upgrade.AttackActions)
        {
            mod.GenerateSequence();
            mod.SetUpSequence(this.weapon);
            attackActions.Clear();
            attackActions.Add(mod);
            
        }

        if (upgrade.overrideSwing)
        {
            swingActions.Clear();
        }
        foreach (var mod in upgrade.SwingActions)
        {
            mod.GenerateSequence();
            swingActions.Add(mod);
            mod.SetUpSequence(this.weapon);
        }

        if (upgrade.overrideImpact)
        {
            impactActions.Clear();
        }
        
        foreach (var mod in upgrade.ImpactActions)
        {
            mod.GenerateSequence();
            mod.SetUpSequence(this.weapon);
            impactActions.Add(mod);
            
        }

        if (upgrade.overrideHit)
        {
            hitActions.Clear();
        }

        foreach (var mod in upgrade.HitActions)
        {
            mod.GenerateSequence();
            hitActions.Add(mod);
            mod.SetUpSequence(this.weapon);
        }
        
        if (upgrade.overrideRetract)
        {
            retractActions.Clear();
        }

        foreach (var mod in upgrade.RetractActions)
        {
            mod.GenerateSequence();
            retractActions.Add(mod);
            mod.SetUpSequence(this.weapon);
        }
        
        if (upgrade.overridePower)
        {
            powerActions.Clear();
        }

        foreach (var mod in upgrade.PowerActions)
        {
            mod.GenerateSequence();
            powerActions.Add(mod);
            mod.SetUpSequence(this.weapon);
        }

        foreach (var mod in upgrade.EndActions)
        {
            mod.GenerateSequence();
            mod.SetUpSequence(this.weapon);
            endActions.Clear();
            endActions.Add(mod);
            
        }
    }
}
