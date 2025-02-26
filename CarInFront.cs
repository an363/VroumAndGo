using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;



public class CarInfront1 : MonoBehaviour
{
    [SerializeField] TextAsset TextFile;
   // private List<Step> steps;
    private int currentIndex = 0; // To track the current data point we're using from the CSV

     private List<float> times = new List<float>(); // List to store time values
    private List<float> zValues = new List<float>(); // List to store density values
    private List<float> xValues = new List<float>(); // List to store density values


    void Start()
    {
        if (TextFile != null)
        {
            ReadDataFromFile(TextFile, times, zValues, xValues);
        }
        else
        {
            Debug.LogError("No data file assigned in the Inspector.");
        }
    } 

   void Update()
    {
        currentTime = Time.time;
        while (times[currentIndex] < currentTime){++currentIndex};

        float alpha = (currentTime - times[currentIndex-1])/(times[currentIndex]-times[currentIndex-1]);
        float currentzValue = zValues[currentIndex-1] + alpha*zValues[currentIndex] ;
        float currentxValue = xValues[currentIndex-1] + alpha*xValues[currentIndex] ;
        transform.position =  new Vector3(currentxValue,0, currentzValue);
    }

    void ReadDataFromFile(TextAsset file, List<float> times, List<float> zvalues, List<float> xvalues)
        {
            // Get the text content from the TextAsset
            string[] lines = file.text.Split('\n');

            foreach (string line in lines)
            {
                // Trim any excess spaces and split the line by space or tab
                string[] data = line.Trim().Split(' ');  // delimiter if necessary

                if (data.Length >= 3)
                {
                    times.Add(float.Parse(data[0]));
                    zvalues.Add(float.Parse(data[1]));
                    xvalues.Add(float.Parse(data[2]));
                }
            }
        }
 }
