using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class StartScene : MonoBehaviour
{

[SerializeField] TextMeshProUGUI CounterTMP_go;
    //public Text displayText1; // Reference to a UI Text element in your scene
    //public Text displayText2; // Reference to a UI Text element in your scene
    //public Text displayText3; // Reference to a UI Text element in your scene
    //public Text displayText4; // Reference to a UI Text element in your scene
    //public Text displayText5; // Reference to a UI Text element in your scene
    //public Text displayText6; // Reference to a UI Text element in your scene
    //public Text displayText7; // Reference to a UI Text element in your scene

    void Start()
    {
    Time.timeScale = 1f;
    StartCoroutine(StartSequence());
    }
    

    // Wait for 5 seconds
    private IEnumerator StartSequence()
    {
    
    //Debug.Log("Starting sequence");
            // Wait for 5 seconds before starting countdown
        //yield return new WaitForSeconds(5f);
        // Clear all messages at the start
        //displayText.text = "";
        //displayText1.text = "";
        //displayText2.text = "";
        //displayText3.text = "";
        //displayText4.text = "";
        //displayText5.text = "";
        //displayText6.text = "";
        //displayText7.text = "";
        //Debug.Log("Text cleared");
        
        
        //Debug.Log("Count Down");

        // Countdown from 5 seconds with a fade-in effect for each number
        for (int i = 5; i > 0; i--)
        {
            // Update the countdown message
            CounterTMP_go.text = $"{i}";

            // Wait for 1 second before showing the next countdown
            yield return new WaitForSeconds(1f);
        }

        // After the countdown, load the MainScene
        SceneManager.LoadScene("MainScene");
    }
    

}
