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
        var envRoot = @"C:\Users\28329\miniconda3\envs\handwrite";
        var path = Environment.GetEnvironmentVariable("PATH").TrimEnd(';');
        path = string.IsNullOrEmpty(path) ? envRoot : path + ";" + envRoot;
        Environment.SetEnvironmentVariable("PATH", path);
        Environment.SetEnvironmentVariable("PYTHONHOME", envRoot);
        Environment.SetEnvironmentVariable("PYTHONPATH", $"{envRoot}\\Lib;{envRoot}\\Lib\\site-packages");

        // C:\Users\28329\miniconda3\envs\handwrite\python.exe
        Runtime.PythonDLL = @"C:\Users\28329\miniconda3\envs\handwrite\python310.dll";
        PythonEngine.Initialize();
        PythonEngine.PythonHome = envRoot;
        PythonEngine.PythonPath = Environment.GetEnvironmentVariable("PYTHONPATH");

        using (Py.GIL())
        {
            sys = Py.Import("sys");
            sys.path.append(pyRoot);
            api = Py.Import("api");
        }
    }
}