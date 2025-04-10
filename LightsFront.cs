using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsFront : MonoBehaviour
{
    private CarInFront FrontFront; // Reference to the CarController script

    [Header("Brake Lights Renderes")]
    public Renderer brakelights;
    public Renderer reverselights;
    public Renderer DayLight; 

    [Header("Brake Lights Materials")]
    public Material LightON;
    public Material LightOFF;


    void Start()
    {
    FrontFront = GetComponent<CarInFront>();

    // Make sure brake lights start with the off material
    brakelights.material = LightOFF;
    reverselights.material = LightOFF;
    DayLight.material = LightOFF;
    }

    // Update is called once per frame
    void Update()
    {
        bool brake = FrontFront.IsBraking();
        if (brake)
        {
            brakelights.material = LightON;
            reverselights.material = LightON;
            DayLight.material = LightON;
        }
        else
        {
            brakelights.material = LightOFF;
            reverselights.material = LightOFF;
            DayLight.material = LightOFF;
        }
    }
}
