using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectile
{
    public void Shoot(Vector3 dir, Vector3 playerPos);
}
