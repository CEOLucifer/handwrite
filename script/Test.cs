using Godot;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Python.Runtime;
using Environment = System.Environment;

[GlobalClass]
public partial class Test : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var pyRoot = @"D:\project\handwrite\py";

		// set environment variables
		var pathToVirtualEnv = @"C:\Users\28329\miniconda3\envs\handwrite";
		var path = Environment.GetEnvironmentVariable("PATH").TrimEnd(';');
		path = string.IsNullOrEmpty(path) ? pathToVirtualEnv : path + ";" + pathToVirtualEnv;
		Environment.SetEnvironmentVariable("PATH", pathToVirtualEnv);
		Environment.SetEnvironmentVariable("PYTHONHOME", pathToVirtualEnv);
		Environment.SetEnvironmentVariable("PYTHONPATH", $"{pathToVirtualEnv}\\Lib\\site-packages;{pathToVirtualEnv}\\Lib");

		// C:\Users\28329\miniconda3\envs\handwrite\python.exe
		Runtime.PythonDLL = "python310.dll";
		PythonEngine.Initialize();
		PythonEngine.PythonHome = pathToVirtualEnv;
		PythonEngine.PythonPath = Environment.GetEnvironmentVariable("PYTHONPATH", EnvironmentVariableTarget.Process);

		using (Py.GIL())
		{
			dynamic sys = Py.Import("sys");
			sys.path.append(pyRoot);
			dynamic pythonScript = Py.Import("pythonscr");
			dynamic res = pythonScript.say_hello();
			res = pythonScript.message("messsage from csharp");
			GD.Print(res);
		}

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
