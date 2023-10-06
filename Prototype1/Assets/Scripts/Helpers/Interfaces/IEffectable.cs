using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusEffects
{
    POISON, FIRE, COLD, SHOCK, ACID
}
public interface IEffectable 
{
    public void ActivateStatus(StatusEffects status);
}
