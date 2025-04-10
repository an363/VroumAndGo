using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelElements : MonoBehaviour
{
    // List to store all WheelElements (this will be visible in the Inspector)
    // Wheel element references

    [Header("Wheel Elements")]
    [SerializeField] private Transform steeringWheel;
    //[SerializeField] private Transform Wheel;
private void FixedUpdate()
{
    float turnInput = Input.GetAxis("Horizontal");
    float turnAngle = Mathf.Clamp(-1*turnInput * 180, -180, 180);
    float turnAngleWheels = Mathf.Clamp(-1*turnInput * 45, -45, 45);

    steeringWheel.localRotation = Quaternion.Euler(0f, 0f, turnAngle);
    //Wheel.localRotation = Quaternion.Euler(0f, turnAngleWheels, 0f);

}




}
