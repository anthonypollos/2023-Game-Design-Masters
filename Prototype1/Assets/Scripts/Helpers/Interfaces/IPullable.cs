using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPullable
{
    public void Pulled(IsoAttackManager player = null);
    public void Lassoed();

    public void Break();
}
