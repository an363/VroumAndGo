using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;
 
 
public class GameMenu : MonoBehaviour

{
 public TextMeshProUGUI PlayAgain;
	public static bool isPaused;

	public void PauseGame()
		{
		Time.timeScale = 0f;
		isPaused = true;
		PlayAgain.text = "'Restart' to play again.";
		}

	public void ResumeGame()
		{
		Time.timeScale = 1f;
		isPaused = false;
		
		}

	public void GoToStartMenue()
		{
		Time.timeScale = 1f;
		isPaused = false;
		PlayAgain.text = "";
		SceneManager.LoadScene("StartScene");
		}
		
		
	public void Quit()
		{
		    Debug.Log("Quit called!");

		#if UNITY_EDITOR
		    UnityEditor.EditorApplication.isPlaying = false; // Stops play mode in Editor
		#else
		    Application.Quit(); // Quits the built app
		#endif
		}


}
 
