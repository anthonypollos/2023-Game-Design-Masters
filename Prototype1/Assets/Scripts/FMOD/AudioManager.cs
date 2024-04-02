using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set;}

    private EventInstance musicEventInstance;
    private EventInstance ambEventInstance;
    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Found more than one Audio Manager in the scene");
        }
        instance = this;
        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    private void InitializeMusic(EventReference musicEventReference)
    {
        musicEventInstance = CreateInstance(musicEventReference);
        musicEventInstance.start();
    }

    private void InitializeAmbiance(EventReference ambEventReference)
    {
        ambEventInstance = CreateInstance(ambEventReference);
        ambEventInstance.start();
    }

    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        InitializeMusic(FMODEvents.instance.music);
        InitializeAmbiance(FMODEvents.instance.ambiance);
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAmbianceArea(AmbianceArea scene)
    {
        musicEventInstance.setParameterByName("scene", (float) scene);
    }
}
