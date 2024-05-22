using Newtonsoft.Json;
using System;
using System.Collections.Generic;

[Serializable]
public class Project
{
    private int ID;
    public string name;// { get; set; } //projects name
    public string start; //date
    public string finish; //date
    public string layername; //pictures, design, interior design, BMI...
    public string model;// gameobject list or url for AssetBundle
}

public class ProjectRoot
{
    public int status;
    public List<Project> data { get; set; }
    public bool success;
}