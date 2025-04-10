using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    public AudioSource drivingSound; // Reference to the AudioSource component
    private CarController carController; // Reference to the CarController script

    void Update()
    {
        if (carController != null && drivingSound != null)
        {
            drivingSound.Play(); //play the background sound while drivimg
        }
    }
}