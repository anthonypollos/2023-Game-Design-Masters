using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingMistGenerator : MonoBehaviour, IPullable
{
    [SerializeField] GameObject healingMistPrefab;
    [SerializeField] bool singleUse = false;
    [SerializeField] float cooldown = 5f;

    bool onCD = false;

    public void Break()
    {
        return;
    }

    public void Lassoed()
    {
        return;
    }

    public void Pulled(IsoAttackManager player = null)
    {
        Mist();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Mist()
    {
        if (!onCD)
        {
            Instantiate(healingMistPrefab, transform.position, Quaternion.identity);
            if (!singleUse)
                StartCoroutine(Cooldown());
            else
                onCD = true;
                
        }
    }

    IEnumerator Cooldown()
    {
        onCD = true;
        yield return new WaitForSeconds(cooldown);
        onCD = false;
    }
}
