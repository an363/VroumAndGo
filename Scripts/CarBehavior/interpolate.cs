using System;
using System.IO;


class Function
{
	static float dt= 1.0f;
	static float dt_input= 0.1f;
	static float noise_amp= 1.0f; 
	static float delay= 1.0f;
	
	static int seed=10;
	static Random Rand = new Random(seed);
	
	static void Main()
	{
		// path of the file that we want to create
		string pathName = "myFile.txt";

		// Create() creates a file at pathName 
		FileStream fs = File.Create(pathName);

		float[] timestamps={0.0f,0.1f,0.2f,0.3f,0.4f,0.5f,0.6f,0.7f,0.8f,0.9f};
		float[] acc_pedal= {1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f};
		float[] brake_pedal= {1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f};

		// check if myFile.txt file is created at the specified path 
		if (File.Exists(pathName))
		{
			Console.WriteLine("File is created.");
			Console.WriteLine(getNewSpeed(0.0f,0.0f,acc_pedal,brake_pedal));
		}
		else
		{
			Console.WriteLine("File is not created.");
		}
	float x=	Interpolate(timestamps, timestamps, 0.19f, presumed_index: 19);
	Console.WriteLine( x);
	}
	
	static float getNewSpeed(float xt, float vt, float[] acc_pedal, float[] brake_pedal)
	// acc_pedal and brake_pedal both contain the history of the control inputs over the last few seconds
	{
		float acceleration= 0.0f;
		// convert pedal controls into desired acceleration
		if (delay > 0.0f)
			{
			int delay_steps=  Convert.ToInt16(delay/dt_input);
			if (delay_steps >= acc_pedal.Length)
				delay_steps= acc_pedal.Length-1;
			acceleration= 10.0f * acc_pedal[delay_steps] - 11.0f * brake_pedal[delay_steps];
			}
		
		// 
		return (float) (vt + acceleration * dt + (float) noise_amp * (Rand.NextDouble()-0.5) * Math.Sqrt(dt));
	}
	
	static float Interpolate(float[] timestamps, float[] x, float t, int presumed_index= -1)
	// returns the linearly interpolated value of x at time t
	// getting as input the array of timestamps and corresponding positions x
	// The presumed index is such that we expect timestamps[presumed_index] to be the timestamp just before 
	{
		int N= Math.Min(timestamps.Length,x.Length);
		float coef= 0.0f;
			
		if (presumed_index>=0 && presumed_index<N && timestamps[presumed_index]<=t)
			{
			for (int cpt=presumed_index+1; cpt<Math.Min(presumed_index+10,N-1); cpt++)
				if (timestamps[cpt]>t)
					{
					coef= (timestamps[cpt]-t) / (timestamps[cpt]-timestamps[cpt-1]);
					return coef * x[cpt-1] + (1.0f - coef) * x[cpt];
					}
			}
		
		
		// check if actual time is out of bounds
		if (timestamps[0]>=t)
			{
			Console.WriteLine("Requested time is out of the bounds");
			return x[0]; // OUT OF BOUNDS!!
			}
		if (timestamps[N-1]<=t)
			{
			Console.WriteLine("Requested time is out of the bounds");
			return x[N-1]; // OUT OF BOUNDS!!
			}
		
		// IF WE REACH THIS POINT, IT MEANS THE ABOVE HAS NOT BEEN SUCCESSFUL!
		int bound_low= 0;
		int bound_high= N-1;
		
		while (bound_high-bound_low>1)
			{
			int new_bound= (int) ((bound_low+bound_high)/2 + 0.5f);
			if (timestamps[new_bound]>t)
				bound_high= new_bound;
			else
				bound_low= new_bound;
			}
		
		coef= (timestamps[bound_high]-t) / (timestamps[bound_high]-timestamps[bound_low]);
		return coef * x[bound_low] + (1.0f - coef) * x[bound_high];
		
	}	
}
