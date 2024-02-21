using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyGenerator : MonoBehaviour, IPullable
{
    [SerializeField]
    [Tooltip("Leave 0 to be infinite")]
    int uses;
    [SerializeField]
    [Tooltip("Leave 0 to be instant")]
    float timeToRecharge;
    bool charged = false;
    public void Break()
    {

    }

    public void Lassoed()
    {

    }

    public void Pulled(IsoAttackManager player = null)
    {
        if(player!=null && charged)
        {
            charged = false;
            player.AquireCharge();
            uses--;
            if(uses>0)
                StartCoroutine(Charging());
        }
    }

    private IEnumerator Charging()
    {
        yield return new WaitForSeconds(timeToRecharge);
        charged = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(uses==0)
        {
            uses = int.MaxValue;
        }
        StartCoroutine(Charging());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
