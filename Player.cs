// This is intended to be a public class. One public instance of it is stored in CarController, to give the specifics of the player (the ID, the test case, etc.)
using System;
using System.IO;

public class Player
// constructs Player
// selects Case
// at the end of the run, call Player.ValidateRun()
{
	static string Folder_Cases= "/home/alexandre/Desktop/Lyon/Production/250115_VRoumAndGo/TEST/CASES";
	static string File_Summary= "/home/alexandre/Desktop/Lyon/Production/250115_VRoumAndGo/TEST/summary.txt";

	static int seed=10; // not necessary -- use the random generator of the game
	static Random Rand = new Random(seed);

	
	public int myID=-1; // ID of the player
	public int mycase; // scenario in which they are playing (noise, delay, etc.) --> call getCaseLabel() to get the letter ("A", "B", "C", etc.)
	public int myseries=-1; // series number within that scenario
	public int last_ID=-1; // ID of the car in front; this is 0 if no other player has played in this case/series yet 
	
		
	static void Main()
	{
	Player player= new Player();
	player.ValidateRun();	
	
	}
	
	public Player()
	// this initializes the player, with their ID and attributes
	{
		
		this.myID = (int) DateTimeOffset.UtcNow.Subtract(new DateTime(2025, 1, 1)).TotalSeconds;
		
		this.SelectCase();
		
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


	this.mycase= (Rand.Next() % nb_cases); // randomly selected case
	
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
	
	public void ValidateRun()
	{
		if (true) // if the run was valid
			File.AppendAllText(File_Summary,string.Format("{0} {1} {2} yes", this.myID,this.getCaseLabel(),this.myseries) );
		else
			File.AppendAllText(File_Summary,string.Format("{0} {1} {2} false", this.myID,this.getCaseLabel(),this.myseries) );
	}
}


