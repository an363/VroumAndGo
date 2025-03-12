using System;
using System.IO;

class CaseSelector
{

	static int seed=10;
	static Random Rand = new Random(seed);
	static string Folder_Cases= "/home/alexandre/Desktop/Lyon/Production/250115_VRoumAndGo/TEST/CASES";
	static string File_Summary= "/home/alexandre/Desktop/Lyon/Production/250115_VRoumAndGo/TEST/summary.txt";
	
	int myID=-1;
	char mycase;
	int myseries=-1;
	
		
	static void Main()
	{
	
	CaseSelector cs= new CaseSelector();
	cs.Initialize();
		
	}
	
	void Initialize()
	{
		this.myID = (int) DateTimeOffset.UtcNow.Subtract(new DateTime(2025, 1, 1)).TotalSeconds;
		
		
		
		(char mycase, int myseries, int last_ID)= SelectCase();
		Console.WriteLine( mycase);
		 Console.WriteLine( myseries);
		 Console.WriteLine(  last_ID);
		
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
		
		this.SelectCase();
		
		return;
		
	}
	

	
	public (char mycase, int myseries, int lastID) SelectCase()
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
			int mycase= (char) fields[1].ToCharArray()[0] - 'A';
			int series= int.Parse(fields[2]);
			if (fields[3]=="yes") // if run was valid
				{
				nb_drivers[mycase,series]++;
				if (ID>last_ID[mycase,series])
					last_ID[mycase,series]= ID;
				}
			}
	
	}
	catch (IOException e)
	{
	    Console.WriteLine("The summary file could not be read:");
	    Console.WriteLine(e.Message);
	}


	int case_selected= 	(Rand.Next() % nb_cases);
	int series_selected= 0;
	while(nb_drivers[case_selected,series_selected]>50) // assume 50 runs are needed
		series_selected++;
		
	return ((char)((char)'A' + case_selected), series_selected,last_ID[case_selected,series_selected]); 
		
	
	}
	
	void ValidateRun()
	{
		if (true) // if the run was valid
			File.AppendAllText(File_Summary,string.Format("{0} {1} {2} {3} yes", Environment.NewLine,this.myID,this.mycase,this.myseries) );
		else
			File.AppendAllText(File_Summary,string.Format("{0} {1} {2} {3} false", Environment.NewLine,this.myID,this.mycase,this.myseries) );
			
	}
}


