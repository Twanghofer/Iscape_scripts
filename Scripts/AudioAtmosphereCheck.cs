using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioAtmosphereCheck : MonoBehaviour
{
    #region PUBLIC MEMBERS

    #endregion

    #region PRIVATE MEMEBERS
    [SerializeField] private bool playAtmo = false;
    [SerializeField] private Interactable interactable = null;
    [SerializeField, EventRef] private string atmosphereSoundEventPath = null;
    [SerializeField] private StudioEventEmitter[] emittersInRoom = null;

    private bool coolerOn = true;
    private bool scriptObjectIsCooler;
    private EventInstance atmosphereSound;

    private const string ATMO_PARAMETER_NAME = "AtmosphereStatus";
    private const int ATMO_ON = 1;
    private const int ATMO_OFF = 0;

    #endregion

    #region MONOBEHAVIOUR METHODS

    private void Awake()
    {
        scriptObjectIsCooler = gameObject.CompareTag(PlayerCalculations.COOLINGROOM_TRIGGER);
    }

    private void Start()
    {
        atmosphereSound = RuntimeManager.CreateInstance(atmosphereSoundEventPath);
        atmosphereSound.setParameterByName(ATMO_PARAMETER_NAME, ATMO_ON);

        if (scriptObjectIsCooler)
        {
            PlayAtmoshpereSound();
        }
    }

    private void Update()
    {
        if (!scriptObjectIsCooler) return;

        if (interactable.stopSound)
        {
            StopAtmoshpereSound();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(PlayerCalculations.PLAYER_TAG)) return;

        StartEmitters();

        atmosphereSound.getPlaybackState(out PLAYBACK_STATE pbState);
        if (pbState != PLAYBACK_STATE.PLAYING && playAtmo)
        {
            PlayAtmoshpereSound();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag(PlayerCalculations.PLAYER_TAG)) return;

        StopEmitters();
        StopAtmoshpereSound();
    }

    #endregion

    #region CUSTOM METHODS

    private void PlayAtmoshpereSound()
    {
        if (scriptObjectIsCooler)
        {
            if (coolerOn)
            { 
                atmosphereSound.start();
            }
        }
        else
        {
            atmosphereSound.setParameterByName(ATMO_PARAMETER_NAME, ATMO_ON);
            atmosphereSound.start();
        }
    }

    private void StopAtmoshpereSound()
    {
        if (scriptObjectIsCooler)
        {
            atmosphereSound.setParameterByName(ATMO_PARAMETER_NAME, ATMO_OFF);

            if (coolerOn)
            {
                coolerOn = false;
                interactable.stopSound = false;
            }
        }
        else
        {
            atmosphereSound.setParameterByName(ATMO_PARAMETER_NAME, ATMO_OFF);
        }
    }

    private void StartEmitters()
    {
        foreach (StudioEventEmitter emitter in emittersInRoom)
        {
            emitter.Play();
        }
    }

    private void StopEmitters()
    {
        foreach (StudioEventEmitter emitter in emittersInRoom)
        {
            emitter.Stop();
        }
    }

    #endregion
}
