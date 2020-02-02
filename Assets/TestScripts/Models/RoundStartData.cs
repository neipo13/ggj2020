using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class RoundStartData : object
{
    public string action = "round_start";
    public string painting;

    public RoundStartData(string painting)
    {
        this.painting = painting;
    }
}
