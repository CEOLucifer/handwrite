using System;
using Godot;
using Python.Runtime;
using Newtonsoft.Json;
using System.IO;
using Environment = System.Environment;

class Config
{
    public string pyRoot { get; set; }
    public string envRoot { get; set; }
    public string PythonDLL { get; set; }
}

public class MyPy
{
    public dynamic sys;
    public dynamic api;
    public string pyRoot;
    public string envRoot;
    public string pythonDLL;

    public MyPy()
    {
        // 读取配置，支持注释
        var configPath = "config/config.json";
        var configText = File.ReadAllText(configPath);
        var config = JsonConvert.DeserializeObject<Config>(configText);
        pyRoot = config.pyRoot;
        envRoot = config.envRoot;
        pythonDLL = config.PythonDLL;

        // set environment variables
        var path = Environment.GetEnvironmentVariable("PATH").TrimEnd(';');
        path = string.IsNullOrEmpty(path) ? envRoot : path + ";" + envRoot;
        Environment.SetEnvironmentVariable("PATH", path);
        Environment.SetEnvironmentVariable("PYTHONHOME", envRoot);
        Environment.SetEnvironmentVariable("PYTHONPATH", $"{envRoot}\\Lib");

        // C:\Users\28329\miniconda3\envs\handwrite\python.exe
        Runtime.PythonDLL = pythonDLL;
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