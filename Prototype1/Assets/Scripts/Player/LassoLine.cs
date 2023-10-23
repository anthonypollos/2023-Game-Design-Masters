using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LassoLine : MonoBehaviour
{
    //LineRenderer lr;
    //List<GameObject> bones;
    //List<GameObject> hingeJoints;
    //[SerializeField] GameObject bone;
    Transform player;
    float maxDistance;
    [SerializeField] Gradient gradient;
    [SerializeField] float distancePerBone;
    [SerializeField] float deleteBuffer = 0.1f;
    [SerializeField] SkinnedMeshRenderer tendrilMaterial;
    // Start is called before the first frame update
    void Awake()
    {
        //Debug.Log("Awake");
        player = null;
        maxDistance = 0;
        //bones = new List<GameObject>();
        //hingeJoints = new List<GameObject>();
        //lr = GetComponent<LineRenderer>();
        //lr.enabled = false;
    }

    public void SetValues(Transform player, float maxDistance)
    {
        this.player = player;
        this.maxDistance = maxDistance;
        //lr.enabled = true;
        Color color = gradient.Evaluate(GetDistance() / maxDistance);
        //lr.startColor = color;
        //lr.endColor = color;
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            //lr.positionCount = 2;
            //lr.SetPosition(0, transform.parent.position);
            //lr.SetPosition(1, player.position);
            Color color = gradient.Evaluate(GetDistance() / maxDistance);
            tendrilMaterial.material.color = color;
            //lr.startColor = color;
            //lr.endColor = color;
            /*if (bones.Count == 0)
             {
                 Vector3 dir = (player.position - transform.position).normalized;
                 NewBone(dir);
             }

             if (bones.Count >= 1)
             {
                 Vector3 previous = bones.Count == 1 ? transform.position : bones[bones.Count - 2].transform.position;
                 GameObject current = bones[bones.Count - 1];
                 float distance = Vector3.Distance(previous, player.position);
                 distance = Mathf.Clamp(distance, 0, distancePerBone);
                 //Debug.Log(distance);
                 current.GetComponent<CharacterJoint>().anchor = new Vector3(0, distance / 2, 0);
                 Transform parent = current.transform.parent;
                 current.transform.parent = null;
                 current.transform.localScale = new Vector3(
                     current.transform.localScale.x,
                     distance,
                     current.transform.localScale.z);
                 current.transform.parent = parent;
                 if (distance >= distancePerBone)
                 {
                     NewBone((player.position - current.transform.position).normalized);
                 }
                 else if (distance < deleteBuffer)
                 {
                     Debug.Log("DeleteBone");
                     bones.Remove(current);
                     Destroy(current);
                 }
             }


             lr.positionCount = bones.Count + 2;

             lr.SetPosition(0, transform.position);
             for (int i = 0; i < bones.Count; i++)
             {
                 Vector3 previous = i == 0 ? transform.position : bones[i - 1].transform.position;
                 Vector3 current = bones[i].transform.position;
                 //bones[i].transform.up = (previous - current).normalized;
                 lr.SetPosition(i + 1, current);
             }
             lr.SetPosition(lr.positionCount-1, player.position);

             Color color = gradient.Evaluate(GetDistance() / maxDistance);
             lr.startColor = color;
             lr.endColor = color;
             */
        }
    }

    private void NewBone(Vector3 dir)
    {
        //GameObject newBone = Instantiate(bone, player.position + dir * deleteBuffer, Quaternion.identity);
       // bones.Add(newBone);
        //Rigidbody rb = bones.Count == 1 ? GetComponent<Rigidbody>() : bones[bones.Count - 2].GetComponent<Rigidbody>();
        //rb.isKinematic = false;
        //newBone.GetComponent<CharacterJoint>().connectedBody = rb;
        //newBone.transform.parent = rb.transform;
        //newBone.transform.up = dir;
    }

    public float GetDistance()
    {
        return Vector3.Distance(player.position, transform.parent.position);
    }

    private void OnDestroy()
    {
        //foreach (GameObject temp in bones)
            //Destroy(temp);
    }

    public Gradient GetGradient()
    {
        return gradient;
    }
}
