using Newtonsoft.Json;
//using SQLite4Unity3d;
using System;
using System.Collections.Generic;

public class NotesInRoot
{
    public int ID { get; set; }
    public string user_id { get; set; }
    public string title { get; set; }
    public string text { get; set; }
    public string @object { get; set; }
    public string project_id { get; set; }
    public string position { get; set; }
}

public class NotesRoot
{
    public int status { get; set; }
    public List<Notes> data { get; set; }
    public bool success { get; set; }
}
