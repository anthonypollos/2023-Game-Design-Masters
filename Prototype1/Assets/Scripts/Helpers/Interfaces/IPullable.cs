using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPullable
{
    public void Pulled();
    public void Lassoed();

    public void Break();
}
