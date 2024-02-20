using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IToggleable
{
    public void Toggle(IsoAttackManager player = null);
    public bool GetToggle();
}
