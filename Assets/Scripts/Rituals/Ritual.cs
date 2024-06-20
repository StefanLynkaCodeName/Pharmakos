using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ritual : ITarget
{
    public Player Owner;

    public Dictionary<OfferingType, int> Costs = new Dictionary<OfferingType, int>();

    public int ID = -1;
    public bool NeedsTargets = true;
    public string Description = "No Desc";

    public Ritual(){ }

    public Ritual(Player player)
    {
        Owner = player;
    }

    public void Init()
    {
        Owner.GameState.TryAssignID(this);
    }

    public int GetID()
    {
        return ID;
    }
    public string GetName()
    {
        return GetType().Name;
    }

    public virtual List<ITarget> GetTargets()
    {
        var targets = new List<ITarget>();

        return targets;
    }

    public virtual bool CanPlay()
    {
        if (!Owner.CanPayForRitual(this)) return false;

        List<ITarget> targets = GetTargets();
        bool hasTargets = targets.Count > 0 || !NeedsTargets;
        if (!hasTargets) return false;

        return true;
    }

    public virtual void Play(ITarget target)
    {
        Owner.PayRitualCosts(this);
    }

    public Ritual MakeBaseCopy(Player newOwner)
    {
        // Get the type of the calling class
        Type callingType = this.GetType();

        // Create a new instance of the calling class
        return (Ritual)Activator.CreateInstance(callingType, newOwner);
    }

    public Ritual DeepCopy(Player newOwner)
    {
        Ritual copy = MakeBaseCopy(newOwner);
        copy.Owner = newOwner;
        copy.ID = ID;
        copy.Owner.GameState.TargetsByID[ID] = copy;
        return copy;
    }

}
