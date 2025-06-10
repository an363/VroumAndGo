using UnityEngine;
using System.IO;
using System;
using IIR_Butterworth_CS_Library;
using System.Globalization;
public class CarController : MonoBehaviour
{

    // Variables for movement
    [Header("Movement Variables")]
    // private float acceleration = 6f; // Acceleration in m/s^2
    [SerializeField] private float turnSpeed = 0.1f; // Turning speed
    [SerializeField] private CarInFront FrontFront;
    public Player player;
    public float currentSpeed = 0f; // Current speed in m/s
    // private Vector3 currentposition; // accessing the cars current position in the scene
    //Manupulate car sound relative to speed
    // private float maxSpeed = 100f;
    // private float pitch;
    // private AudioSource engine;

    [Header("Wheel Tyres")]

    [SerializeField] private Transform RearLeftWheel;
    [SerializeField] private Transform FrontLeftWheel;
    [SerializeField] private Transform RearRightWheel;
    [SerializeField] private Transform FrontRightWheel;

    [Header("Car Interior Rotations")]
    [SerializeField] private Transform steeringWheel;
    [SerializeField] private Transform speedPointer;
    [SerializeField] private Transform rpmPointer;


    private float moveInput; //Variable to hold up/down key input
    private float turnInput = 0.0f; //Variable to hold horizontql key input


    // Saving car controls
    [Header("Saved car controls")]
    static int N_cells = 100000;// number of saved values in the array
    private float minTimeStep = 10.0f;
    private float[] timeHistory = new float[N_cells]; // array to save the time
    private float[] acceleratorPedal = new float[N_cells]; // array to save the acceleration
    private float[] brakePedal = new float[N_cells]; // array to save the braking
    private float[] xHistory = new float[N_cells]; // array to save the x history
    private float[] yHistory = new float[N_cells]; // array to save the y history
    private float[] carOrientationHistory = new float[N_cells];
    private float[] speedHistory = new float[N_cells];
    private int[] IsTestDelay = new int[N_cells];

    private int cptHistory = 0;

    public int cptTooClose = 0;
    public int cptTooFar = 0;
    public float ratioTooClose, ratioTooFar;
    private const float c1 = 0.3f;
    private const float c2 = 5f;
    private const float closeDistanceTolerance = c1*c2*CAR_LENGTH;
    private const float farDistanceTolerance = (1-c1)*c2*CAR_LENGTH;
    private const float T_CONSTANT = 5;

    private void Start(){
        Application.targetFrameRate=60;
        rpmPointer.localRotation = Quaternion.Euler(0f, 0f, 300f);
        player = GetComponent<Player>();
    }


    private void Update()
    {
        //float accelerationInput = Input.GetAxis("Vertical"); 
        //float brakeInput = Input.GetAxis("Jump"); 

	float accelerationInput = Input.GetAxis("Vertical6");
//        if (accelerationInput == 0f)
//            accelerationInput = -1f;
//            else accelerationInput = Mathf.InverseLerp(0.5f, 1f, accelerationInput);
        //accelerationInput = (accelerationInput + 1f) * 0.5f;
        //accelerationInput = Mathf.InverseLerp(0f, 1f, accelerationInput);
	
	
	
	
	
	
	
	
	
	
        float brakeInput = Input.GetAxis("Jump");

        Debug.Log("Acceleration pedal: "+accelerationInput);

        acceleratorPedal[cptHistory] = accelerationInput;






/*
// List all axis names as defined in Edit > Project Settings > Input Manager
   string[] axisNames = {
        "Horizontal",
        "Vertical",
        "Jump",
        "VerticalY","Vertical3","Vertical4","Vertical5","Vertical6","Vertical7","Vertical8","Vertical9","Vertical10",
        "Vertical11",
        "Vertical12",
        "Vertical13",
        "Vertical14",
        "Vertical15",
        "Vertical16",
        "Vertical17",
        "Vertical18",
        "Vertical19",
        "Vertical20","Vertical21","Vertical22","Vertical23","Vertical24","Vertical26","Vertical27","Vertical28",
        // Add any custom axes you have defined
    };

	foreach (string axis in axisNames)
        {
            float value = Input.GetAxis(axis);
            if (Mathf.Abs(value) > 0.0001f) // Use a small threshold for floating point precision
            {
                Debug.Log($"{axis}: {value}");
            }
        }*/



























        //acceleratorPedal[cptHistory] = Mathf.Clamp((accelerationInput), 0.0f, 1.0f); // Clamp the
        brakePedal[cptHistory] = Mathf.Clamp((brakeInput), 0.0f, 1.0f);//Mathf.Clamp(Input.GetAxis("Jump"), 0.0f, 1.0f);//  		Convert.ToSingle(Input.GetButton("Jump")); // value between 0 and 1 (space bar on the keyboard)
        turnInput = Input.GetAxis("Horizontal"); // Left/Right arrow keys
        minTimeStep = Math.Min(Time.deltaTime, minTimeStep);
        timeHistory[cptHistory] = Time.time;


        // get the rotation angle of the car around z axis wrt to the inital state of the car (t=0)
        float currentCarOrientation = WrapAngleTo180(transform.rotation.eulerAngles.y); // current orientation of the car
        float frontWheelOrientation = Mathf.Clamp(turnInput * 40.0f, -35.0f, 35.0f);//Mathf.Clamp(turnInput * 180.0f, -180.0f, 180.0f); // front wheel orientation in degrees
        
        float[] currentPosition = { transform.position.z, transform.position.x, currentCarOrientation };

        // Update position based on acceleration and input

        float[] newPosition = ComputeNewPosition(currentPosition, frontWheelOrientation);
        
        if (Math.Abs(newPosition[0] - FrontFront.GetCurrentPosition()[2]) > farDistanceTolerance){cptTooFar++;}
        else if (Math.Abs(newPosition[0] - FrontFront.GetCurrentPosition()[2]) < closeDistanceTolerance){cptTooClose++;}
        // Save the car's position and orientation
        xHistory[cptHistory + 1] = newPosition[0];
        yHistory[cptHistory + 1] = newPosition[1];
        carOrientationHistory[cptHistory + 1] = newPosition[2];
        IsTestDelay[cptHistory + 1] = FrontFront.TestReactionTime;

        Vector3 position_xyz = new Vector3(newPosition[1], 0.0f, newPosition[0]);
        // Apply translational movement to the car
        transform.position = position_xyz;

        // Apply rotational movement to the car
        Quaternion newRotation = Quaternion.Euler(0.0f, WrapAngleTo360(newPosition[2]), 0.0f); // the angle should be in degrees
        transform.rotation = newRotation;

        // Apply rotational movement to the front wheels
        UpdateWheelRotations();

        cptHistory++;
        
    }

    // Method to calculate the car's current speed
    public float GetCurrentSpeed(){return currentSpeed;}


    public Vector3 GetCurrentPosition(){return transform.position;}

    // Method to update wheel rotations based on car's speed
    private void UpdateWheelRotations()
    {
        float turnInput = Input.GetAxis("Horizontal"); // Left/Right arrow keys
        float accelerationInput = Input.GetAxis("Vertical"); // Left/Right arrow keys

        //Check if all the wheels are attached in the inspector
        if (RearLeftWheel == null || RearRightWheel == null || FrontLeftWheel == null || FrontRightWheel == null)
        {
            Debug.LogError("Please attache Wheeel elements from the inspector");
        }
        else
        {
            // Rotate the wheels based on the car's speed
            float wheelRotation = turnSpeed * Time.deltaTime * GetCurrentSpeed();//Rotates all the 4 Wheels on one axis and leaves all other axis unchanged

            //Wheel spining during the movement
            RearLeftWheel.Rotate(wheelRotation, 0f, 0f);
            FrontLeftWheel.Rotate(wheelRotation, 0f, 0f);
            RearRightWheel.Rotate(wheelRotation, 0f, 0f);
            FrontRightWheel.Rotate(wheelRotation, 0f, 0f);

            float steeringWheelAngle = Mathf.Clamp(-1 * turnInput * 180.0f, -180.0f, 180.0f);
            float speedometerWheelAngleforward = Mathf.Clamp(-1.0f * GetCurrentSpeed() * 6.0f, -300.0f, 20.0f);
            float speedometerWheelAnglereverse = Mathf.Clamp(GetCurrentSpeed() * 6.0f, -300.0f, 20.0f);

            steeringWheel.localRotation = Quaternion.Euler(0f, 0f, steeringWheelAngle);
            rpmPointer.Rotate(0f, 0f, Time.deltaTime*(0.3f-accelerationInput)*200f);
            Vector3 angle = rpmPointer.localRotation.eulerAngles;
            float anglePointer = Mathf.Clamp(angle.z, 0f, 300f);
            rpmPointer.localRotation = Quaternion.Euler(0f, 0f, anglePointer);

            if (GetCurrentSpeed() >= 0)
            {
                speedPointer.localRotation = Quaternion.Euler(0f, 0f, speedometerWheelAngleforward);
            }
            else if (GetCurrentSpeed() < 0)
            {
                speedPointer.localRotation = Quaternion.Euler(0f, 0f, speedometerWheelAnglereverse);
            }
        }
    }



    // Boolean Variable to state the conditions of the games
    public bool IS_NOISE_REMOVED = true;

    // Physical constants
    //public const float CAR_LENGTH = 4.0f; // length of the car (m)
    //public const float MASS = 1000.0f; // mass of the car (kg)
    //public const float AIR_DENSITY = 1.2f; // air density (kg/m^3)
    //public const float CROSS_SECTIONAL_AREA = 2.5f; // cross-sectional area of the car (m^2)
    //public const float DRAG_COEFFICIENT = 0.3f; // drag coefficient of the car
    //public const float GRAVITY = 9.81f; // acceleration due to gravity (m/s^2)
    //public const float ROLLING_RESISTANCE = 0.2f; // rolling resistance coefficient
    
    
    public const float CAR_LENGTH = 4.0f; // length of the car (m)
    public const float MASS = 1140.0f; // mass of the car (kg)
    public const float a_p = 5.8f;
    public const float a_b = 10.0f;

    public const float GRAVITY = 9.81f; // acceleration due to gravity (m/s^2)
    public const float ROLLING_RESISTANCE = 0.012f; // rolling resistance coefficient
    public const float INTERNAL_FRICTION = 50.0f;
    public const float DRAG_COEFFICIENT = 210f; // drag coefficient

    
    
    
    
    
    
    
    
    

    // Constants for simulation configuration
    public const float sigmoid_threshold = 0.1f; // threshold for the sigmoid function
    public const float alpha = 1000.0f; // alpha parameter for the sigmoid function

    // Constants for the low pass filter
    public const float CUTOFF_TIME = 0.3f; // cutoff time for the low-pass filter (s)
    public const int FILTER_ORDER = 2;
    public const float DURATION = 5.0f; // duration of the noise reduction (s)

    // Random number generator with a fixed seed for reproducibility
    public const int SEED = 10;
    public readonly System.Random RandomGenerator = new(SEED); // Random number generator that will return a random number in [0,1)

    // Compute the magnitude of the car velocity (Newtons second law projected in the direction of the velocity ie cars orientation)
    public float ComputeNewSpeed()
    {
        if (player.delay < 0.0f)
        {
            return ComputeNewSpeedNegativeDelay();
        }

        return ComputeNewSpeedPositiveDelay(); // Delay can be 0
    }

    public float ComputeCurrentTimeStep()
    {
        return timeHistory[cptHistory] - timeHistory[Math.Max(0, cptHistory - 1)];
    }

    // Calculates the new speed based on current state and control inputs
    public float ComputeNewSpeedPositiveDelay()
    {
        int delaySteps = TimeToSteps(player.delay);
        int cptHistory_delayed = Math.Max(0, cptHistory - delaySteps);

        float deterministicForces = DeterministicForces(acceleratorPedal[cptHistory_delayed], brakePedal[cptHistory_delayed], GetCurrentSpeed());
        float currentTimeStep = ComputeCurrentTimeStep();

        float newSpeed = Convert.ToSingle(GetCurrentSpeed() + currentTimeStep * deterministicForces / MASS + Math.Sqrt(currentTimeStep) * StochasticForce(GetCurrentSpeed()));
        newSpeed = Convert.ToSingle(Math.Max(0.0, newSpeed));

        // Removal of the noise
        speedHistory[cptHistory] = newSpeed; 
        // Save the new speed in the history so that it can be used for noise reduction

        int durationSteps = TimeToSteps(DURATION);

        if (IS_NOISE_REMOVED && cptHistory_delayed >= durationSteps)
        {
            Span<float> speedHistoryCut = speedHistory.AsSpan(Math.Max(0, cptHistory_delayed - durationSteps), durationSteps + 1);
            newSpeed = ReduceNoise(speedHistoryCut.ToArray());

            return Math.Max(0.0f, newSpeed);
        }
        // If the noise reduction is not applied, return the new speed
        return Math.Max(0.0f, newSpeed);
    }

    public int TimeToSteps(float duration)
    {
        // Case where the history is empty explicitly handled
        float positive_duration = Math.Abs(duration);
        if (cptHistory == 0)
        {
            return 0;
        }
        int delaySteps = 0;
        float timeToReach = timeHistory[cptHistory] - positive_duration;
        // Handle the case where delay steps exceed the bounds of the timeStepsCut array
        while (delaySteps < cptHistory && timeHistory[cptHistory - delaySteps] > timeToReach)
        {
            ++delaySteps;
        }
        // Ensure the return value does not exceed the length of the array
        return Math.Min(delaySteps, cptHistory);
    }











    // Converts pedal inputs into desired acceleration // http://docenti.ing.unipi.it/guiggiani-m/Michelin_Tire_Rolling_Resistance.pdf
   // public float DeterministicForces(float acceleratorPedalValue, float brakePedalValue, float speed)
   // {
    //    float tractionForce = MASS * 5.0f * acceleratorPedalValue;
      //  float brakeForce = -MASS * 8.7f * brakePedalValue;
        //float dragAirForce =  Convert.ToSingle(-0.5f * AIR_DENSITY * DRAG_COEFFICIENT * CROSS_SECTIONAL_AREA * Math.Pow(speed, 2));
        //float weightForce = MASS * GRAVITY;
       // float rollingResistanceForce = 0.0f; // contact tyre on ground (and internal drag not put)

        //if (speed > 0.0f)
        //rollingResistanceForce = -ROLLING_RESISTANCE * weightForce;

        //return tractionForce + brakeForce + dragAirForce + rollingResistanceForce;
   // }
   
   
   
   // Converts pedal inputs into desired acceleration // http://docenti.ing.unipi.it/guiggiani-m/Michelin_Tire_Rolling_Resistance.pdf
    public float DeterministicForces(float acceleratorPedalValue, float brakePedalValue, float speed)
    {
        float tractionForce = MASS * a_p * acceleratorPedalValue;
        float brakeForce = -MASS * a_b * brakePedalValue;
        float dragAirForce = - DRAG_COEFFICIENT * speed ;
        float weightForce = MASS * GRAVITY;
        float rollingResistanceForce = 0.0f; // contact tyre on ground (and internal drag not put)
        float internalFrictionForce = 0.0f;

        if (speed > 0.0f)
            rollingResistanceForce = -ROLLING_RESISTANCE * weightForce;
            internalFrictionForce = -INTERNAL_FRICTION;

        return tractionForce + brakeForce + dragAirForce + rollingResistanceForce + internalFrictionForce;
    }







    // Generates random noise uniformly distributed in [-NoiseAmplitude, NoiseAmplitude]
    public float StochasticForce(float speed)
    {
        return Convert.ToSingle(player.noise * 2.0f * (RandomGenerator.NextDouble() - 0.5f) * Molifier(speed));
    }


    // Integrate the signal using the trapezoidal rule
    public float IntegrateTrap(float[] signal, float[] timeSteps)
    {
        float integral = 0.0f;

        for (int i = 0; i < signal.Length - 1; i++)
        {
            integral += (signal[i] + signal[i + 1]) * timeSteps[i + 1] / 2.0f;  // Trapezoidal rule
        }

        return integral;
    }

    public float ComputeNewSpeedNegativeDelay()
    {
        int delaySteps = TimeToSteps(player.delay);
		int time_t = cptHistory;
		int time_tmDt = Math.Max(0, cptHistory - 1);

        float acceleratorPedal_tmDt = acceleratorPedal[time_tmDt];
        float acceleratorPedal_t = acceleratorPedal[time_t];
        float brake_pedal_tmDt = brakePedal[time_tmDt];
        float brake_pedal_t = brakePedal[time_t];

        if (acceleratorPedal_tmDt > acceleratorPedal_t)
            acceleratorPedal_tmDt = acceleratorPedal_t;
        if (brake_pedal_tmDt > brake_pedal_t)
            brake_pedal_tmDt = brake_pedal_t;

        float deterministicForces_t = DeterministicForces(acceleratorPedal_t, brake_pedal_t, GetCurrentSpeed());

        float[] predictedForces = new float[delaySteps];
        float[] timeStepsIntegral = new float[delaySteps];

        // loop over delay steps
        for (int i = 0; i < delaySteps; i++)
        {
            float predictedForces_tmDt = DeterministicForces(acceleratorPedal_tmDt, brake_pedal_tmDt, speedHistory[Math.Max(0, time_tmDt - i)]);
            float predictedForces_t =  DeterministicForces(acceleratorPedal_t, brake_pedal_t, speedHistory[Math.Max(0, time_t - i)]);
            predictedForces[i] = predictedForces_t - predictedForces_tmDt;
            timeStepsIntegral[i] = Math.Abs(timeHistory[Math.Max(0, time_t - i)] - timeHistory[Math.Max(0, time_tmDt - i)]);
        }

        // integral of the predicted forces over the delay
        float integralPredictedForces = IntegrateTrap(predictedForces, timeStepsIntegral);
        float currentTimeStep = ComputeCurrentTimeStep();
        float newSpeed = Convert.ToSingle(GetCurrentSpeed() + currentTimeStep * deterministicForces_t / MASS + integralPredictedForces / MASS + Math.Sqrt(currentTimeStep) * StochasticForce(currentSpeed));

        return Math.Max(0.0f, newSpeed);
    }

    // Reduce noise from the signal using a low-pass Butterworth filter zero phase shift
    public float ReduceNoise(float[] signal)
    {
        float NyquistFrequency = 1.0f / (2.0f * minTimeStep); // Nyquist frequency for the low-pass filter
        float normalizedCutoffFrequency = Convert.ToSingle((1.0f / CUTOFF_TIME) / (2.0f * NyquistFrequency)); // = cutoff frequency / (2 * Nyquist frequency)
        if (normalizedCutoffFrequency >= 1.0f)
        {
            normalizedCutoffFrequency = 0.99f;
        }
        if (normalizedCutoffFrequency <= 0.0f)
        {
            normalizedCutoffFrequency = 0.01f;
        }

        // Create an instance of ButterworthFilter
        var coeff = new IIR_Butterworth();

        // Get filter coefficients for the low-pass filter
        double[][] filterCoefficients = coeff.Lp2lp(normalizedCutoffFrequency, FILTER_ORDER);

        // Check if the filter is stable
        bool isStable = coeff.Check_stability_iir(filterCoefficients);
        if (!isStable)
        {
            throw new InvalidOperationException("The filter is unstable! Adjust parameters.");
        }

        // Apply reflection padding to reduce edge effects
        int paddingLength = signal.Length; // Padding length based on filter order
        // convert the signal to double
        double[] signal_double = Array.ConvertAll(signal, item => Convert.ToDouble(item));

        double[] paddedSignal = ApplySymmetricPadding(signal_double, paddingLength);

        // Apply the filter in the forward direction
        double[] forwardFilteredSignal = coeff.Filter_Data(filterCoefficients, paddedSignal);

        // Reverse the signal for backward filtering
        Array.Reverse(forwardFilteredSignal);

        // Apply the filter again in the reverse direction
        double[] backwardFilteredSignal = coeff.Filter_Data(filterCoefficients, forwardFilteredSignal);

        // Reverse the signal back to its original order
        Array.Reverse(backwardFilteredSignal);

        // Remove padding from the filtered signal
        double[] finalFilteredSignal = RemovePadding(backwardFilteredSignal, paddingLength);

        // Return the zero-phase filtered signal
        return Convert.ToSingle(finalFilteredSignal[finalFilteredSignal.Length - 1]);
    }


    /// <summary>
    /// Applies symmetric padding to a given signal array.
    /// The signal is extended at both ends by reflecting its values.
    /// </summary>
    public double[] ApplySymmetricPadding(double[] signal, int paddingLength)
    {
        // Validate input
        if (signal == null || signal.Length == 0)
            throw new ArgumentException("Signal cannot be null or empty.");
        if (paddingLength < 0)
            throw new ArgumentException("Padding length cannot be negative.");

        int n = signal.Length;
        double[] paddedSignal = new double[n + 2 * paddingLength];

        // Symmetrically pad the start of the signal
        for (int i = 0; i < paddingLength; i++)
        {
            paddedSignal[i] = signal[paddingLength - 1 - i]; // Reflect values from the start
        }

        // Copy the original signal
        Array.Copy(signal, 0, paddedSignal, paddingLength, n);

        // Symmetrically pad the end of the signal
        for (int i = 0; i < paddingLength; i++)
        {
            paddedSignal[n + paddingLength + i] = signal[n - 1 - i]; // Reflect values from the end
        }

        return paddedSignal;
    }

    public double[] RemovePadding(double[] paddedSignal, int paddingLength)
    {
        // Validate input
        if (paddedSignal == null || paddedSignal.Length == 0)
            throw new ArgumentException("Padded signal cannot be null or empty.");
        if (paddingLength < 0)
            throw new ArgumentException("Padding length cannot be negative.");
        if (2 * paddingLength >= paddedSignal.Length)
            throw new ArgumentException("Padding length is too large for the given signal.");

        // Calculate the length of the trimmed signal
        int n = paddedSignal.Length - 2 * paddingLength;

        // Create a new array for the trimmed signal
        double[] trimmedSignal = new double[n];

        // Copy the unpadded portion of the signal
        Array.Copy(paddedSignal, paddingLength, trimmedSignal, 0, n);

        return trimmedSignal;
    }

    /// Method to update the state of the car // https://dingyan89.medium.com/simple-understanding-of-kinematic-bicycle-model-81cac6420357 ///
    public float[] ComputeNewPosition(float[] currentPosition, float frontWheelOrientation)
    {
        // Extract current state from the input array
        float xCar = currentPosition[0]; // Current x position
        float yCar = currentPosition[1]; // Current y position
        float speedOrientation = currentPosition[2] * Mathf.Deg2Rad; // Current orientation of the car in degree

        // Convert front wheel orientation to radians (assuming it's normalized between -180 and 180 degrees)
        float frontWheelOrientation_rad = frontWheelOrientation * Mathf.Deg2Rad;

        // Compute derivatives using kinematic bicycle model
        float newSpeed = ComputeNewSpeed();
        currentSpeed = newSpeed;

        float xDot = Convert.ToSingle(newSpeed * Math.Cos(speedOrientation + frontWheelOrientation_rad / 2.0f)); // Small angle approximation
        float yDot = Convert.ToSingle(newSpeed * Math.Sin(speedOrientation + frontWheelOrientation_rad / 2.0f));
        float thetaDot = Convert.ToSingle(newSpeed / CAR_LENGTH * Math.Tan(frontWheelOrientation_rad) * Math.Cos(frontWheelOrientation_rad / 2.0f));

        // Update states using discrete time model
        float currentTimeStep = ComputeCurrentTimeStep();
        xCar += xDot * currentTimeStep;
        yCar += yDot * currentTimeStep;
        speedOrientation += thetaDot * currentTimeStep;
        float[] newPosition = { xCar, yCar, speedOrientation * Mathf.Rad2Deg };

        if (FrontFront.GetCurrentPosition()[2] - 5.0f < newPosition[0])
        {
            newPosition[0] = Mathf.Min(newPosition[0], FrontFront.GetCurrentPosition()[2] - 5.0f);
            newSpeed = FrontFront.GetCurrentSpeed();
            currentSpeed = newSpeed;
        }
        speedHistory[cptHistory] = newSpeed;

        return newPosition;
        // Compute state change rates
        // float lr = carLength / 2.0 ; // distance between the rear wheel and cg
        // float beta = Math.Atan(lr * Math.Tan(frontWheelOrientation_rad) / carLength); // almost  frontWheelOrientation_rad/2.0
    }


    public void SaveTrajectory(string path)
    {   
        bool isdelay = true;
        float t0 = 0;
        int idx_5second = 0;
        int idx_delay = 0;
        float t, x, y, v, acc, brake, theta;
        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.WriteLine("time\tx\ty\tspeed\tacceleration\torientation\tbrake\tReactiontest");
            for (int i = 10; i < cptHistory; i++)
            {
                if (IsTestDelay[i] == 0 && isdelay)
                {
                    t0 = timeHistory[i];
                    isdelay = false;
                    idx_delay = i;
		    idx_5second = i;

                }
                if (!isdelay)
                {
                    if (timeHistory[i] - t0 < T_CONSTANT)
                    {
                        t = timeHistory[i] - t0;
                        x = xHistory[idx_delay] + (timeHistory[i] - t0) * 30f/3.6f;
                        y = 0f;
                        v = 30f/3.6f;
                        acc = 0f;
                        brake = 0f;
                        theta = 0f;
		    	idx_5second++;
                    }
                    else
                    {
                        t = timeHistory[i] - t0;



                        //Debug.Log(i+" "+ idx_5second+ " "+ xHistory[i]+ xHistory[idx_5second-1]);
                        x = xHistory[idx_delay] + xHistory[i] - xHistory[idx_5second] + T_CONSTANT * 30f/3.6f ;
                        y = yHistory[i];
                        v = speedHistory[i];
                        acc = acceleratorPedal[i];
                        brake = brakePedal[i];
                        theta = carOrientationHistory[i];

                    }

                }
                else
                {
                    t = timeHistory[i];
                    x = xHistory[i];
                    y = yHistory[i];
                    v = speedHistory[i];
                    acc = acceleratorPedal[i];
                    brake = brakePedal[i];
                    theta = carOrientationHistory[i];
                    
                }
                writer.WriteLine($"{t.ToString(CultureInfo.InvariantCulture)}\t" +

                $"{x.ToString(CultureInfo.InvariantCulture)}\t" +

                $"{y.ToString(CultureInfo.InvariantCulture)}\t" +

                $"{v.ToString(CultureInfo.InvariantCulture)}\t" +

                $"{acc.ToString(CultureInfo.InvariantCulture)}\t" +

                $"{theta.ToString(CultureInfo.InvariantCulture)}\t" +

                $"{brake.ToString(CultureInfo.InvariantCulture)}\t" +

                $"{IsTestDelay[i].ToString(CultureInfo.InvariantCulture)}");

            }
        }
    }


    public float WrapAngleTo180(float angle)
    {
        // Wrap the angle to the range 0–360 first
        angle = angle % 360.0f;

        // Convert to the range (-180,180)
        if (angle > 180.0f)
        {
            angle -= 360.0f;
        }
        else if (angle < -180.0f)
        {
            angle += 360.0f;
        }

        return angle;
    }

    public float WrapAngleTo360(float angle)
    {
        // Wrap the angle using modulo to handle values outside the range
        angle = angle % 360.0f;

        // If the angle is negative, add 360 to bring it into the range 0–360
        if (angle < 0.0f)
        {
            angle += 360.0f;
        }

        return angle;
    }
    public static double Molifier(double v)
    {
        const double limSup = 708.0;
        double exponent = -alpha * (v - sigmoid_threshold);
        return 1.0 / (1.0 + Math.Exp(Math.Min(limSup, exponent)));
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    

}

