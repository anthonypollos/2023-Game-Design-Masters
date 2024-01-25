using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISlowable
{
    public void EnterSlowArea(float slowPercent);
    public void ExitSlowArea(float slowPercent);
}
