using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Purpose: A trigger that damages whoever's in it
/// 
/// Author: Sean Lee 2/2/2024
/// CAVEAT: At the moment, this will only damage one thing inside it at a time if tick damage is on
/// </summary>

///Known issues:
///If you are inside a hurt trigger with a negative value (a trigger that HEALS instead of HURTS) and
///you have full health and then you take damage WHILE STILL INSIDE THE TRIGGER, it will not check you
///a second time, and you will instead have to exit and re-enter the trigger to heal.
public class TriggerHurt : MonoBehaviour
{
    [Header("Activator Options")]
    [Tooltip("Dude, do you really need a tooltip for what this does?")]
    public bool hurtPlayer = true;
    [Tooltip("Hurt objects with 'Generic Item'")]
    public bool hurtItem = true;
    [Tooltip("Hurt enemies")]
    public bool hurtEnemy = true;
    [Space(10)]
    [Header("Damage Options")]
    [Tooltip("Take a wild guess what this does")]
    public int damageAmount = 10;
    [Tooltip("Does this trigger do tick damage or does it only hurt once upon entering it?")]
    public bool tickDamage = false;
    [Tooltip("How often does this refire?")]
    public float tickRate = 0.5f;
    [Space(10)]
    [Tooltip("Remove this trigger after it's used once?")]
    public bool destroyAfterUse = false;

    //This bool becomes true if the object that entered the trigger region actually CAN be hurt.
    //We use this to determine whether or not we can run the code for tick damage, destroyAfterUse, etc.
    private bool hurtable;

    [SerializeField] private JukeBox jukebox;

    private void Awake()
    {
        jukebox.SetTransform(transform);
    }

    // Start is called before the first frame update
    void Start()
    {
        //First thing's first, set hurtable to false
        hurtable = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        //If an object enters this trigger, try to hurt it.
        Hurt(other.gameObject);
    }

    //Main Hurting Function
    private void Hurt(GameObject HurtObject)
    {
        //If the object is the player and is valid, hurt the player!
        if (HurtObject.CompareTag("Player") && hurtPlayer)
        {
            hurtable = true;
            HurtPlayer(HurtObject);
        }

        //If the object is an enemy and is valid, hurt the enemy!
        if (HurtObject.CompareTag("Enemy") && hurtEnemy)
        {
            hurtable = true;
            HurtEnemy(HurtObject);
        }

        //If the object has a GenericItem script and is valid, hurt the item!
        if (HurtObject.CompareTag("Interactable") && hurtItem)
        {
            hurtable = true;
            HurtItem(HurtObject);
        }

        //This code will only run if we have succesfully met the bounds for hurting something
        if (hurtable)
        {
            //First off, turn off hurtable
            hurtable = false;

            //If we're supposed to work only once, destroy this trigger
            if (destroyAfterUse) Destroy(gameObject);

            //Still going? Cool! We don't get destroyed!
            //Now let's check for tick damage
            if (tickDamage)
            {
                //turn off the trigger's collider...
                GetComponent<Collider>().enabled = false;
                //...and turn it back on after tickRate seconds
                Invoke("turnBackOn", tickRate);
            }
        }
    }

    //Hurt the player
    private void HurtPlayer(GameObject Player)
    {
        //Only hurt if the health is above 0 (prevents damage during death anim)
        if (Player.GetComponent<PlayerHealth>().GetHealth() > 0)
        {
            //Do not run "Take Damage" if this is a healing trigger and the player's HP is max
            if (!((damageAmount < 0) && (Player.GetComponent<PlayerHealth>().GetHealth() == Player.GetComponent<PlayerHealth>().GetMaxHealth())))
            {
                Player.GetComponent<PlayerHealth>().TakeDamage(damageAmount);
                jukebox.PlaySound(0);
            }
            //if the trigger heals and health IS max, turn off hurtable so other functions don't run.
            else hurtable = false;
        }
    }

    private void HurtEnemy(GameObject Enemy)
    {
        //Only hurt if the health is above 0 (prevents damage during death anim)
        if (Enemy.GetComponent<EnemyHealth>().GetHealth() > 0)
        {
            //Do not run "Take Damage" if this is a healing trigger and the player's HP is max
            if (!((damageAmount < 0) && (Enemy.GetComponent<EnemyHealth>().GetHealth() == Enemy.GetComponent<EnemyHealth>().GetMaxHealth())))
            {
                Enemy.GetComponent<EnemyHealth>().TakeDamage(damageAmount);
            }
            //if the trigger heals and health IS max, turn off hurtable so other functions don't run.
            else hurtable = false;
        }
    }

    //Hurt the item
    private void HurtItem(GameObject Item)
    {
       // Debug.Log("HurtItem called on " + Item + "\n");
        //Only hurt if the health is above 0 (prevents damage during death anim)
        if (Item.GetComponent<GenericItem>().GetHealth() > 0)
        {
            //Debug.Log(Item + "'s health is " + Item.GetComponent<GenericItem>().GetHealth());
            //Do not run "Take Damage" if this is a healing trigger and the player's HP is max
            if (!((damageAmount < 0) && (Item.GetComponent<GenericItem>().GetHealth() == Item.GetComponent<GenericItem>().GetMaxHealth())))
            {
                Item.GetComponent<GenericItem>().TakeDamage(damageAmount);
            }
            //if the trigger heals and health IS max, turn off hurtable so other functions don't run.
            else hurtable = false;
        }
    }

    //turn the collider for the trigger back on. Needed for tick damage
    private void TurnBackOn()
    {
        GetComponent<Collider>().enabled = true;
    }
}
