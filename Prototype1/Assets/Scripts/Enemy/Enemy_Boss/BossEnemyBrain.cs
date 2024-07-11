using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
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

    [Header("Boss Sound Files")]
    StudioEventEmitter studioEventEmitter;
    [SerializeField] bool isBossFightThemeFilled = false;
    [SerializeField] EventReference bossFightTheme;

    [SerializeField] float minTimeBetweenBarks;
    [SerializeField] float maxTimeBetweenBarks;
    [SerializeField] List<EventReference> zeroCystsBrokenBarks;
    [SerializeField] List<EventReference> oneCystBrokenBarks;
    [SerializeField] List<EventReference> twoCystsBrokenBarks;
    [SerializeField] List<EventReference> threeCystsBrokenBarks;

    [Header("These will need 3 parameters for each cyst")]
    [SerializeField] List<EventReference> cystBrokenBarks;
    [SerializeField] List<EventReference> cystSpawnBarks;


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
        studioEventEmitter = GetComponent<StudioEventEmitter>();

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

    public override void Aggro()
    {
        if (!isAggro)
        {
            if (isBossFightThemeFilled)
            {
                //Debug.Log("Starting Boss Fight Music");
                FindObjectOfType<GameController>().StartBossFight(bossFightTheme);
            }
            if(studioEventEmitter!=null)
            {
                StartCoroutine(BattleBarks());
            }
            base.Aggro();
        }
    }

    IEnumerator BattleBarks()
    {
        List<int> usedBag = new List<int>();
        while(true)
        {
            float timer = Random.Range(minTimeBetweenBarks, maxTimeBetweenBarks);
            yield return new WaitForSeconds(timer);
            yield return new WaitUntil(() => !studioEventEmitter.IsPlaying());
            List<EventReference> currentList = null;
            switch(bossManager.GetTargetsHit())
            {
                case 0: 
                    if(currentList!=zeroCystsBrokenBarks && zeroCystsBrokenBarks.Count!=0)
                    {
                        currentList = zeroCystsBrokenBarks;
                        usedBag.Clear();
                    }
                    break;
                case 1:
                    if(currentList!=oneCystBrokenBarks && oneCystBrokenBarks.Count!=0)
                    {
                        currentList = oneCystBrokenBarks;
                        usedBag.Clear();
                    }
                    break;
                case 2:
                    if(currentList!=twoCystsBrokenBarks && twoCystsBrokenBarks.Count!=0)
                    {
                        currentList = twoCystsBrokenBarks;
                        usedBag.Clear();
                    }
                    break;
                case 3:
                    if(currentList!=threeCystsBrokenBarks && threeCystsBrokenBarks.Count!=0)
                    {
                        currentList = threeCystsBrokenBarks;
                        usedBag.Clear();
                    }
                    break;
            }

            EventReference reference = default;
            if(currentList == null)
            {
                continue;
            }
            List<int> bag = new List<int>();
            for (int i = 0; i<currentList.Count; i++)
            {
                if(!usedBag.Contains(i))
                {
                    bag.Add(i);
                }
            }

            int idx = Random.Range(0, bag.Count);
            reference = currentList[idx];
            usedBag.Add(idx);
            if (usedBag.Count == currentList.Count)
            {
                usedBag.Clear();
            }

            studioEventEmitter.Stop();
            studioEventEmitter.ChangeEvent(reference);
            studioEventEmitter.Play();
        }
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

        if(studioEventEmitter!=null)
        {
            EventReference reference = default;

            switch(bossManager.GetTargetsHit())
            {
                case 0:
                    if (cystSpawnBarks.Count < 1)
                        return;
                    reference = cystSpawnBarks[0];
                    break;
                case 1:
                    if (cystSpawnBarks.Count < 2)
                        return;
                    reference = cystSpawnBarks[1];
                    break;
                case 2:
                    if (cystSpawnBarks.Count < 3)
                        return;
                    reference = cystSpawnBarks[2];
                    break;
            }

            studioEventEmitter.Stop();
            studioEventEmitter.ChangeEvent(reference);
            studioEventEmitter.Play();
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

        if (studioEventEmitter != null)
        {
            EventReference reference = default;

            switch (bossManager.GetTargetsHit())
            {
                case 1:
                    if (cystSpawnBarks.Count < 1)
                        return;
                    reference = cystSpawnBarks[0];
                    break;
                case 2:
                    if (cystSpawnBarks.Count < 2)
                        return;
                    reference = cystSpawnBarks[1];
                    break;
                case 3:
                    if (cystSpawnBarks.Count < 3)
                        return;
                    reference = cystSpawnBarks[2];
                    break;
            }

            studioEventEmitter.Stop();
            studioEventEmitter.ChangeEvent(reference);
            studioEventEmitter.Play();
        }
    }
}
