using System;
using Godot;
using Python.Runtime;
using Environment = System.Environment;

class MyPy
{
    public dynamic sys;

    public dynamic api;

    public MyPy()
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
        Runtime.PythonDLL = @"python310.dll";
        PythonEngine.Initialize();
        PythonEngine.PythonHome = pathToVirtualEnv;
        PythonEngine.PythonPath = Environment.GetEnvironmentVariable("PYTHONPATH", EnvironmentVariableTarget.Process);

        using (Py.GIL())
        {
            sys = Py.Import("sys");
            sys.path.append(pyRoot);
            api = Py.Import("api");
        }
    }
}