using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class ExplosionBehavior : MonoBehaviour
{
    [SerializeField] int dmg;
    [SerializeField] int Playerdmg;
    [SerializeField] float explosiveForce;
    [SerializeField] float explosiveCarryDistance;
    [SerializeField] float fireTime = 4f;
    [SerializeField] LayerMask layerMask;
    List<GameObject> hit;
    ParticleSystem[] particleSystems;

    //[Header("Sound")]
    //[SerializeField] private JukeBox jukebox;
    [SerializeField] private EventReference explosionSound;

    private void Awake()
    {
        //jukebox.SetTransform(transform);
    }
    private void Start()
    {
        //jukebox.PlaySound(0);
        AudioManager.instance.PlayOneShot(explosionSound, this.transform.position);
        StartCoroutine(DestroySelf());
        hit = new List<GameObject>();
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }
    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(0.1f);
        GetComponent<Collider>().enabled = false;
    }

    private void Update()
    {
        bool stillPlaying = false;
        foreach(ParticleSystem ps in  particleSystems)
        {
            stillPlaying = ps.isPlaying;
        }
        if (!stillPlaying)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        GameObject entity = other.gameObject;
        bool visible = CanSee(other.transform);

        if (visible && !hit.Contains(entity))
        {
            hit.Add(entity);
            IDamageable damaged = entity.GetComponent<IDamageable>();
            if (damaged != null)
            {
                if (entity.gameObject.CompareTag("Player"))
                {
                    damaged.TakeDamage(Playerdmg, DamageTypes.EXPLOSION);
                }

                else
                {
                    damaged.TakeDamage(dmg, DamageTypes.EXPLOSION);
                }

            }
            Moveable moveable = entity.GetComponent<Moveable>();
            if (moveable != null)
            {
                Vector3 dir = (entity.transform.position - transform.position);
                dir.y = 0;

                moveable.Launched(dir.normalized * explosiveCarryDistance , explosiveForce);

            }
            IKickable kicked = entity.GetComponent<IKickable>();
            if (kicked != null) kicked.Kicked();

            Flammable flammable = entity.GetComponent<Flammable>();
            if(flammable != null)
            {
                flammable.Activate(fireTime);
            }
        }
    }

    private bool CanSee(Transform target)
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position, target.position - transform.position, Color.red);
        if (Physics.Raycast(transform.position, target.position - transform.position, out hit, Mathf.Infinity, layerMask))
        {
            //Debug.Log(hit.transform.name);
            if (hit.transform == target)
            {
                return true;
            }
            else
            {
                //Debug.Log(hit.transform.name);
                return false;
            }
        }
        //Debug.Log("no hit");
        return false;
    }
}
