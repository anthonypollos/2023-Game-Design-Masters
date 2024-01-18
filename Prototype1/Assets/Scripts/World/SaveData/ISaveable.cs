using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveable 
{
    public void SaveData(ref SavedValues savedValues);
    public void LoadData(SavedValues savedValues);

}
