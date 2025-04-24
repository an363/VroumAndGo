using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineSounds : MonoBehaviour
{
    public AudioSource engineSound; // Reference to the AudioSource component for engine sounds
    public AudioSource klaxon; // Reference to the AudioSource component for engine sounds
    public AudioClip[] engineClips; // Array of engine sound clips (idle, low, medium, high, max) 
    // engineClips[3]: horn.wav
    // engineClips[4]: horn2.wav

    //Sound volume relative to speed
    private float maxPitch = 3f; // Maximum pitch when the car is at max speed
    private float maxVolume = 1f; // Maximum volume when the car is at max speed
    private float minPitch = 1f; // Minimum pitch for idle
    private float minVolume = 0.3f; // Minimum volume for idle
    private float maxSpeed = 100f; // Minimum volume for idle
    private float pitch;
    private float pitchscalar; //Variable to control the pitch of the sound
    private float pitchscalarsmoothed;
    private float time_nextPossibleHorn= -1f; // there will be no horn before Time.time>=time_nextPossibleHorn
    
    private CarController carController; // Reference to the CarController script
    private CarBehind carBehind; //!   Reference to the CarBehind script
    
    [SerializeField] private Transform carBehind_ser; //!

    //private AudioClip currentClip; // Track the currently playing clip

    void Start()
    {
        // Find and assign the CarController script
        carController = GetComponent<CarController>();
	    carBehind=carBehind_ser.GetComponent<CarBehind>(); //!
	   
        if (engineSound != null && engineClips.Length > 0)
        {
            AssignEngineClip(engineClips[0]); //Start Engine
            engineSound.loop = false;

            //Play Idle sound emmediately after the start sound
            engineSound.Play();
            // Schedule the idle sound to play after the start sound finishes
            StartCoroutine(PlayIdleSoundAfterStart());
        }
        else
        {
            Debug.LogError("Engine sound or clips are not assigned!");
        }
    }

    void Update()
    {
    	
    	float pitchscalar = Mathf.Clamp01(Input.GetAxis("Vertical")); // Up/Down arrow keys
    	//Debug.Log($"pitchscalar: {pitchscalar}");
        //Variables
        
        
        if (carController != null && engineClips.Length >= 1 && engineSound != null)
        {
        	float speed = Mathf.Abs(carController.GetCurrentSpeed())*3.6f; // Access the current speed of the car
            //Variables to manipulate the sound volume


            if (speed>0)
            {
		       AssignEngineClip(engineClips[2]); // Idle sound
		       engineSound.loop = true;
		       pitch = Mathf.Lerp(0, 3, speed / maxSpeed);
		       if (pitchscalar >= 0.4f){engineSound.pitch = pitch*Mathf.Sqrt(pitchscalar);} else {engineSound.pitch = pitch*0.4f;}
            }// else {AssignEngineClip(engineClips[1];}
            
            //! HONK IF NEEDED
            float gap_behind= carController.GetCurrentPosition().z - carBehind.GetCurrentPosition().z - 6f; //! distance between leading car and car behind minus 5 metres (the car length)
     	  if ( (Time.time > time_nextPossibleHorn) && (speed<35f) && (gap_behind<3f)) //! if time of possible next horn has passed speed is less than 35km/h and gap is less than 3m
     	  {
     	  //! Decide if the car behind will honk
     	  double aleas = carController.RandomGenerator.NextDouble(); //! between 0 and 1
     	  if (aleas < 0.20 * Mathf.Exp(-gap_behind/3f)) // prefactor such that at 20fps the sound may be played every second
     	  	{
     	  	time_nextPossibleHorn += 3.0f * (float)carController.RandomGenerator.NextDouble(); // no more than every 3 s
     	  	if (carController.RandomGenerator.NextDouble()>0.5)
	     	  	AssignKlaxon(engineClips[3]); //! horn 1
	     	else
	     		AssignKlaxon(engineClips[4]); //! horn 2
		     engineSound.loop = false;
     	  	} 
     	  }
     	   //! END OF HONK SECTION
        }
        else
        {
            Debug.LogError("CarController or engine clips are not properly assigned!");
        }
        
   
     
    }

    private void AssignEngineClip(AudioClip clip)
    {
        if (engineSound.clip != clip) // Only change clip if it's different
        {
            engineSound.clip = clip;
            engineSound.Play();
        }
    }
    private void AssignKlaxon(AudioClip clip)
    {
        if (klaxon.clip != clip) // Only change clip if it's different
        {
            klaxon.clip = clip;
            klaxon.Play();
        }
    }


   private IEnumerator PlayIdleSoundAfterStart()
    {
        // Wait for the engine start sound to finish
       while (engineSound.isPlaying)
        {
            yield return null;
        }

        // Play the idle sound continuously
        engineSound.clip = engineClips[1]; // Idle engine sound
        engineSound.loop = true; // Loop the idle sound
        engineSound.Play();
    }
}
