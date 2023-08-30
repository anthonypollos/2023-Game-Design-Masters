using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] Material[] materials;
    Material debugStartingMaterial;
    MeshRenderer mr;
    bool moved;
    // Start is called before the first frame update
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        moved = false;
        mr.material = materials[0];
    }

    public void Lassoed()
    {
        Stunned();
        //Insert lassoed animation
    }

    public void Pulled()
    {
        if(!moved)
            StartCoroutine(Moved());
        //Insert pulled animation
    }

    public void Kicked()
    {
        if (!moved)
            StartCoroutine(Moved());
        //Insert damage here
        //Insert kicked animation here
    }


    private void Stunned()
    {
        mr.material = materials[1];
        //insert stunned code here
    }
    private void UnStunned()
    {
        Debug.Log("change material back");
        mr.material = materials[0];
        //insert unstunned code here
    }
    private IEnumerator Moved()
    {
        Stunned();
        yield return new WaitForSeconds(0.1f);
        moved = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Moved: " + moved + "\ncollision tag: " + collision.gameObject.tag);
        if(moved && collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Unstun");
            moved = false;
            UnStunned();
        }
    }



    
}
