using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectivePage : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI objectiveText;
    MissionFolder mf;

    private void OnEnable()
    {
        if(mf == null)
            mf = FindObjectOfType<MissionFolder>();
        objectiveText.text = mf.GetText();
    }


}
