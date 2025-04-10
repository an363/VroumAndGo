using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;


public class CarInFront : MonoBehaviour
{
    private int currentIndex = 1; // To track the current data point we're using from the CSV
     
    // Lists to load the variables of the previous car
    private List<float> times = new List<float>();
    private List<float> zValues = new List<float>(); 
    private List<float> xValues = new List<float>();
    private List<float> orientationValues = new List<float>();
    private List<float> brakePedal = new List<float>();

    private float currentzValue, currentxValue;
    private float currentTime;
    private string PathFile;

    // References to the active car and pre computed trajectories 
    [SerializeField] private Transform PlayerCar; 
    [SerializeField] private TextAsset first_run;
    [SerializeField] private TextAsset first_run_delay;

    private Player Player;
    private bool FrontIsPlayer = false;

    // Store if the reaction test is playing (1) or the real experiment (0)
    public int TestReactionTime = 1;
    private float StartTime = 0;

    // Maximal time length of the experiment
    private float Tmax = 120f;

    private int idx_5second = 0;

    void Start()
    {
        Player = PlayerCar.GetComponent<Player>();       
        // Load the delay test trajectory for the car in front
        ReadDataFromTestAsset(first_run_delay, times, zValues, xValues, orientationValues);
    } 

   void Update()
    {
        currentTime = Time.time - StartTime;
        // Find the index of the load text that corresponds to the real time
        while (currentIndex < times.Count && times[currentIndex] <= currentTime)
        {
            ++currentIndex;
        }
        
        // Check if we reach the end of the loaded trajectory or if we reach the maximal time of simulation
        // to load the next trajectory or save the data and close the app
        if (currentIndex >= times.Count || (TestReactionTime == 0 && (currentTime > Tmax || currentIndex >= times.Count)))
        {
            // The loaded trajectory was the reaction time test
            if (TestReactionTime ==1)
            {
                times = new List<float>();
                zValues = new List<float>();
                xValues = new List<float>();
                orientationValues = new List<float>();
                // If this is the first player we load the automatic trajectory
                if (Player.last_ID == 0)
                {
                    ReadDataFromTestAsset(first_run, times, zValues, xValues, orientationValues);
                }
                // Else we load the trajectory of the previous player
                else
                {
                    PathFile = Path.Combine(Application.persistentDataPath, "DATA", Player.last_ID + ".txt");
                    ReadDataFromFile(PathFile, times, zValues, xValues, brakePedal, orientationValues);
                    FrontIsPlayer = true;
                    while (times[idx_5second] <5){idx_5second++;}
                }
                // Reset the position of the active player to the 10 m behind the previous player
                PlayerCar.position = new Vector3(0, 0, zValues[0]-1/Player.density);
                PlayerCar.rotation = Quaternion.Euler(0, 0, 0);
                PlayerCar.GetComponent<CarController>().currentSpeed = 30f/3.6f;
                currentIndex = 1;
                StartTime = Time.time;
                TestReactionTime = 0;
                currentTime = Time.time - StartTime;
                
                }
            // It was the actual experiment and we save (and/or validate) the data 
            else
            {
                PlayerCar.GetComponent<CarController>().SaveTrajectory(Player.File_saving);
                float ratioTooClose = PlayerCar.GetComponent<CarController>().ratioTooClose;
                float ratioTooFar = PlayerCar.GetComponent<CarController>().ratioTooFar;
                Player.ValidateRun(ratioTooClose, ratioTooFar);
                #if UNITY_EDITOR
                                UnityEditor.EditorApplication.isPlaying = false;
                #else
                                Application.Quit();
                #endif
            }
        }

        // Interpolate the position between the two discretized time steps
        float alpha;
        if (times[currentIndex] == times[currentIndex-1]){alpha = 0f;}
        else{alpha = (currentTime - times[currentIndex-1])/(times[currentIndex]-times[currentIndex-1]);}

        float currentzValue = zValues[currentIndex-1] + alpha*(zValues[currentIndex]- zValues[currentIndex-1]);
        float currentxValue = xValues[currentIndex-1] + alpha*(xValues[currentIndex]- xValues[currentIndex-1]);
        // Update the position and orientation of the car in front

        transform.position = new Vector3(currentxValue, 0f, currentzValue); 
        transform.rotation = Quaternion.Euler(0.0f, orientationValues[currentIndex], 0.0f); // the angle should be in degrees
    }

    void ReadDataFromFile(string PathFile, List<float> times, List<float> zvalues, List<float> xvalues, List<float> brakePedal, List<float> orientationValues)
    {
        // Get the text content from the TextAsset
        string[] lines = File.ReadAllLines(PathFile);
        bool isdelay = true;
        float t0 = 0;
        foreach (string line in lines.Skip(1))
        {
            // Trim any excess spaces and split the line by space or tab
            string[] data = line.Trim().Split("\t");  // delimiter if necessary
            if (data.Length >= 7 && int.Parse(data[7]) == 0)
            {
                if (isdelay)
                {
                    t0 = float.Parse(data[0], CultureInfo.InvariantCulture);
                    Debug.Log(t0);
                    isdelay = false;
                } 
                times.Add(float.Parse(data[0], CultureInfo.InvariantCulture) - t0);
                zvalues.Add(float.Parse(data[1], CultureInfo.InvariantCulture));
                xvalues.Add(float.Parse(data[2], CultureInfo.InvariantCulture));
                orientationValues.Add(float.Parse(data[5], CultureInfo.InvariantCulture));
                brakePedal.Add(float.Parse(data[6], CultureInfo.InvariantCulture));
            }
        }
    }


    void ReadDataFromTestAsset(TextAsset file, List<float> times, List<float> zvalues, List<float> xvalues, List<float> orientationValues)
    {
        // Get the text content from the TextAsset
        string[] lines = file.text.Split('\n');

        foreach (string line in lines)
        {
            // Trim any excess spaces and split the line by space or tab
            string[] data = line.Trim().Split(",");  // delimiter if necessary

            if (data.Length >= 3)
            {
                times.Add(float.Parse(data[0], CultureInfo.InvariantCulture));
                zvalues.Add(float.Parse(data[1], CultureInfo.InvariantCulture));
                xvalues.Add(float.Parse(data[2], CultureInfo.InvariantCulture));
                orientationValues.Add(0f);
            }
        }
    }

    
    public Vector3 GetCurrentPosition(){return transform.position;}
    public float GetCurrentSpeed(){return (zValues[currentIndex] - zValues[currentIndex - 1])/(times[currentIndex] - times[currentIndex -1]);}
    
    public bool IsBraking()
    // Check if the car is braking to turn on the braking lights
    {
        // if the car in front was a pplayer use the data from braking pedals
        if (FrontIsPlayer)
        {
            return brakePedal[currentIndex] > 0;
        }
        // if it was the automatic trajectory compute the acceleration
        if (currentIndex > 3)
        {
            double currentSpeed = zValues[currentIndex-1] - zValues[currentIndex - 2];
            double previousSpeed = zValues[currentIndex-2] - zValues[currentIndex - 3];
            double acc = (currentSpeed - previousSpeed)/(times[currentIndex - 2] - times[currentIndex-3]);
            return acc < - 0.2;
        }
        return false;
    }
    
         // Method to reset everything
    public void RestartMovement()
    {
        // Reset variables to restart the process
        ReadDataFromFile(PathFile, times, zValues, xValues, brakePedal, orientationValues);
        currentIndex = 1;
        currentTime = 0f; // You might want to reset this as well to start over
        Debug.Log("Movement restarted");

        // Optionally, you could reset the object's position to the start
        transform.position = new Vector3(xValues[currentIndex], 0f, zValues[currentIndex]); // Resets position to first point
    }
 }
