using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioCharacterMovement : MonoBehaviour
{
    #region PUBLIC MEMBERS

    #endregion

    #region PRIVATE MEMBERS

    [SerializeField] private PlayerAggregation playerAggregation = null;
    [SerializeField] private AudioFootsteps solidMovement;

    [SerializeField, EventRef] private string liquidMovementEventPath = null;
    [SerializeField, EventRef] private string gasMovementEventPath = null;
    [SerializeField, EventRef] private string solidTransformationEventPath = null;
    [SerializeField] private bool playLiquidImpact = false;
    [SerializeField, EventRef] private string liquidImpactEventPath = null;

    private const string MOVEMENT_PARAMETER_NAME = "Status";

    private bool switchedToLiquid;
    private bool switchedToGas;
    private bool switchedToSolid = true;
    private bool previosulyTouchingGround;

    [SerializeField]private PlayerController player = null;

    private EventInstance liquidSound;
    private EventInstance gasSound;
    private EventInstance solidTransformationSound;

    #endregion

    #region MONOBEHAVIOUR METHODS

    private void Start()
    {
        solidMovement = GetComponent<AudioFootsteps>();
    }

    private void Update()
    {
        HandlePreperationForStates();

        if (!playLiquidImpact) return;
        if (!previosulyTouchingGround && player.IsGrounded())
        {
            PlayLiquidImpact();
        }

        previosulyTouchingGround = player.IsGrounded();
    }
    #endregion

    #region CUSTOM METHODS

    private void HandlePreperationForStates()
    {
        switch (playerAggregation.CurrentStateOfAggregation)
        {
            case StateOfAggregation.SolidState:
                PrepSolidState();
                break;
            case StateOfAggregation.LiquidState:
                PrepLiquidState();
                break;
            case StateOfAggregation.GasState:
                PrepGasState();
                break;
            default:
                Debug.LogError("StateOfAggregation not set!");
                break;
        }
    }

    private void PrepSolidState()
    {
        if (switchedToSolid) return;

        if (!switchedToSolid)
        { 
            PlaySolidTransformation();
        }

        StopAllEmitters();

        solidMovement.enabled = true;
            
        switchedToSolid = true;
        switchedToLiquid = false;
        switchedToGas = false;
    }

    private void PrepLiquidState()
    {
        if (switchedToLiquid) return;

        StopAllEmitters();

        solidMovement.enabled = false;
            
        switchedToSolid = false;
        switchedToLiquid = true;
        switchedToGas = false;

        PlayLiquid();
    }

    private void PrepGasState()
    {
        if (switchedToGas) return;

        StopAllEmitters();

        solidMovement.enabled = false;
            
        switchedToSolid = false;
        switchedToLiquid = false;
        switchedToGas = true;

        PlayGas();
    }

    private void StopAllEmitters()
    {
        liquidSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        gasSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    private void PlayLiquid()
    {
        liquidSound = RuntimeManager.CreateInstance(liquidMovementEventPath);
        liquidSound.setParameterByName(MOVEMENT_PARAMETER_NAME, 0);
        liquidSound.start();                                                                                      
    }

    private void PlayGas()
    {
        gasSound = RuntimeManager.CreateInstance(gasMovementEventPath);
        gasSound.setParameterByName(MOVEMENT_PARAMETER_NAME, 0);
        gasSound.start();
    }

    private void PlaySolidTransformation()
    {
        solidTransformationSound = RuntimeManager.CreateInstance(solidTransformationEventPath);
        solidTransformationSound.setParameterByName(MOVEMENT_PARAMETER_NAME, 0);
        solidTransformationSound.start();
    }

    private void PlayLiquidImpact()
    {
        if (switchedToLiquid)
        {
            EventInstance liquidImpactSound = RuntimeManager.CreateInstance(liquidImpactEventPath);
            RuntimeManager.AttachInstanceToGameObject(liquidImpactSound, transform, GetComponent<Rigidbody>());
            liquidImpactSound.start();
            liquidImpactSound.release();
        }
    }

    #endregion
}