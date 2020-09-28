using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioFootsteps : MonoBehaviour
{
    #region PUBLIC MEMBERS

    public string[] MaterialTypes;
    [HideInInspector] public int DefaultMaterialValue;
    public PlayerController Player;
    
    #endregion

    #region PRIVATE MEMBERS

    [Header("FMOD Settings")]
    [SerializeField, EventRef] private string footstepsEventPath = null;    
    [SerializeField, EventRef] private string jumpingEventPath = null;    
    
    [Space, SerializeField] private string materialParameterName = null;                      
    [SerializeField] private string speedParameterName = null;                         
    [SerializeField] private string jumpOrLandParameterName = null;  
    
    [Header("Footstep Settings")]
    [SerializeField] private float runningStepDistance = 2.0f; 
    [SerializeField] private float walkingStepDistance = 1.65f; 
    private float stepDistance = 2.0f;   
    
    private const float RAY_DISTANCE = 0.11f;                          
    private const float START_RUNNING_TIME = 0.3f;                     
    private const string JUMP_INPUT_NAME = "Jump";                              

    //These variables are used to control when the player executes a footstep.
    //private float StepRandom;                                                   
    private Vector3 prevPos;                                                    
    private float distanceTravelled; 
    
    //These variables are used when checking the Material type the player is on top of.
    private RaycastHit hit;                                                     
    private int fMaterialValue;     
    
    //These booleans will hold values that tell us if the player is touching the ground currently and if they were touching it during the last frame.
    private bool previosulyTouchingGround;     
    
    //These floats help us determine when the player should be running.
    private float timeTakenSinceStep;                                           
    private int fPlayerRunning;
    
    #endregion

    #region MONOBEHAVIOUR METHODS

    private void Start() 
    {
        // 'PrevPos' now holds the location that the player is starting at as co-ordinates (x, y, z).
        prevPos = Player.transform.position;               
    }

    private void Update()
    {

        stepDistance = Input.GetKey(KeyCode.LeftShift) ? runningStepDistance : walkingStepDistance;

        if (Player.IsGrounded() && Input.GetButtonDown(JUMP_INPUT_NAME))    
        {
            MaterialCheck();                                               
            PlayJumpOrLand(true);
        }
        
        if (!previosulyTouchingGround && Player.IsGrounded())             
        {
            MaterialCheck();                                               
            PlayJumpOrLand(false);
        }

        previosulyTouchingGround = Player.IsGrounded(); 


        //This section determines when and how the PlayFootstep method should be told to trigger, thus playing the footstep event in FMOD.
        timeTakenSinceStep += Time.deltaTime;
        distanceTravelled += (Player.transform.position - prevPos).magnitude;

        if (distanceTravelled >= stepDistance / 10 )          
        {
            MaterialCheck();                                                 
            SpeedCheck();                                                    

            PlayFootstep();
            distanceTravelled = 0;
        }

        prevPos = Player.transform.position; 
    }

    #endregion

    #region CUSTOM METHODS

    private void MaterialCheck() 
    {
        
        if (Physics.Raycast(Player.transform.position, Vector3.down, out hit, RAY_DISTANCE))
        {
            fMaterialValue = hit.collider.gameObject.GetComponent<AudioMaterialSetter>() ? 
                hit.collider.gameObject.GetComponent<AudioMaterialSetter>().materialValue : DefaultMaterialValue;
        }
        else                                                                                                        
        {
            fMaterialValue = DefaultMaterialValue;                                                                  
        }
    }

    private void SpeedCheck() 
    {
        if (timeTakenSinceStep < START_RUNNING_TIME)
        {                     
            fPlayerRunning = 1;                                    
        }
        else
        {                                                          
            fPlayerRunning = 0;                                    
            timeTakenSinceStep = 0;
        }
    }

    private void PlayFootstep() 
    {
        if (!Player.IsGrounded()) return;

        EventInstance Footstep = RuntimeManager.CreateInstance(footstepsEventPath);        
        RuntimeManager.AttachInstanceToGameObject(Footstep, Player.transform, GetComponent<Rigidbody>());
        Footstep.setParameterByName(materialParameterName, fMaterialValue);                                     
        Footstep.setParameterByName(speedParameterName, fPlayerRunning);                                        
        Footstep.start();                                                                                       
        Footstep.release();                                                                                     
    }

    private void PlayJumpOrLand(bool fJumpLandCalc) 
    {
        EventInstance jumpLand = RuntimeManager.CreateInstance(jumpingEventPath);        
        RuntimeManager.AttachInstanceToGameObject(jumpLand, Player.transform, GetComponent<Rigidbody>());    
        jumpLand.setParameterByName(materialParameterName, fMaterialValue);                                    
        jumpLand.setParameterByName(jumpOrLandParameterName, fJumpLandCalc ? 0f : 1f);                         
        jumpLand.start();                                                                                      
        jumpLand.release();                                                                                    
    }

    #endregion
}

