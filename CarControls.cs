using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarController : MonoBehaviour
{
    // Variables for movement
    [Header("Movement Variables")] 
    private float acceleration = 6f; // Acceleration in m/s^2
    [SerializeField] private float turnSpeed = 0.1f; // Turning speed

    public float currentSpeed = 0f; // Current speed in m/s
    private Vector3 currentposition; // accessing the cars current position in the scene
    private Rigidbody rb;

    //Manupulate car sound relative to speed
    private float maxSpeed = 100f;
    private float pitch;
    private AudioSource engine;

    [Header("Wheel Tyres")]

    [SerializeField] private Transform RearLeftWheel;
    [SerializeField] private Transform FrontLeftWheel;
    [SerializeField] private Transform RearRightWheel;
    [SerializeField] private Transform FrontRightWheel;

    [Header("Car Interior Rotations")]
    [SerializeField] private Transform steeringWheel;
    [SerializeField] private Transform rpmPointer;
    [SerializeField] private Transform speedPointer;
    

    //Braking System Variables
    [Header("Lighting and Braking")]
    public bool brakeLights;
    public bool flashLights;
    [SerializeField] private Transform frontLeftWheelBrakeClipper;
    [SerializeField] private Transform frontRightWheelBrakeClipper;
    public bool reverseLights;

    private float moveInput; //Variable to hold up/down key input
    private float turnInput; //Variable to hold horizontql key input

    [SerializeField] private float maxFrontWheelsRotationngAngle = 45f; //Turning angle for the wheels



      // Ground Check Variables
    [Header("Ground Detection")]
    [SerializeField] private float groundCheckDistance = 1.5f; // Distance for the raycast to check for ground
    [SerializeField] private LayerMask groundLayer; // The ground layer to check against
    private bool isOnGround; // True if the car is grounded


   private  void Start()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody is missing from the car object. Please add one.");
        }
    }






  private  void FixedUpdate()
  {
        // Read input from arrow keys
    
        float moveInput = Input.GetAxis("Vertical"); // Up/Down arrow keys
        float turnInput = Input.GetAxis("Horizontal"); // Left/Right arrow keys
        bool brake = Input.GetButton("Jump");// For breaking, upon pressing the burton reduce to zero velocity



        // Update current speed based on acceleration and input
       if (moveInput > 0)
        {
            currentSpeed += acceleration * Time.deltaTime;
        }
        else if (moveInput < 0)
        {
            currentSpeed -= acceleration * Time.deltaTime;
        }
        else
        {
            // Gradually reduce speed when no input is provided (friction)
            currentSpeed = Mathf.Lerp(currentSpeed, 0f, Time.deltaTime * acceleration * 0.09f);//This determines how fast the speed reduces when the peddle is up
        }

        // Clamp the speed to the maximum allowed speed
        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);

        // Calculate movement and turning
        Vector3 forwardMovement = transform.forward * currentSpeed * Time.deltaTime;

        Vector3 turning = Vector3.up * turnInput * turnSpeed * Time.deltaTime;

        float delterturn = rb.position.y* turnInput * turnSpeed * Time.deltaTime;

        // Apply forward/backward movement
        rb.MovePosition(rb.position + forwardMovement);

        // Apply rotation
       Quaternion turnRotation = Quaternion.Euler(turning);
       rb.MoveRotation(rb.rotation * turnRotation);



       // Add Wheel rotations
        UpdateWheelRotations();







       
      if (brake == true)
        {
            currentSpeed -= Mathf.Pow(acceleration/4,2) * Time.deltaTime;
            currentSpeed = Mathf.Max(currentSpeed, 0f); // Ensure speed doesn't go below zero
        }
    }




     // Method to calculate the car's current speed
    public float GetCurrentSpeed()
    {
        if (rb != null)
        {
            // Calculate the magnitude of the velocity vector to get the speed
            return currentSpeed;//rb.velocity.magnitude; // Convert from meters/second to km/h
        }

        return 0f; // Return 0 if Rigidbody is not found
    }


    public Vector3 GetCurrentPosition()
    {
        Vector3 currentposition = rb.position;

        return currentposition;
    }
    
    
     //Here we access the current player in the game who should set the player details
     
     public Vector3 Player()
    {
        Vector3 currentposition = rb.position;

        return currentposition;
    }



// Method to check if the car is on the ground using a raycast
    private bool IsGrounded()
    {
        // Cast a ray downwards from the car's position to detect the ground
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance, groundLayer))
        {
            return true; // The car is on the ground
        }
        return false; // The car is not on the ground
    }





    // Method to update wheel rotations based on car's speed
    private void UpdateWheelRotations()
    {
        float turnInput = Input.GetAxis("Horizontal"); // Left/Right arrow keys

        //Check if all the wheels are attached in the inspector
        if (RearLeftWheel == null || RearRightWheel == null || FrontLeftWheel == null || FrontRightWheel == null)
        {
            Debug.LogError("Please attache Wheeel elements from the inspector");
        } else {
        
        // Rotate the wheels based on the car's speed
       float wheelRotation = turnSpeed * Time.deltaTime * GetCurrentSpeed();//Rotates all the 4 Wheels on one axis and leaves all other axis unchanged



//Wheel spining during the movement

        RearLeftWheel.Rotate(wheelRotation, 0f, 0f);
        FrontLeftWheel.Rotate(wheelRotation, 0f, 0f);
        RearRightWheel.Rotate(wheelRotation, 0f, 0f);
        FrontRightWheel.Rotate(wheelRotation, 0f, 0f);

        float steeringWheelAngle = Mathf.Clamp(-1*turnInput * 180, -180, 180);
        float speedometerWheelAngleforward = Mathf.Clamp(currentSpeed * -20, -330, -22);
        float speedometerWheelAnglereverse = Mathf.Clamp(-1*currentSpeed * -20, -330, -22);
        float rpmWheelAngle = Mathf.Clamp(moveInput, -300, 0);
        
        
        
        float turnAngleWheels = Mathf.Clamp(turnInput * 45, -45, 45);//wheels rotation angle
        steeringWheel.localRotation = Quaternion.Euler(0f, 0f, steeringWheelAngle);

      
        if (currentSpeed >= 0)
        {
            speedPointer.localRotation = Quaternion.Euler(0f, 0f, speedometerWheelAngleforward);
        } else if (currentSpeed < 0)
        {
            speedPointer.localRotation = Quaternion.Euler(0f, 0f, speedometerWheelAnglereverse);
        }
        }
    }


}
