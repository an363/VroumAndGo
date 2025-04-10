using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform car; // Reference to the car's Transform
   // private Vector3 offsetPosition = new Vector3(0, 2, -1); // Offset for the camera position
  //  private Vector3 offsetPosition = new Vector3(-0.09f, 1.0969f, 1f); // Position the camera relative to the car.
    private Vector3 offsetPosition = new Vector3(0f, 0f, 0f); // Position the camera relative to the car.

    private Vector3 offsetRotation = new Vector3(5, 0, 0); // Offset for the camera rotation
    private float smoothSpeed = 5f; // Smoothing factor for camera movement

    // Update is called once per frame
    void LateUpdate()
    {
        if (car != null)
        {
            // Target position for the camera based on the car's position and offset
            Vector3 targetPosition = car.position + car.TransformDirection(offsetPosition);

            // Smoothly interpolate the camera's position
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);

            // Synchronize the rotation with the car
            Quaternion targetRotation = Quaternion.Euler(car.rotation.eulerAngles + offsetRotation);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed);
        }
        else
        {
            Debug.LogError("Car Transform not assigned!");
        }
    }
}
