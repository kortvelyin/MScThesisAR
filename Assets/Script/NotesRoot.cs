using Newtonsoft.Json;
//using SQLite4Unity3d;
using System;
using System.Collections.Generic;


public class NotesRoot
{
    public int status { get; set; }
    public List<Notes> data { get; set; }
    public bool success { get; set; }
}
