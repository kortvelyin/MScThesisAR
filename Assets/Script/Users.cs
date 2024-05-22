using Newtonsoft.Json;
using System;
using System.Collections.Generic;
[Serializable]
public class User
{
    public int ID { get; set; }
    public string name { get; set; }
    public string company { get; set; }
    public string job { get; set; }
    public string title { get; set; }
}

public class UserRoot
{
    public int status { get; set; }
    public List<User> data { get; set; }
    public bool success { get; set; }
}
