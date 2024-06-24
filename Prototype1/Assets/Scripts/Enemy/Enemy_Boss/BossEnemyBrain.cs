using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyBrain : EnemyBrain
{
    [HideInInspector]
    public BossEnemyAttacks bossAttacks;
    [HideInInspector]
    public BossEnemyHealth bossHealth;
    [HideInInspector]
    public BossEnemyInteractions bossInteractions;
    [HideInInspector]
    public BossEnemyMovevment bossMovevment;

    private NeoBossFightController bossManager;

    [SerializeField] Material enragedMaterial;
    private Material defaultMaterial;

    [SerializeField] GameObject model;

    [SerializeField]
    bool debugToggleTrigger = false;
    protected override void Starting()
    {
        base.Starting();
        bossAttacks = attack.GetComponent<BossEnemyAttacks>();
        bossAttacks.bossBrain = this;
        bossHealth = health.GetComponent<BossEnemyHealth>();
        bossHealth.bossBrain = this;
        bossInteractions = interaction.GetComponent<BossEnemyInteractions>();
        bossInteractions.bossBrain = this;
        bossMovevment = movement.GetComponent<BossEnemyMovevment>();
        bossMovevment.bossBrain = this;
        bossManager = FindObjectOfType<NeoBossFightController>();

        //if we can find a material in a child of this, assign that as the default material
        if (model.GetComponentInChildren<MeshRenderer>().material != null)
        {
            defaultMaterial = model.GetComponentInChildren<MeshRenderer>().material;
            //Return as soon as we get one, we don't need to go through every child object in this so long as we have something.
            return;
        }
        //if we can't find a material in the children, give up and set it to the one we have serialized. This should NOT occur but it beats being broken
        else
        {
            defaultMaterial = enragedMaterial;
        }
    }

    private void FixedUpdate()
    {
        if(!interaction.stunned && state == EnemyStates.CHARGING)
        {
            CheckMovement();
        }
    }
    protected override void Updating()
    {
        if (state != EnemyStates.DEAD)
        {
            //if not stunned and not attacking
            if (!interaction.stunned && state == EnemyStates.NOTHING)
            {
                CheckMovement();
                CheckRotation();
                CheckAttack();
                if(isAggro)
                    CheckArea();
            }
            else if(!interaction.stunned && state == EnemyStates.ENRAGED)
            {
                CheckAttack();
            }
            //if stunned stop all movement calculations
            else if (interaction.stunned && moveable != null)
            {
                if (!moveable.isLaunched)

                    movement.Stop();
            }
        }

        if(debugToggleTrigger)
        {
            debugToggleTrigger = false;
            if(state == EnemyStates.ENRAGED)
            {
                Calm();
            }
        }
    }

    protected override void CheckAttack()
    {
        base.CheckAttack();
    }

    protected override void CheckMovement()
    {
        base.CheckMovement();
    }

    public void Enrage()
    {
        bossAttacks.Enrage();
        state = EnemyStates.ENRAGED;
        bossManager.Enrage();  

        //Iterate each material in the child and set it to the enraged mat
        foreach(Transform child in model.transform)
        {
            //if this child has a renderer, set its material to enraged
            if (child.GetComponent<Renderer>() != null) child.GetComponent<Renderer>().material = enragedMaterial;
        }

    }

    public void Calm()
    {
        an.SetTrigger("Calmed");
        state = EnemyStates.NOTHING;

        //Iterate each material in the child and set it to the normal mat
        foreach (Transform child in model.transform)
        {
            //if this child has a renderer, set its material to default
            if (child.GetComponent<Renderer>() != null) child.GetComponent<Renderer>().material = defaultMaterial;
        }
    }
}
