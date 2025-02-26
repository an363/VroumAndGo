using System;
using UnityEngine;

public class CarBehind : MonoBehaviour
{
    // Constants
    public const sbyte l0 = 10;
    public const float v0 = 30.0f / 3.6f;
    public const float Tmin = 0.5f;
    public const float tau = 1.0f;

    // Car properties
    private float speedBehind, TBehind, zBehind;

    // Reference to the leading car (assigned from Inspector)
    private CarController carController; // Reference to the CarController script
    private CarFront CarFront; // Reference to the CarController script
   // [SerializeField] private GameObject carObject; // Reference to the GameObject

    void Start()
    {
        zBehind = transform.position;
        carController = leadingCar.GetComponent<CarController>();
        nextSpeed = 0.0f;
        nextT = 1.0f;
    }



    void Update()
    {   
        // Get the speed of the controller car
        float controllerCarSpeed = carController.GetCurrentSpeed();
        // Get the current position of the car in front and the controller's car
        Vector3 controllerCarPosition = carController.GetCurrentPosition();
        Vector3 frontCarPosition = CarFront.GetCurrentPosition();

        // Determine the next position, speed, and time gap for the following car
        (zBehind, speedBehind, TBehind) = DetermineTrajectory.F(controllerCarPosition.z, controllerCarSpeed, frontCarPosition.z, zBehind, speedBehind, TBehind);
        transform.position = Vector3(0.0f, 0.0f, zBehind);
    }



    // Trajectory calculation class
    public static class DetermineTrajectory
    {
        // Compute the minimum time gap based on the distance between cars
        public static float T0(float deltaX_n)
        {
            return Mathf.Max(Tmin, 
                             2.0f * l0 / v0 - deltaX_n * 
                             (1f / v0 - Tmin / (2.0f * l0)));
        }

        // Function to calculate the next position, speed, and time step for the car behind
        public static (float, float, float) F(float x_n, float v_n, float x_nMinus1, float x, float v, float T)
        {
            // Update the position
            x = v * Time.deltaTime + x;

            // Update the time step based on the difference between the leading car and current car
            T = T + Time.deltaTime * (1.0f / tau) * (T0(x_nMinus1 - x_n) - T);
            T = Mathf.Max(Tmin, T);  // Ensure the time gap doesn't go below the minimum threshold

            // Update the car's position based on the speed
            x_n = x_n + v_n * Time.deltaTime;

            // Calculate the new speed based on the updated position
            v = (x_n - x) / T;

            return (x, v, T);
        }
    }
}
