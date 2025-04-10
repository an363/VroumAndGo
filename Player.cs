// This is intended to be a public class. One public instance of it is stored in CarController, to give the specifics of the player (the ID, the test case, etc.)
using System;
using System.IO;
using UnityEngine;
using System.Globalization;

public class Player: MonoBehaviour

// constructs Player
// selects Case
// at the end of the run, call Player.ValidateRun()
{
	//static int seed=10; // not necessary -- use the random generator of the game
	static System.Random Rand = new System.Random();

	
	public int myID=-1; // ID of the player
	public int mycase; // scenario in which they are playing (noise, delay, etc.) --> call getCaseLabel() to get the letter ("A", "B", "C", etc.)
	public int myseries=-1; // series number within that scenario
	public int last_ID=-1; // ID of the car in front; this is 0 if no other player has played in this case/series yet 
	public string Folder_Cases;
	public string File_Summary;
	public string File_saving;
		
	public float density=-1.0f; // car_length + gap is the inverse density
	public float delay=-1.0f;
	public float noise=-1.0f;

	void Start()
	// this initializes the player, with their ID and attributes
	{
		Folder_Cases= Path.Combine(Application.persistentDataPath, "CASES");
		File_Summary= Path.Combine(Application.persistentDataPath, "summary.txt");
		this.myID = (int) DateTimeOffset.UtcNow.Subtract(new DateTime(2025, 1, 1)).TotalSeconds;
		File_saving= Path.Combine(Application.persistentDataPath, "DATA", this.myID + ".txt");
		
		//Debug.Log("Player ID: " + this.myID);
		//Debug.Log("Folder: " + Folder_Cases);
		//Debug.Log("Summary: " + File_Summary);

		this.SelectCase();
		this.ReadParameters();
		
		// check if myFile.txt file is created at the specified path 
		if (File.Exists(File_Summary))
		{
			Console.WriteLine("File already existed.");
		}
		else
		{
			File.Create(File_Summary);
			Console.WriteLine("File was not created.");
		}
	}
	

	
	public void SelectCase()
	// this sets the case for the player, and the other properties
	{
	
	// count number of cases 
	int nb_cases = Directory.GetFiles(Folder_Cases, "*", SearchOption.TopDirectoryOnly).Length;

	int[,] nb_drivers = new int[nb_cases,20]; // I assume that there are at most 20 series per case; nb_drivers[case,series]
	int[,] last_ID = new int[nb_cases,20]; // ID of the last driver in that [case,series]
	
	 
	// count number of series for that case
	try
	{
		// Open the text file using a stream reader.
		// "ID case series valid"
		string  text= File.ReadAllText (File_Summary);
		string[] lines = text.Split(new string[] { "\n" }, StringSplitOptions.None);
		
		
		
		for (int cpt=1;cpt<lines.Length;cpt++)
			{
			if (lines[cpt].Length<1)
				continue;
			string[] fields = lines[cpt].Split(new string[] { " " }, StringSplitOptions.None);
			int ID= int.Parse(fields[0]);
			int case_loc= (char) fields[1].ToCharArray()[0] - 'A';
			int series= int.Parse(fields[2]);
			if (fields[3]=="yes") // if run was valid
				{
				nb_drivers[case_loc,series]++;
				if (ID>last_ID[case_loc,series])
					last_ID[case_loc,series]= ID;
				}
			}
	
	}
	catch (IOException e)
	{
	    Console.WriteLine("The summary file could not be read:");
	    Console.WriteLine(e.Message);
	}


	this.mycase= Rand.Next() % nb_cases; // randomly selected case
	
	int series_selected= 0;
	while(nb_drivers[this.mycase,series_selected]>50) // assume 50 runs are needed
		series_selected++;
		
	this.myseries= series_selected;
	this.last_ID= last_ID[this.mycase,series_selected];
	return;
		
	
	}
	
	public char getCaseLabel()
	{
		return (char) ((int) 'A' + this.mycase);
	}
	
	public void ReadParameters()
	{
	string CasePath= Path.Combine(Folder_Cases,  Char.ToString(getCaseLabel()) + ".txt");
	string  text= File.ReadAllText (CasePath);
	string[] lines = text.Split(new string[] { "\n" }, StringSplitOptions.None);
	
	try
		{



		string[] fields = lines[0].Split(new string[] { "=" }, StringSplitOptions.None);
		
		this.density= float.Parse(fields[1], CultureInfo.InvariantCulture);
		fields = lines[1].Split(new string[] { "=" }, StringSplitOptions.None);
		this.delay= float.Parse(fields[1], CultureInfo.InvariantCulture);
		fields = lines[2].Split(new string[] { "=" }, StringSplitOptions.None);
		this.noise= float.Parse(fields[1], CultureInfo.InvariantCulture);		
		}
	catch (Exception e)
		{
		Console.WriteLine("The config file " + Char.ToString(getCaseLabel()) +".txt could not be read.");
		Console.WriteLine(e.Message);
		}		
	}

	public void ValidateRun(float ratioTooClose, float ratioTooFar)
	{
		//if (true) // if the run was valid
		if ((ratioTooClose < 0.5) && (ratioTooFar < 0.5) && (ratioTooClose + ratioTooFar < 0.75))
		{
			File.AppendAllText(File_Summary,string.Format("{0} {1} {2} yes \n", this.myID,this.getCaseLabel(),this.myseries));
		}
		else
		{
			File.AppendAllText(File_Summary,string.Format("{0} {1} {2} false", this.myID,this.getCaseLabel(),this.myseries) );
		}
	}
}


