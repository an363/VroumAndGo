using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenueStystem20250314 : MonoBehaviour
{
    //[SerializeField] private Button startButton; // Start button to start the game
    [SerializeField] private Button pauseButton; // Button to pause the game
    //[SerializeField] private GameObject loadingText;
    //[SerializeField] private Text loadingTextUI; // Reference to the UI Text component on the loading text object
    public Text speedText; // Reference to the UI Text component to display the speed
    [SerializeField] private Transform leadingCar;
    
    private CarController carController; // Reference to the CarController script
    
    private bool isGameRunning = false; // Flag to track if the game is running or stopped
    private bool isGamePaused = false; // Flag to track if the game is paused
    
    void Start()
    {
    
        // Set up button listeners
        //startButton.onClick.AddListener(ToggleStartStop);
        //loadingText.SetActive(false); // Show loading text
        carController = leadingCar.GetComponent<CarController>();

        // Optional: Display speed info if needed
        if (carController != null && leadingCar != null)
        {
            float speed = carController.GetCurrentSpeed();
            speedText.text = "Moving at: " + speed.ToString("F0") + " m/s"; // Format to 2 decimal places
        }

        // Initialize button colors based on game state
        //UpdateButtonStates();
    }

    void Update()
    {
        // Optional: Display speed info if needed
        if (carController != null && leadingCar != null)
        {
            float speed = carController.GetCurrentSpeed()*3.6f;
            speedText.text = speed.ToString("F0"); // Format to 2 decimal places
        }
        
        // Toggle full-screen when Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
    }
}
    /*
    // Toggles between starting and stopping the game
    public void ToggleStartStop()
    {
        if (!isGameRunning && !isGamePaused)
        {
            // Game is not running and not paused, start the game
            StartGame();
        }
        else if (isGameRunning && isGamePaused)
        {
            // If game is running but paused, clicking Play should resume
            ResumeGame();
        }
        else
        {
            Debug.Log("Game is already running and not paused.");
        }
    }

    // Start the game (load scene, etc.)
    public void StartGame()
    {
        loadingText.SetActive(true); // Show loading text
        StartCoroutine(WaitAndStart());
        isGameRunning = true;
        isGamePaused = false; // Reset paused state to false when starting the game
        Time.timeScale = 1; // Ensure the game time is running when it starts
		// Reset the CarInfront1 script
		CarInFront Carinfront2 = FindFirstObjectByType<CarInFront>(); // Find the CarInfront1 component in the scene
		if (Carinfront2 != null)
		{
		    Carinfront2.RestartMovement(); // Reset the car state
		} else {Debug.Log("Car In front not found");}
        UpdateButtonStates();
    }

    // Stop the game (reset time scale)
    public void StopGame()
    {
        Time.timeScale = 0; // Stop the game time
        isGameRunning = false;
        isGamePaused = false; // Reset paused state when stopping the game
        
        CarInFront Carinfront2 = FindFirstObjectByType<CarInFront>(); // Find the CarInfront1 component in the scene
		if (Carinfront2 != null)
		{
		    Carinfront2.RestartMovement(); // Reset the car state
		} else {Debug.Log("Car In front not found");}
        UpdateButtonStates();
    }

    // Resume the game (set time scale back to 1)
    public void ResumeGame()
    {
        Time.timeScale = 1; // Resume normal game time
        isGamePaused = false;
        UpdateButtonStates();
        Debug.Log("Game Resumed");
    }

    private void UpdateButtonStates()
    {
        // Play Button: 
        // Green when not running (can start the game), Blue when game is running and cannot be pressed
        ColorBlock startButtonColors = startButton.colors;
        startButtonColors.normalColor = (isGameRunning && !isGamePaused) ? Color.blue : Color.green;
        startButtonColors.selectedColor = (isGameRunning && !isGamePaused) ? Color.blue : Color.green;
        startButton.colors = startButtonColors;
    }

    IEnumerator WaitAndStart()
    {        // Show countdown (1, 2)
        for (int i = 1; i <= 3; i++)
        {
            loadingTextUI.text = "Loading:"+i.ToString(); // Update the loading text to show the countdown number
            yield return new WaitForSeconds(1f); // Wait for 1 second before the next countdown number
        }
        //yield return new WaitForSeconds(5f); // Wait for 1 second for loading
        loadingText.SetActive(false); // Hide loading text
        SceneManager.LoadScene("VRTeamProjectScene");
        Debug.Log("Game Started!");
    }
    
}
*/
