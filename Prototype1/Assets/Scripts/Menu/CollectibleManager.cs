using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CollectibleManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI collectName, collectDesc;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayName(string name)
    {
        collectName.text = name;
    }

    public void DisplayDesc(string description)
    {
        collectDesc.text = description;
    }
}
